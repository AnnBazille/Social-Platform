namespace Models.Entities;

public class Post
{
    public string? Id { get; set; }

    public string? UserId { get; set; }

    public User? User { get; set; }

    public string? ParentId { get; set; }

    public Post? Parent { get; set; }

    public string? Message { get; set; }

    public DateTime Date { get; set; }
}
