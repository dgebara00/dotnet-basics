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