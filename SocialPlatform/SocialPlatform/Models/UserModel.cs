namespace SocialPlatform.Models;

public class UserModel
{
    public string? Id { get; set; }

    public string? Handle { get; set; }

    public string? DisplayName { get; set; }

    public bool? IsFollowing { get; set; }
}
