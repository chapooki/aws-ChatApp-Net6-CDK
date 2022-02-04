using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using App.interfaces;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace App.Lambdas
{
    public class UserLambdas
    {
        private IUserService _userService;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public UserLambdas()
        {
            var startup = new LambdaStartup();
            this._userService = startup.App.Services.GetRequiredService<IUserService>();
        }


        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public APIGatewayProxyResponse Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation("Get Request\n");

            string userId;
            if (!request.PathParameters!.TryGetValue("userId", out userId))
                throw new Exception("userId parameter was not found");
            var user = _userService.GetById(new Guid(userId));

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(user),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}
