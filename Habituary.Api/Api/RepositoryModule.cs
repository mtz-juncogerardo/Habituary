using Habituary.Api.Authentication.Google;
using Habituary.Api.Habit.Repository;
using Habituary.Api.User.Repository;

namespace Habituary.Api;

public static class RepositoryModule
{
    //This method is called from the Startup.cs file in the Habituary.Api project
    //This method is used to register the dependencies of the base repositories
    //Add your repositories here
    public static void SetDependencies(IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
        services.AddScoped<GoogleAuthRepository>();
        services.AddScoped<HabitRepository>();
    }
}