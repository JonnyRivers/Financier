# Financier

To apply DbContext updates do the following:

1. Make changes to the Entities types in Financier.Core
2. Financier.Web> dotnet ef migrations add <MIGRATION_NAME> --project ..\Financier.Core
3. Eyeball the migration's Up and Down methods
4. Financier.Web> dotnet ef database update <MIGRATION_NAME> --project ..\Financier.Core