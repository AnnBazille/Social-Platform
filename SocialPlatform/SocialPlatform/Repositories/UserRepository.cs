﻿using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Data.Entities;
using SocialPlatform.Models;

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

    public async Task<string?> TrySetNewSessionId(string email, string password)
    {
        var user = await DatabaseContext
            .Users
            .FirstOrDefaultAsync(x => x.Email == email.ToLower());

        if (user is not null)
        {
            string passwordHash = GetPasswordHash(
                password,
                Convert.FromHexString(user.Salt!.Replace(" ", "")));

            if (passwordHash == user.PasswordHash)
            {
                var sessionId = Guid.NewGuid().ToString();
                user.SessionId = sessionId;
                await DatabaseContext.SaveChangesAsync();

                return sessionId;
            }
        }

        return null;
    }

    public async Task<string?> TryAddNewUser(
        string? email,
        string? password,
        string? handle,
        string? displayName)
    {
        var salt = GetSalt();
        var passwordHash = GetPasswordHash(password!, salt);
        var id = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = id,
            Handle = handle,
            DisplayName = displayName,
            Email = email,
            Salt = Convert.ToHexString(salt),
            PasswordHash = passwordHash,
        };

        try
        {
            await DatabaseContext.Users.AddAsync(user);
            await DatabaseContext.SaveChangesAsync();
            return id;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> TryUpdateUser(
        string? email,
        string? password,
        string? handle,
        string? displayName,
        string id)
    {
        var user = await DatabaseContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is not null)
        {
            if (email is not null)
            {
                user.Email = email;
            }

            if (password is not null)
            {
                var salt = GetSalt();
                var passwordHash = GetPasswordHash(password!, salt);

                user.PasswordHash = passwordHash;
                user.Salt = Convert.ToHexString(salt);
            }

            if (handle is not null)
            {
                user.Handle = handle;
            }

            if (displayName is not null)
            {
                user.DisplayName = displayName;
            }

            try
            {
                await DatabaseContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    public async Task RemoveSessionId(string id)
    {
        var user = await DatabaseContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is not null)
        {
            user.SessionId = string.Empty;
            await DatabaseContext.SaveChangesAsync();
        }
    }

    public async Task<UserModel?> GetUserModel(string? userId, string? currentUserId)
    {
        var result = await DatabaseContext
            .Users
            .AsNoTracking()
            .Select(x => new UserModel
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Handle = x.Handle,
            })
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (result is not null)
        {
            result.IsFollowing = await DatabaseContext
                .Followings
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.FollowerId == currentUserId);
        }

        return result;
    }

    public async Task ChangeFollowing(string? userId, string? followerId)
    {
        var following = await DatabaseContext
            .Followings
            .FirstOrDefaultAsync(x => x.UserId == userId && x.FollowerId == followerId);

        if (following is not null)
        {
            DatabaseContext.Followings.Remove(following);
        }
        else
        {
            following = new Following
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FollowerId = followerId,
            };

            DatabaseContext.Followings.Add(following);
        }

        await DatabaseContext.SaveChangesAsync();
    }

    private string GetPasswordHash(string password, byte[] salt)
    {
        const int iterationCount = 100000;
        const int bytes = 256 / 8;

        return Convert.ToHexString(
            KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA256,
                iterationCount,
                bytes));
    }

    private byte[] GetSalt()
    {
        return RandomNumberGenerator.GetBytes(16);
    }
}
