namespace Data.Entities;

public class Like
{
    public string? Id { get; set; }

    public string? UserId { get; set; }

    public User? User { get; set; }

    public string? PostId { get; set; }

    public Post? Post { get; set; }
}
