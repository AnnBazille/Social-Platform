namespace Models.Entities;

public class Feed
{
    public string? Id { get; set; }

    public string? ReceiverId { get; set; }

    public User? Receiver { get; set; }

    public string? PostId { get; set; }

    public Post? Post { get; set; }
}
