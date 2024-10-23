﻿namespace Customer.Api.Data;

public record CustomerEntity
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Title { get; init; }
}
