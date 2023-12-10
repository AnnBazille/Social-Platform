namespace SocialPlatform.Models;

public class PostModel
{
    public string? Message { get; set; }

    public DateTime Date { get; set; }

    public string? Id { get; set; }

    public string? ParentId { get; set; }

    public string? UserId { get; set; }

    public string? Handle { get; set; }

    public string? DisplayName { get; set; }

    public int Likes { get; set; }

    public int Replies { get; set; }

    public bool IsLiked { get; set; }
}
