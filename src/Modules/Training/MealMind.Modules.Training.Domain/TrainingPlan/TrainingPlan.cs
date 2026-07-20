using MealMind.Modules.Training.Domain.Events;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class TrainingPlan : AggregateRoot<TrainingPlanId>
{
    private readonly List<TrainingSession> _sessions = [];
    public Name Name { get; private set; } = null!;
    public DayOfWeek PlannedOn { get; private set; }
    public bool IsActive { get; private set; }
    public UserId UserId { get; private set; }
    public IReadOnlyList<TrainingSession> Sessions => _sessions.AsReadOnly();

    private TrainingPlan()
    {
    }

    private TrainingPlan(TrainingPlanId id, Name name, DayOfWeek plannedOn, UserId userId) : base(id)
    {
        Name = name;
        PlannedOn = plannedOn;
        IsActive = true;
        UserId = userId;
    }

    public static TrainingPlan Create(Name name, DayOfWeek planedOn, UserId userId) =>
        new(TrainingPlanId.New(), name, planedOn, userId);

    public void AddSession(TrainingSession session)
    {
        if (_sessions.Any(x => x.Id == session.Id))
            throw new DomainException("Cannot add identical training session to the training plan.");

        _sessions.Add(session);

        RaiseDomainEvent(new TrainingPlanChangedDomainEvent(UserId.Value, Id.Value));
    }

    public void SessionSetAsEnded(TrainingSession session)
    {
        if (_sessions.All(x => x.Id != session.Id))
            throw new DomainException(
                "Cannot set as ended training session that does not belong to the training plan.");

        session.SetAsEnded();
        RaiseDomainEvent(new TrainingPlanChangedDomainEvent(UserId.Value, Id.Value));
    }

    public void AddSessionExercise(TrainingSession session, SessionExercise exercise)
    {
        var trainingSession = _sessions.FirstOrDefault(x => x.Id == session.Id);
        if (trainingSession == null)
            throw new DomainException("Training session does not belong to the training plan.");

        trainingSession.AddExercise(exercise);
        RaiseDomainEvent(new TrainingPlanChangedDomainEvent(UserId.Value, Id.Value));
    }
}
