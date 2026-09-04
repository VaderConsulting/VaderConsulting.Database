# VaderConsulting.Database

C# .NET Framework 3.5 class library with SQLServer and SQLite wrappers for connections, queries, and non-query commands. Both types expose ConnectionString, Connection, and ConnectionResult, plus GetImage which selects PagePreviewImage from a VisioPagePreviews table by ObjectID. SQLite retries Execute up to three times with a constructor-set query timeout; SQLServer fills a DataSet via SqlDataAdapter with MissingSchemaAction.AddWithKey. The .csproj compiles only SQLServer.cs (SQLite.cs is present but not listed); leftover App.config Entity Framework 6 LocalDB and packages.config AsyncBridge 0.1.1 are not referenced by the project.

**Source last updated:** 2015-02-09 · **Language:** C# · **Target:** .NET Framework 3.5 · **Output:** class library (`Library`)

## Solution structure

| Project | Language | Type | Purpose |
|---------|----------|------|---------|
| `VaderConsulting.Database` | C# | class library (`net35`) | `SQLServer` and `SQLite` connection/query helpers, including `GetImage` from `VisioPagePreviews`. |

## How to open

Open `VaderConsulting.Database.csproj` in Visual Studio 2013 or later (ToolsVersion 12.0). There is no `.sln` in this folder. Add a reference to System.Data.SQLite if you compile `SQLite.cs`; it is not listed in the `.csproj`.

## Requirements

- Visual Studio 2013 or later, .NET Framework 3.5

## Attribution and provenance

Working copy from Dave Robinson's OneDrive Historical Dev folder `VaderConsulting.Database`. Assembly title/product `VaderConsulting.Database`; copyright `Copyright ©  2015`; company empty. Namespace `VaderConsulting.Database`. `packages.config` lists AsyncBridge 0.1.1 (not referenced by the `.csproj`). `App.config` has leftover Entity Framework 6 LocalDB section, not referenced by the project.

## License

MIT © 2026 VaderConsulting. See `LICENSE`.
