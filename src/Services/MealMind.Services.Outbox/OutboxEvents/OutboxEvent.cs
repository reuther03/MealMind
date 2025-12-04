namespace MealMind.Services.Outbox.OutboxEvents;

public class OutboxEvent
{
    public Guid Id { get; }
    public string Type { get; } = null!;
    public object Payload { get; } = null!;
    public EventState State { get; private set; }
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
    public DateTime? ProcessedOn { get; private set; }
    public string? Error { get; private set; }
    public int Attempts { get; private set; }

    private OutboxEvent()
    {
    }

    private OutboxEvent(Guid id, string type, object payload)
    {
        Id = id;
        Type = type;
        Payload = payload;
        State = EventState.Pending;
        CreatedOn = DateTime.UtcNow;
        ProcessedOn = null;
        Error = null;
        Attempts = 0;
    }

    public static OutboxEvent Create(Guid id, Type type, object payload)
        => new(id, type.FullName!, payload);

    public void SetProcessed()
    {
        State = EventState.Processed;
        ProcessedOn = DateTime.UtcNow;
        Attempts++;
    }

    public void SetFailed(string error)
    {
        State = EventState.Failed;
        ProcessedOn = DateTime.UtcNow;
        Error = error;
        Attempts++;
    }
}