using Models;
using Microsoft.EntityFrameworkCore;

namespace SocialPlatform.Repositories;

public class UserRepository
{
    public DatabaseContext DatabaseContext { get; set; }

    public UserRepository(DatabaseContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public async Task<string?> TryGetUserIdBySessionId(string sessionId)
    {
        return (await DatabaseContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SessionId == sessionId))
            ?.Id;
    }
}
