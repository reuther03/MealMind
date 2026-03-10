using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using MealMind.Shared.Abstractions.Kernel.ValueObjects;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.Training.Domain.TrainingPlan;

public class TrainingPlan : AggregateRoot<TrainingPlanId>
{
    private readonly List<TrainingSession> _sessions = [];
    public Name Name { get; private set; } = null!;
    public DayOfWeek PlanedOn { get; private set; }
    public bool IsActive { get; private set; }
    public UserId UserId { get; private set; }
    public IReadOnlyList<TrainingSession> Sessions => _sessions.AsReadOnly();

    private TrainingPlan()
    {
    }

    private TrainingPlan(TrainingPlanId id, Name name, DayOfWeek planedOn, bool isActive, UserId userId) : base(id)
    {
        Name = name;
        PlanedOn = planedOn;
        IsActive = isActive;
        UserId = userId;
    }

    public static TrainingPlan Create(Name name, DayOfWeek planedOn, bool isActive, UserId userId) =>
        new(TrainingPlanId.New(), name, planedOn, isActive, userId);

    public void AddSession(TrainingSession session)
    {
        if (_sessions.Any(s => s.Name == session.Name))
            throw new DomainException($"Session with name {session.Name} already exists in this training plan.");

        _sessions.Add(session);
    }
}