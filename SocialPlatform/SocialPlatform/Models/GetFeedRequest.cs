namespace SocialPlatform.Models;

public class GetFeedRequest
{
    public string? UserId { get; set; }

    public int? Skip { get; set; }

    public string? PostId { get; set; }
}
