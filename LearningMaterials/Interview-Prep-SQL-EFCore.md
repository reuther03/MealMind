# Przygotowanie do rozmowy — SQL + EF Core (junior+)

> Uzupełnienie do głównego pliku prep (ENOLOGY). Tematy które junior z EF Core powinien znać i które padają na rozmowach. T-SQL głęboki (kursory, triggery, procedury) pominięty — do nauki w pracy.

---

## Indeksy (powtórka — to się rozjaśniło)

**Czym jest:** posortowana struktura (B-tree) która pozwala bazie znaleźć wiersze **bez czytania całej tabeli** — zamiana Table Scan (rekord po rekordzie) na Index Seek (skok do celu). Analogia: skorowidz na końcu książki (hasło → strona).

**Clustered:**
- Fizycznie układa wiersze w tabeli wg klucza. Liść drzewa trzyma **cały wiersz**.
- **Tylko jeden** na tabelę (nie da się ułożyć danych w dwóch kolejnościach naraz).
- MSSQL: domyślnie na PK. PostgreSQL: tabele to heap, brak domyślnego clustered — insert idzie od razu na stertę.
- Trade-off: bardzo szybki odczyt, ale insert wolniejszy (wiersz musi trafić w posortowane miejsce).

**Non-clustered:**
- Osobny skorowidz obok tabeli: wartość kolumny + wskaźnik do wiersza. **Wiele** na tabelę.
- Zakładasz na kolumny często wyszukiwane (np. Email).
- **Key Lookup (bookmark lookup):** jak zapytanie potrzebuje kolumn których nie ma w indeksie → baza robi drugi skok do tabeli. Przy 1 wierszu trywialne, przy 10 tys. kosztowne.
- **Rozwiązanie: covering index** — dokładasz brakujące kolumny przez `INCLUDE`, indeks "pokrywa" całe zapytanie, drugi skok znika.

```sql
CREATE NONCLUSTERED INDEX IX_Users_Email
ON Users(Email)        -- po czym szukamy
INCLUDE (Name);        -- co jeszcze zwracamy → brak Key Lookup
```

**Czemu nie indeksować wszystkiego:** każdy indeks spowalnia INSERT/UPDATE/DELETE (trzeba go aktualizować) i zajmuje miejsce. Zakładaj na kolumny w WHERE/JOIN/ORDER BY o **wysokiej selektywności** (dużo różnych wartości — Email dobry, Płeć M/K bezużyteczny).

---

## A. Relacje (1:1, 1:N, N:M)

- **1:1** — rzadkie, często łączone w jedną tabelę.
- **1:N** — najczęstsze. FK po stronie "wielu" (Orders.UserId).
- **N:M** — wymaga tabeli pośredniej (StudentCourses). EF Core 5+ tworzy ją automatycznie.

EF mapuje przez **navigation properties** + FK; konfiguracja Fluent API (`OnModelCreating`) lub atrybuty.

```csharp
public class User  { public List<Order> Orders { get; set; } }      // 1:N
public class Order { public Guid UserId { get; set; } public User User { get; set; } }
```

---

## B. NULL = "nieznane" (logika trójwartościowa)

```sql
WHERE Cena = NULL    -- ❌ nigdy nie zadziała (NULL = NULL daje NULL, nie true)
WHERE Cena IS NULL   -- ✅
```
- `COALESCE(a, b, 0)` — pierwsza nie-NULL wartość (standard SQL).
- `ISNULL(a, b)` — to samo, MSSQL-specyficzne.
- W EF: `x.Rabat ?? 0` → COALESCE.

---

## C. UNION vs UNION ALL, DISTINCT

- `UNION` — skleja + usuwa duplikaty (kosztuje sortowanie).
- `UNION ALL` — skleja bez usuwania (szybsze). Domyślnie używaj jak nie ma duplikatów.
- `DISTINCT` — usuwa duplikaty. Nie dawaj "na wszelki wypadek" — wymusza sortowanie.

---

## D. EXISTS vs IN vs JOIN

```sql
SELECT * FROM Users u
WHERE EXISTS (SELECT 1 FROM Orders o WHERE o.UserId = u.Id);
```
- `EXISTS` — przerywa po pierwszym trafieniu, dobre do "czy istnieje".
- `IN (lista)` — OK dla małych list; pułapka z NULL (`NOT IN (...NULL...)` → pustka).
- `NOT EXISTS` bezpieczniejszy niż `NOT IN`.
- EF: `.Any()` → EXISTS, `.Contains()` → IN.

---

## E. Paginacja (Skip/Take)

```csharp
var page = await ctx.Orders
    .OrderBy(o => o.CreatedAt)   // WYMAGANE — bez sortowania kolejność niezdefiniowana
    .Skip(pageNumber * pageSize)
    .Take(pageSize)
    .ToListAsync();
```
EF → `OFFSET ... FETCH NEXT ...`. Nigdy nie ściągaj całej tabeli do UI.

---

## F. Constraints + normalizacja

**Constraints:** PRIMARY KEY, FOREIGN KEY (integralność referencyjna), UNIQUE, NOT NULL, CHECK (`Cena > 0`).

**Normalizacja (idea):** eliminacja redundancji przez rozbicie danych na powiązane tabele.
- 1NF — atomowe komórki (brak list w kolumnie).
- 2NF — brak częściowych zależności od części klucza złożonego.
- 3NF — kolumny zależą tylko od klucza. **Praktyczny standard.**
- Czasem świadoma denormalizacja dla wydajności odczytu.

---

## G. Owned Types w EF Core (MAM w MealMind — Subscription, PersonalData)

Value Object mapowany jako **owned type** — bez własnej tabeli/Id, pola lądują jako kolumny w tabeli właściciela.

```csharp
builder.OwnsOne(u => u.Subscription);  // → kolumny Subscription_Tier, Subscription_StripeId... w Users
```
Talking point: *"Subscription to value object bez niezależnego cyklu życia — mapuję jako owned type, jego pola to kolumny w IdentityUser, bez sztucznego Id i osobnej tabeli."*

---

## H. Optimistic concurrency (RowVersion)

Problem: dwóch edytuje ten sam rekord, drugi zapis nadpisuje pierwszy (lost update).

```csharp
[Timestamp] public byte[] RowVersion { get; set; }
```
EF dodaje `WHERE RowVersion = <stara>` przy UPDATE. Jak ktoś zmienił w międzyczasie → 0 wierszy → `DbUpdateConcurrencyException`. Ty decydujesz: błąd / reload / merge.

Formuła: *"zakładam że konflikty rzadkie — nie blokuję rekordu, sprawdzam przy zapisie czy się nie zmienił."*

---

## I. Deferred execution (IQueryable)

`IQueryable` nie wykonuje się przy `.Where()` — buduje zapytanie. SQL leci dopiero przy materializacji (`ToListAsync`, `FirstAsync`, `CountAsync`, `foreach`).

```csharp
var query = ctx.Users.Where(u => u.IsActive);  // zero SQL
query = query.Where(u => u.Age > 18);           // zero SQL
var result = await query.ToListAsync();          // TERAZ jeden SQL z oboma WHERE
```
Zaleta: budujesz zapytanie warunkowo, baza dostaje jeden zoptymalizowany SQL.
Pułapka: `.ToList()` za wcześnie → reszta LINQ w pamięci (wolno).

---

## J. Raw SQL (escape hatch)

```csharp
var users = await ctx.Users
    .FromSqlRaw("SELECT * FROM Users WHERE Age > {0}", 18)  // parametryzowane!
    .ToListAsync();
```
**Zawsze parametryzuj** (`{0}`, nie sklejaj stringów) — inaczej SQL Injection. Dotyczy też `ExecuteSqlRaw`.

---

## Szybka ściąga "co padło i jest opanowane"

| Temat | Status |
|---|---|
| JOIN-y (INNER/LEFT/RIGHT/FULL) | ✅ |
| WHERE vs HAVING | ✅ |
| GROUP BY + agregaty | ✅ |
| Indeksy (clustered/non/covering/Key Lookup) | ✅ mocno |
| N+1 + Include jako lek | ✅ |
| Tracking / AsNoTracking / projekcja DTO | ✅ |
| Unit of Work | ✅ |
| Find vs FirstOrDefault / AsSplitQuery | ✅ |

**Trzy zdania na pamięć:**
- Indeks = posortowana ściąga; clustered = sama tabela (jeden), non-clustered = skorowidz obok (wiele).
- N+1 to choroba, `Include`/projekcja to lek.
- `IQueryable` jest leniwy — SQL leci dopiero przy materializacji.
