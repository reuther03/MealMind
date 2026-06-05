# Przygotowanie do rozmowy — ENOLOGY (Programista .NET / C# + ERP)

> Rzeszów, ENOLOGY sp. z o.o. — enova365 + Comarch ERP Optima, .NET / C# / MSSQL / T-SQL / ASP.NET Web API.
> Stanowisko mid/regular w ofercie, ale skoro to Twoja pierwsza praca — celuj w **junior z ambicją mida**: pokaż że ogarniasz podstawy, masz projekt portfolio i nie boisz się dziedziny której nie znasz (ERP).

---

## 0. Jak nastawić się do tej rozmowy

**Klucz #1 — szczera transparentność co do ERP.**
Nigdy nie pracowałeś z enova365 ani Optima. Nie ukrywaj tego. Mów wprost:
> "Z systemami ERP nie miałem dotąd styczności, ale przygotowałem się do rozmowy — wiem mniej więcej do czego służą, jak są zbudowane (moduły: finanse, HR, magazyn, sprzedaż), i wiem że enova365 i Optima rozszerza się przez pluginy/dodatki w C# odwołujące się do API systemu. Liczę że to obszar w którym sporo nauczę się od zespołu."

To **mocniejsza odpowiedź** niż udawanie wiedzy. Rekruter szuka kogoś kto chce się uczyć ERP — sami to napisali ("rozwój w kierunku eksperta ERP").

**Klucz #2 — projekt jako dowód że umiesz pisać kod produkcyjnie podobny.**
MealMind ma: warstwy (Domain/Application/Infrastructure/Api), DI, EF Core, migracje, repozytoria, integracje (Stripe, Ollama), webhooki, value objects, modułowość. To **świetna baza do rozmowy** — wszystkie te pojęcia wracają w ERP.

**Klucz #3 — kotwica salary range 7–12k brutto.**
Skoro nigdy nie pracowałeś — celuj realnie 7–8k brutto. Nie zaniżaj się pod 7. Jak zapytają o oczekiwania, możesz powiedzieć: "7–8 tys. brutto, ale jestem otwarty na rozmowę zwłaszcza jeśli na początku jest większy nacisk na naukę systemu".

---

## 1. Podstawy C# / .NET (rzeczy które *muszą* wejść)

Junior bez tych odpowiedzi się nie obroni. Wszystkie używasz w MealMind — przyporządkuj sobie do konkretnego pliku.

### 1.1 Typy referencyjne vs wartościowe
- **Wartościowe** (`struct`, `int`, `bool`, `DateTime`, enum) — kopiowane przy przypisaniu, na stosie (zwykle).
- **Referencyjne** (`class`, `string`, tablice, delegaty) — przekazywane przez referencję, na stercie.
- Przykład w Twoim kodzie: `Subscription` to `record` (referencyjny ale z value equality), `Guid` to struct.

### 1.2 `class` vs `record` vs `struct`
- `class` — mutowalna domyślnie, equality po referencji.
- `record` — immutable-friendly (`init`), value equality, `with` expression do kopii ze zmianą.
- `struct` — value type, kopiowana, używana do małych niemutowalnych danych.
- W MealMind: `Subscription`, `Password`, `PersonalData` to `record` — wymuszają niemutowalność (Value Objects w DDD).

### 1.3 `string` vs `StringBuilder`
- `string` jest **immutable** — każda operacja `s += "x"` tworzy nowy obiekt.
- `StringBuilder` mutuje wewnętrzny bufor — używasz w pętli (>4-5 konkatenacji).

### 1.4 `null` i nullable reference types
- `string?` vs `string` — kompilator ostrzega gdy używasz `?` bez sprawdzenia.
- `??`, `?.`, `!` (null-forgiving — używaj sporadycznie, mówi kompilatorowi "wiem co robię").

### 1.5 `async` / `await` / `Task`
- **`async` nie znaczy "równolegle"** — znaczy "ten kod ma punkty zawieszenia". Wątek wraca do puli czekając na I/O.
- `Task` = obietnica wyniku. `await` rozpakowuje wynik kiedy będzie gotowy.
- **Nigdy nie używaj `.Result` ani `.Wait()` w kodzie aplikacji** — to deadlock w określonych kontekstach (ASP.NET classic; .NET Core jest bezpieczniejszy, ale i tak unikaj).
- `ConfigureAwait(false)` — w bibliotekach. W aplikacji web .NET Core zwykle niepotrzebne.

### 1.6 Dependency Injection
- Trzy lifetime'y:
  - **Singleton** — jedna instancja na całą aplikację (np. config readonly, IClock).
  - **Scoped** — jedna instancja per request HTTP (np. `DbContext`, repozytoria).
  - **Transient** — nowa za każdym razem (lekkie usługi).
- **Najczęstszy bug:** Scoped (np. DbContext) wstrzyknięty do Singletona → ten sam DbContext "uwięziony" na całe życie aplikacji. Zazwyczaj kompilator/DI wykryje przy `ValidateOnBuild`.
- W MealMind: spójrz na `*Module.cs` — tam rejestrujesz serwisy.

### 1.7 Interfejsy vs klasy abstrakcyjne
- Interfejs — kontrakt, brak stanu (od C# 8 można default implementation, ale rzadko używane).
- Klasa abstrakcyjna — kontrakt + wspólny kod/stan, dziedziczenie pojedyncze.
- W MealMind: `IRepository<T>` to interfejs, `Document` to klasa abstrakcyjna (RagDocument, ConversationDocument dziedziczą).

### 1.8 SOLID — przygotuj 1-zdaniowe definicje
- **S** Single Responsibility — klasa ma jeden powód do zmiany.
- **O** Open/Closed — otwarte na rozszerzenie (przez kompozycję/abstrakcję), zamknięte na modyfikację.
- **L** Liskov — podtyp musi być zamienialny z bazowym bez psucia logiki.
- **I** Interface Segregation — wiele małych interfejsów > jeden gruby.
- **D** Dependency Inversion — kod zależy od abstrakcji, nie od konkretów.

> W MealMind DI = D, command handlery = S (każdy handler robi jedno), repozytoria za interfejsami = D.

### 1.9 Co to LINQ i `IEnumerable` vs `IQueryable`
- `IEnumerable<T>` — kolekcja w pamięci, LINQ wykonuje się **lokalnie** (LINQ to Objects).
- `IQueryable<T>` — drzewo wyrażeń tłumaczone na SQL przez EF (LINQ to Entities).
- **Bug klasyczny:** `.ToList()` przed `.Where()` ściąga wszystko z bazy → filtruje w pamięci. Najpierw `.Where()`, potem `.ToList()`.

---

## 2. ASP.NET Web API (mają to w "Wymagane")

### 2.1 Czym jest Web API
- ASP.NET Core to framework do budowania aplikacji webowych. **Web API** = endpointy HTTP zwracające zwykle JSON (REST), bez widoków.
- W MealMind używasz **Minimal API** (`MapPost`, `MapGet`) — alternatywa dla kontrolerów. ENOLOGY może mieć w projektach kontrolery (atrybuty `[ApiController]`, `[HttpGet]`). Znaj **oba** podejścia.

### 2.2 Kontroler vs Minimal API — krótki kontrast
```csharp
// Kontroler
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) { ... }
}

// Minimal API (tak masz w MealMind)
app.MapGet("/users/{id}", async (Guid id, ISender sender) => ...);
```
- Minimal API → lżejsze, mniej ceremonii, dobre do mikro-serwisów.
- Kontrolery → więcej konwencji, łatwiej dodać atrybuty (`[Authorize]`, filtry), naturalne dla większych zespołów.

### 2.3 Cykl życia requestu HTTP
1. Kernel routingu rozpoznaje URL → endpoint.
2. **Middleware pipeline** (np. uwierzytelnianie, logowanie, exception handling, CORS).
3. Wywołanie handlera/akcji.
4. Serializacja odpowiedzi.
- Middleware konfigurujesz w `Program.cs` lub `Startup.cs` — kolejność ma znaczenie (Auth przed Endpointami).

### 2.4 Standardowe kody odpowiedzi
- `200 OK`, `201 Created`, `204 No Content`
- `400 Bad Request` (walidacja), `401 Unauthorized` (brak/zła autoryzacja), `403 Forbidden` (autoryzowany, ale brak praw), `404 Not Found`
- `500 Internal Server Error` (nieoczekiwany wyjątek)

### 2.5 Walidacja
- DataAnnotations (`[Required]`, `[StringLength]`) — proste.
- **FluentValidation** — chętnie używane w komercyjnych projektach .NET (oddzielne klasy walidatorów, łańcuchowe reguły).

### 2.6 Autoryzacja vs Autentykacja
- **Autentykacja** — *kim jesteś?* (login, token JWT).
- **Autoryzacja** — *co możesz?* (role, policy, claims).

### 2.7 CORS
- Cross-Origin Resource Sharing — przeglądarka blokuje fetch z innego origin niż API, chyba że API zwraca odpowiednie nagłówki. Konfigurujesz w `Program.cs`.

---

## 3. Entity Framework Core — odświeżenie (na MealMind)

EF Core to ORM Microsoftu — mapuje klasy C# na tabele SQL. **W ofercie jest MSSQL/T-SQL**, więc poza EF będzie też potrzebny raw SQL (punkt 4). EF nie zwalnia Cię od znajomości SQL — wręcz przeciwnie.

### 3.1 DbContext i DbSet
```csharp
public class NutritionDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder b) { ... }
}
```
- `DbSet<T>` = reprezentacja tabeli.
- `DbContext` to **Scoped** (jeden per request) — **nie używaj jako Singleton**.

### 3.2 Code-first vs Database-first
- Code-first — masz klasy, EF generuje schemat + migracje (Twoje podejście w MealMind).
- Database-first — istnieje baza, generujesz modele (`Scaffold-DbContext`). Częste w ERP-podobnych projektach gdzie baza jest "święta".

### 3.3 Migracje
```bash
dotnet ef migrations add NazwaMigracji --project ... --startup-project ...
dotnet ef database update
```
- Migracja = wygenerowany skrypt (`Up`/`Down`) + snapshot.
- Nigdy nie edytuj wygenerowanej migracji *po* jej zaaplikowaniu na środowisku — twórz kolejną.

### 3.4 Tracking vs No-tracking
- Domyślnie EF śledzi pobrane encje (na potrzeby `SaveChanges`).
- `AsNoTracking()` — read-only zapytania, **dużo szybsze**, brak Change Trackera.
- W projekcie produkcyjnym 80% queries powinno być No-tracking.

### 3.5 Eager / Lazy / Explicit loading
- **Eager:** `.Include(x => x.Orders)` — JOIN w jednym zapytaniu. Standardowe podejście.
- **Lazy:** automatyczne ładowanie przy dostępie do property. **W produkcji wyłączaj** — generuje N+1 zapytań.
- **Explicit:** `context.Entry(user).Collection(u => u.Orders).Load()` — ręczne dociąganie.

### 3.6 Problem N+1
**Najczęstszy bug wydajnościowy w EF.**
```csharp
var users = await ctx.Users.ToListAsync();
foreach (var u in users)
    Console.WriteLine(u.Orders.Count); // każde u.Orders = osobne SELECT
```
- Rozwiązanie: `Include` albo projekcja:
```csharp
var data = await ctx.Users
    .Select(u => new { u.Name, OrderCount = u.Orders.Count })
    .ToListAsync(); // jeden SELECT z subselectem
```

### 3.7 Projekcje (`Select`) zamiast pełnych encji
- Dla widoków/list — projektuj do DTO. Mniej kolumn, brak trackingu, mniej danych.

### 3.8 Transakcje
- Domyślnie `SaveChangesAsync` jest atomowy (jedna transakcja na call).
- Wieloetapowo: `using var tx = await ctx.Database.BeginTransactionAsync(); ... await tx.CommitAsync();`

### 3.9 Unit of Work + Repository (jak w MealMind)
- DbContext **już jest** Unit of Work + DbSet to repozytorium. Wzorce na wierzchu nie są obowiązkowe, ale w MealMind je masz dla rozdzielenia warstw (Domain nie zna EF). To ważny argument na rozmowie — pokazujesz świadomość architektoniczną.

### 3.10 Migracje konkurencyjne / współbieżność
- **Optimistic concurrency** — kolumna `RowVersion`/`Timestamp`, EF rzuca `DbUpdateConcurrencyException` jeśli rekord zmienił się między pobraniem a zapisem.

### 3.11 Pytania-pułapki na EF
- *"Czy `Find()` różni się od `FirstOrDefault(x => x.Id == id)`?"* → Tak, `Find` najpierw sprawdza change tracker w pamięci, nie zawsze idzie do bazy.
- *"Co zwróci `Where` na nieistniejących danych?"* → Pustą sekwencję, **nie null**.
- *"Po co `AsSplitQuery()`?"* → Gdy `Include` powoduje kartezjański join (mnoży wiersze) — `SplitQuery` rozbije na kilka zapytań i połączy w pamięci.

---

## 4. SQL i T-SQL — podstawy i optymalizacja

**To tu w ERP rozjeżdża się 90% wydajności.** Mają w wymaganiach **MSSQL i T-SQL** — przygotuj się solidnie.

### 4.1 Czego MUSISZ umieć napisać na żywo
```sql
-- INNER / LEFT / RIGHT / FULL JOIN — różnice
SELECT u.Name, COUNT(o.Id) AS OrderCount
FROM Users u
LEFT JOIN Orders o ON o.UserId = u.Id
GROUP BY u.Name
HAVING COUNT(o.Id) > 5
ORDER BY OrderCount DESC;

-- Podzapytanie vs CTE vs JOIN
WITH UserOrders AS (
    SELECT UserId, COUNT(*) AS Total
    FROM Orders
    GROUP BY UserId
)
SELECT u.Name, uo.Total
FROM Users u
JOIN UserOrders uo ON uo.UserId = u.Id
WHERE uo.Total > 10;

-- Okienkowe (window functions) — często padają na rozmowach
SELECT
    Id, UserId, Amount,
    ROW_NUMBER() OVER (PARTITION BY UserId ORDER BY CreatedAt DESC) AS RowNum,
    SUM(Amount) OVER (PARTITION BY UserId) AS UserTotal
FROM Orders;
```

### 4.2 WHERE vs HAVING
- `WHERE` — filtruje **przed** grupowaniem.
- `HAVING` — filtruje **po** grupowaniu (na zagregowanych wynikach).

### 4.3 Indeksy — podstawa optymalizacji
- **Clustered index** — fizyczne ułożenie wierszy w tabeli. **Tylko jeden** na tabelę. Domyślnie po PK.
- **Non-clustered** — osobna struktura (B-tree) wskazująca na wiersze. Wiele na tabelę.
- **Covering index** — zawiera wszystkie kolumny potrzebne do query (`INCLUDE`).
- Kiedy zakładać? Kolumny w `WHERE`, `JOIN`, `ORDER BY`. Selektywne (dużo różnych wartości).
- **Kiedy NIE?** Mała tabela, kolumna często aktualizowana, niska selektywność (np. bool).

### 4.4 Plan wykonania (Execution Plan)
- W SSMS: `Ctrl + M` (Include Actual Execution Plan) → wykonaj zapytanie.
- Szukasz: `Table Scan` / `Clustered Index Scan` (źle dla dużych tabel — pełny przegląd), `Index Seek` (dobrze — punktowe), `Key Lookup` (kosztowne — brak covering index).
- Jeśli widzisz "Estimated rows" mocno różne od "Actual rows" → statystyki są nieaktualne (`UPDATE STATISTICS`).

### 4.5 Typowe pułapki wydajnościowe
- `SELECT *` — ściąga wszystko, niszczy covering index. Zawsze listuj kolumny.
- Funkcje na kolumnie w `WHERE`: `WHERE YEAR(CreatedAt) = 2026` → blokuje użycie indeksu. Lepiej `WHERE CreatedAt >= '2026-01-01' AND CreatedAt < '2027-01-01'`.
- `LIKE '%foo%'` — leading wildcard = full scan.
- `OR` na różnych kolumnach — często nie korzysta z indeksów; rozważ `UNION ALL`.
- Implicit conversion — `WHERE VarCharCol = N'abc'` (nvarchar vs varchar) potrafi zabić plan.

### 4.6 Transakcje i poziomy izolacji
- `READ UNCOMMITTED` — dirty reads, najszybsze, niebezpieczne.
- `READ COMMITTED` — domyślny w MSSQL, brak dirty reads.
- `REPEATABLE READ` — to samo zapytanie zwraca to samo (blokuje update'y).
- `SERIALIZABLE` — pełna izolacja, najwolniejsze, ryzyko deadlocków.
- `SNAPSHOT` — wersjonowanie wierszy, brak blokad odczytu.

### 4.7 Deadlock vs Blocking
- **Blocking** — jedna transakcja czeka na drugą (normalne).
- **Deadlock** — dwie transakcje wzajemnie się blokują → SQL Server zabija jedną (deadlock victim).
- Rozwiązanie: krótkie transakcje, ten sam porządek zasobów, odpowiednie indeksy (mniej blokad).

### 4.8 T-SQL specyfiki
- `IDENTITY` / `SCOPE_IDENTITY()` — autoincrement.
- `MERGE` — upsert (uwaga: ma znane bugi, niektóre zespoły zakazują).
- Tabele tymczasowe `#temp` vs zmienne tabelowe `@var` — `#temp` ma statystyki, `@var` ich nie ma (małe sety = `@var`, większe = `#temp`).
- `TRY...CATCH` w T-SQL, `THROW`/`RAISERROR`.
- Procedury składowane (stored procedures) — kompilowane raz, pre-cached plan. **W ERP bardzo częste**.

### 4.9 Stored Procedures vs zapytania z aplikacji
- ERP-y często mają **dużo logiki w SP** (historycznie wydajniejsze, łatwiej zmienić bez deployu kodu).
- Nowoczesne podejście — logika w aplikacji, EF generuje SQL. Bądź gotów rozmawiać o obu — bez fanatyzmu.

### 4.10 Co konkretnie powtórzyć przed rozmową
1. Napisz na kartce JOIN typów (Inner, Left, Right, Cross) i co zwracają.
2. Wymyśl 3 scenariusze N+1 i jak je naprawić.
3. Wytłumacz `GROUP BY` + agregacje (SUM/COUNT/AVG/MIN/MAX) + `HAVING`.
4. Co to indeks i kiedy go zakładać.
5. Czym różni się `WHERE` od `JOIN ... ON` w przypadku `LEFT JOIN` (filtrowanie po stronie prawej tabeli kasuje "leftowość").

---

## 5. Obrona projektu MealMind

Pewnie zapytają: *"Opowiedz nam o swoim projekcie."* — to **Twoja przewaga**. Przygotuj 2-3 minutową narrację + odpowiedzi na drążące pytania.

### 5.1 Pitch (2 minuty)
> MealMind to modularny monolit w .NET 9, który buduję od kilku miesięcy jako projekt portfolio. Pomaga użytkownikom śledzić odżywianie i trening — z dwoma głównymi feature'ami: subskrypcje przez Stripe i chat AI z RAG na lokalnym Ollama. Architektonicznie: każdy moduł (Identity, Nutrition, AiChat, Training) ma własne warstwy Api/Application/Domain/Infrastructure, własny DbContext na PostgreSQL, komunikuje się z innymi przez integration events. Używam CQRS przez MediatR, repozytoriów, value objectów. Najwięcej nauczyłem się o DDD, pisaniu testów (TUnit + Moq) i o tym jak komunikować moduły bez tight couplingu.

### 5.2 Pytania które mogą paść — i jak odpowiedzieć

**"Dlaczego modularny monolit a nie mikroserwisy?"**
> Bo to jeden zespół (ja) i jeden deploy. Mikroserwisy dodają złożoność operacyjną (sieć, observability, eventual consistency), która ma sens przy wielu zespołach lub niezależnym skalowaniu. Modularny monolit daje mi granice modułów (rozdzielne warstwy, integration events) bez kosztu sieci. Jakbym musiał skalować jeden moduł niezależnie — wyciąłbym go jako serwis.

**"Po co Value Objects?"**
> Żeby logika domenowa była w domenie. `Password` waliduje swoje reguły w `Create()`, nie w handlerze. Niemutowalne (`record` + `init`) — nie ma "ktoś po cichu zmienił". Wymiana na nowy obiekt przez `with` zamiast settera.

**"Skąd wziąłeś pomysł na CQRS?"**
> Z potrzeby separacji intencji — `SignUpCommand` opisuje co chcę zrobić, `Handler` jak. Łatwiej pisać testy handlerów, łatwiej wpiąć cross-cutting (logowanie, walidacja) przez behaviory MediatR.

**"Po co Repository skoro DbContext sam jest UoW + repo?"**
> Cienka warstwa abstrakcji od EF — w Domain/Application nie ma `DbContext`. Łatwiej zamockować w testach (Moq), łatwiej wymienić provider gdyby kiedyś trzeba było (mało prawdopodobne, ale konceptualnie czysto). To trade-off — uznałem że dla projektu portfolio warto pokazać, że umiem ten wzorzec.

**"Jak komunikują się moduły?"**
> Integration events przez MediatR. Po commitcie transakcji moduł publikuje event (np. `IdentityUserCreatedEvent`), inne moduły mają handlery. Nie wołam się bezpośrednio przez interfejsy między modułami — to złamałoby granicę.

**"Dlaczego PostgreSQL a nie MSSQL?"**
> Bo używam pgvector do embeddingów w module AiChat (RAG, 768-wymiarowe wektory, cosine similarity). MSSQL nie miał wbudowanego vector type w momencie kiedy startowałem. Same query do biznesowych danych poszłyby na MSSQL bez problemu — EF Core jest agnostyczny.

**"Co to RAG?"**
> Retrieval-Augmented Generation — zanim LLM wygeneruje odpowiedź, najpierw wyszukuję semantycznie (embeddingi + cosine similarity) najbardziej trafne fragmenty z dokumentów i wkładam jako kontekst do promptu. Dzięki temu LLM odpowiada na podstawie *moich* danych, a nie tylko ogólnej wiedzy treningowej.

**"Jak działa Stripe integration?"**
> Komendy do tworzenia checkout session → użytkownik płaci po stronie Stripe → Stripe wysyła webhook (`checkout.session.completed`, `customer.subscription.updated`, `customer.subscription.deleted`). Weryfikuję podpis webhooka (`EventUtility.ConstructEvent`), aktualizuję `Subscription` value object na encji `IdentityUser`. Daty okresów rozliczeniowych biorę z `Invoice` a nie `Subscription` — to nieoczywiste, nauczyłem się przez błąd.

**"Jak testowałeś?"**
> TUnit + Moq na warstwę Application (handlery), repozytoria mockowane. Testy domeny — czyste, bez mocków, na value objectach i metodach domenowych. Integracyjnych webowych jeszcze nie mam — to świadome ograniczenie projektu portfolio.

**"Co byś zmienił dziś?"**
> Pierwsze co przychodzi do głowy: zaczynałbym z mniejszą liczbą modułów i dopiero wyodrębniał kiedy widać granice. Drugie — w niektórych miejscach przesadziłem z abstrakcją (np. cienkie repozytoria z metodami jeden-do-jeden po DbSet). Trzeci feature który dorzucę: pełny CI/CD do Azure App Service.

**"Co poszło najtrudniej?"**
> Webhook Stripe. Dane okresów rozliczeniowych są rozproszone między obiektami Stripe (Subscription vs Invoice), debugowałem to pół dnia. Drugie — embedingi i pgvector, bo musiałem zrozumieć jak skonfigurować EF z customowym typem kolumny.

### 5.3 Co pokazać na ekranie (jeśli poproszą)
- `IdentityUser.cs` + `Subscription.cs` — Value Object, immutability, Create factory.
- `SignUpCommand.cs` + Handler — CQRS, walidacja, repozytorium, UoW commit, integration event.
- Jeden Module (`Program.cs`/`*Module.cs`) — DI, modułowość.
- Migration folder — pokazuje że ogarniasz EF migracje.

### 5.4 Czego **nie** mówić
- Nie nazywaj projektu "production-ready" — nie jest.
- Nie udawaj że robiłeś RAG profesjonalnie — to projekt naukowy.
- Nie wchodź zbyt głęboko w teorię DDD ("aggregate roots", "bounded contexts") jeśli nie używasz tych terminów na co dzień — łatwo o pułapkę.

---

## 6. ERP — co MUSISZ wiedzieć (bo nigdy nie miałeś styczności)

### 6.1 Co to w ogóle ERP

**ERP (Enterprise Resource Planning)** = system zintegrowany do zarządzania firmą. Jedna baza danych dla wszystkich kluczowych procesów:
- **Finanse / Księgowość** — faktury, rozrachunki, księga główna, deklaracje (VAT, JPK).
- **HR / Kadry-Płace** — pracownicy, umowy, listy płac, urlopy, ZUS.
- **Magazyn / Logistyka** — stany magazynowe, przyjęcia, wydania, rezerwacje.
- **Sprzedaż / CRM** — kontrahenci, oferty, zamówienia, faktury sprzedażowe.
- **Produkcja** (w większych) — BOM, planowanie, zlecenia produkcyjne.

**Sens biznesowy:** firma zamiast 5 osobnych programów (księgowy, magazyn, sklep) ma jeden, w którym dane "płyną" — faktura sprzedaży automatycznie zdejmuje stan magazynowy i tworzy zapis w księdze.

### 6.2 enova365 (Soneta) — krótko

- Polski ERP średniej wielkości, popularny w MŚP.
- **Plugin / dodatek** = projekt .NET DLL, który podpinasz do enova. Rozszerzasz w nim funkcje (np. nowy raport, własny algorytm, integracja z zewnętrznym systemem).
- API enova: dostęp do obiektów biznesowych przez **Sessions** (`Soneta.Business.Session`), zarządzanie transakcjami przez `Transaction`.
- Konfiguracja przez **konfigurator** enova; metadane (np. dodatkowe pola na encjach) bez kodu.
- Domyślna baza: **MSSQL**.
- W ofercie ENOLOGY: "tworzenie dodatków i rozszerzeń do enova365" → najpewniej będziesz pisał właśnie pluginy.

### 6.3 Comarch ERP Optima — krótko

- Polski ERP, jeszcze popularniejszy w MŚP (głównie biura rachunkowe, małe firmy).
- Rozszerza się przez **API (COM/.NET)** i **SDK** Comarcha.
- Też MSSQL pod spodem.

> **Nie ucz się szczegółów API** — na rozmowę wystarczy że wiesz **czym są te systemy** i jak się je rozszerza (pluginy/dodatki). Konkretne klasy/metody nauczysz się w pracy.

### 6.4 Charakterystyka pracy z ERP — czego się spodziewać

- Dużo logiki w bazie (SP, triggery, widoki).
- Schematy danych **duże i historyczne** — tabele z setkami kolumn, dziwne nazwy, polskie skróty.
- Klienci mają oczekiwania księgowo-fiskalne, deklaracje muszą się **bilansować** co do grosza.
- Integracje z systemami zewnętrznymi: bankowość, sklepy internetowe, kurierzy, JPK do US, e-Faktury.
- Migracje danych z innych systemów ("import z Excela").
- Często **praca z konkretnymi klientami** — wymaga komunikacji nie tylko technicznej (oferta ENOLOGY wprost to mówi: "bezpośredni kontakt z klientem").

### 6.5 Typowe pytania o ERP — odpowiedzi modelowe

**"Co wiesz o systemach ERP?"**
> Wiem że to systemy zintegrowane do zarządzania firmą — łączą finanse, magazyn, sprzedaż, HR, produkcję w jednej bazie. Sens jest taki, że dane "przepływają" — faktura sprzedaży automatycznie zdejmuje stan i tworzy zapis księgowy. enova365 i Optima to polskie ERP-y dla małych i średnich firm, oba osadzone na MSSQL. Rozszerza się je pisząc dodatki — DLL podpinane do hosta, używające API systemu. Sam nie pracowałem z żadnym ERP-em, ale wiem do czego służą i jak są zbudowane.

**"Czemu chcesz pracować z ERP-em?"**
> Bo to dziedzina w której technologia łączy się z konkretnym biznesem — księgowość, magazyn, procesy mają realne wymagania prawne i biznesowe. Łatwo zobaczyć efekt swojej pracy u klienta. Dodatkowo MSSQL + .NET + integracje to stack który chcę rozwijać, a ERP daje na to dużo okazji.

**"Jakbyś podszedł do feature'u w nieznanym systemie ERP?"**
> Najpierw bym zrozumiał wymaganie biznesowe — z kim trzeba pogadać, jaki dokument źródłowy, jaki efekt końcowy. Potem dokumentacja API systemu, eksploracja schematu bazy (które tabele dotykane), sprawdzenie istniejących dodatków podobnych do tego co mam zrobić. Implementację robiłbym małymi krokami, testując na bazie testowej. Pytałbym dużo seniora w zespole — szczególnie o pułapki specyficzne dla tego ERP.

**"Czytałeś coś o księgowości?"**
> Nie pracowałem w księgowości, ale rozumiem podstawy: dwustronny zapis (debet/kredyt), bilans, deklaracje VAT, JPK. W razie potrzeby douczam się szybko z dokumentacji klienta — to nie jest dla mnie problem.

### 6.6 Co warto przejrzeć przed rozmową (10-15 min wystarczy)

- Strona soneta.pl / enova365 — przeczytaj zakładkę "Dla programistów" i "Moduły".
- Strona comarch.pl/erp-optima — przejrzyj listę modułów.
- Hasła do googlowania: "enova365 SDK", "enova dodatki", "Comarch ERP Optima ERP XL API".
- Polskie pojęcia: faktura VAT, JPK_V7, deklaracja PIT-11, lista płac, środki trwałe, kontrahent, kartoteka towarowa. Nie musisz znać szczegółów — wystarczy że nie zaskakują Cię w rozmowie.

---

## 7. Pytania "miękkie" / behawioralne

### 7.1 Najczęstsze i jak odpowiadać
- **"Opowiedz o sobie"** → 60 sekund: skąd jestem, co studiowałem/jak się uczyłem, co buduję teraz (MealMind), czego szukam.
- **"Dlaczego my?"** → uczepić się "rozwój w kierunku eksperta ERP", "praca przy realnych projektach", "praca stacjonarna w Rzeszowie" (jeśli mieszkasz blisko).
- **"Najtrudniejszy bug?"** → konkret z MealMind (np. webhook Stripe + daty z Invoice). Pokaż proces: jak diagnozowałem, co się okazało, czego się nauczyłem.
- **"Praca w zespole?"** → szczerze: jeszcze nie pracowałem komercyjnie, ale w projektach studenckich/własnych mam doświadczenie z code review, gitem, rozdzielaniem zadań. Cenię konkretną informację zwrotną.
- **"Mocne / słabe strony?"** → mocne: ogarniam architekturę i dbam o jakość kodu. Słabe: brak komercyjnego doświadczenia, ale skompensowałem to projektem portfolio i czytaniem.
- **"Gdzie się widzisz za 3 lata?"** → mid .NET dev specjalizujący się w ERP/biznesowych systemach, z doświadczeniem w MSSQL i integracjach. To pasuje 1:1 do ich ścieżki rozwoju.

### 7.2 Pytania PUŁAPKI
- *"Czego oczekujesz finansowo?"* — daj zakres (7-8k brutto na start), pokaż otwartość.
- *"Kiedy możesz zacząć?"* — odpowiedź szczera, najlepiej "od zaraz" lub "z miesiącem wypowiedzenia" jeśli aktualnie gdzieś jesteś.
- *"Masz inne procesy?"* — szczerze, ale bez nazwisk konkretnych firm.

### 7.3 Pytania KTÓRE TY zadasz im

**Mocne pytania pokazujące że myślisz o pracy, nie tylko o złapaniu jakiejkolwiek:**
1. "Jak wygląda onboarding dla kogoś bez doświadczenia w ERP — czy jest dedykowany czas na naukę systemu, czy uczy się przez projekty?"
2. "Z jakim ERP-em pracujecie częściej — enova365 czy Optima? Czy klient zwykle wybiera jeden i się trzyma, czy bywa że migrujecie?"
3. "Jak duży jest zespół developerski? Ilu seniorów / midów / juniorów?"
4. "Czy są code review na każdym PR-ze? Jakie macie zasady gita (trunk-based, feature branches, GitFlow)?"
5. "Jakie macie testy automatyczne — jednostkowe, integracyjne, end-to-end?"
6. "Czy macie własne biblioteki/frameworki budowane na bazie enova/Optima, czy każdy dodatek piszecie od zera?"
7. "Jak wygląda kontakt z klientem — analityk biznesowy jest między, czy programista bezpośrednio?"
8. "Jakie macie środowiska (dev/test/staging/prod) i jak wygląda deploy dodatku do produkcji?"

**Pytania o rozwój:**
- "Jakie kursy/certyfikaty wspieracie? Czy enova ma program certyfikacji który warto zrobić?"
- "Jaką ścieżkę rozwoju widzicie dla juniora — mid → senior → ekspert ERP / architekt?"

> **Wybierz 3-4 pytania, nie zadawaj wszystkich.** Najgorsze co możesz zrobić to milczeć kiedy zapytają "masz jakieś pytania?".

---

## 8. Lista kontrolna na dzień przed rozmową

- [ ] Ścieżki w MealMind: `IdentityUser.cs`, `SignUpCommand.cs`, `Subscription.cs`, jeden Module. Otwórz w VS, miej pod ręką.
- [ ] Krótki opis projektu w głowie (max 2 minuty).
- [ ] Podstawowe pytania C# (1.1 - 1.9) — przeczytaj na głos, sprawdź czy umiesz wyjaśnić swoimi słowami.
- [ ] JOIN-y, GROUP BY/HAVING, indeksy, N+1 — masz w głowie.
- [ ] EF Core: tracking, Include, projekcje, migracje.
- [ ] ERP punkty 6.1 - 6.3 przeczytane, model odpowiedzi 6.5 przyswojony.
- [ ] 3 pytania DO NICH wybrane i zanotowane.
- [ ] Jeśli rozmowa zdalna: kamera, mikrofon, oświetlenie, internet.
- [ ] CV pod ręką, projekt na GitHubie linkowalny.
- [ ] Sen. Naprawdę. 7+ godzin > kolejne 2h "przygotowań" w nocy.

---

## 9. Mini-szpargał na sam dzień rozmowy

**Jeśli czegoś nie wiesz:**
> "Nie miałem dotąd okazji tego używać, ale wiem że dotyczy [X]. Spróbowałbym podejść do tego tak: [A, B, C]. Czy mógłbyś naprowadzić mnie na to jak wy to robicie u siebie?"

To **lepsza** odpowiedź niż wymyślanie. Pokazuje uczciwość + chęć nauki + zdolność analitycznego myślenia + komunikację.

**Jeśli nie rozumiesz pytania:**
> "Czy mógłbyś doprecyzować — pytasz o [A] czy bardziej o [B]?"

**Jeśli zacinasz się na technicznym pytaniu:**
> Mów na głos co Ci przychodzi do głowy. "Pierwsza myśl: ... Druga: ... Sprawdziłbym to przez ..."
> Lepiej myśleć na głos niż milczeć 30 sekund.

**Najważniejsze:**
- Mów wolno. Łatwo nawalić tempa pod stresem.
- Słuchaj do końca pytania, nie przerywaj.
- Uśmiechaj się — nawet przez telefon słychać.
- Powodzenia.

---

## 10. AI / RAG / LLM w MealMind — głęboka obrona

> **Twoja przewaga.** Większość juniorów nie ma żadnego AI w projekcie. Ty masz pełen RAG + integrację z LLM. ENOLOGY niekoniecznie korzysta z AI, ale jeśli ktoś z zespołu się zainteresuje — masz czym pokazać że umiesz osadzić nowoczesne narzędzia w prawdziwej architekturze, a nie tylko klikać OpenAI Playground.

### 10.1 Słownik — musisz tym żonglować płynnie

- **LLM** (Large Language Model) — model językowy (np. GPT, Llama, Claude). Działa probabilistycznie — przewiduje kolejny token.
- **Token** — kawałek tekstu (~3-4 znaki / 0.75 słowa). LLM przetwarza tokeny, nie znaki.
- **Context window** — maksymalna liczba tokenów którą model widzi naraz (prompt + odpowiedź). Llama 3.2:3b ma 128k, mniejsze konfiguracje 8k.
- **Prompt** — to co wysyłasz do modelu. Składa się z `system` (instrukcje persony/zadania) + `user` (pytanie) + opcjonalnie `assistant` (poprzednie odpowiedzi w konwersacji).
- **Inference** — uruchomienie modelu na promptcie żeby dostać odpowiedź.
- **Embedding** — wektor liczb (u Ciebie 768-wymiarowy) reprezentujący semantyczne znaczenie tekstu. Dwa teksty o podobnym znaczeniu mają wektory blisko siebie w przestrzeni.
- **Cosine similarity** — miara podobieństwa dwóch wektorów (kąt między nimi). 1 = identyczne, 0 = ortogonalne, -1 = przeciwne.
- **RAG** (Retrieval-Augmented Generation) — wzorzec gdzie zanim LLM odpowie, wyszukujemy istotne fragmenty z naszej bazy wiedzy i dokładamy do promptu.
- **Hallucination** — kiedy LLM wymyśla fakty (bo nie wie, ale gada). RAG zmniejsza ryzyko bo dajesz konkretny kontekst.
- **Fine-tuning** — doszkalanie modelu na własnych danych. Drogie, wymaga GPU, rzadko potrzebne.
- **Chunking** — dzielenie długich dokumentów na mniejsze kawałki (chunki) — bo embedding całej książki nie ma sensu (uśredniony wektor traci semantykę).
- **Top-K retrieval** — wyciągasz K najbardziej podobnych chunków (np. K=5).
- **Semantic search** vs **keyword search** — semantyczne łapie "wegetariańska dieta" gdy szukasz "jedzenie bez mięsa". Keyword (full-text) wymaga zgodności słów.

### 10.2 Pitch RAG w 60 sekund

> RAG to wzorzec łączący LLM z zewnętrznym źródłem wiedzy. W MealMind chat AI nie zgaduje tylko opiera odpowiedź o konkretne dokumenty użytkownika — diet summary, plan treningowy, materiały dietetyczne. Dokument chunkuję na ~500-tokenowe fragmenty (Microsoft.SemanticKernel.Text.TextChunker), każdy wektoryzuję modelem `nomic-embed-text` przez Ollama (768 wymiarów), zapisuję w PostgreSQL z pgvector. Gdy user zadaje pytanie — embedduję pytanie, robię cosine similarity search w bazie, biorę top-K najbardziej trafnych chunków, wkładam jako kontekst do promptu i wysyłam do `llama3.2:3b`. Dzięki temu LLM odpowiada na podstawie *moich* danych, nie tylko ogólnej wiedzy treningowej.

### 10.3 Pytania techniczne — gotowe odpowiedzi

**"Dlaczego RAG a nie fine-tuning?"**
> Fine-tuning ma sens kiedy chcesz zmienić *styl* odpowiedzi albo nauczyć modelu specyficznej domeny ogólnej. RAG ma sens kiedy chcesz żeby model odpowiadał o **świeżych, użytkowych danych** — diet summary aktualizuje się codziennie, fine-tuning wymagałby retreningu. Dodatkowo RAG jest tańszy (nie wymaga GPU), łatwiejszy do debugowania (widzę które chunki poszły do promptu) i pozwala kontrolować dostęp (per-user dokumenty nie trafiają do wag modelu).

**"Co to embedding i czemu 768 wymiarów?"**
> Embedding to wektor liczb reprezentujący semantyczne znaczenie tekstu. Model embeddingowy uczy się tak, żeby semantycznie podobne teksty miały geometrycznie bliskie wektory. 768 wymiarów to liczba wymiarów wyjścia `nomic-embed-text` — to kompromis między jakością a kosztem przechowywania. OpenAI ada-002 ma 1536, większe modele 3072. Dla projektu MealMind 768 wystarczy.

**"Jak chunkujesz dokument?"**
> Używam `TextChunker.SplitPlainTextParagraphs` z Semantic Kernel — dzieli tekst na akapity i łączy w kawałki ~500 tokenów z ~100 tokenowym overlapem. Overlap jest po to żeby kontekst nie urywał się w środku zdania i kluczowa informacja nie wypadła na granicy chunków. Każdy chunk to osobny wiersz w bazie z polem `DocumentGroupId` (logiczne grupowanie chunków z tego samego źródłowego dokumentu) i `ChunkIndex` (pozycja w oryginale).

**"Dlaczego pgvector a nie dedykowana vector database (Pinecone, Weaviate, Qdrant)?"**
> Bo mam już PostgreSQL dla danych biznesowych. Trzymanie embeddingów w tym samym Postgresie to mniej infrastruktury (jedna baza, jeden backup, jedna transakcja), prostsze JOIN-y (chunki + metadane konwersacji w jednym query). pgvector daje mi indeks IVFFlat / HNSW który skaluje się do milionów wektorów. Dla projektu rozmiarów MealMind dedykowana vector DB to overkill.

**"Co to cosine similarity i czemu nie zwykła odległość Euklidesowa?"**
> Cosine mierzy kąt między wektorami, więc skupia się na *kierunku* w przestrzeni a nie na *długości*. Embeddingi koncepcyjnie kodują znaczenie w kierunku — dwa teksty o podobnym sensie ale różnej długości będą wskazywać podobnie. Euklidesowa karałaby różnice długości wektorów, co nie odpowiada temu co nas interesuje. W praktyce dla znormalizowanych embeddingów cosine i Euklidesowa dają ten sam ranking, ale cosine jest bardziej naturalna.

**"Jak wyglądałby przepływ od pytania użytkownika do odpowiedzi?"**
1. User wpisuje wiadomość w czacie.
2. `GetChatResponseCommand` handler bierze pytanie.
3. Wywołuje `IEmbeddingService.EmbedAsync(question)` → wektor 768-wymiarowy.
4. Query do `Conversation Documents` (chunki przypiętych dokumentów) + `RagDocuments` (globalna baza wiedzy) sortuje po cosine similarity, bierze top-K.
5. Buduje prompt: system instructions + retrieved chunks jako kontekst + historia ostatnich wiadomości + nowe pytanie.
6. Wysyła do Ollama (`llama3.2:3b`) przez Semantic Kernel.
7. Zwraca odpowiedź użytkownikowi, zapisuje wymianę w `Conversation`.

**"Dlaczego Ollama a nie OpenAI?"**
- **Koszt:** Ollama lokalnie = zero kosztu per token. OpenAI by mnie kosztowało za każdą wiadomość.
- **Prywatność:** dane użytkowników (dieta, zdrowie) nie wychodzą na zewnętrzne API.
- **Edukacja:** chciałem nauczyć się jak działa LLM stack pod spodem, a nie tylko strzelać do API.
- **Trade-off:** jakość odpowiedzi gorsza niż GPT-4. Latencja zależy od mojego sprzętu. Skalowanie ograniczone do jednej maszyny. Akceptowalne dla MealMind, w prod-grade systemie pewnie zaadoptowałbym OpenAI lub Azure OpenAI.

**"Co to Semantic Kernel?"**
> Microsoftowy framework do budowy aplikacji LLM-owych w .NET (alternatywa LangChain dla Pythonu). Daje abstrakcje na chat completion, embeddings, tool calling, vector stores, plugins. Używam `Microsoft.SemanticKernel.Connectors.Ollama` żeby gadać z Ollamą tym samym kodem co poszedłby do OpenAI/Azure — wymiana providera to zmiana konfiguracji, nie kodu.

**"Co to function calling / tool use w LLM?"**
> Model zwraca w odpowiedzi nie tylko tekst ale również strukturalny request "wywołaj funkcję X z argumentami Y". W moim Food Vision AI feature LLM analizuje zdjęcie jedzenia i zwraca strukturalny obiekt `{ "name": "salad", "calories": 250, "categories": ["vegetarian"] }` zamiast wolnego tekstu. To kluczowe gdy chcesz wpinać LLM do procesów biznesowych — strukturalna odpowiedź jest deserializowalna.

**"Co robisz z halucynacjami?"**
> RAG redukuje ryzyko bo wkładam konkretny kontekst. Dodatkowo w system promptie wprost piszę "odpowiadaj tylko na podstawie podanego kontekstu, jeśli nie wiesz powiedz że nie wiesz". To nie eliminuje halucynacji do zera ale znacznie zmniejsza. Bardziej rygorystyczne podejście: druga LLM-owa pass która ocenia czy odpowiedź faktycznie wynika z kontekstu (LLM-as-judge), ale w MealMind tego nie zaimplementowałem.

**"Jak dobierałeś rozmiar chunka?"**
> ~500 tokenów to standardowy punkt startu — wystarczająco duży żeby zachować lokalny kontekst (kilka akapitów), wystarczająco mały żeby kilka chunków zmieściło się w prompt context window. Mniejsze chunki (100-200) dają precyzyjniejszy retrieval ale gubią szerszy kontekst. Większe (1000+) trafiają w więcej tematów ale tracą precyzję podobieństwa. Dla diet summary i ogólnej wiedzy żywieniowej 500 + 100 overlap działa OK.

**"Co to Table Per Concrete Type (TPC) i czemu tak masz dokumenty?"**
> TPC to strategia mapowania dziedziczenia w EF — każda konkretna klasa pochodna ma własną tabelę bez kolumny dyskryminatora. Klasa abstrakcyjna `Document` nie ma tabeli, `RagDocument` i `ConversationDocument` mają osobne. Plusy: czystszy schemat, nie ma `NULL`-i nieużywanych kolumn dla innych typów, prostsze indeksy per typ. Minusy: nie da się trzymać heterogenicznej kolekcji `List<Document>` w jednej tabeli, query "wszystkie dokumenty" robi UNION.

**"Jak testujesz RAG?"**
> Trudne — retrieval jakościowo, generation jakościowo. W moim projekcie portfolio mam testy jednostkowe na `ChunkingService` (czy podział wychodzi rozsądny) i `EmbeddingService` (czy zwraca poprawny wymiar). Pełnego eval RAG (np. RAGAS framework, sprawdzanie ground-truth pytań) nie ma — to świadome ograniczenie projektu.

### 10.4 Co pokazać na ekranie (gdyby chcieli zobaczyć kod AI)

- **`Document.cs`** + `RagDocument.cs` + `ConversationDocument.cs` — TPC, abstrakcyjna baza.
- **`ChunkingService.cs`** — użycie `TextChunker`, granica chunków.
- **`EmbeddingService.cs`** — wywołanie Ollama, mapowanie do `Vector`.
- **`GetChatResponseCommand` handler** — pełen flow RAG od pytania do odpowiedzi.
- **`AttachDocumentCommand`** — cross-module wpięcie dokumentu z Nutrition/Training.

### 10.5 Sygnał ostrzegawczy — czego NIE mówić

- Nie udawaj że to "produkcyjny system AI". To projekt portfolio bez evaluacji jakości, monitoringu driftu, A/B testów promptów.
- Nie używaj słowa "AGI", "świadomość modelu", "rozumie".
- Nie obiecuj że LLM "nigdy nie halucynuje" — kłamstwo i pułapka.
- Nie sprzedawaj RAG jako rozwiązanie wszystkiego. Powiedz że dla wąskich faktualnych pytań działa świetnie, dla kreatywności / brainstormu zbędne.

---

## 11. Wzorce projektowe (Design Patterns)

> Wzorce to **wspólny słownik**. Senior nie musi tłumaczyć Ci pół godziny — wystarczy "to jest Strategy" i wiesz o co chodzi. W MealMind używasz **co najmniej 10** wzorców. Tu nazwiemy te które tam są — będziesz mógł wprost wskazać plik gdy zapytają.

### 11.1 Architektoniczne / strukturalne (wysoki poziom)

**Modular Monolith** — jeden deploy, ale podział na moduły z granicami (Identity, Nutrition, AiChat, Training). U Ciebie: każdy moduł = osobny projekt z warstwami Api/Application/Domain/Infrastructure.

**Clean Architecture / Onion** — warstwy ułożone tak że *Domain* nie wie o niczym, *Application* zależy od Domain, *Infrastructure* implementuje interfejsy z Domain/Application, *Api* jest na samej górze. Reguła: zależności idą do środka, nigdy na zewnątrz. U Ciebie: `IRepository<T>` jest w Domain/Application, implementacja w Infrastructure.

**Dependency Injection (DI)** — kod zależy od **abstrakcji**, kontener wstrzykuje konkret. To **implementacja** zasady Dependency Inversion (litera D z SOLID). U Ciebie: każdy `*Module.cs` rejestruje serwisy.

**CQRS** (Command Query Responsibility Segregation) — komendy (modyfikują stan, nic nie zwracają biznesowo) i zapytania (czytają, zwracają DTO) idą osobnymi ścieżkami. U Ciebie: `SignUpCommand` vs `GetTrainingPlanQuery`.

### 11.2 GoF — klasyczne wzorce

**Repository** — abstrakcja nad dostępem do danych. Domena nie wie czy idzie do EF, Mongo czy pliku. U Ciebie: `IIdentityUserRepository` + implementacja w Infrastructure.

**Unit of Work** — pojedyncza transakcja obejmująca wiele operacji. W EF Core `DbContext` *jest* UoW (`SaveChangesAsync` commituje wszystkie zmiany śledzone). U Ciebie: `IUnitOfWork.CommitAsync()`.

**Factory Method** — statyczna metoda tworząca obiekt z walidacją. U Ciebie: `IdentityUser.Create(...)`, `Password.Create(...)`, `Subscription.Create(...)`. Konstruktor jest prywatny / chroniony, jedyna droga do utworzenia idzie przez `Create` która waliduje inwarianty.

**Builder** — krokowe budowanie złożonego obiektu. EF `ModelBuilder` w `OnModelCreating` to builder. ASP.NET `WebApplicationBuilder` też.

**Strategy** — wymiana algorytmu przez interfejs. U Ciebie potencjalnie: `IEmbeddingService` mógłby mieć implementację Ollama lub OpenAI — wymiana przez konfigurację DI.

**Decorator** — opakowanie obiektu dodatkową funkcjonalnością z zachowaniem interfejsu. W MealMind **pipeline behaviors MediatR** to wzorzec decoratora — `ValidationBehavior`, `LoggingBehavior` opakowują handler.

**Mediator** — obiekty komunikują się przez pośrednika zamiast bezpośrednio. U Ciebie: `MediatR.ISender`. Endpoint nie woła handlera bezpośrednio, woła `sender.Send(command)` — `Mediator` znajduje handler. Plus: luźne powiązanie endpoint ↔ logika.

**Observer / Pub-Sub** — obiekt powiadamia subskrybentów o zdarzeniach. U Ciebie: **integration events** (`IdentityUserCreatedEvent` publikowany po commit, inne moduły mają handlery).

**Singleton** — jedna instancja na całą aplikację. W DI: `services.AddSingleton<T>()`. U Ciebie: konfiguracje, klient HTTP.

**Template Method** — bazowa klasa definiuje szkielet algorytmu, podklasy nadpisują kroki. U Ciebie: bazowy `EndpointBase` definiuje strukturę endpointów, każdy konkretny dodaje swoją mapę.

### 11.3 DDD — taktyczne wzorce

**Entity** — obiekt z tożsamością (`Id`). Dwie `IdentityUser` z tym samym `Id` to ta sama jednostka. U Ciebie: `Entity<TId>` jako baza.

**Value Object** — obiekt definiowany przez wartość. Dwa `Password` z tym samym hashem to ten sam VO. Niemutowalny. U Ciebie: `Password`, `Subscription`, `PersonalData`, `NutritionTarget`. Implementacja przez `record`.

**Aggregate Root** — encja będąca punktem wejścia do "agregatu" (graf powiązanych obiektów chroniony jako całość). Reguła: dostęp do encji wewnątrz agregatu *tylko* przez root. U Ciebie: `IdentityUser` jest rootem dla `Subscription`.

**Domain Event** — wydarzenie wewnątrz agregatu, ważne dla domeny. Publikowane przez agregat, łapane synchronicznie wewnątrz tego samego procesu.

**Integration Event** — wydarzenie *między* modułami / serwisami. Publikowane po komicie transakcji. U Ciebie: po `SaveChanges` na Identity wysyła `IdentityUserCreatedEvent` do Nutrition.

**Repository** — patrz wyżej, w DDD jest to wzorzec taktyczny.

**Specification** — enkapsulacja warunku zapytania jako obiekt. U Ciebie raczej nie ma, ale warto wiedzieć — bywa w komercyjnych projektach.

### 11.4 Mniejsze ale często padają na rozmowach

**Adapter** — przejściówka między dwoma niekompatybilnymi interfejsami. U Ciebie: connector Ollama z Semantic Kernel adaptuje API Ollama do interfejsów SK.

**Facade** — uproszczony interfejs na złożony podsystem. U Ciebie: `IChunkingService` to fasada na `TextChunker` z Semantic Kernel.

**Chain of Responsibility** — żądanie przechodzi przez łańcuch handlerów dopóki któryś nie obsłuży. W ASP.NET **middleware pipeline** to dokładnie ten wzorzec. MediatR pipeline behaviors też pasują.

**Null Object** — zwracasz "pusty" obiekt zamiast `null`. Rzadko używane explicit, ale `Enumerable.Empty<T>()` to forma tego.

**Result / Either pattern** — zamiast wyjątku zwracasz obiekt `Result<T>` z `IsSuccess` + `Error`. Częste w projektach .NET (FluentResults, OneOf). U Ciebie? Sprawdź — jeśli używasz `Result<T>` w handlerach, jest sens go nazwać.

### 11.5 Anty-wzorce — co WIEDZIEĆ żeby NIE robić

- **God Object** — klasa robiąca wszystko (Userów, faktury, raporty). W ERP to częsta pułapka legacy.
- **Anemic Domain Model** — encje to tylko worki na property, cała logika w serwisach. DDD się temu sprzeciwia. U Ciebie *Twoja* domena jest pełna metod (`IdentityUser.UpdateSubscription`, `Password.Verify`).
- **Service Locator** — pobieranie zależności z kontenera "z ręki" zamiast wstrzykiwania w konstruktorze. Ukrywa zależności, utrudnia testowanie.
- **Smart UI** — logika biznesowa w kontrolerach / endpointach. U Ciebie endpoint = 15 linii, logika w handlerze.

### 11.6 Pytania-pułapki

**"Czy zawsze stosujesz Repository?"**
> Nie. EF Core *jest* już repozytorium + UoW. Cienka warstwa repo ma sens kiedy chcesz: (a) abstrahować od ORM dla testów, (b) ujednolicić zapytania, (c) zachować czystość warstwy domeny. W mniejszych projektach często wystarczy `DbContext` bezpośrednio w handlerze.

**"Czym różni się Strategy od State?"**
> Strategy — *ty* z zewnątrz wybierasz algorytm. State — obiekt sam zmienia swoje zachowanie zależnie od stanu wewnętrznego.

**"Co jest złego w Singletonie?"**
> Globalny stan, ukryte zależności, trudne testowanie. W DI Singleton jest OK gdy jest **bezstanowy** (config, klient HTTP) — wtedy to tylko cache instancji.

**"DDD trzeba stosować zawsze?"**
> Nie. DDD ma koszt: krzywa nauki, więcej klas, więcej dyscypliny. Ma sens przy **złożonej domenie biznesowej** (ubezpieczenia, finanse, logistyka, **ERP**). Dla CRUD-owego widoku z trzema tabelami DDD to przerost formy.

### 11.7 Krótka ściąga dopasowań plik ↔ wzorzec (MealMind)

| Plik / klasa | Wzorzec |
|---|---|
| `IdentityUser.Create(...)` | Factory Method |
| `Password : ValueObject` | Value Object |
| `IIdentityUserRepository` | Repository |
| `IUnitOfWork.CommitAsync` | Unit of Work |
| `SignUpCommand` + `Handler` | CQRS, Mediator (via MediatR) |
| `ValidationBehavior<,>` | Decorator / Pipeline |
| `IdentityUserCreatedEvent` | Observer / Integration Event |
| `Document` + `RagDocument`/`ConversationDocument` | Template Method / TPC |
| `IChunkingService` → `TextChunker` | Facade |
| `EndpointBase` | Template Method |
| `*Module.cs` (DI registration) | Dependency Injection (Composition Root) |
| ASP.NET middleware pipeline | Chain of Responsibility |

---

> **Notatka odnośnie ścieżki**: tego pliku nie commituj do MealMind — to materiał osobisty. Możesz go przerzucić na zewnątrz, do Notion, do wydrukowania albo na telefon.
