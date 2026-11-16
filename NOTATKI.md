## Zadanie 0
1. 8 
2. gielda.db
3. Dane znikają, ale dopóki korzystamy z aplikacji w trybie development to przy kazdym starcie servera będzie odpalana migracja - odpowiada za to `await db.Database.MigrateAsync();` i powstaje nowa baza.

## Zadanie 1
1. pierwszy plik po uruchomieniu dotnet run odpala się GieldaApi.csproj - tam mamy wszystkie zalezności projektu, które są konieczne do działania aplikacji i wtedy kod jest kompilowany a następnie odpalany jest Program.cs (punkt startowy aplikacji - to tutaj odpalamy server, dodajemy controllery, globalny handler error'ów, dependency injection, dodajemy kontekst bazy danych, wystawiamy endpoint /openapi dla postmana, ...itd)
2. w appsettings.json
3. Domain/Company to kształ† encji, mamy tutaj wszystkie właściwości, na których podstawie przy seedzie są tworzone kolumny, tak jak komentarz mówi, nigdy tego nie zwracamy bezpośrednio w API - zmiana w encji nie powoduje zmian w response. Natomiast Contracts/CompanyContracts –> CompanyResponse zawiera jak sama nazwa wskazuje typ odpowiedzi, czy to z serwisu, czy z kontrolera - jest to typ, który zapewnia stały kształt odpowiedzi. W tym przypadku rózni się tez kształtem. Mając dwie klasy łatwiej zapobiec wycieknięciu prywatnych pól.
4. Data/Configurations/CompanyConfiguration.cs - `builder.Property(c => c.Ticker).HasMaxLength(10).IsRequired();` i `builder.HasIndex(c => c.Ticker).IsUnique();`.
5. Kolejność wywołania Kontroler -> Serwis -> DbContext -> SQLite
Kontroler potrzebuje serwisu, serwis potrzebuje kontekstu bazy - wtedy odpalane jest `OnModelCreating`, które tworzy model bazy danych na podstawie encji i ich konfiguracji. Następnie tworzony jest serwis z kontekstem bazy i później kontroler z konteksem serwisu. Model jest stworzony na początku i jest przechowywany w pamięci do końca zycia aplikacji. Później tworzone są juz tylko instancje klas przy kazdym ządaniu, ale model jest ten sam.


dotnet-tools.json - to plik, który zawiera zewnętrzne paczki, niepotrzebne do działania aplikacji, a bardziej dla developera, tzn. prettier, ef do migracji, itd...
GieldaApi.csproj - to plik, który jest czytany przy starcie aplikacji, tutaj są wszystkie zalezności potrzebne do kompilacji i działania projektu.

## Zadanie 2
1. CompaniesController –> GetByTicker - ASP.NET wie dokładnie, ze to tę metodę ma wywołać po linijce `[HttpGet("{ticker}", Name = nameof(GetByTicker))]` gdzie mapujemy metodę do requesta typu 'GET' z parametrem `ticker` i następnie nadajemy na ten endpoint unikalną nazwę, mozemy to wykorzystać np. przy `POST` kiedy zwracamy nagłówek Location w headerze.
2. GetByTickerAsync.
3. Zadziała, `var normalized = Normalize(ticker);` metoda Normalize powoduje, ze zawsze zamieniamy ticker na duze litery i wycinamy białe znaki. Dodatkowo pod spodem działa `ToUpperInvariant` metoda, która działa wg. reguł języka angielskiego, co istotne, zwykłe `ToUpper` mogłoby spowodować rózny zapis w zalezności od regionu uzytkowania aplikacji. Przy bazach danych powinno się korzystać z tej odmiany.
4. Najpierw szukamy w tabeli Companies po znormalizowanym tickerze i bierzemy FirstOrDefaultAsync, jako, ze nie znalazło to default dla klasy to null. Następnie mamy operator null coalescing i rzucamy błąd dokładnie taki jaki otrzymujemy w response `?? throw new NotFoundException($"Spółka {normalized} nie istnieje.");`, później ten błąd jest złapany przez GlobalExceptionHandler, który jest zarejestrowany w Program.cs i na podstawie szczegółów błędu ustawia odpowiedni response.
5. Dlatego, ze FirstOrDefaultAsync jak sama nazwa wskazuje zwraca null jeśli nie znajdzie spółki, a jeśli tak się stanie to serwis rzuca `NotFoundException` i następnie GlobalExceptionHandler handluje te wyjątki i zwraca odpowiednią odpowiedź. Aplikacja korzysta z patternu ErrorBoundary.

## Zadanie 3
2 a. Nie ma to większego znaczenia ile parametrów będzie, zawsze będą wykonane 2 zapytania do bazy danych, najpierw jedno o total count, a później o konkretne firmy, które spełniają warunki. Metoda z 'Async' tzn. `await companies.CountAsync(ct)` powoduje, ze jest triggerowane zapytanie do bazy, a później ToListAsync.
2 b. `.Where(c => c.Sector == ...)` to `WHERE "c"."Sector" = @query_Sector` a `.Skip(...).Take(...)` to `LIMIT @p0 OFFSET @p`
2 c. `.Select(c => new CompanyResponse(c.Ticker, c.Name, c.Sector, c.CreatedAtUtc))` Zapytanie jest mapowane na CompanyResponse DTO przez co zwraca tylko te kolumny, które potrzebuje kontroler.
2 d. Tak samo jak poprzedni endpoint, tutaj tez dwa razy robimy zapytanie, najpierw o spółkę, która jest koniecznym warunkiem do spełnienia, a następnie o najświezsze notowanie, które moze być nullem.