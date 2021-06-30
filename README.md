# ProjectManagerAPI
## Net Core API, MS SQL Server

###Configurations
```
Config your ConnectionStrings in appsettings.json
  LocalDB - your local SQL Server
```

###Migration
- Select on the menu: Tools -> NuGet Package Manager -> Package Manager Console and execute the following commands:
```
PM> add-migration <name of the new migration>
```
```
PM> update-database –verbose
```
- For dotnet CLI:
```
> dotnet ef migrations add <name of the new migration>
```
```
dotnet ef database update
```
