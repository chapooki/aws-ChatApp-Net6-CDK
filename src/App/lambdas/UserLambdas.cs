using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using App.Models.CognitoLambdaTriggers;

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
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation("Get Request\n");

            string userId;
            if (!request.QueryStringParameters!.TryGetValue("userId", out userId))
                throw new Exception("userId parameter was not found");
            var user = await _userService.GetById(new Guid(userId));

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(user),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> GetList(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Get Request\n.");

            string userIds;
            if (!request.PathParameters!.TryGetValue("userIds", out userIds))
                throw new Exception("userIds parameter was not found");

            var guidUsersIds = new List<Guid>();
            try
            {
                foreach (var stringUserId in userIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    guidUsersIds.Add(new Guid(stringUserId.Trim()));
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid parameter format", ex);
            }
            var users = await _userService.GetByIdsList(guidUsersIds);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(users),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }


        public async Task<PostConfirmationRequest> RegisterUser(PostConfirmationRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"RegisterUser Request. {JsonConvert.SerializeObject(request)}");

            try
            {
                await _userService.RegisterUser(new Models.NewChatUser
                {
                    Id = new Guid(request.request.userAttributes["sub"]),
                    Username = request.userName
                });
            }
            catch (Exception)
            {
                context.Logger.LogError("error in getting user data");
                throw;
            }

            return request;
        }
    }
}
