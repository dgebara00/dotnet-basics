## Zadanie 0
1. Ile spółek zwraca lista i ile wynosi totalCount? - 8
2. Jaki nowy plik pojawił się w src/GieldaApi po pierwszym uruchomieniu? - Pojawił się plik z bazą danych gielda.db 
3. Zatrzymaj API (Ctrl+C), usuń ten plik i uruchom ponownie. Co się stało z danymi? - Przy restarcie projektu migracja znowu się odpali i powstanie baza danych

parametr --project przy dotnet run..., czy dotnet ef migrations add InitialCreate... pozwala wskazać gdzie znajduje się plik .csproj bez fizycznego bycia w tym katalogu