using SocialPlatform.Models;
using SocialPlatform.Repositories;

namespace SocialPlatform.Services;

public class AccountService
{
    private readonly UserRepository _userRepository;

    public AccountService(
        UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string?> TryGetUserIdBySessionId(string sessionId)
    {
        return await _userRepository.TryGetUserIdBySessionId(sessionId);
    }

    public async Task<string?> TryLogIn(LogInRequest request)
    {
        return await _userRepository.TrySetNewSessionId(request.Email!, request.Password!);
    }

    public async Task<bool> TryRegister(RegisterRequest request)
    {
        var id = await _userRepository.TryAddNewUser(
            request.Email,
            request.Password,
            request.Handle,
            request.DisplayName);

        return id is not null;
    }

    public async Task<bool> TryUpdate(
        string? email,
        string? password,
        string? handle,
        string? displayName,
        string id)
    {
        return await _userRepository.TryUpdateUser(
            email,
            password,
            handle,
            displayName,
            id);
    }

    public async Task LogOut(string id)
    {
        await _userRepository.RemoveSessionId(id);
    }

    public async Task ChangeFollowing(string? userId, string? followerId)
    {
        await _userRepository.ChangeFollowing(userId, followerId);
    }
}
