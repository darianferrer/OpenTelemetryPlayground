using Customer.Api.Domain;
using FraudCheck.Contracts.Api;
using Refit;

namespace Customer.Api.Clients.FraudCheck;

public interface IFraudCheckClient
{
    [Post("/api/fraudcheck")]
    internal Task<HttpResponseMessage> FraudCheckInternalAsync(
        [Body] CustomerVerificationContract contract,
        CancellationToken stopToken);
    
    public async Task<FraudCheckResponse> FraudCheckAsync(NewCustomer newCustomer, CancellationToken stopToken)
    {
        var response = await FraudCheckInternalAsync(Map(newCustomer), stopToken);
        return response.IsSuccessStatusCode switch
        {
            true => FraudCheckResponse.GoodToGo,
            _ => FraudCheckResponse.PossibleFraud,
        };

        static CustomerVerificationContract Map(NewCustomer newCustomer)
            => new(newCustomer.Email);
    }
}

public enum FraudCheckResponse
{
    PossibleFraud,
    GoodToGo,
}
