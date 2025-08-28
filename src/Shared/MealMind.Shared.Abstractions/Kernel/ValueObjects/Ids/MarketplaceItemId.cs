using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

public record MarketplaceItemId : EntityId
{
    public MarketplaceItemId(Guid value) : base(value)
    {
    }

    public static MarketplaceItemId New() => new(Guid.NewGuid());
    public static MarketplaceItemId From(Guid value) => new(value);
    public static MarketplaceItemId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(MarketplaceItemId marketplaceItemId) => marketplaceItemId.Value;
    public static implicit operator MarketplaceItemId(Guid marketplaceItemId) => new(marketplaceItemId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}