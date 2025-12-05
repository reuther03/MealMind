using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Services.Outbox.Database.JsonConverters;

public class UserIdGuidJsonConverter : ValueObjectConverter<UserId, Guid>
{
    protected override Guid GetValue(UserId obj)
        => obj.Value;

    protected override UserId CreateFrom(Guid value)
        => UserId.From(value);
}