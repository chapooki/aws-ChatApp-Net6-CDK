using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using App.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using App.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using App.Helpers;

namespace App.Lambdas
{
    public class RoomLambdas
    {
        private IRoomService _roomService;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public RoomLambdas()
        {
            var startup = new LambdaStartup();
            this._roomService = startup.App.Services.GetRequiredService<IRoomService>();
        }


        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation("Get Request\n");

            string roomId;
            if (!request.PathParameters!.TryGetValue("roomId", out roomId))
                throw new Exception("roomId parameter was not found");
            var room = await _roomService.GetById(new Guid(roomId));

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(room),
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
            context.Logger.LogInformation("Get Request\n");

            string roomIds;
            if (!request.PathParameters!.TryGetValue("roomIds", out roomIds))
                throw new Exception("roomIds parameter was not found");

            var guidRoomsIds = new List<Guid>();
            try
            {                
                foreach (var stringRoomId in roomIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    guidRoomsIds.Add(new Guid(stringRoomId.Trim()));
            }
            catch(Exception ex)
            {
                throw new Exception("Invalid parameter format", ex);
            }
            var rooms = await _roomService.GetByIdsList(guidRoomsIds);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(rooms),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }


        public async Task<APIGatewayProxyResponse> Put(APIGatewayProxyRequest request, ILambdaContext context)
        {
            NewChatRoom room = null;
            context.Logger.LogInformation("Put Request\n");
            var token = new JWTHelper().GetTokenData(request);

            try
            {
                room = JsonConvert.DeserializeObject<NewChatRoom>(request.Body, 
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            catch (Exception ex)
            {
                throw new Exception("Error in parsing the request body", ex);
            }

            if (!room.UserIds.Contains(token.UserId))
                room.UserIds.Add(token.UserId);

            var roomId = await _roomService.Create(room);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = roomId.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            return response;
        }
    }
}
