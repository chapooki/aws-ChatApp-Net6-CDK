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
        public async Task<ChatRoom> GetById(Guid roomId)
        {
            var room = new ChatRoom { Id = roomId };
            var client = new AmazonDynamoDBClient();

            var request = new QueryRequest
            {
                TableName = Shared.Constants.MainTableName,
                KeyConditionExpression = $"{Shared.Constants.partitionKeyField} = :v_table AND " +
                    $"{Shared.Constants.sortKeyField} = :v_roomId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_table", new AttributeValue { S =  Shared.Constants.RoomsTableName }},
                    {":v_roomId", new AttributeValue { S =  roomId.ToString() }}}
            };

            var response = await client.QueryAsync(request);

            if (response.Items == null || response.Items.Count == 0)
                throw new Exception($"Invalid roomId. {roomId}");

            var data = response.Items[0];

            room.Id = new Guid(data[Shared.Constants.sortKeyField].S);
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
            item.Add(Constants.partitionKeyField, new AttributeValue { S = Constants.RoomsTableName });
            item.Add(Constants.sortKeyField, new AttributeValue { S = newId.ToString() });
            item.Add("name", new AttributeValue { S = room.Name });
            item.Add("userIds", new AttributeValue { SS = room.UserIds.Select(id => id.ToString() ).ToList() });
            item.Add("latestMessageDateTime", new AttributeValue { N = DateTime.Now.ToUnixTime().ToString() });

            var request = new PutItemRequest
            {
                TableName = Shared.Constants.MainTableName,
                Item = item
            };

            var response = await client.PutItemAsync(request);

            return newId;
        }
    }
}
