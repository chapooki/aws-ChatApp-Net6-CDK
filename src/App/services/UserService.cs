using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using App.Helpers;
using App.Models;
using App.Services.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services
{
    public class UserService : IUserService
    {
        public async Task RegisterUser(NewChatUser user)
        {
            var client = new AmazonDynamoDBClient();

            var item = new Dictionary<string, AttributeValue>();
            item.Add(Constants.PartitionKeyField, new AttributeValue { S = Constants.UsersTableName });
            item.Add(Constants.SortKeyField, new AttributeValue { S = user.Id.ToString() });
            item.Add("name", new AttributeValue { S = user.Username });

            var request = new PutItemRequest
            {
                TableName = Shared.Constants.MainTableName,
                Item = item
            };

            // TODO replace this with client.TransactWriteItems to support transactionScope

            var response = await client.PutItemAsync(request);
        }

        public async Task<ChatUser> GetById(Guid userId)
        {
            ChatUser user = new ChatUser { Id = userId };
            var client = new AmazonDynamoDBClient();

            var request = new QueryRequest
            {
                TableName = Shared.Constants.MainTableName,
                KeyConditionExpression = $"{Shared.Constants.PartitionKeyField} = :v_table and {Shared.Constants.SortKeyField} = :v_userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_table", new AttributeValue { S =  Shared.Constants.UsersTableName }},
                    {":v_userId", new AttributeValue { S =  userId.ToString() }}}
            };

            var response = await client.QueryAsync(request);

            if (response.Items == null || response.Items.Count == 0)
                throw new Exception($"Invalid userId. {userId}");

            var userData = response.Items[0];

            user.Username = userData["name"].S;
            user.RoomIds = userData["roomIds"].SS.Where(id => id != "").Select(id => new Guid(id)).ToList();

            return user;
        }

        public async Task<List<ChatUser>> GetByIdsList(List<Guid> userIds)
        {
            var list = new List<ChatUser>();

            foreach (var id in userIds)
            {
                var user = await GetById(id);
                list.Add(user);
            }

            return list;
        }

        public async Task AddToRoom(Guid userId, Guid roomId)
        {
            var client = new AmazonDynamoDBClient();

            var key = new Dictionary<string, AttributeValue>();
            key.Add(Constants.PartitionKeyField, new AttributeValue { S = Constants.UsersTableName });
            key.Add(Constants.SortKeyField, new AttributeValue { S = userId.ToString() });

            var attributeUpdates = new Dictionary<string, AttributeValueUpdate>();
            attributeUpdates.Add("roomIds", new AttributeValueUpdate
            {
                Action = AttributeAction.ADD,
                Value = new AttributeValue { SS = new List<String>{ roomId.ToString() } }
            });

            var request = new UpdateItemRequest
            {
                TableName = Shared.Constants.MainTableName,
                Key = key,
                AttributeUpdates = attributeUpdates
            };

            await client.UpdateItemAsync(request);
        }
    }
}
