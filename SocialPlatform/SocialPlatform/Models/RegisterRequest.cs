namespace SocialPlatform.Models;

public class RegisterRequest
{
    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Handle { get; set; }

    public string? DisplayName { get; set; }
}
