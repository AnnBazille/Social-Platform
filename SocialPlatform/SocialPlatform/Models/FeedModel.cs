namespace SocialPlatform.Models;

public class FeedModel
{
    public List<PostModel>? PostStack { get; set; }

    public List<PostModel>? Children { get; set; }

    public UserModel? User { get; set; }

    public int? Skip { get; set; }
}
