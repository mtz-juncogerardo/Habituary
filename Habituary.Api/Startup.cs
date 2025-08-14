using System.Reflection;
using Habituary.Core.Interfaces;
using Habituary.Core.User;
using Habituary.Data.Context;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

namespace Habituary.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddCors(options =>
        {
            var clientUrl = _configuration["Authentication:Google:PostLoginRedirectUrl"] ?? _configuration["ClientUrl"] ?? "http://localhost:4200";
            options.AddPolicy("AllowOrigin",
                builder => builder
                    .WithOrigins(clientUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });
        services.AddSession();
        // Configurar PostgreSQL con Entity Framework Core
        services.AddDbContext<HabituaryDbContext>(options =>
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
        );
        services.AddMvc();
        services.AddHttpContextAccessor();
        RepositoryModule.SetDependencies(services);
        SetDependencyInjection(services);
        services.AddMediatR(typeof(Startup).Assembly);
        SetAuthentication(services);
    }

    private void SetAuthentication(IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // devolver 401 en APIs
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "habituary.auth"; // nombre consistente
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None; // requerido para envio cross-site
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // requiere HTTPS
                options.Cookie.Path = "/";
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (IsApiRequest(ctx.Request))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        if (IsApiRequest(ctx.Request))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }
                        ctx.Response.Redirect(ctx.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = _configuration.GetSection("Authentication:Google:ClientId").Value;
                options.ClientSecret = _configuration.GetSection("Authentication:Google:ClientSecret").Value;
            });
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase)
               || request.Headers["Accept"].Any(h => h.Contains("application/json"));
    }

    private void SetDependencyInjection(IServiceCollection services)
    {
        services.AddSingleton(_configuration);
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //Do not mess with the order of the Middleware it can make the app crash
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Intranet API");
            c.RoutePrefix = string.Empty;
        });
        app.UseRouting();
        app.UseCors("AllowOrigin");
        app.UseHttpsRedirection();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None
        });
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AuthenticationMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}