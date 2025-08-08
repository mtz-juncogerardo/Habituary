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
        SetAuthentication(services);
        services.AddControllers();
        services.AddOptions();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddSwaggerGen();
        services.AddCors(c =>
        {
            c.AddPolicy("AllowOrigin",
                options =>
                {
                    options.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
        // Configurar PostgreSQL con Entity Framework Core
        services.AddDbContext<HabituaryDbContext>(options =>
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
        );
        services.AddMvc();
        services.AddHttpContextAccessor();
        RepositoryModule.SetDependencies(services);
        SetDependencyInjection(services);
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
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = _configuration.GetSection("Authentication:Google:ClientId").Value;
                options.ClientSecret = _configuration.GetSection("Authentication:Google:ClientSecret").Value;
            });
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