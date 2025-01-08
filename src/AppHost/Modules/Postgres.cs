namespace AppHost.Modules;

internal static class Postgres
{
    public static IResourceBuilder<PostgresDatabaseResource> AddPostgressContainer(this IDistributedApplicationBuilder builder)
    {
        var postgresPassword = builder.AddParameterFromConfiguration("pwd", "postgres:password", true);
        var postgres = builder.AddPostgres("customers-db", password: postgresPassword, port: 5432)
            .WithImage("postgres")
            .WithImageTag("16.4")
            .WithLifetime(ContainerLifetime.Persistent);
        return postgres.AddDatabase("customers");
    }
}
