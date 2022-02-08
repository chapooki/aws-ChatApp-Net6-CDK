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
    public class RoomService : IRoomService
    {
        private readonly IUserService _userService;

        public RoomService(IUserService userService)
        {
            this._userService = userService;
        }

        public async Task<ChatRoom> GetById(Guid roomId)
        {
            var room = new ChatRoom { Id = roomId };
            var client = new AmazonDynamoDBClient();

            var request = new QueryRequest
            {
                TableName = Shared.Constants.MainTableName,
                KeyConditionExpression = $"{Shared.Constants.PartitionKeyField} = :v_table AND " +
                    $"{Shared.Constants.SortKeyField} = :v_roomId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_table", new AttributeValue { S =  Shared.Constants.RoomsTableName }},
                    {":v_roomId", new AttributeValue { S =  roomId.ToString() }}}
            };

            var response = await client.QueryAsync(request);

            if (response.Items == null || response.Items.Count == 0)
                throw new Exception($"Invalid roomId. {roomId}");

            var data = response.Items[0];

            room.Id = new Guid(data[Shared.Constants.SortKeyField].S);
            room.Name = data["name"].S;
            room.UserIds = data["userIds"].SS.Where(id => id != "").Select(id => new Guid(id)).ToList();
            room.LatestMessageDateTime = DateTimeHelper.FromUnixTime(long.Parse(data["latestMessageDateTime"].N));

            return room;
        }

        public async Task<List<ChatRoom>> GetByIdsList(List<Guid> roomIds)
        {
            var list = new List<ChatRoom>();

            foreach (var id in roomIds)
            {
                var room = await GetById(id);
                list.Add(room);
            }

            return list;
        }

        public async Task<Guid> Create(NewChatRoom room)
        {
            var newId = Guid.NewGuid();
            var client = new AmazonDynamoDBClient();

            var item = new Dictionary<string, AttributeValue>();
            item.Add(Constants.PartitionKeyField, new AttributeValue { S = Constants.RoomsTableName });
            item.Add(Constants.SortKeyField, new AttributeValue { S = newId.ToString() });
            item.Add("name", new AttributeValue { S = room.Name });
            item.Add("userIds", new AttributeValue { SS = room.UserIds.Select(id => id.ToString() ).ToList() });
            item.Add("latestMessageDateTime", new AttributeValue { N = DateTime.Now.ToUnixTime().ToString() });

            var request = new PutItemRequest
            {
                TableName = Shared.Constants.MainTableName,
                Item = item
            };

            // TODO replace this with client.TransactWriteItems to support transactionScope

            var response = await client.PutItemAsync(request);

            foreach(Guid userId in room.UserIds)
                await _userService.AddToRoom(userId, newId);

            return newId;
        }

        public async Task ReceiveANewMessage(ChatMessage message)
        {
            var client = new AmazonDynamoDBClient();

            var key = new Dictionary<string, AttributeValue>();
            key.Add(Constants.PartitionKeyField, new AttributeValue { S = Constants.RoomsTableName });
            key.Add(Constants.SortKeyField, new AttributeValue { S = message.RoomId.ToString() });

            var attributeUpdates = new Dictionary<string, AttributeValueUpdate>();
            attributeUpdates.Add("latestMessageDateTime", new AttributeValueUpdate { 
                Action = AttributeAction.PUT,
                Value = new AttributeValue { N = message.SendDateTime.ToUnixTime().ToString() }
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
