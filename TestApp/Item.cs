namespace TestApp;

public class Item : IEquatable<Item>
{
    public string Name { get; }
    public int Weight { get; }

    public Item(string name, int weight)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Weight = weight;
    }

    public bool Equals(Item other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj) => Equals(obj as Item);
        
    public override int GetHashCode() => Name.ToUpperInvariant().GetHashCode();
}
