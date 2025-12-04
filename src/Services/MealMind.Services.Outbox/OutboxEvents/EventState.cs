namespace MealMind.Services.Outbox.OutboxEvents;

public enum EventState
{
    Pending,
    Processed,
    Failed
}