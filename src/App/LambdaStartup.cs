using App.interfaces;
using App.services;
using App.services.interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App
{
    public class LambdaStartup : ILambdaStartup
    {
        public WebApplication App { get; private set; }

        public LambdaStartup()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddScoped<IUserService, UserService>();

            this.App = builder.Build();
        }
    }
}
