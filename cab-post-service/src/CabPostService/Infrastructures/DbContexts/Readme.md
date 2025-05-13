# Step to create migration 
Reference https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli
Upgrade command
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```
- dotnet ef migrations add {InitialProject} --context ApplicationDbContext
- 
dotnet ef migrations add FixNullIsImageUrlPostEntity --context PostgresDbContext
