# GieldaApi

Małe REST API w ASP.NET Core do nauki backendu w .NET: spółki z GPW i ich notowania.
Dane startowe są przykładowe (zmyślone liczby).

**Stack:** .NET 10, ASP.NET Core (kontrolery), Entity Framework Core 10, SQLite, OpenAPI.

## Uruchomienie (pierwszy raz)

Wymagane: .NET 10 SDK. Sprawdź: `dotnet --version` (musi zaczynać się od `10.`).
Jeśli masz starszy: `brew install --cask dotnet-sdk` albo instalator z dot.net.

W głównym folderze repo:

```bash
# 1. Narzędzie do migracji (wersja przypięta w .config/dotnet-tools.json)
dotnet tool restore

# 2. Wygeneruj pierwszą migrację na podstawie modelu
dotnet ef migrations add InitialCreate --project src/GieldaApi --output-dir Data/Migrations

# 3. Uruchom API
dotnet run --project src/GieldaApi
```

Przy starcie (tylko w trybie Development) aplikacja sama nakłada migracje i wrzuca dane startowe.
Baza to plik `src/GieldaApi/gielda.db`. Żeby zacząć od zera: zatrzymaj API i usuń ten plik.

API działa pod **http://localhost:5080**.

Podgląd w terminalu: `curl http://localhost:5080/api/companies`

## Postman

1. **Import** → plik `postman/GieldaApi.postman_collection.json`.
2. Zmienne kolekcji: `baseUrl` (domyślnie `http://localhost:5080`) i `ticker` (domyślnie `PKO`).
3. Kolekcja ma testy kodów odpowiedzi – możesz odpalić całość przez **Run collection**.

Dokument OpenAPI: `http://localhost:5080/openapi/v1.json` – Postman też potrafi go zaimportować.

## Endpointy

| Metoda | Ścieżka | Co robi | Odpowiedzi |
| --- | --- | --- | --- |
| GET | `/api/companies?sector=&page=&pageSize=` | lista ze stronicowaniem i filtrem | 200, 400 |
| GET | `/api/companies/{ticker}` | szczegóły + ostatnie notowanie | 200, 404 |
| POST | `/api/companies` | dodaje spółkę | 201, 400, 409 |
| PUT | `/api/companies/{ticker}` | zmienia nazwę i sektor | 204, 400, 404 |
| DELETE | `/api/companies/{ticker}` | usuwa spółkę razem z notowaniami | 204, 404 |
| GET | `/api/companies/{ticker}/quotes?from=&to=` | notowania, opcjonalnie z zakresu dat | 200, 404 |
| POST | `/api/companies/{ticker}/quotes` | dodaje dzienne notowanie | 201, 400, 404, 409 |

Błędy wracają w standardowym formacie **ProblemDetails**, np.:

```json
{ "title": "Nie znaleziono", "status": 404, "detail": "Spółka XYZ nie istnieje." }
```

## Struktura

```
GieldaApi.slnx                    plik solucji (nowy format XML)
.config/dotnet-tools.json         przypięta wersja dotnet-ef
postman/                          kolekcja do Postmana
src/GieldaApi/
├── Program.cs                    rejestracja usług (DI) i pipeline żądania
├── appsettings*.json             konfiguracja, connection string
├── Controllers/                  HTTP: przyjmuje żądanie, woła serwis, zwraca kod
├── Contracts/                    DTO – kształt danych wejścia/wyjścia API + walidacja
├── Services/                     logika biznesowa i zapytania do bazy
├── Domain/                       encje (tabele w bazie)
├── Data/
│   ├── AppDbContext.cs           połączenie z bazą (EF Core)
│   ├── Configurations/           konfiguracja tabel: długości pól, indeksy, relacje
│   ├── DataSeeder.cs             dane startowe
│   └── Migrations/               pojawi się po "dotnet ef migrations add"
└── Common/Errors/                wyjątki aplikacji i globalna obsługa błędów
```

Przepływ żądania: **Controller → Service → AppDbContext → SQLite**, a w drugą stronę encja jest zamieniana na DTO.

## Migracje – ściąga

```bash
# nowa migracja po zmianie encji lub konfiguracji
dotnet ef migrations add NazwaZmiany --project src/GieldaApi --output-dir Data/Migrations

# nałóż migracje ręcznie (aplikacja robi to też sama przy starcie w Development)
dotnet ef database update --project src/GieldaApi

# cofnij ostatnią migrację, jeśli NIE była jeszcze nałożona
dotnet ef migrations remove --project src/GieldaApi

# podgląd SQL, który wygenerują migracje
dotnet ef migrations script --project src/GieldaApi
```

## Dobrze wiedzieć

- **SQLite i `decimal`:** SQLite nie umie sortować ani sumować kolumn `decimal` po stronie bazy.
  Dlatego `OrderBy(q => q.Close)` w zapytaniu EF rzuci wyjątek. Przy SQL Serverze czy Postgresie tego problemu nie ma.
- **Logi SQL:** w Development każde zapytanie do bazy widać w konsoli – dobry sposób, żeby zobaczyć, na co LINQ zamienia się w SQL.
- **CancellationToken:** jest przekazywany wszędzie. Gdy klient przerwie żądanie, zapytanie do bazy też zostanie przerwane.
