using System.Net;
using System.Net.Http.Json;
using Customer.Api.FunctionalTests.Server;
using Customer.Contracts.Api;

namespace Customer.Api.FunctionalTests.Features.Customer.Api;

[Collection(nameof(AspireHostFactoryCollection))]
public class CreateTentantFeature
{
    private readonly AspireHostFactory _appHost;

    public CreateTentantFeature(AspireHostFactory appHost)
    {
        _appHost = appHost;
    }

    [Fact]
    public async Task GivenNewTenant_WhenCreatingWithValidData_ShouldReturnCreated201()
    {
        // Arrange
        var httpClient = _appHost.CreateHttpClient("customer-api");

        var request = new TenantContract("test");

        // To output logs to the xUnit.net ITestOutputHelper, 
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        // Act
        var response = await httpClient.PostAsJsonAsync(
            "/api/tenants",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GivenNewTenant_WhenCreatingWithTooLongId_ShouldReturnBadRequest400()
    {
        // Arrange
        var httpClient = _appHost.CreateHttpClient("customer-api");

        var request = new TenantContract("tenant-id-very-very-long");

        // To output logs to the xUnit.net ITestOutputHelper, 
        // consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        // Act
        var response = await httpClient.PostAsJsonAsync(
            "/api/tenants",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
