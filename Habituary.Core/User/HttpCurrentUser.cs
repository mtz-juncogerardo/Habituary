using Habituary.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Habituary.Core.User;

public class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid IRN
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("IRN")?.Value;
            return claim != null ? Guid.Parse(claim) : Guid.Empty;
        }
    }

    public string Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ??
        string.Empty;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}