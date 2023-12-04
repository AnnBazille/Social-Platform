namespace Data.Entities;

public class Following
{
    public string? Id { get; set; }

    public string? UserId { get; set; }

    public User? User { get; set; }

    public string? FollowerId { get; set; }

    public User? Follower { get; set; }
}
