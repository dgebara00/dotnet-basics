## Zadanie 0
1. 8 
2. gielda.db
3. dopóki korzystamy z api lokalnie to przy kazdym starcie servera będzie odpalana migracja - odpowiada za to `await db.Database.MigrateAsync();`

## Zadanie 1
1. pierwszy plik po uruchomieniu dotnet run odpala się GieldaApi.csproj - tam mamy wszystkie zalezności projektu pobieranie i injectowane do projektu i wtedy kod jest kompilowany a następnie odpalany jest Program.cs (punkt startowy aplikacji - to tutaj odpalamy server, dodajemy controllery, globalny handler error'ów, dependency injection, dodajemy kontekst bazy danych, mapujemy api, zeby pliki były dostępne dla postmana, ...itd)
2. w appsettings.json
3. Domain/Company to kształ† encji, mamy tutaj wszystkie właściwości, na których podstawie przy seedzie są tworzone kolumny, tak jak komentarz mówi, nigdy tego nie zwracamy bezpośrednio w API - zmiana w encji nie powoduje zmian w response. Natomiast Contracts/CompanyContracts –> CompanyResponse zawiera jak sama nazwa wskazuje typ odpowiedzi, czy to z serwisu, czy z kontrolera - jest to typ, który zapewnia stały kształt odpowiedzi na ten endpoint, dzięki niemu mamy podpowiedzi w IDE + jeśli coś by się nie zgadzało to aplikacja by się nie skompilowała
4. Data/Configurations/CompanyConfiguration.cs - w Program.cs mamy "przepis" na połączenie z bazą danych, który wykorzystuje AppDbContext klasę. Dopiero przy wywołaniu któregoś z kontrolera, który potrzebuje kontekst bazy danych EF Core odpala pod spodem metodę `OnModelCreating`, która pozwala na komunikację z fizyczną bazą danych, następnie tworzone są instancje bazy i są wtłaczane do klas serwisów.
5. Tak jak napisałem w 4 zadaniu, na początku jest przepis w Program.cs, ale to wywołanie kontrolera decyduje o stworzeniu realnej klasy bazy danych. Czyli najpierw kontroler, później serwis, następnie kontekst i sama baza.