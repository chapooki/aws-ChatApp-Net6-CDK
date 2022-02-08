using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using App.interfaces;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using App.Helpers;
using App.Models;
using Newtonsoft.Json.Serialization;

namespace App.Lambdas
{
    public class MessageLambdas
    {
        private IMessageService _messageService;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public MessageLambdas()
        {
            var startup = new LambdaStartup();
            this._messageService = startup.App.Services.GetRequiredService<IMessageService>();
        }


        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> Put(APIGatewayProxyRequest request, ILambdaContext context)
        {
            NewChatMessage messageReceived = null;
            var messageToSave = new ChatMessage();
            context.Logger.LogInformation("Put Request\n");
            var token = new JWTHelper().GetTokenData(request);

            try
            {
                messageReceived = JsonConvert.DeserializeObject<NewChatMessage>(request.Body,
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in parsing the request body", ex);
            }

            messageToSave.RoomId = messageReceived.RoomId;
            messageToSave.Content = messageReceived.Content;
            messageToSave.SenderUserId = token.UserId;
            messageToSave.SendDateTime = DateTime.Now;

            await _messageService.SendMessage(messageToSave);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = messageToSave.Id.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}
