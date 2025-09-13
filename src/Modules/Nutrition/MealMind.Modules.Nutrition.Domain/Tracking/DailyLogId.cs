using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public record DailyLogId : EntityId
{
    public DailyLogId(Guid value) : base(value)
    {
    }

    public static DailyLogId New() => new(Guid.NewGuid());
    public static DailyLogId From(Guid value) => new(value);
    public static DailyLogId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(DailyLogId dailyLogId) => dailyLogId.Value;
    public static implicit operator DailyLogId(Guid dailyLogId) => new(dailyLogId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}