using Habituary.Core.Extensions;
using Habituary.Core.Interfaces;
using Habituary.Core.Types;
using Habituary.Data.Context;
using Habituary.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.Authentication.Google;

public class GoogleAuthRepository(
    IHttpContextAccessor httpContextAccessor,
    HabituaryDbContext dbContext,
    ICurrentUser currentUser)
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext;

    public async Task<IActionResult> Login(string redirectUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };
        return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
    }
    
    public async Task<IActionResult> LoginCallback()
    {
        var result =
            await _httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded) return new UnauthorizedResult();

        var oauthCredential = await dbContext.OauthCredentials.FirstOrDefaultAsync(r =>
            r.UserIRN == currentUser.IRN
            && r.AuthType == AuthenticationType.Google);
        if (oauthCredential == null) CreateOauthCredential();

        return new OkResult();
    }

    public async Task<IActionResult> Logout()
    {
        await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return new OkResult();
    }

    private void CreateOauthCredential()
    {
        var user = dbContext.Users.FirstOrDefault(r => r.Email == currentUser.Email);
        if (user == null)
        {
            //It's first time user ever login create new user
            user = new UserRecord
            {
                Username = currentUser.Email,
                Email = currentUser.Email
            };
            user.SetAudit(currentUser.Email);
        }

        var oauthCredential = new OauthCredentialRecord
        {
            User = user,
            AuthType = AuthenticationType.Google
        };
        oauthCredential.SetAudit(currentUser.Email);
        dbContext.OauthCredentials.Add(oauthCredential);
        dbContext.SaveChanges();
    } 
}