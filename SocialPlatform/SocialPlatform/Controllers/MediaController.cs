using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using SocialPlatform.Models;
using SocialPlatform.Services;

namespace SocialPlatform.Controllers;

public class MediaController : Controller
{
    private readonly MediaService _mediaService;
    private readonly AccountService _accountService;

    public MediaController(
        MediaService mediaService,
        AccountService accountService)
    {
        _mediaService = mediaService;
        _accountService = accountService;
    }

    public ActionResult Feed(FeedModel feedModel)
    {
        return View("Feed", feedModel);
    }

    [HttpGet]
    public async Task<ActionResult> GetFeed(GetFeedRequest request)
    {
        var userId = default(string);

        if (Request.Cookies.TryGetValue(nameof(Data.Entities.User.SessionId), out var sessionId))
        {
            userId = await _accountService.TryGetUserIdBySessionId(sessionId);
        }

        var feedmodel = await _mediaService.PopulateFeedModel(
            request.UserId,
            request.Skip,
            request.PostId,
            userId);

        return Feed(feedmodel);
    }

    public async Task<ActionResult> ChangeLike(string postId)
    {
        if (Request.Cookies.TryGetValue(nameof(Data.Entities.User.SessionId), out var sessionId))
        {
            var userId = await _accountService.TryGetUserIdBySessionId(sessionId);

            await _mediaService.ChangeLike(userId, postId);
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost]
    public async Task<ActionResult> Post(string message, string? parentId)
    {
        if (Request.Cookies.TryGetValue(nameof(Data.Entities.User.SessionId), out var sessionId))
        {
            var userId = await _accountService.TryGetUserIdBySessionId(sessionId);

            await _mediaService.AddPost(message, parentId, userId!);
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }
}
