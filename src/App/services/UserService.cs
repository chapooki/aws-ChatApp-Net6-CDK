using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using App.Helpers;
using App.models;
using App.services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.services
{
    public class UserService : IUserService
    {
        public string Test()
        {
            return "DI test worked";
        }

        public async Task<ChatUser> GetById(Guid userId)
        {
            ChatUser user = new ChatUser { Id = userId };
            var client = new AmazonDynamoDBClient();

            var request = new QueryRequest
            {
                TableName = Shared.Constants.MainTableName,
                KeyConditionExpression = $"{Shared.Constants.partitionKeyField} = :v_table and {Shared.Constants.sortKeyField} = :v_userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_table", new AttributeValue { S =  Shared.Constants.UsersTableName }},
                    {":v_userId", new AttributeValue { S =  userId.ToString() }}}
            };

            var response = await client.QueryAsync(request);

            if (response.Items == null || response.Items.Count == 0)
                throw new Exception($"Invalid userId. {userId}");

            var userData = response.Items[0];

            user.Username = userData["name"].S;
            user.RoomIds = userData["roomIds"].SS.Select(id => new Guid(id)).ToList();

            return user;
        }
    }
}
