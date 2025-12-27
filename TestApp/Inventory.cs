using System.Collections.Concurrent;

namespace TestApp;

public class Inventory
    {
        private const int MaxWeight = 100;
        private readonly ConcurrentDictionary<string, Item> _items = new ConcurrentDictionary<string, Item>();
        private int _totalWeight = 0;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public IReadOnlyCollection<Item> Items
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Values.ToList().AsReadOnly();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public bool AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _lock.EnterWriteLock();
            try
            {
                if (_totalWeight + item.Weight > MaxWeight)
                    return false;

                if (_items.TryGetValue(item.Name.ToUpperInvariant(), out var existingItem))
                {
                    // Если добавляем существующий предмет, проверяем вес
                    if (_totalWeight + item.Weight > MaxWeight)
                        return false;
                    
                    // Создаем новый объект с увеличенным весом
                    var updatedItem = new Item(existingItem.Name, existingItem.Weight + item.Weight);
                    _items[item.Name.ToUpperInvariant()] = updatedItem;
                    _totalWeight += item.Weight;
                    return true;
                }
                else
                {
                    if (_items.TryAdd(item.Name.ToUpperInvariant(), item))
                    {
                        _totalWeight += item.Weight;
                        return true;
                    }
                    return false;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _lock.EnterWriteLock();
            try
            {
                if (_items.TryRemove(item.Name.ToUpperInvariant(), out var removedItem))
                {
                    _totalWeight -= removedItem.Weight;
                    return true;
                }
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IReadOnlyList<Item> SearchItems(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Array.Empty<Item>();

            _lock.EnterReadLock();
            try
            {
                return _items.Values
                    .Where(item => item.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList()
                    .AsReadOnly();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public int CurrentWeight
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _totalWeight;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
    }