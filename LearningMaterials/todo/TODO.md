# MealMind Development TODO

This file tracks important refactoring and feature tasks that need to be completed.

**Last Updated:** 2025-11-27

---

## ✅ Recently Completed

- [x] **ImageAnalyzeCreatedEventHandler** - Implemented and working
- [x] **AddFoodCommand refactored** - Removed DailyLog creation, using Result pattern
- [x] **QuantityInGrams type** - Changed to decimal (was double)
- [x] **Quantity validation** - Added in AddFoodCommand (line 54-55)
- [x] **SignUp endpoint documentation** - Updated with nutrition targets example
- [x] **GetCaloriesFromImage endpoint documentation** - Updated with all parameters
- [x] **EstimationMode simplified** - Single mode for all nutrients instead of per-nutrient

---

## 🔴 Critical Priority (Before Deploy)

### 1. Implement Background Job for Future DailyLogs
**Status:** ❌ Not Started
**Priority:** CRITICAL (system breaks after 90 days!)
**Estimated Time:** 2 hours

**Current State:**
- DailyLogs are pre-created for 90 days at signup
- No mechanism to extend the window as time passes
- After 90 days, users will have no DailyLogs!

**What to do:**
1. Create `DailyLogMaintenanceJob` as a BackgroundService
2. Run daily
3. For each user, ensure next 90 days have DailyLogs created
4. Create any missing dates

**Location:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure/BackgroundJobs/DailyLogMaintenanceJob.cs`

**See WEEK-1-TASKS.md for full implementation**

---

### 2. Add Logging to ImageAnalyzeCreatedEventHandler
**Status:** ❌ Not Started
**Priority:** HIGH
**Estimated Time:** 20 minutes

**Current Issue:**
- No logging in event handler
- Throws exception on error (should gracefully handle)
- Uses first meal instead of Snack meal

**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Events/Integration/ImageAnalyzeCreatedEventHandler.cs`

**What to do:**
1. Add `ILogger<ImageAnalyzeCreatedEventHandler>`
2. Replace `throw` with `return` + error logging
3. Change `dailyLog.Meals[0]` to `dailyLog.Meals.FirstOrDefault(m => m.MealType == MealType.Snack)`

---

## 🟡 High Priority (Do Soon)

### 3. Refactor AddNutritionTargetCommand to Update/PATCH
**Status:** ❌ Not Started
**Estimated Time:** 1-2 hours

**Current Issue:**
- Command is called "Add" but nutrition targets are set at signup
- Should be "Update" or use PATCH semantics

**What to do:**
1. Rename to `UpdateNutritionTargetCommand`
2. Change endpoint from POST to PATCH
3. Allow partial updates
4. Only update future DailyLogs (not historical)

---

### 4. Remove Unused CurrentWeight Parameter
**Status:** ❌ Not Started
**Estimated Time:** 15 minutes

**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Features/Commands/AddFoodCommand/AddFoodCommand.cs`

**Current Issue:**
Line 13: `decimal CurrentWeight` is accepted but never used in the handler.

**Options:**
- A) Remove the parameter (if not needed)
- B) Use it to update user's weight or DailyLog weight

---

### 5. Add Null Safety to IdentityUserCreatedEventHandler
**Status:** ❌ Not Started
**Estimated Time:** 15 minutes

**File:** `src/Modules/Nutrition/MealMind.Modules.Nutrition.Application/Events/Integration/IdentityUserCreatedEventHandler.cs`

**Current Issue (Line ~86):**
```csharp
var calorieTarget = userProfile.NutritionTargets
    .FirstOrDefault(...)!.Calories;  // ⚠️ Null-forgiving operator
```

**Fix:**
```csharp
var matchingTarget = userProfile.NutritionTargets
    .FirstOrDefault(x => x.ActiveDays.Any(z => z.DayOfWeek == logDate.DayOfWeek));

var calorieTarget = matchingTarget?.Calories
    ?? userProfile.NutritionTargets.First().Calories;
```

---

## 🟢 Medium Priority (Nice to Have)

### 6. Add FluentValidation to Commands
**Status:** ❌ Not Started
**Estimated Time:** 3 hours

**Commands that need validators:**
- [ ] `SignUpCommand`
- [ ] `AddFoodCommand`
- [ ] `GetCaloriesFromImageCommand`
- [ ] `UpdateNutritionTargetCommand`

---

### 7. Add MealType Parameter to Image Analysis
**Status:** 💭 Design Decision
**Estimated Time:** 30 minutes

**Current:** AI-analyzed foods go to first meal (should be Snack)
**TODO comment in code:** "currently adding to the first meal, later we can improve this by allowing users to specify meal type"

**Options:**
1. Keep Snack as default (simplest)
2. Add optional `MealType` parameter to `GetCaloriesFromImageCommand`
3. Add `Unassigned` MealType enum value

---

### 8. Convert Remaining Validator.ValidateNotNull to Result Pattern
**Status:** 🔄 Partially Done
**Estimated Time:** 2 hours

**Files still using exceptions:**
- `GetChatResponseCommand.cs` (lines 51, 54)
- `CreateConversationCommand.cs` (line 48)
- `AddNutritionTargetCommand.cs` (line 36)
- Stripe commands (various)
- Event handlers (various)

**Already fixed:**
- ✅ `AddFoodCommand` - Uses Result pattern
- ✅ `GetCaloriesFromImageCommand` - Uses Result pattern

---

## 🔵 Low Priority (Future)

### 9. Implement TDEE Calculator Service
**Status:** 💭 Nice to Have

Calculate nutrition targets automatically based on user's:
- Weight, height, age, gender
- Activity level
- Fitness goal (weight loss/gain/maintain)

---

### 10. Add Integration Tests
**Status:** ❌ Not Started

**Test flows:**
1. Signup → DailyLog creation → Nutrition targets
2. Image analysis → FoodEntry creation → Daily log update
3. Add food → Verify nutrition totals

---

### 11. Image Storage Optimization
**Status:** 💭 Future Enhancement

**Current:** Images stored as byte arrays in database
**Better:** Store in cloud (Azure Blob, S3) and save URL in database

---

## 📝 Architecture Decisions Made

1. ✅ DailyLogs pre-created for 90 days at signup
2. ✅ Nutrition targets can combine to cover all days (weekday/weekend split)
3. ✅ Nullable `FoodEntry.FoodId` for AI-analyzed foods
4. ✅ Separate `FoodEntrySource` enum to track origin (Database vs ImageAnalysis)
5. ✅ Single `EstimationMode` for all nutrients (simplified from per-nutrient)
6. ✅ Integration events for cross-module communication (not HTTP calls)

---

## 📊 Progress Summary

| Category | Total | Done | Remaining |
|----------|-------|------|-----------|
| Critical | 2 | 0 | 2 |
| High | 3 | 0 | 3 |
| Medium | 3 | 0 | 3 |
| Low | 3 | 0 | 3 |

**Next Action:** Implement Background Job for DailyLogs (Critical #1)

---

_See WEEK-1-TASKS.md for day-by-day implementation guide._
_See PRODUCTION-READY-CHECKLIST.md for full deployment roadmap._
