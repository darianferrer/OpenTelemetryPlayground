using System.ComponentModel.DataAnnotations;

namespace EmailSender;

public record RabbitMqConfig
{
    public const string Postition = "RabbitMqConfig";

    [Required]
    public required string Host { get; set; }
    [Required]
    public required string Username { get; set; }
    [Required] 
    public required string Password { get; set; }
};