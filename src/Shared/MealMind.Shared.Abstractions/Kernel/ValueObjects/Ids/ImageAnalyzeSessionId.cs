using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

public record ImageAnalyzeSessionId : EntityId
{
    private ImageAnalyzeSessionId(Guid value) : base(value)
    {
    }

    public static ImageAnalyzeSessionId New() => new(Guid.NewGuid());
    public static ImageAnalyzeSessionId From(Guid value) => new(value);
    public static ImageAnalyzeSessionId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(ImageAnalyzeSessionId id) => id.Value;
    public static implicit operator ImageAnalyzeSessionId(Guid id) => new(id);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
