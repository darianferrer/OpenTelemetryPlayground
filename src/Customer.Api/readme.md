### Database migrations

This project uses EntityFramework Core for the data access. Migrations are created using the command below when there's a new model change:

```
dotnet ef migrations add {name-of-migration}
```

### Database schema update

To update the database schema, execute this command using the `postgres` password stored in `src/AppHost/appsettings.Development.json`:

```
dotnet ef database update --connection "Persist Security Info=True;Password=*;Username=postgres;Host=localhost;Database=customer"
```

Alternatively, you can run `AppHost` and `Customer.Api` will execute the migrations when running on `Development` environment.