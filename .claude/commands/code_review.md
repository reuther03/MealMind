description: Review current branch changes for MealMind conventions (layering, TUnit tests, value object immutability, endpoint brevity)
  ---

  Review the changes on this branch against these MealMind conventions:

## Layering

- Endpoints under 30 lines, extract to private methods if longer
- No direct repository access in endpoints — use commands via ISender
- Commands live in Application, domain logic in Domain, EF in Infrastructure

## Testing

- TUnit only — `[Test]` attribute, `await Assert.That(x).IsTrue()`
- NO FluentAssertions, NO xUnit/NUnit attributes
- Test classes named `SomethingTest` (singular)
- Test methods: `Method_Scenario_ShouldOutcome`

## Domain

- Value objects are immutable — `init` setters, `with` expressions
- Factory methods `Create(...)` on entities, not public constructors

## Output

  For each violation: file:line, the rule broken, and the fix.
  If everything is clean, say so explicitly.
