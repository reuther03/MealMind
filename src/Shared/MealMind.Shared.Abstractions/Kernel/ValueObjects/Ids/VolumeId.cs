using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

public record VolumeId : EntityId
{
    public VolumeId(Guid value) : base(value)
    {
    }

    public static VolumeId New() => new(Guid.NewGuid());
    public static VolumeId From(Guid value) => new(value);
    public static VolumeId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(VolumeId volumeId) => volumeId.Value;
    public static implicit operator VolumeId(Guid volumeId) => new(volumeId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}