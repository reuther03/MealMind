# Przygotowanie do rozmowy — IDEO (praktyki .NET, ~tydzień)

> Praktyki miesięczne z szansą na zostanie. Stack Ideo = nowoczesny .NET / ASP.NET Core / EF Core / Web API — czyli **dokładnie to co masz w MealMind**. Strategia: bronić projektu naturalnie + mieć pewne podstawy. Nikt nie oczekuje seniora — chcą zobaczyć że rozumiesz to co napisałeś i że da się z Tobą pracować.

---

## Sekcja 0 — Nastawienie

- To **praktyki**, nie senior role. Nie musisz wiedzieć wszystkiego. Liczy się: rozumiesz swój kod, umiesz się uczyć, jesteś komunikatywny.
- Jak czegoś nie wiesz: **"nie miałem jeszcze z tym styczności, ale domyślam się że…"** — pokazujesz sposób myślenia. To lepsze niż zmyślanie.
- Twój as w rękawie: **MealMind**. Masz realny projekt z RAG, Stripe, modularnym monolitem. Większość kandydatów na praktyki ma TODO-appkę. Prowadź rozmowę na swój teren.
- Mów **konkretnie o decyzjach**: nie "użyłem CQRS" tylko "rozdzieliłem zapis od odczytu, bo…". Junior który tłumaczy *dlaczego* > junior który zna nazwy.

---

## Sekcja 1 — Podstawy C# (typy referencyjne vs wartościowe)

**Typ wartościowy (value type):** `int`, `bool`, `double`, `struct`, `enum`, `Guid`, `DateTime`.

- Trzyma **samą wartość**. Kopiowanie = kopia danych (dwie niezależne kopie).
- Domyślnie ląduje na **stosie** (stack) — szybki, automatycznie sprzątany gdy metoda się kończy. (Wyjątek: jak value type jest polem w klasie, leży na stercie razem z obiektem.)

**Typ referencyjny (reference type):** `class`, `record` (domyślnie), `string`, tablice, kolekcje, `interface`.

- Zmienna trzyma **referencję (adres)** do obiektu na **stercie** (heap).
- Kopiowanie = kopia adresu → dwie zmienne wskazują **ten sam obiekt**. Zmiana przez jedną widać przez drugą.
- Sterta sprzątana przez **Garbage Collector**.

**Pytanie-pułapka:** *"Co się stanie jak przekażesz obiekt do metody i zmienisz jego pole?"*
→ Zmiana widoczna na zewnątrz (przekazujemy kopię referencji, ale wskazuje ten sam obiekt). Przy value type — nie, bo metoda dostaje kopię.

**class vs record vs struct:**

- `class` — referencyjny, porównywany przez **referencję** (czy to ten sam obiekt).
- `record` — referencyjny, ale porównywany przez **wartość** (czy pola równe). Immutable-friendly (`with`). Idealny do DTO, Value Objects, Command/Query.
- `struct` — wartościowy, małe lekkie dane bez tożsamości (np. punkt, pieniądze).

**string:**

- Referencyjny, ale **immutable** — każda "zmiana" tworzy nowy obiekt.
- Sklejanie w pętli = mnóstwo śmieci na stercie → `StringBuilder`.

> **Talking point:** *"W MealMind Command i Query to recordy — chcę porównania przez wartość i niezmienności, pasuje do CQRS."*

---

## Sekcja 2 — LINQ, IEnumerable vs IQueryable, deferred execution ⭐

(To podkreśliłeś — pewnie lubią o to pytać.)

**Deferred execution (odroczone wykonanie):**
LINQ (`.Where`, `.Select`, `.OrderBy`) **nie wykonuje się od razu** — buduje przepis. Realne wykonanie dopiero przy **materializacji**: `ToList()`, `ToArray()`, `First()`, `Count()`, `foreach`.

```csharp
var query = list.Where(x => x.IsActive);  // nic się nie dzieje, to tylko przepis
query = query.Where(x => x.Age > 18);      // dalej nic
var result = query.ToList();               // DOPIERO TERAZ przechodzi po danych
```

**Pułapka którą wymieniłeś:** dopóki zostawiasz `IEnumerable`/`IQueryable`, zapytanie się "nie wyzwoliło". Wyzwala się dopiero gdy zaczynasz iterować (pętla / `ToList`). Dlatego:

- ❌ `ToList()` za wcześnie → ściągasz wszystko do pamięci, reszta filtrów leci w RAM (wolno).
- ✅ Trzymaj `IQueryable` jak najdłużej, materializuj na końcu.

**IEnumerable vs IQueryable — kluczowa różnica:**

| | `IEnumerable<T>` | `IQueryable<T>` |
|---|---|---|
| Gdzie działa | w **pamięci** (LINQ to Objects) | tłumaczone na **SQL** (LINQ to Entities) |
| `.Where()` | filtruje w C# po pobraniu | dokłada `WHERE` do SQL-a |
| Typowo | kolekcje w RAM | `DbSet` z EF Core |

**Pułapka N+ pamięć:**

```csharp
IEnumerable<User> users = ctx.Users;        // jako IEnumerable
var adults = users.Where(u => u.Age > 18);  // ❌ filtr w pamięci → SELECT * całej tabeli!
```

vs

```csharp
IQueryable<User> users = ctx.Users;         // jako IQueryable
var adults = users.Where(u => u.Age > 18);  // ✅ → WHERE Age > 18 w SQL
```

> **Talking point:** *"IQueryable buduje wyrażenie tłumaczone na SQL — filtruję w bazie. Jak rzutuję na IEnumerable za wcześnie, filtr poleci w pamięci i ściągnę całą tabelę."*

**`First` vs `FirstOrDefault` vs `Single`:**

- `First` — pierwszy, rzuca wyjątek jak pusto.
- `FirstOrDefault` — pierwszy albo `null`/default.
- `Single` — dokładnie jeden, rzuca jak jest 0 lub >1.

---

## Sekcja 3 — async / await

- `async/await` służy do operacji **I/O-bound** (baza, HTTP, dysk) — czekania na coś zewnętrznego.
- **NIE tworzy nowego wątku.** Wątek który czekałby bezczynnie na odpowiedź bazy **wraca do puli** i robi co innego. Gdy odpowiedź wraca — kod wznawia się (może na innym wątku z puli).
- Zysk: **skalowalność** — serwer obsługuje więcej żądań tym samym ograniczeniem wątków (nie marnuje ich na czekanie).
- `await` "rozpakowuje" `Task<T>` → `T` i wstrzymuje metodę bez blokowania wątku.

**Pułapki:**

- `async void` — tylko event handlery, nigdzie indziej (nie da się złapać wyjątku/awaitować).
- `.Result` / `.Wait()` — blokuje wątek, ryzyko deadlocka. Zawsze `await`.
- async to **nie** to samo co wielowątkowość/równoległość. To "nie blokuj wątku gdy czekasz".

> **Talking point:** *"W MealMind handlery są async — czekanie na EF Core czy Ollamę nie blokuje wątku, wraca do puli. To nie nowy wątek, tylko nieblokujące czekanie na I/O."*

---

## Sekcja 4 — OOP + SOLID (lekko, hasłowo)

**4 filary OOP:**

- **Enkapsulacja** — ukrywanie stanu, dostęp przez metody (`private set`, value objects).
- **Dziedziczenie** — `Document` → `RagDocument`/`ConversationDocument`.
- **Polimorfizm** — ta sama metoda, różne zachowanie (override / interfejs).
- **Abstrakcja** — interfejs mówi *co*, nie *jak* (`IRepository<T>`, `IEmbeddingService`).

**SOLID — wystarczy zdanie na literę + przykład z projektu:**

- **S** (Single Responsibility) — handler robi jedno: jeden command = jeden use case.
- **O** (Open/Closed) — nowy moduł `IModule` dodaję bez ruszania bootstrappera.
- **L** (Liskov) — `RagDocument` da się użyć wszędzie gdzie `Document`.
- **I** (Interface Segregation) — wąskie interfejsy (`IChunkingService`, `IEmbeddingService`) zamiast jednego molocha.
- **D** (Dependency Inversion) — handler zależy od `IRepository`, nie od konkretnej klasy EF. Wstrzykiwane przez DI.

**interfejs vs klasa abstrakcyjna:**

- Interfejs — kontrakt, **wiele** naraz, bez implementacji/stanu.
- Klasa abstrakcyjna — wspólny **kod + stan** dla pokrewnych klas, dziedziczysz **jedną**. (`Document` ma wspólne pola → abstrakcyjna.)

---

## Sekcja 5 — ASP.NET Core, Web API, DI

**Pipeline / middleware:** żądanie przechodzi przez łańcuch middleware (auth, logging, exception handling) → endpoint → odpowiedź wraca tą samą drogą.

**Dependency Injection — cykle życia:**

- **Singleton** — jedna instancja na całą apkę. Bezstanowe serwisy, konfiguracja.
- **Scoped** — jedna na żądanie HTTP. **DbContext jest scoped.**
- **Transient** — nowa za każdym pobraniem. Lekkie, bezstanowe.

**Captive dependency (pytanie-pułapka):** wstrzyknięcie **Scoped do Singletona** → scoped żyje tak długo jak singleton (czyli wiecznie). DbContext (scoped) w singletonie = współdzielony change tracker, brak thread-safety, wycieki. Dlatego DI to wyłapuje i rzuca błąd.

**REST / kody HTTP:**

- `200 OK`, `201 Created`, `204 No Content`
- `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`
- `500 Internal Server Error`
- Czasowniki: GET (pobierz), POST (utwórz), PUT (zastąp), PATCH (zmień częściowo), DELETE.

> **Talking point:** *"Endpointy w MealMind są cienkie — przyjmują Command/Query i oddają do MediatR przez ISender. Logika w handlerach, nie w kontrolerze."*

---

## Sekcja 6 — EF Core (skrót)

Pełna powtórka: **plik `Interview-Prep-SQL-EFCore.md`**. Najważniejsze do powiedzenia:

- **DbContext = Unit of Work + Identity Map.** Scoped. Śledzi zmiany (change tracker), `SaveChangesAsync` wysyła wszystko w **jednej transakcji** (atomowo).
- **Tracking vs `AsNoTracking`** — odczyt tylko do wyświetlenia → `AsNoTracking` (szybciej, bez śledzenia).
- **Projekcja do DTO** (`.Select(x => new Dto{...})`) — nie śledzi, nie trzeba `Include`, ściąga tylko potrzebne kolumny.
- **N+1** — pętla z lazy loadingiem = 1 + N zapytań. **Lek: `Include` albo projekcja.**
- **Migracje** — code-first, `Up`/`Down`, tabela `__EFMigrationsHistory`.
- **Owned types** — `Subscription`, `PersonalData` jako kolumny w tabeli właściciela (value object bez własnego Id).

---

## Sekcja 7 — Wzorce w MealMind (umiej wytłumaczyć SWOIMI słowami)

| Wzorzec | Gdzie w MealMind | Jednozdaniowa obrona |
|---|---|---|
| **Modular Monolith** | moduły Identity/Nutrition/AiChat | Jeden proces, jeden deploy, ale moduły odseparowane. Prościej niż mikroserwisy, a gdyby trzeba — łatwiej wydzielić. **Uwaga: jeden proces = brak izolacji awarii** (to mają mikroserwisy). |
| **Clean Architecture** | Api / Application / Domain / Infrastructure | Zależności wskazują **do środka, na Domain**. Domain nie wie o bazie ani API. |
| **CQRS** | Command / Query + MediatR | Rozdzielam zapis (Command) od odczytu (Query). Inne modele, inne potrzeby. |
| **Mediator (MediatR)** | `ISender.Send(command)` | Endpoint nie zna handlera — wysyła wiadomość, MediatR **routuje po typie** do właściwego handlera. Odpina nadawcę od odbiorcy. |
| **Repository** | `IRepository<T>`, `IdentityUserRepository` | Abstrakcja nad dostępem do danych — handler nie wie że pod spodem EF. |
| **Unit of Work** | `IUnitOfWork.CommitAsync` (DbContext) | Grupuje zmiany w jedną transakcję — albo wszystko, albo nic. |
| **Factory Method** | `IdentityUser.Create(...)`, `Password.Create(...)` | Statyczna fabryka pilnuje **niezmienników** przy tworzeniu — nie da się stworzyć obiektu w złym stanie. |
| **Value Object** | `Subscription`, `PersonalData`, `NutritionTarget` | Brak tożsamości, **immutable**, równość przez wartość. Update = `with` (nowa instancja). |
| **Observer / Pub-Sub** | integration events (`IdentityUserCreatedEvent`) | Moduł publikuje zdarzenie, inne **nasłuchują**. `Publish` = 0..wielu odbiorców, nadawca ich nie zna. |
| **Decorator** | pipeline behaviors MediatR (walidacja, logowanie) | Owijam handler dodatkową odpowiedzialnością bez zmiany jego kodu. |

**Pytanie-pułapka:** *"Czemu modularny monolit a nie mikroserwisy?"*
→ *"Jestem jednym deweloperem, projekt nie potrzebuje niezależnego skalowania ani deployu. Monolit modularny daje mi czyste granice modułów bez kosztu sieci, rozproszonych transakcji i devops. Gdyby moduł urósł — granice są gotowe do wydzielenia."*

---

## Sekcja 8 — Obrona projektu MealMind ⭐ (serce rozmowy)

Przygotuj **30-sekundowy pitch**:
> *"MealMind to aplikacja do żywienia i treningu, modularny monolit w .NET. Trzy moduły: Identity — logowanie i subskrypcje przez Stripe; Nutrition — profile i cele; AiChat — czat z AI opartym o RAG, gdzie model odpowiada w oparciu o dane użytkownika. Architektura clean + CQRS przez MediatR, EF Core na PostgreSQL z pgvector do embeddingów."*

**Pewnie zapytają — przygotuj odpowiedzi:**

1. **"Opowiedz jak działa Twój RAG."**
   → Dokument → **chunking** (dzielę na kawałki) → **embedding** (każdy chunk na wektor **1024-wym przez Ollamę lokalnie**) → zapis do **pgvector**. Pytanie użytkownika embeduję tym samym modelem i szukam **podobieństwem kosinusowym (cosine similarity)** najbliższych chunków → wrzucam je jako **kontekst** do system promptu → **Gemini** (przez Semantic Kernel, interfejs `IChatCompletionService`) odpowiada z danych użytkownika, a nie zmyśla. Embedding = zamiana **znaczenia** tekstu na wektor liczb; podobny sens → bliskie wektory.
   **LLM przez `IChatCompletionService`** = łatwa podmiana providera (Gemini ↔ inny) bez ruszania reszty. Flow: `GenerateStructuredResponseAsync` → system prompt + user prompt jako `ChatMessage` → wysyłka → odbiór response → serializacja do typowanej odpowiedzi.
   **Czemu chunking:** za duży dokument = rozmyty (kontekst nieostry), za mały = za mało treści w temacie.

2. **"Po co RAG, czemu nie sam LLM?"**
   → LLM nie zna prywatnych danych użytkownika i **halucynuje**. RAG daje mu konkretny kontekst (dieta, treningi) → odpowiedzi oparte na faktach. Tańsze i szybsze niż fine-tuning, i dane są aktualne.

3. **"Najtrudniejsza rzecz w projekcie?"**
   → (wybierz prawdziwą — np. webhooki Stripe: daty okresu rozliczeniowego są w obiekcie `Invoice`, nie `Subscription`; trzeba weryfikować podpis webhooka; obsłużyć `checkout.session.completed`, `subscription.updated`, `subscription.deleted`.)

4. **"Czemu modularny monolit / clean architecture?"** → patrz Sekcja 7.

5. **"Co byś poprawił / co dalej?"**
   → Testy (TUnit — w planach), CI/CD, cache, lepsza obsługa błędów. **Pokaż samoświadomość** — to robi dobre wrażenie.

6. **"Jak komunikują się moduły?"**
   → Integration events (pub/sub). Np. Identity publikuje `IdentityUserCreatedEvent`, Nutrition nasłuchuje i tworzy profil. Luźne powiązanie — moduły się nie znają bezpośrednio.

**Złota zasada:** mów o **decyzjach i kompromisach**, nie tylko "użyłem X". I bądź szczery o tym czego jeszcze nie zrobiłeś (testy, prod).

---

## Sekcja 9 — Pytania behawioralne + Twoje pytania do nich

**Mogą zapytać:**

- "Czemu chcesz u nas pracować / czemu .NET?"
- "Jak się uczysz nowych rzeczy?" → kursy DevMentors, dokumentacja, budowanie MealMind w praktyce.
- "Pracowałeś w zespole / z Gitem?" → bądź szczery; pokaż że znasz flow (branch, commit, PR).
- "Gdzie się widzisz za rok?"

**Ty zapytaj ich (zawsze miej 2-3):**

- "Jak wygląda dzień praktykanta — nad czym bym pracował?"
- "Jakiego stacku używacie w projektach? (.NET, front, baza?)"
- "Czy jest mentoring / code review dla praktykantów?"
- "Od czego zależy przejście z praktyk na stałe?"

---

## Sekcja 11 — DDD (Domain-Driven Design) — na tym stoi MealMind

**Co to jest:** podejście do modelowania kodu **wokół domeny biznesowej** (a nie wokół bazy/technologii). Kod ma odzwierciedlać język i reguły biznesu.

**Klocki taktyczne (musisz znać):**

| Pojęcie | Co to | Przykład w MealMind |
|---|---|---|
| **Entity** | obiekt z **tożsamością (Id)**, rozróżniany po Id nie po wartościach | `IdentityUser`, `Document` |
| **Value Object** | bez Id, równość **po wartości**, immutable | `Subscription`, `PersonalData`, `Email`, `Password` |
| **Aggregate** | grupa encji+VO traktowana jako **jedna całość** spójności | `IdentityUser` + jego `Subscription` |
| **Aggregate Root** | "brama" do agregatu — świat z zewnątrz dotyka **tylko roota**, on pilnuje reguł | `IdentityUser` (do Subscription dostajesz się przez usera) |
| **Domain Event** | "coś ważnego się stało w domenie" | `IdentityUserCreatedEvent` |
| **Repository** | jedno repo **na agregat root** (nie na każdą tabelę) | `IdentityUserRepository` |
| **Invariant** | reguła biznesowa która **zawsze** musi być prawdziwa | "user ma zawsze poprawny email", pilnuje `Create()` |

**Kluczowe idee na rozmowę:**
- **Aggregate Root pilnuje niezmienników** — nie zmieniasz `Subscription` bezpośrednio, tylko przez metodę na `IdentityUser`. Dzięki temu obiekt nie wpadnie w niepoprawny stan.
- **Bogata domena (rich domain), nie anemiczna** — logika siedzi w encjach/VO (metody), a nie w "serwisach" operujących na gołych property. Encja sama pilnuje swoich reguł.
- **Ubiquitous language** — te same nazwy w kodzie co w biznesie (`Subscription`, `NutritionTarget`), żeby kod i rozmowa o nim mówiły tym samym językiem.
- **Bounded Context** — wyraźna granica gdzie dany model obowiązuje. W MealMind ~ pokrywa się z **modułami** (Identity, Nutrition, AiChat) — każdy ma swój model, swoje pojęcia.

> **Talking point:** *"MealMind buduję zgodnie z DDD — encje mają tożsamość, value objecty jak Subscription są bez Id i niezmienne, a IdentityUser jako aggregate root pilnuje niezmienników. Logika siedzi w domenie (rich domain), nie w anemicznych serwisach. Moduły to w praktyce bounded contexty."*

**Pytanie-pułapka:** *"Czemu nie zmieniasz Subscription bezpośrednio?"*
→ *"Bo to część agregatu IdentityUser. Zmiana idzie przez roota, który pilnuje reguł — inaczej mógłbym zostawić usera w niespójnym stanie. Plus Subscription to value object, więc i tak jest immutable — update zwraca nową instancję."*

---

## Sekcja 12 — Docker (odśwież — używasz go w MealMind)

**Po co:** spakować apkę + zależności (baza, Ollama) tak, żeby **działały tak samo wszędzie** — "u mnie działa" przestaje być problemem. Zamiast ręcznie instalować PostgreSQL i Ollamę, odpalasz kontenery.

**Podstawowe pojęcia (musisz rozróżnić obraz od kontenera):**

| Pojęcie | Co to | Analogia |
|---|---|---|
| **Image (obraz)** | szablon tylko-do-odczytu: apka + zależności + konfiguracja | przepis / klasa |
| **Container (kontener)** | **uruchomiona instancja** obrazu, izolowany proces | danie z przepisu / obiekt (`new`) |
| **Dockerfile** | przepis jak **zbudować** obraz (krok po kroku) | — |
| **docker-compose.yml** | orkiestracja **wielu** kontenerów naraz + ich konfiguracja | — |
| **Volume** | trwałe miejsce na dane **poza** cyklem życia kontenera | dysk zewnętrzny |
| **Port mapping** | `5432:5432` = port hosta : port w kontenerze | przekierowanie |

**W MealMind (docker-compose):**
- **PostgreSQL + pgvector** — baza z rozszerzeniem do wektorów (embeddingi RAG), port `5432`.
- **Ollama** — lokalny LLM + embeddingi, port `11434`.
- Kontenery gadają ze sobą po **nazwie serwisu** (np. `http://mealmind.ollama:11434`), nie po `localhost` — to wewnętrzna sieć Dockera.
- **Volume** trzyma dane bazy, żeby `docker-compose down` ich nie skasował.

**Komendy które używasz:**
```bash
docker-compose up -d        # odpal wszystko w tle (-d = detached)
docker-compose down         # zatrzymaj i usuń kontenery
docker exec -it <nazwa> ... # wejdź do działającego kontenera (np. ollama pull model)
docker ps                   # lista działających kontenerów
```

> **Talking point:** *"Bazę i Ollamę odpalam w Dockerze przez docker-compose — jedna komenda stawia PostgreSQL z pgvector i model. Obraz to szablon, kontener to jego uruchomiona instancja. Dane bazy trzymam na volume, żeby przetrwały restart kontenera. W trybie kontenerowym serwisy gadają po nazwach, nie po localhost."*

**Jak nie wiesz szczegółu:** *"Konfigurację compose mam ustawioną, ale na co dzień korzystam z gotowego — gdyby trzeba, doczytam w dokumentacji."* — szczerze i bez wstydu, to praktyki.

---

## Sekcja 10 — Ściąga na dzień przed (przeczytaj rano)

- **Value vs reference:** wartościowy = kopia wartości (stack), referencyjny = kopia adresu, ten sam obiekt (heap, GC).
- **record** = porównanie przez wartość, immutable. **string** = referencyjny ale immutable.
- **LINQ deferred:** przepis aż do `ToList`/`foreach`. **IQueryable** → SQL, **IEnumerable** → pamięć. Nie materializuj za wcześnie.
- **async:** nie nowy wątek — nieblokujące czekanie na I/O, wątek wraca do puli.
- **DI:** Singleton / Scoped (DbContext!) / Transient. Scoped w Singletonie = captive dependency.
- **EF:** DbContext = UoW + change tracker, `SaveChanges` = transakcja. N+1 → `Include`/projekcja. `AsNoTracking` do odczytu.
- **CQRS:** Command (zapis) vs Query (odczyt), MediatR routuje po typie.
- **Modularny monolit:** jeden proces, granice modułów, brak izolacji awarii.
- **RAG pitch:** chunk → embedding (1024d, Ollama lokalnie) → pgvector → cosine similarity → kontekst do system promptu → **Gemini** (przez IChatCompletionService) → mniej halucynacji. Embedding = znaczenie tekstu jako wektor.
- **DDD:** Entity = ma Id, VO = bez Id + immutable, Aggregate Root pilnuje niezmienników (Subscription zmieniam przez IdentityUser). Rich domain, nie anemiczna.
- **Docker:** obraz = szablon (read-only), kontener = uruchomiona instancja. compose stawia PostgreSQL+pgvector i Ollamę. Volume = trwałe dane. Serwisy gadają po nazwie.
- **Jak nie wiem:** "nie miałem styczności, ale myślę że…". Nie zmyślam.

**Trzy zdania na pamięć:**

1. *"Endpointy cienkie, logika w handlerach, MediatR routuje po typie wiadomości."*
2. *"IQueryable tłumaczy się na SQL — filtruję w bazie, materializuję na końcu."*
3. *"RAG: embeduję dane i pytanie, szukam kosinusowo, wrzucam kontekst do LLM — model nie zmyśla."*
