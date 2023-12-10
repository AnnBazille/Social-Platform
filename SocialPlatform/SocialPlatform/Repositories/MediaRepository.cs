using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace SocialPlatform.Repositories;

public class MediaRepository
{
    private const int PageSize = 10;

    public DatabaseContext DatabaseContext { get; set; }

    public MediaRepository(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public async Task<List<Post?>> GetPosts(
        string? userId,
        int? skip,
        string? postId,
        string? currentUserId)
    {
        if (userId is null && postId is null)
        {
            var posts = await DatabaseContext
                .Feeds
                .Include(x => x.Post)
                .ThenInclude(x => x!.User)
                .Include(x => x.Post)
                .ThenInclude(x => x!.Parent)
                .AsNoTracking()
                .Where(x => x.ReceiverId == currentUserId)
                .OrderByDescending(x => x.Post!.Date)
                .Skip(skip ?? 0)
                .Take(PageSize)
                .Select(x => x.Post)
                .ToListAsync();

            if (posts.Count() == 0)
            {
                posts = await DatabaseContext
                    .Posts
                    .Include(x => x.User)
                    .Include(x => x.Parent)
                    .AsNoTracking()
                    .OrderByDescending(x => x.Date)
                    .Skip(skip ?? 0)
                    .Take(PageSize)
                    .Select(x => (Post?)x)
                    .ToListAsync();
            }

            return posts;
        }
        else
        {
            return await DatabaseContext
                .Posts
                .Include(x => x.User)
                .Include(x => x.Parent)
                .AsNoTracking()
                .Where(x => userId != null ? x.UserId == userId : x.ParentId != null && x.ParentId == postId)
                .OrderByDescending(x => x.Date)
                .Skip(skip ?? 0)
                .Take(PageSize)
                .Select(x => (Post?)x)
                .ToListAsync();
        }
    }

    public async Task<int> GetLikesCount(string? postId)
    {
        return await DatabaseContext
            .Likes
            .AsNoTracking()
            .CountAsync(x => x.PostId == postId);
    }

    public async Task<int> GetRepliesCount(string? postId)
    {
        return await DatabaseContext
            .Posts
            .AsNoTracking()
            .CountAsync(x => x.ParentId == postId);
    }

    public async Task<List<Post>> GetPostStack(
        string? postId,
        List<Post> stack)
    {
        var post = await DatabaseContext
            .Posts
            .Include(x => x.User)
            .Include(x => x.Parent)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == postId);

        if (post == null)
        {
            return stack;
        }
        else
        {
            stack.Insert(0, post);
            return await GetPostStack(post.ParentId, stack);
        }
    }

    public async Task<bool> IsLiked(string? postId, string? userId)
    {
        return await DatabaseContext
            .Likes
            .AsNoTracking()
            .AnyAsync(x => x.PostId == postId && x.UserId == userId);
    }

    public async Task ChangeLike(string? userId, string? postId)
    {
        var like = await DatabaseContext
            .Likes
            .FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);

        if (like is not null)
        {
            DatabaseContext.Likes.Remove(like); 
        }
        else
        {
            like = new Like
            {
                Id = Guid.NewGuid().ToString(),
                PostId = postId,
                UserId = userId,
            };

            DatabaseContext.Likes.Add(like);
        }

        await DatabaseContext.SaveChangesAsync();
    }

    public async Task AddPost(
        string message,
        string? parentId,
        string userId)
    {
        var postId = Guid.NewGuid().ToString();

        var post = new Post
        {
            Id = postId,
            Date = DateTime.UtcNow,
            Message = message,
            ParentId = parentId,
            UserId = userId,
        };

        DatabaseContext
            .Posts
            .Add(post);

        var followerIds = await DatabaseContext
            .Followings
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.FollowerId)
            .ToListAsync();

        followerIds.Add(userId);

        foreach (var id in followerIds)
        {
            var feedItem = new Feed
            {
                Id = Guid.NewGuid().ToString(),
                PostId = postId,
                ReceiverId = id,
            };

            DatabaseContext
                .Feeds
                .Add(feedItem);
        }

        await DatabaseContext
            .SaveChangesAsync();
    }
}
