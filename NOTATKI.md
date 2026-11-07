## Zadanie 0
1. Ile spółek zwraca lista i ile wynosi totalCount? - 8
2. Jaki nowy plik pojawił się w src/GieldaApi po pierwszym uruchomieniu? - Pojawił się plik z bazą danych gielda.db 
3. Zatrzymaj API (Ctrl+C), usuń ten plik i uruchom ponownie. Co się stało z danymi? - Przy restarcie projektu migracja znowu się odpali, poleci seed z DataSeeder i powstanie nowa baza danych

parametr --project przy dotnet run..., czy dotnet ef migrations add InitialCreate... pozwala wskazać gdzie znajduje się plik .csproj bez fizycznego bycia w tym katalogu

## Zadanie 1
1. Który plik uruchamia się jako pierwszy i co robi? - 1 krokiem jest kompilacja aplikacji na binarkę, do tego potrzebuje pliku GieldaApi.csproj, tam ma wszystkie dependency, jakich bibliotek wymaga itd... Jeśli wszystko jest ok, to następnym krokiem jest Program.cs, który zawiera w sobie instrukcje tzn. dodajemy kontrolery, kontekst db, standard rzucanych błędów, wystawiamy tez endpoint dla postmana (openapi) , no i przede wszystkim startujemy server
2. Gdzie jest adres bazy danych (connection string)? - w appsettings.json
3. Czym różni się Domain/Company.cs od CompanyResponse w Contracts/ ? Po co dwie klasy o prawie tych samych polach? - Przede wszystkim tym, ze GieldaApi.Domain to encje, na których podstawie jest tworzona migracja i właściwości są wdrazane do kolumn. CompanyResponse to Record, który słuzy do tego, zeby otypować response w kontrolerze/serwisie.
4. W którym pliku jest zapisane, że ticker ma maksymalnie 10 znaków i musi być unikalny? - w CompanyConfiguration.cs tutaj jest zapisana cała konfiguracja poszczególnych kolumn
5. pierwszy jest kontroler, następnie serwis, który ma kontekst bazy danych i w ostatnim kroku są zwracane dane z tej bazy.