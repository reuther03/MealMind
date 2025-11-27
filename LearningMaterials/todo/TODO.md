# MealMind Development TODO

This file tracks important refactoring and feature tasks that need to be completed.

---

## 🔴 Critical Priority (Do First)

### 2. Remove DailyLog Creation from AddFoodCommand
**Status:** ❌ Not Started
**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Features/Commands/AddFoodCommand/AddFoodCommand.cs`

**Current State (Lines 47-68):**
```csharp
if (!await _dailyLogRepository.ExistsWithDateAsync(request.DailyLogDate, user.Id, cancellationToken))
{
    dailyLog = DailyLog.Create(
        request.DailyLogDate,
        request.CurrentWeight,
        user.NutritionTargets
            .Where(x => x.ActiveDays.Any(z => z.DayOfWeek == request.DailyLogDate.DayOfWeek))
            .Select(x => x.Calories)
            .FirstOrDefault(),
        user.Id);

    //delete this
    foreach (var type in Enum.GetValues<MealType>())
    {
        var meal = Meal.Initialize(type, user.Id);
        dailyLog.AddMeal(meal);
    }

    await _dailyLogRepository.AddAsync(dailyLog, cancellationToken);
}
```

**What to do:**
1. Remove lines 47-68 (DailyLog creation logic)
2. Simplify to just fetch the DailyLog:
```csharp
var dailyLog = await _dailyLogRepository.GetByDateAsync(request.DailyLogDate, user.Id, cancellationToken);
Validator.ValidateNotNull(dailyLog, "DailyLog not found. This should be pre-created.");
```

**Why Critical:**
- DailyLogs are now pre-created at signup (90 days)
- This code is redundant and creates confusion
- You already have a comment "//delete this" on line 60
- Violates single responsibility - AddFood should only add food, not create infrastructure

**Note:** Keep the TODO comment on line 78 about checking barcode in database before external service

---

## 🟡 High Priority (Do Soon)

### 3. Refactor AddNutritionTargetCommand to PATCH
**Status:** ❌ Not Started
**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Features/Commands/AddNutritionTargetCommand/AddNutritionTargetCommand.cs`

**Current Issue:**
- Command is called "Add" but nutrition targets are set at signup
- Should be "Update" or use PATCH semantics

**What to do:**
1. Rename to `UpdateNutritionTargetCommand` or `PatchNutritionTargetCommand`
2. Update endpoint to use PATCH instead of POST
3. Allow partial updates (e.g., only change weekday calories, keep weekend the same)
4. Consider: Should updating targets retroactively update future DailyLogs?
   - **Recommendation:** Only update future DailyLogs (today + forward)
   - Don't touch historical data

**Why High Priority:**
- Users will want to adjust targets as their fitness journey progresses
- Current "Add" semantics are misleading
- Need to decide on retroactive update strategy

---

### 4. Implement Background Job for Future DailyLogs
**Status:** ❌ Not Started
**Location:** Create new background job in Infrastructure

**Current State:**
- DailyLogs are pre-created for 90 days at signup
- No mechanism to extend the window as time passes

**What to do:**
1. Create `DailyLogMaintenanceJob` as a hosted service
2. Run daily (e.g., at midnight)
3. For each user, ensure next 90 days have DailyLogs created
4. Create any missing dates

**Example Implementation:**
```csharp
public class DailyLogMaintenanceJob : IHostedService
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Run daily at midnight
        var users = await _userRepository.GetAllAsync(cancellationToken);

        foreach (var user in users)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var endDate = today.AddDays(90);

            var existingDates = await _dailyLogRepository
                .GetDateRangeForUserAsync(user.Id, today, endDate, cancellationToken);

            // Create any missing dates
            var missingDates = Enumerable.Range(0, 90)
                .Select(i => today.AddDays(i))
                .Except(existingDates);

            foreach (var date in missingDates)
            {
                var dailyLog = DailyLog.Create(date, null,
                    GetCaloriesForDay(user, date), user.Id);

                foreach (var mealType in Enum.GetValues<MealType>())
                {
                    dailyLog.AddMeal(Meal.Initialize(mealType, user.Id));
                }

                await _dailyLogRepository.AddAsync(dailyLog, cancellationToken);
            }
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
```

**Why High Priority:**
- After 90 days, users will run out of pre-created DailyLogs
- System will break unless this is implemented
- Should be done before first user reaches day 90

---

## 🟢 Medium Priority (Nice to Have)

### 5. Add Null Safety to IdentityUserCreatedEventHandler
**Status:** ❌ Not Started
**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Events/Integration/IdentityUserCreatedEventHandler.cs`

**Current Issue (Line 86-88):**
```csharp
var calorieTarget = userProfile.NutritionTargets
    .FirstOrDefault(x => x.ActiveDays.Any(z => z.DayOfWeek == logDate.DayOfWeek))!.Calories;  // ⚠️ Null-forgiving operator
```

**What to do:**
Replace with explicit null check:
```csharp
var matchingTarget = userProfile.NutritionTargets
    .FirstOrDefault(x => x.ActiveDays.Any(z => z.DayOfWeek == logDate.DayOfWeek));

if (matchingTarget == null)
    throw new InvalidOperationException(
        $"No nutrition target found for {logDate.DayOfWeek}. This indicates a validation error.");

var calorieTarget = matchingTarget.Calories;
```

**Why Medium Priority:**
- Validation ensures this should never happen
- But defensive programming is better
- Clearer error messages for debugging

---

### 6. Fix QuantityInGrams Type Mismatch
**Status:** ❌ Not Started
**File:** `src/Shared/MealMind.Shared.Abstractions/Events/Integration/ImageAnalyzeCreatedEvent.cs`

**Current State:**
```csharp
public record ImageAnalyzeCreatedEvent(
    UserId UserId,
    Name FoodName,
    double QuantityInGrams,  // ⚠️ Inconsistent type
    decimal TotalCalories,
    // ...
)
```

**What to do:**
Change `double` to `decimal` for consistency:
```csharp
public record ImageAnalyzeCreatedEvent(
    UserId UserId,
    Name FoodName,
    decimal QuantityInGrams,  // ✅ Consistent with other nutrition values
    decimal TotalCalories,
    // ...
)
```

**Why Medium Priority:**
- All other nutrition values use `decimal`
- Prevents potential precision loss
- Minor breaking change (need to update event publisher)

---

### 7. Consider Adding "Unassigned" Meal Type
**Status:** 💭 Design Decision Needed

**Current State:**
- AI-analyzed foods go to "Snack" meal by default
- Users might forget they're in Snack

**Options:**
1. Keep current approach (Snack)
2. Add `MealType.Unassigned = 4` for AI foods
3. Make `Meal.MealId` nullable in `FoodEntry` (uncategorized foods)

**Pros/Cons:**

**Option 1 (Current - Snack):**
- ✅ Simple
- ✅ Works immediately
- ❌ Not semantically correct

**Option 2 (Unassigned MealType):**
- ✅ Clear intent
- ✅ Frontend can show "Review AI Foods" section
- ❌ Requires migration

**Option 3 (Nullable Meal):**
- ✅ Most flexible
- ❌ Complicates data model
- ❌ Requires nullable FK

**Recommendation:** Go with Option 2 after MVP is working

---

### 8. Add Validation to SignUpCommand
**Status:** ❌ Not Started
**File:** `src/Modules/Identity/MealMind.Modules.Identity.Application/Features/Commands/SignUpCommand/SignUpCommand.cs`

**What to do:**
Create FluentValidation validator:
```csharp
public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.InputPassword).NotEmpty().MinimumLength(8);

        RuleFor(x => x.PersonalData.Weight).GreaterThan(0).LessThan(500);
        RuleFor(x => x.PersonalData.Height).GreaterThan(0).LessThan(300);
        RuleFor(x => x.PersonalData.WeightTarget).GreaterThan(0);

        RuleFor(x => x.NutritionTargets)
            .NotEmpty()
            .WithMessage("At least one nutrition target is required");

        RuleForEach(x => x.NutritionTargets)
            .ChildRules(target =>
            {
                target.RuleFor(t => t.Calories).GreaterThan(0).LessThan(10000);
                target.RuleFor(t => t.WaterIntake).GreaterThanOrEqualTo(0);
            });
    }
}
```

**Why Medium Priority:**
- Prevents invalid data at API boundary
- Better error messages for users
- Current validation is done in event handler (too late)

---

## 🔵 Low Priority (Future Enhancements)

### 9. Implement TDEE Calculator
**Status:** 💭 Nice to Have

**What to do:**
Create calculator service for automatic target calculation:
```csharp
public class TDEECalculator
{
    public decimal CalculateTDEE(decimal weight, decimal height, int age,
                                  Gender gender, ActivityLevel activity)
    {
        // Mifflin-St Jeor Equation
        decimal bmr = gender == Gender.Male
            ? (10 * weight) + (6.25m * height) - (5 * age) + 5
            : (10 * weight) + (6.25m * height) - (5 * age) - 161;

        var multiplier = activity switch
        {
            ActivityLevel.Sedentary => 1.2m,
            ActivityLevel.Light => 1.375m,
            ActivityLevel.Moderate => 1.55m,
            ActivityLevel.Active => 1.725m,
            ActivityLevel.VeryActive => 1.9m,
            _ => 1.2m
        };

        return bmr * multiplier;
    }
}
```

**Why Low Priority:**
- Frontend can calculate this and send targets
- Backend can be stateless
- Nice to have for mobile apps or onboarding flow

---

### 10. Add Integration Tests
**Status:** ❌ Not Started

**What to do:**
Test complete flows:
1. Signup → DailyLog creation → Nutrition targets
2. Image analysis → FoodEntry creation → Daily log update
3. Add food → Verify nutrition totals
4. Update targets → Verify future DailyLogs use new targets

**Why Low Priority:**
- Current focus is getting features working
- Tests can be added after core functionality is stable

---

## 📝 Notes

### Architecture Decisions Made:
1. ✅ DailyLogs pre-created for 90 days at signup
2. ✅ Nutrition targets can combine to cover all days (weekday/weekend split allowed)
3. ✅ Nullable `FoodEntry.FoodId` for AI-analyzed foods
4. ✅ Separate `FoodEntrySource` enum to track origin

### Questions to Resolve:
- [ ] Should updating nutrition targets affect historical DailyLogs?
  - **Recommendation:** No - only future logs
- [ ] Which meal should AI-analyzed foods default to?
  - **Current:** Snack
  - **Consider:** Add Unassigned meal type
- [ ] How far in advance should DailyLogs be created?
  - **Current:** 90 days
  - **Consider:** Making this configurable

---

## Tracking Progress

**Last Updated:** 2025-11-26

**Completed:**
- ✅ SignUp creates nutrition targets
- ✅ Validation for all days covered
- ✅ 90-day DailyLog pre-creation
- ✅ ImageAnalyze entity and command

**In Progress:**
- 🔄 ImageAnalyzeCreatedEventHandler implementation

**Blocked:**
- None

---

_This file is maintained as a living document. Update as tasks are completed or priorities change._