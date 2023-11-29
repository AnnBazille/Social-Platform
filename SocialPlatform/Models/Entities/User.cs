namespace Models.Entities;

public class User
{
    public string? Id { get; set; }

    public string? Handle { get; set; }

    public string? DisplayName { get; set; }

    public string? Email { get; set; }

    public string? Salt { get; set; }

    public string? PasswordHash { get; set; }

    public string? SessionId { get; set; }
}
