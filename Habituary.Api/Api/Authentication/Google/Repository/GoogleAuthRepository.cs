using System.Security.Claims;
using Habituary.Core.Extensions;
using Habituary.Core.Types;
using Habituary.Data.Context;
using Habituary.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api.Authentication.Google.Repository;

public class GoogleAuthRepository
{
    private readonly HabituaryDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GoogleAuthRepository(
        IHttpContextAccessor httpContextAccessor,
        HabituaryDbContext dbContext,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _configuration = configuration;
    }
    
    public Task<IActionResult> Login(string redirectUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };
        return Task.FromResult<IActionResult>(new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties));
    }
    
    public async Task<IActionResult> LoginCallback()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return new UnauthorizedResult();
        var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded) return new UnauthorizedResult();

        var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return new UnauthorizedResult();

        var user = await _dbContext.Users.FirstOrDefaultAsync(r => r.Email == email);
        if (user == null)
        {
            user = new UserRecord { Username = email, Email = email };
            user.SetAudit(email);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        var identity = result.Principal?.Identity as ClaimsIdentity;
        if (identity == null) return new UnauthorizedResult();

        if (!identity.HasClaim(c => c.Type == "IRN"))
        {
            identity.AddClaim(new Claim("IRN", user.IRN.ToString()));
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        var oauthCredential = await _dbContext.OauthCredentials.FirstOrDefaultAsync(r =>
            r.UserIRN == user.IRN && r.AuthType == AuthenticationType.Google);
        if (oauthCredential == null) CreateOauthCredential(user, email);

        // Nueva lógica: tomar URL de redirección desde configuración específica, con fallback
        var redirectUrl = _configuration["Authentication:Google:PostLoginRedirectUrl"] ?? _configuration["ClientUrl"]; 
        if (string.IsNullOrEmpty(redirectUrl)) return new BadRequestObjectResult("Redirect URL is not configured.");
        return new RedirectResult(redirectUrl);
    }

    public async Task<IActionResult> Logout()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return new NoContentResult();
        }
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        httpContext.Session?.Clear();
        var cookieOpts = new CookieOptions
        {
            Path = "/",
            Secure = true,
            SameSite = SameSiteMode.None,
            HttpOnly = true
        };
        httpContext.Response.Cookies.Delete("habituary.auth", cookieOpts);
        httpContext.Response.Cookies.Delete(".AspNetCore.Cookies", cookieOpts);
        cookieOpts.Expires = DateTimeOffset.UnixEpoch;
        httpContext.Response.Cookies.Append("habituary.auth", string.Empty, cookieOpts);
        httpContext.Response.Cookies.Append(".AspNetCore.Cookies", string.Empty, cookieOpts);
        return new NoContentResult();
    }

    private void CreateOauthCredential(UserRecord user, string auditUser)
    {
        var oauthCredential = new OauthCredentialRecord
        {
            User = user,
            AuthType = AuthenticationType.Google
        };
        oauthCredential.SetAudit(auditUser);
        _dbContext.OauthCredentials.Add(oauthCredential);
        _dbContext.SaveChanges();
    } 
}