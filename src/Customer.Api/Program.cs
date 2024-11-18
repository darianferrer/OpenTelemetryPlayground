using Customer.Api.Clients;
using Customer.Api.Endpoints;
using Customer.Api.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomerServices().AddMessaging().AddClients();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.MigrateDatabaseAsync();
}

app.MapCustomerEndpoints();

app.Run();
