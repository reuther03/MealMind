# MealMind Frontend Implementation Plan

> **You've already built the hard part - the backend. The frontend is just connecting the dots.**

## Current Progress
- [x] Project setup (Blazor WebAssembly)
- [x] Login page
- [x] Register page
- [x] Basic layout
- [x] API client infrastructure
- [x] Auth state management

---

## Phase 1: Core Dashboard
*Goal: See your daily nutrition at a glance*

### Task 1.1: Daily Log Dashboard
- [ ] Create `Pages/Dashboard.razor`
- [ ] Call `GET /dailylog?dateOnly={date}`
- [ ] Display today's date with prev/next navigation
- [ ] Show calories consumed vs target (simple text for now)
- [ ] Show macros breakdown (protein, carbs, fats)

### Task 1.2: Meal Sections
- [ ] Create `Components/MealSection.razor`
- [ ] Display 4 sections: Breakfast, Lunch, Dinner, Snack
- [ ] List food entries in each meal
- [ ] Show per-meal calorie totals

**Endpoints used:** `GET /dailylog`

---

## Phase 2: Food Search & Logging
*Goal: Add food to your daily log*

### Task 2.1: Food Search Component
- [ ] Create `Components/FoodSearch.razor`
- [ ] Input field with search button
- [ ] Call `GET /food/search?SearchTerm={term}`
- [ ] Display results in a list (name, calories, brand)
- [ ] Pagination (load more button)

### Task 2.2: Add Food Modal
- [ ] Create `Components/AddFoodModal.razor`
- [ ] Select meal type (Breakfast/Lunch/Dinner/Snack)
- [ ] Input quantity in grams
- [ ] Call `POST /dailylog/food/add`
- [ ] Close modal and refresh dashboard

### Task 2.3: Quick Add Button
- [ ] Add "+" button to each meal section
- [ ] Opens food search modal
- [ ] Pre-selects meal type based on which section clicked

**Endpoints used:** `GET /food/search`, `POST /dailylog/food/add`

---

## Phase 3: Weight Tracking
*Goal: Track daily weight*

### Task 3.1: Weight Input on Dashboard
- [ ] Add weight input field on dashboard
- [ ] Show current logged weight for the day
- [ ] Call `PATCH /daily-log/user-weight` on change
- [ ] Show success feedback

### Task 3.2: Weight History (Optional)
- [ ] Create simple weight chart/list
- [ ] Fetch last 7-30 days of daily logs
- [ ] Display weight trend

**Endpoints used:** `PATCH /daily-log/user-weight`, `GET /dailylog`

---

## Phase 4: Custom Foods
*Goal: Add your own foods to the database*

### Task 4.1: Add Custom Food Page
- [ ] Create `Pages/AddFood.razor`
- [ ] Form fields: name, calories, protein, carbs, fats per 100g
- [ ] Optional: barcode, brand, image URL
- [ ] Call `POST /food`
- [ ] Redirect to dashboard on success

### Task 4.2: Favorites
- [ ] Add heart/star icon on food search results
- [ ] Call `POST /add-favorite-food` on click
- [ ] Show favorites tab in food search

**Endpoints used:** `POST /food`, `POST /add-favorite-food`

---

## Phase 5: AI Features
*Goal: Chat with AI and scan food photos*

### Task 5.1: AI Chat Page
- [ ] Create `Pages/AiChat.razor`
- [ ] Chat message list (user + AI messages)
- [ ] Input field for new messages
- [ ] Call `POST /create-chat-response` for new conversation
- [ ] Call `POST /get-chat-response` for follow-ups
- [ ] Store conversationId in component state

### Task 5.2: Image Calorie Scanner
- [ ] Create `Pages/ScanFood.razor`
- [ ] File upload for food photo
- [ ] Call `POST /get-calories-from-image` (multipart/form-data)
- [ ] Display detected foods with calorie estimates
- [ ] Option to add detected foods to daily log

**Endpoints used:** `POST /create-chat-response`, `POST /get-chat-response`, `POST /get-calories-from-image`

---

## Phase 6: Settings & Subscription
*Goal: User settings and premium features*

### Task 6.1: Nutrition Targets Page
- [ ] Create `Pages/Settings/NutritionTargets.razor`
- [ ] Form for calories, macros (grams OR percentage)
- [ ] Active days checkboxes
- [ ] Call `PUT /nutrition-target`

### Task 6.2: Subscription Page
- [ ] Create `Pages/Settings/Subscription.razor`
- [ ] Show current tier (Free/Standard/Premium)
- [ ] Upgrade buttons for each tier
- [ ] Call `POST /session-checkout`
- [ ] Redirect to Stripe checkout URL

**Endpoints used:** `PUT /nutrition-target`, `POST /session-checkout`

---

## Phase 7: Polish
*Goal: Make it feel complete*

### Task 7.1: Navigation
- [ ] Sidebar or bottom nav with icons
- [ ] Links: Dashboard, Search, AI Chat, Scan, Settings
- [ ] Highlight active page

### Task 7.2: Loading States
- [ ] Add loading spinners to all API calls
- [ ] Skeleton loaders for dashboard

### Task 7.3: Error Handling
- [ ] Toast notifications for errors
- [ ] Retry buttons where appropriate

### Task 7.4: Responsive Design
- [ ] Mobile-friendly layouts
- [ ] Touch-friendly buttons

---

## File Structure (Target)

```
client/MealMind.Client/
├── Presentation/
│   ├── Pages/
│   │   ├── Dashboard.razor          # Phase 1
│   │   ├── AiChat.razor             # Phase 5
│   │   ├── ScanFood.razor           # Phase 5
│   │   ├── AddFood.razor            # Phase 4
│   │   ├── Settings/
│   │   │   ├── NutritionTargets.razor  # Phase 6
│   │   │   └── Subscription.razor      # Phase 6
│   │   ├── Login.razor              # DONE
│   │   ├── Register.razor           # DONE
│   │   └── Home.razor               # DONE
│   ├── Components/
│   │   ├── MealSection.razor        # Phase 1
│   │   ├── FoodSearch.razor         # Phase 2
│   │   ├── AddFoodModal.razor       # Phase 2
│   │   ├── ChatMessage.razor        # Phase 5
│   │   └── LoadingSpinner.razor     # Phase 7
│   └── Layout/
│       └── MainLayout.razor         # DONE (update in Phase 7)
```

---

## Quick Wins (Do These When Motivation is Low)

These take 15-30 minutes and give instant satisfaction:

1. [ ] Add a logout button
2. [ ] Show username in header
3. [ ] Add "Today" button on dashboard date picker
4. [ ] Color-code calories (green=under, red=over target)
5. [ ] Add meal type icons (coffee for breakfast, etc.)
6. [ ] Format numbers nicely (1,500 kcal instead of 1500)
7. [ ] Add placeholder text to empty meal sections

---

## API Client Methods to Add

```csharp
// Add these to IApiClient/ApiClient as you need them:

// Phase 1
Task<DailyLogResponse> GetDailyLog(DateOnly date);

// Phase 2
Task<FoodSearchResponse> SearchFood(string term, int page = 1);
Task<Guid> AddFoodEntry(AddFoodEntryRequest request);

// Phase 3
Task<bool> UpdateWeight(DateOnly date, double weight);

// Phase 4
Task<Guid> AddCustomFood(AddCustomFoodRequest request);
Task AddFavoriteFood(Guid foodId);

// Phase 5
Task<ChatResponse> CreateConversation(string prompt);
Task<ChatResponse> SendMessage(Guid conversationId, string prompt);
Task<CalorieAnalysisResponse> AnalyzeFoodImage(Stream image, Guid sessionId);

// Phase 6
Task<bool> UpdateNutritionTarget(NutritionTargetRequest request);
Task<string> CreateCheckoutSession(int tier);
```

---

## API Endpoints Reference

### Identity Module (6 endpoints)
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/sign-up` | No | Register new user |
| POST | `/sign-in` | No | Login, returns JWT |
| POST | `/session-checkout` | Yes | Create Stripe checkout |
| GET | `/payment-cancel` | No | Stripe cancel redirect |
| GET | `/payment-success` | No | Stripe success redirect |
| POST | `/webhook/stripe` | No | Stripe webhooks |

### Nutrition Module (8 endpoints)
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/dailylog` | No | Get daily log by date |
| GET | `/food/search` | No | Search foods |
| POST | `/food` | Yes | Add custom food |
| POST | `/add-favorite-food` | Yes | Favorite a food |
| POST | `/add-favorite-meal` | Yes | Favorite a meal |
| POST | `/dailylog/food/add` | Yes | Add food to daily log |
| PATCH | `/daily-log/user-weight` | Yes | Update weight |
| PUT | `/nutrition-target` | Yes | Update nutrition target |

### AI Chat Module (3 endpoints)
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/create-chat-response` | Yes | Start new conversation |
| POST | `/get-chat-response` | Yes | Continue conversation |
| POST | `/get-calories-from-image` | Yes | Analyze food image |

---

## Remember

- **One task at a time.** Don't look at the whole list.
- **Commit after each task.** Small wins matter.
- **It doesn't have to be pretty first.** Make it work, then make it nice.
- **Skip phases if stuck.** Phase 5 sounds fun? Do it. Come back to Phase 3 later.
- **The backend is done.** You're just building forms and lists now.

---

## Progress Tracker

| Phase | Status | Tasks Done |
|-------|--------|------------|
| 1. Dashboard | Not Started | 0/2 |
| 2. Food Logging | Not Started | 0/3 |
| 3. Weight | Not Started | 0/2 |
| 4. Custom Foods | Not Started | 0/2 |
| 5. AI Features | Not Started | 0/2 |
| 6. Settings | Not Started | 0/2 |
| 7. Polish | Not Started | 0/4 |

**Total: 0/17 tasks complete**
