# Food Vision AI Feature - Planning & Ideas

## Overview
Implement a feature that allows users to upload food images and automatically estimate calories/nutrition. The vision model analyzes the image and optionally uses user descriptions to provide more accurate estimates.

## Core Concept

**Flow:**
1. User uploads food image (+ optional text description)
2. Vision model analyzes image → detects foods, quantities, portions
3. System cross-references with OpenFoodFacts database
4. System estimates weight based on visual cues or standard portions
5. User reviews/confirms estimation
6. Creates FoodEntry automatically

## Technology Choices

### Vision Model Options
- **Ollama with LLaVA** (Free, local)
  - LLaVA 1.6 (7B) - Faster, decent accuracy
  - LLaVA 1.6 (13B) - Better accuracy, slower
  - BakLLaVA - May be specialized for food detection
- **Pros:** Free, works offline, already using Ollama
- **Cons:** Slower, less accurate than paid APIs, needs GPU
- **Decision:** Start with free Ollama models, upgrade to paid APIs later if needed

### Database Integration
- **Approach:** Database First with Model Fallback
  1. Search OpenFoodFacts DB for detected foods
  2. If found → use accurate nutrition data per 100g
  3. If not found → fallback to model's nutrition knowledge
  4. **Rationale:** Real product data is more accurate

## Vision Model Output Structure

### Basic Detection
```json
{
  "detectedFoods": [
    {
      "name": "egg",
      "quantity": 4,
      "confidence": 0.95,
      "estimatedWeight": 200,
      "weightRange": { "min": 190, "max": 230 }
    },
    {
      "name": "bread",
      "quantity": 2,
      "confidence": 0.87,
      "estimatedWeight": 60,
      "weightRange": { "min": 50, "max": 70 }
    }
  ],
  "userDescription": "scrambled eggs with toast",
  "totalConfidence": 0.91
}
```

## Confidence Scoring System

### What is Confidence?
- **Probability measure (0.0 to 1.0)** indicating how certain the model is
- `0.95` = 95% sure, `0.50` = guessing, `0.20` = very unsure

### How to Use Confidence

#### 1. Threshold Filtering
- **High Confidence (≥ 0.85):** Auto-suggest with confirmation
- **Medium Confidence (0.60-0.84):** Show but require explicit confirmation
- **Low Confidence (< 0.60):** Don't show or mark as "uncertain"

#### 2. User Experience Based on Confidence

**High Confidence (≥ 0.85):**
```
✓ "We detected 4 eggs (200g). Add to meal?"
[Confirm] [Adjust]
```

**Medium Confidence (0.60-0.84):**
```
⚠ "We think this might be 2 slices of bread. Is this correct?"
[Yes] [No, it's ___] [Skip]
```

**Low Confidence (< 0.60):**
```
❓ "We're not sure what this is. Can you tell us?"
[Manual input field]
```

#### 3. Weight Estimation Adjustment
- **High confidence (≥ 0.80):** ±15% weight range
- **Low confidence (< 0.80):** ±30% weight range

**Example:**
```csharp
// High confidence (0.95): 200g → range 170-230g (±15%)
// Low confidence (0.65): 200g → range 140-260g (±30%)
```

#### 4. Estimation Status
```csharp
public enum EstimationStatus
{
    ReadyToAdd,          // Avg confidence ≥ 0.85 - Auto-suggest
    NeedsConfirmation,   // Avg confidence 0.60-0.84 - Ask user
    ManualInputRequired  // Avg confidence < 0.60 - Too uncertain
}
```

#### 5. Future: Learning from Corrections
Store user corrections to improve future estimates:
```csharp
public class FoodEstimationFeedback
{
    public string ModelDetection { get; set; }     // "bread"
    public double ModelConfidence { get; set; }    // 0.72
    public string UserCorrection { get; set; }     // "croissant"
    public int ModelEstimatedWeight { get; set; }  // 100g
    public int UserActualWeight { get; set; }      // 150g
}
```

## Handling Edge Cases

### Edge Case 1: Ambiguous Portion Size (No User Input)

**Problem:** Model sees scrambled eggs but can't tell if it's 100g or 300g

**Solution:** Use visual cues + standard portions + ask user

```json
{
  "detectedFoods": [
    {
      "name": "scrambled eggs",
      "quantity": "unknown",
      "confidence": 0.88,
      "visualEstimate": {
        "portionSize": "medium", // small/medium/large
        "estimatedWeight": 150,
        "weightRange": { "min": 100, "max": 200 }
      }
    }
  ],
  "ambiguityReason": "Cannot count individual eggs when scrambled."
}
```

**User Response:**
```
📸 We detected: Scrambled Eggs

⚠️ We can't count individual eggs when scrambled.

Based on the portion size, we estimate:
🍳 ~150g (2-3 eggs)
📊 Range: 100-200g
🔥 ~240 kcal (160-320 kcal range)

How much did you actually use?
[2 eggs] [3 eggs] [4 eggs] [Custom: ___ grams]
```

**Visual Portion Estimation Strategy:**
- Analyze plate coverage percentage (0.0 to 1.0)
- Compare to reference objects (plate size, utensils)
- Map to standard portion sizes:
  - < 30% plate = small (~100g)
  - 30-60% plate = medium (~150g)
  - > 60% plate = large (~225g)

### Edge Case 2: User Provides Detailed Input

**User input:** "I used 4 eggs and 10 grams of butter"

**This is the BEST scenario!** User input overrides model detection:

```json
{
  "modelDetection": [
    { "name": "scrambled eggs", "confidence": 0.88, "estimatedWeight": 150 }
  ],
  "userInput": {
    "raw": "i used 4 eggs and i use 10 grams of butter",
    "parsed": [
      { "ingredient": "egg", "quantity": 4, "unit": "count" },
      { "ingredient": "butter", "quantity": 10, "unit": "grams" }
    ]
  },
  "finalEstimation": {
    "foods": [
      {
        "name": "egg",
        "weight": 200, // 4 × 50g standard
        "source": "user_input",
        "confidence": 1.0
      },
      {
        "name": "butter",
        "weight": 10,
        "source": "user_input",
        "confidence": 1.0
      }
    ]
  }
}
```

**User Experience:**
```
📸 Image Analysis + Your Input:

From your description:
✓ 4 eggs (200g, ~286 kcal) ✅ Confirmed
✓ 10g butter (10g, ~72 kcal) ✅ Confirmed

We also detected in the image:
❓ Salt/pepper (seasoning) - Add this? [Yes] [No]

Total: ~358 kcal
[Add to Meal] [Adjust]
```

## Data Priority System

**Priority Order (highest to lowest):**
1. **User Explicit Input** - User says "4 eggs" → Trust 100%
2. **Database Lookup** - Found exact product in OpenFoodFacts
3. **Vision High Confidence** - Model 85%+ sure
4. **Vision Medium Confidence** - Model 60-85% sure (ask confirmation)
5. **Standard Portion Fallback** - Generic estimate when uncertain

```csharp
public enum FoodSourcePriority
{
    UserExplicitInput = 1,
    DatabaseLookup = 2,
    VisionHighConfidence = 3,
    VisionMediumConfidence = 4,
    StandardPortionFallback = 5
}
```

## Weight Estimation Logic

### For Countable Items (eggs, slices, pieces)
```
Standard egg weight: 50g
User says "4 eggs" → 4 × 50g = 200g
```

### For Mixed/Scrambled Foods
- Use visual portion analysis
- Compare plate coverage to standard portions
- Provide range instead of exact number

### User Description Parsing (NLP)
Parse patterns like:
- "4 eggs" → 4 count of eggs
- "10 grams of butter" → 10g butter
- "200g chicken" → 200g chicken

Simple regex or use LLM for better natural language understanding.

## Architecture Design

### Module Placement
- **Nutrition Module** - This feature belongs here
- Reuse existing Ollama infrastructure from AiChat module

### New Components

#### 1. Command
```csharp
public class AnalyzeFoodImageCommand : ICommand<FoodEstimationResult>
{
    public IFormFile Image { get; set; }
    public string? UserDescription { get; set; }
}
```

#### 2. Service
```csharp
public interface IFoodVisionService
{
    Task<VisionAnalysisResult> AnalyzeImageAsync(Stream imageStream);
}
```

#### 3. Processing Flow
```csharp
public class Handler
{
    public async Task<FoodEstimationResult> Handle(AnalyzeFoodImageCommand request)
    {
        // 1. Vision model analyzes image
        var visionResult = await _visionService.AnalyzeAsync(request.Image);

        // 2. Parse user description if provided
        var userIngredients = request.UserDescription != null
            ? await _nlpService.ParseIngredientsAsync(request.UserDescription)
            : null;

        // 3. Merge: User input overrides model detection
        var estimation = MergeEstimations(visionResult, userIngredients);

        // 4. Lookup foods in database
        foreach (var food in estimation.Foods)
        {
            var dbFood = await _foodRepository.GetByNameAsync(food.Name);
            if (dbFood != null)
            {
                food.NutritionData = dbFood.NutritionPer100G;
                food.Source = FoodSource.Database;
            }
        }

        // 5. Calculate nutrition for estimated weights
        foreach (var food in estimation.Foods)
        {
            food.CalculatedNutrition = food.NutritionData
                .CalculateForGrams(food.EstimatedWeight);
        }

        return estimation;
    }
}
```

### User Flow

```
1. User uploads image (+ optional text)
   ↓
2. API analyzes image with vision model
   ↓
3. API returns estimation with confidence scores
   ↓
4. Frontend shows results:
   - High confidence items: Ready to add
   - Medium confidence: "Is this correct?"
   - Low confidence: Manual input
   ↓
5. User reviews and adjusts
   ↓
6. User confirms
   ↓
7. API creates FoodEntry (or multiple)
```

## User Input Options

### Option A: Free Text (Recommended)
- User types naturally: "4 eggs and 10g butter"
- NLP parses the text
- More user-friendly

### Option B: Structured Form
- Dropdowns/inputs for quantity + food
- More accurate parsing
- Less flexible

### Option C: Hybrid (Best)
- Free text input with parsing
- If parsing fails or uncertain → show structured form
- Best of both worlds

## Future Enhancements (Post-MVP)

1. **Multi-image support** - Upload multiple angles
2. **Reference object** - User includes coin/hand for scale
3. **Recipe detection** - "This is homemade lasagna" → parse recipe
4. **Historical learning** - Learn from user corrections over time
5. **Barcode scanning** - Complement vision with barcode lookup
6. **Meal context** - Time of day helps (breakfast → probably eggs/cereal)

## Next Steps (When Ready to Implement)

### Phase 1: Research & Testing
- [ ] Test Ollama LLaVA models with food images
- [ ] Evaluate accuracy of different model sizes (7B vs 13B)
- [ ] Test prompt engineering for better food detection
- [ ] Benchmark response times

### Phase 2: Core Implementation
- [ ] Create `AnalyzeFoodImageCommand` in Nutrition module
- [ ] Implement `IFoodVisionService` with Ollama integration
- [ ] Build NLP parser for user descriptions
- [ ] Create estimation merging logic (vision + user input)
- [ ] Implement confidence-based UX flow

### Phase 3: Database Integration
- [ ] Connect to OpenFoodFacts food repository
- [ ] Implement food name matching/fuzzy search
- [ ] Add standard portion size lookup table
- [ ] Build weight calculation logic

### Phase 4: API Endpoints
- [ ] POST `/nutrition/analyze-food-image` - Upload & analyze
- [ ] POST `/nutrition/confirm-food-estimation` - User confirms & creates entry
- [ ] GET `/nutrition/standard-portions` - Get standard portion sizes

### Phase 5: Frontend (Future)
- [ ] Image upload UI
- [ ] Estimation review/edit interface
- [ ] Confidence score visualization
- [ ] Quick adjustment controls

## Questions to Answer Later

1. **Storage:** Should we store uploaded images for future ML training?
2. **Rate limiting:** How many analyses per user per day?
3. **Image size limits:** Max file size and dimensions?
4. **Supported formats:** JPG, PNG, HEIC?
5. **Privacy:** How long to retain images? Delete immediately after analysis?
6. **Analytics:** Track which foods are commonly misidentified?

## Success Criteria

- **Accuracy:** 70%+ correct food identification (high confidence only)
- **Speed:** Analysis completes in < 5 seconds
- **User satisfaction:** Users confirm estimation 60%+ of the time without adjustments
- **Coverage:** Successfully handles 80%+ of common foods in OpenFoodFacts DB

---

**Status:** Planning phase - Complete Stripe integration first
**Priority:** Medium (nice-to-have feature, not core functionality)
**Estimated Effort:** 2-3 weeks implementation + 1 week testing/refinement
