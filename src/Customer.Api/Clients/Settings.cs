using System.ComponentModel.DataAnnotations;

namespace Customer.Api.Clients;

public class ClientSettings
{
    public const string Position = nameof(ClientSettings);

    [Required]
    public required ClientSetting FraudCheckApi { get; init; }
}

public record ClientSetting
{
    [Required]
    public required Uri BaseAddress { get; init; }

    public required TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
}
