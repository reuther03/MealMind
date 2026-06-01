# MealMind — Inwentarz braków

Dokument zbiorczy z analizy stanu projektu. Pogrupowany per obszar, z priorytetami:
- **P0** — bez tego aplikacja jest niefunkcjonalna end-to-end
- **P1** — znaczna dziura, ale można udawać że nie ma
- **P2** — polish, nice-to-have

---

## 1. Per-moduł — kompletność funkcjonalna

### Identity
- **[P0] Brak GET `/identity/me`** — po loginie nie da się pobrać aktualnego usera (tier, email). `AuthState` na frontendzie nie ma się z czego synchronizować.
- **[P1] Brak GET subscription status** — Stripe webhooks aktualizują DB, ale nikt nie czyta.
- **[P1] Brak edycji profilu** (email, hasło).
- **[P1] Brak `DeleteAccountCommand`** — RODO, kasacja konta + danych z innych modułów.
- **[P1] Brak reset hasła** (`ForgotPasswordCommand`/`ResetPasswordCommand`).
- **[P1] `Subscription`** ma `CreateFreeTier()` ale `Standard`/`Premium` tworzone ad-hoc w komendach — niespójność.

### Nutrition
- **[P0] Brak GET listy custom foods użytkownika** (`GetCustomFoodsByUserQuery`).
- **[P0] Brak GET listy favorite foods/meals** — `Add...` istnieją, nikt nie czyta.
- **[P0] Brak GET DailyLog na zakres dat** (week/month) — tylko per dzień. Bez tego nie ma raportu/historii.
- **[P1] Brak GET Food po Id** (`GetFoodByIdQuery`).
- **[P1] Brak GET UserProfile summary** (profil + targety).
- **[P1] Brak `RemoveFoodEntryCommand`** — usuwanie wpisu z dnia.
- **[P1] Brak `UpdateFoodEntryCommand`** — zmiana gramatury.
- **[P1] Brak `DeleteCustomFoodCommand`**.
- **[P1] Brak `RemoveFavoriteFood/MealCommand`**.

> *Uwaga: `NutritionTarget` jest immutable z założenia (value object) — update zwraca nową instancję przez `with`-expression. To zgodne z konwencją projektu, nie bug.*

### Training
- **[P0] Brak GET TrainingPlan (single + list)** — tworzysz plan, nie da się go odczytać.
- **[P0] Brak GET TrainingSession ze szczegółami** (`GetTrainingSessionQuery` nie istnieje).
- **[P0] Brak GET historii sesji**.
- **[P0] Brak walidacji w VO** — `ExerciseSet` (ujemne reps/weight), `CardioDetails` (ujemne wartości), `StrengthDetails` (pusta lista + publicznie mutowalna `Sets`).
- **[P0] `StartTrainingSessionCommand` z kopiowaniem ćwiczeń z poprzedniej zakończonej sesji** — zaprojektowane, jeszcze nie zbudowane.
- **[P0] `TrainingPlan.Activate()/Deactivate()`** — `IsActive` istnieje, brak metod domenowych.
- **[P1] Brak endpointu dla `SetAsStarted`/`SetAsEnded`** — domena ma, API nie eksponuje.
- **[P1] Brak `DeleteTrainingPlanCommand`**.
- **[P1] Brak `RemoveExerciseFromSessionCommand`** i edycji sesji.
- **[P1] Brak GET pojedynczego `Exercise` endpointu** (`GetExerciseQuery` jest w kodzie, endpointu brak — tylko `GetExercisesEndpoint` listujący).

### AiChat
- **[P1] Brak GET pełnej historii konwersacji** — `GetConversationDetailsEndpoint` istnieje, ale niejasne czy serwuje cały message log.
- **[P1] Brak `DeleteConversationCommand`** + `UpdateConversationTitleCommand`.
- **[P1] Brak GET listy dokumentów w konwersacji**.

---

## 2. Cross-module integration
- **[P1] Brak `TrainingSummaryService`** — Nutrition ma analog karmiący AiChat (`NutritionSummaryService` → `AnalyzeNutritionCommand`), Training nie ma. Bez tego AI nie może analizować treningów.
- **[P1] Brak integration testów flow** (np. SignUp → tworzy UserProfile w Nutrition).
- **[P1] Brak ILogger w event handlerach** — debugging w razie awarii outboxa = ciemność.
- **[P2] Brak dead-letter / metryk outboxa**.

---

## 3. Frontend (Blazor WASM)
- **[P0] Brak globalnego auth guard / redirect** — `AuthState` jest, ale nic nie przekierowuje niezalogowanego na `/login`. Brak `AuthorizeView` w layoutach. Brak refresh tokenów.
- **[P0] Zero stron Training** — żadnej.
- **[P0] Brak onboardingu** — po rejestracji user trafia w pustkę; nie ma flow „ustaw profil → ustaw cel kaloryczny".
- **[P1] Nutrition strony niekompletne** — brak edycji wagi w UI, brak delete entry, brak listy ulubionych, brak historii.
- **[P1] Brak stron subscription management** (zmiana planu, billing history).
- **[P1] Brak globalnego error boundary / toast notifications**.
- **[P1] Brak strony settings / „Moje konto"**.

---

## 4. Testy
- **Training module: 0 testów.** Żaden handler, żaden VO.
- **[P1] Brak integration testów** (cross-module event flow).
- **[P2] Brak testów VO** (`ExerciseSet`, `CardioDetails`, `StrengthDetails`).
- **[P2] Brak e2e** (Playwright/Selenium).

---

## 5. Cross-cutting (jakość)
- **[P0] Bug: `"TaskManager.Modules."`** w `src/Bootstrapper/MealMind.Bootstrapper/ModuleLoader.cs:10` i `src/Shared/MealMind.Shared.Infrastructure/Extensions.cs:107` — copy-paste z innego projektu, do zmiany na `"MealMind.Modules."`.
- **[P0] Brak globalnego exception middleware** — `DomainException` leci jako 500 bez mapowania na HTTP status.
- **[P1] Brak żadnego FluentValidation validatora** — pakiet jest w `.csproj`, ale walidacja siedzi ręcznie po handlerach.
- **[P1] ILogger brak w wielu handlerach** (`AddFoodEntryCommand`, `AnalyzeNutritionCommand`, `UpdateSubscriptionTierCommand`).
- **[P1] Brak ról / autoryzacji opartej na claimach** — wszystko binarnie „zalogowany/nie".
- **[P1] Brak health check** (`GET /health`) i healthcheck dla API w docker-compose.
- **[P1] Brak `appsettings.Production.json`** — secrets hardcoded w appsettings.json (JWT, Stripe).
- **[P2] Brak distributed tracing / metryk**.

---

## 6. Deployment / DevOps
- **[P1] docker-compose bez frontendu** — brak nginx/reverse proxy serwującego WASM + proxy `/api`.
- **[P1] Brak healthcheck dla API** w compose (Postgres ma).
- **[P1] README niekompletny** — brak „getting started" z kluczami (Stripe test, Gemini), troubleshootingiem (Ollama models).
- **[P2] Brak `.github/workflows`** — żadnego CI.
- **[P2] Brak seed scriptu** (default exercises, food bootstrap).

---

## 7. UX / featurowe braki
- **[P0] Brak onboardingu** (omówione wyżej).
- **[P0] Brak jakiegokolwiek UI Training**.
- **[P1] Brak wykresów / weekly summary view**.
- **[P1] Brak barcode scannera w UI** (backend `GetByBarcode` jest).
- **[P1] Brak rest timera w session UI**.
- **[P1] Brak progress trackingu** („ostatnio: 3x8@60kg, target: 3x8@65kg").
- **[P1] Brak eksportu danych** (CSV/PDF).
- **[P2] Brak push/email notyfikacji**.

---

## 8. Brakująca logika domeny (poza tym co wyżej)
- **[P1] `DailyLog.RemoveMeal()` / `Meal.RemoveFood()`** — brak operacji usuwania.
- **[P1] `IdentityUser.ChangeEmail()` / `ChangePassword()`**.

---

## Sugerowana kolejność prac (MVP w ~2 tygodnie)

1. **Naprawić bugi infrastruktury** (1 dzień): magic string `"TaskManager.Modules."` × 2, globalny exception middleware, ILogger we wszystkich handlerach.
2. **Identity domknąć** (1-2 dni): GET `/me`, change password, delete account.
3. **Nutrition read-side + edycja** (2 dni): GET zakresu DailyLogów, GET ulubionych/custom foods, DELETE/UPDATE FoodEntry.
4. **Training domknąć** (3 dni): walidacje VO, `StartTrainingSessionCommand` z kopiowaniem, endpointy start/end, GET planów + sesji + historii, delete planu.
5. **Frontend — luki krytyczne** (2-3 dni): auth guard + redirect, onboarding, strony Training, edycja w Nutrition.
6. **TrainingSummaryService + integracja z AiChat** (1 dzień).
7. **Testy Training + integration testy flow** (1-2 dni).
8. **Polish deployment** (1 dzień): healthcheck API, frontend w docker-compose, README.

---

**Statystyka braków:** ~10× P0, ~30× P1, ~10× P2. P0 + krytyczne luki frontendu = ścieżka MVP. Reszta iteracyjnie.
