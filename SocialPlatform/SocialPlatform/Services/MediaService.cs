using Data.Entities;
using SocialPlatform.Models;
using SocialPlatform.Repositories;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialPlatform.Services;

public class MediaService
{
    private readonly MediaRepository _mediaRepository;
    private readonly UserRepository _userRepository;

    public MediaService(
        MediaRepository mediaRepository,
        UserRepository userRepository)
    {
        _mediaRepository = mediaRepository;
        _userRepository = userRepository;
    }

    public async Task<FeedModel> PopulateFeedModel(
        string? userId,
        int? skip,
        string? postId,
        string? currentUserId)
    {
        var result = new FeedModel
        {
            Skip = skip,
        };

        if (postId is not null)
        {
            var postStack = await _mediaRepository
                .GetPostStack(postId, new List<Post>());
            result.PostStack = new List<PostModel>();
            foreach (var post in postStack)
            {
                var postModel = await GetPostModel(post, currentUserId);
                result.PostStack.Add(postModel);
            }
        }
        
        if (userId is not null)
        {
            result.User = await _userRepository.GetUserModel(userId, currentUserId);
        }

        var posts = await _mediaRepository.GetPosts(userId, skip, postId, currentUserId);
        result.Children = new List<PostModel>();
        foreach (var post in posts)
        {
            var postModel = await GetPostModel(post, currentUserId);
            result.Children.Add(postModel);
        }

        return result;
    }

    public async Task<PostModel> GetPostModel(Post? post, string? currentUserId)
    {
        var result = new PostModel
        {
            Message = post!.Message,
            Date = post.Date,
            Id = post.Id,
            ParentId = post.ParentId,
            UserId = post.UserId,
            Handle = post.User?.Handle,
            DisplayName = post.User?.DisplayName,
        };

        result.Likes = await _mediaRepository.GetLikesCount(post.Id);
        result.Replies = await _mediaRepository.GetRepliesCount(post.Id);
        result.IsLiked = await _mediaRepository.IsLiked(post.Id, currentUserId);

        return result;
    }

    public async Task ChangeLike(string? userId, string? postId)
    {
        await _mediaRepository.ChangeLike(userId, postId);
    }

    public async Task AddPost(
        string message,
        string? parentId,
        string userId)
    {
        await _mediaRepository.AddPost(message, parentId, userId);
    }
}
