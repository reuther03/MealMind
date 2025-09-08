using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Modules.Nutrition.Domain.Tracking;

public record WeekLogId : EntityId
{
    public WeekLogId(Guid value) : base(value)
    {
    }

    public static WeekLogId New() => new(Guid.NewGuid());
    public static WeekLogId From(Guid value) => new(value);
    public static WeekLogId From(string value) => new(Guid.Parse(value));

    public static implicit operator Guid(WeekLogId weekLogId) => weekLogId.Value;
    public static implicit operator WeekLogId(Guid dailyLogId) => new(dailyLogId);

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}