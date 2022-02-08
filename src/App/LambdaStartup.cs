using Amazon;
using Amazon.Extensions.NETCore.Setup;
using App.interfaces;
using App.Services;
using App.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace App
{
    public class LambdaStartup : ILambdaStartup
    {
        public WebApplication App { get; private set; }

        public LambdaStartup()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoomService, RoomService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Configuration.AddSystemsManager($"/{Constants.SSMRoot}/", new AWSOptions 
            // TODO: add environment e.g. Dev or Prod after the SSMRoot if using the same aws account for staging
            {
                Region = RegionEndpoint.APSoutheast2
            });


            this.App = builder.Build();
        }
    }
}
