# pulaVocab

pulaVocab es una aplicación para aprender vocabulario en inglés y alemán.

## Tecnologías

- .NET 10
- ASP.NET Core Web API
- Blazor WebAssembly
- PostgreSQL
- Entity Framework Core
- xUnit

## Estructura de la solución

- pulaVocab.Domain
- pulaVocab.Application
- pulaVocab.Infrastructure
- pulaVocabApi
- pulaVocab.Client
- pulaVocab.Shared
- pulaVocab.Tests

## Configuración de la base de datos

Crea una base de datos PostgreSQL local y configura la cadena de conexión con user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=vocabmaster;Username=postgres;Password=TU_CLAVE" --project src/pulaVocabApi
```

### Migraciones de Entity Framework Core

```powershell
dotnet ef migrations add InitialVocabularySchema --project src/pulaVocab.Infrastructure --startup-project src/pulaVocabApi
dotnet ef database update --project src/pulaVocab.Infrastructure --startup-project src/pulaVocabApi
```

## Compilar

```bash
dotnet build pulaVocab.sln
```

## Ejecutar la API

```bash
dotnet run --project src/pulaVocabApi/pulaVocabApi.csproj
```

## Ejecutar el cliente Blazor

```bash
dotnet run --project src/pulaVocab.Client/pulaVocab.Client.csproj
```

## Ejecutar pruebas

```bash
dotnet test tests/pulaVocab.Tests/pulaVocab.Tests.csproj
```
