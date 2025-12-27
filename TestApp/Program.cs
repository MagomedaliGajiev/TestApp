namespace TestApp;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Testing Inventory System");
            
        // Тестовый пример использования
        var inventory = new Inventory();
            
        inventory.AddItem(new Item("Sword", 10));
        inventory.AddItem(new Item("Shield", 15));
        inventory.AddItem(new Item("Potion", 2));
            
        Console.WriteLine($"Current weight: {inventory.CurrentWeight}/100");
            
        var found = inventory.SearchItems("pot");
        Console.WriteLine($"Found {found.Count} items");
            
        foreach (var item in inventory.Items)
        {
            Console.WriteLine($"{item.Name} - {item.Weight}kg");
        }
    }
}