using SocialPlatform.Repositories;

namespace SocialPlatform.Services;

public class AccountService
{
    private readonly UserRepository _userRepository;

    public AccountService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string?> TryGetUserIdBySessionId(string sessionId)
    {
        return await _userRepository.TryGetUserIdBySessionId(sessionId);
    }
}
