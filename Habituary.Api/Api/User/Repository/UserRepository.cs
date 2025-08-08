using Habituary.Api.User.Entities;
using Habituary.Core.Interfaces;
using Habituary.Data.Context;
using Habituary.Data.Mapper;
using Habituary.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.User.Repository;

public class UserRepository
{
    private readonly HabituaryDbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRepository(HabituaryDbContext dbContext, IHttpContextAccessor httpContextAccessor,
        ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _currentUser = currentUser;
    }

    public async Task<UserEntity> GetLoggedUser()
    {
        if (_currentUser.Email == null) throw new Exception("Not Authenticated");

        var dbUser = await _dbContext.Users.FirstOrDefaultAsync(r => r.Email == _currentUser.Email);
        if (dbUser == null) throw new Exception("User not found");

        var oauth = await _dbContext.OauthCredentials.Where(r => r.UserIRN == dbUser.IRN).ToListAsync();
        return EntityRecordMapper<UserRecord, UserEntity>.MapToEntity(dbUser, new
        {
            AuthTypes = oauth.Select(r => r.AuthType)
        });
    }

    public Task<bool> UserIsRegistered(string email)
    {
        var dbUser = _dbContext.Users.FirstOrDefault(r => r.Email == email);
        return Task.FromResult(dbUser != null);
    }

    public Task<UserEntity> UpdateUser(UserEntity userEntity)
    {
        var dbUser = _dbContext.Users.FirstOrDefault(r => r.IRN == _currentUser.IRN);
        if (userEntity.Email != _currentUser.Email) throw new Exception("Email cannot be changed");

        dbUser.Username = userEntity.Username;
        _dbContext.SaveChanges();
        return Task.FromResult(userEntity);
    }

    public async Task<IActionResult> DeleteUser()
    {
        var user = _dbContext.Users.FirstOrDefault(r => r.IRN == _currentUser.IRN);
        if (user == null) throw new Exception("User not found");

        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();
        await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return new OkResult();
    }
}