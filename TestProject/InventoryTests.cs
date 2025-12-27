using TestApp;

namespace TestProject;

public class InventoryTests
    {
        [Fact]
        public void AddItem_NullItem_ThrowsArgumentNullException()
        {
            var inventory = new Inventory();
            Assert.Throws<ArgumentNullException>(() => inventory.AddItem(null));
        }

        [Fact]
        public void AddItem_ExceedsMaxWeight_ReturnsFalse()
        {
            var inventory = new Inventory();
            var heavyItem = new Item("Anvil", 150);
            
            Assert.False(inventory.AddItem(heavyItem));
        }

        [Fact]
        public void AddItem_DuplicateItem_SumsWeight()
        {
            var inventory = new Inventory();
            var item1 = new Item("Gold", 10);
            var item2 = new Item("Gold", 20);
            
            Assert.True(inventory.AddItem(item1));
            Assert.True(inventory.AddItem(item2));
            
            var items = inventory.Items;
            Assert.Single(items);
            Assert.Equal(30, items.First().Weight);
            Assert.Equal(30, inventory.CurrentWeight);
        }

        [Fact]
        public void AddItem_DuplicateItemExceedsWeight_ReturnsFalse()
        {
            var inventory = new Inventory();
            var item1 = new Item("Gold", 90);
            var item2 = new Item("Gold", 20);
            
            Assert.True(inventory.AddItem(item1));
            Assert.False(inventory.AddItem(item2));
        }

        [Fact]
        public void SearchItems_FindsMatchingItems()
        {
            var inventory = new Inventory();
            inventory.AddItem(new Item("Sword", 10));
            inventory.AddItem(new Item("Long Sword", 12));
            inventory.AddItem(new Item("Shield", 8));
            
            var results = inventory.SearchItems("sword");
            
            Assert.Equal(2, results.Count);
            Assert.Contains(results, i => i.Name == "Sword");
            Assert.Contains(results, i => i.Name == "Long Sword");
        }

        [Fact]
        public void RemoveItem_RemovesItemAndWeight()
        {
            var inventory = new Inventory();
            var item = new Item("Sword", 10);
            
            inventory.AddItem(item);
            Assert.Equal(10, inventory.CurrentWeight);
            
            Assert.True(inventory.RemoveItem(item));
            Assert.Equal(0, inventory.CurrentWeight);
            Assert.Empty(inventory.Items);
        }

        [Fact]
        public void Items_PropertyReturnsCopy()
        {
            var inventory = new Inventory();
            var item = new Item("Sword", 10);
            
            inventory.AddItem(item);
            var items = inventory.Items;
            
            Assert.Single(items);
            Assert.Equal("Sword", items.First().Name);
        }

        [Fact]
        public async Task ThreadSafety_MultipleThreadsAddingItems()
        {
            var inventory = new Inventory();
            var tasks = new Task[10];
            
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        inventory.AddItem(new Item($"Item{index}_{j}", 1));
                    }
                });
            }
            
            await Task.WhenAll(tasks);
            
            // Максимальный вес 100, не должно быть больше
            Assert.True(inventory.CurrentWeight <= 100);
            
            // Проверяем, что нет дубликатов с одинаковыми именами
            var items = inventory.Items;
            var distinctNames = items.Select(i => i.Name.ToUpperInvariant()).Distinct().Count();
            Assert.Equal(distinctNames, items.Count);
        }

        [Fact]
        public void Item_Equality_IsCaseInsensitive()
        {
            var item1 = new Item("Sword", 10);
            var item2 = new Item("SWORD", 10);
            var item3 = new Item("Shield", 10);
            
            Assert.Equal(item1, item2);
            Assert.NotEqual(item1, item3);
            Assert.Equal(item1.GetHashCode(), item2.GetHashCode());
        }
    }