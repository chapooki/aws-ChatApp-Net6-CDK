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
    public class RoomService : IRoomService
    {
        public async Task<ChatRoom> GetRoom(Guid roomId)
        {
            ChatRoom room = new ChatRoom();
            var client = new AmazonDynamoDBClient();

            var request = new QueryRequest
            {
                TableName = Shared.Constants.MainTableName,
                KeyConditionExpression = $"{Shared.Constants.partitionKeyField} = :v_table && " +
                    $"{Shared.Constants.sortKeyField} = :v_roomId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {":v_table", new AttributeValue { S =  Shared.Constants.RoomsTableName }},
                    {":v_roomId", new AttributeValue { S =  roomId.ToString() }}}
            };

            var response = await client.QueryAsync(request);

            if (response.Items == null || response.Items.Count == 0)
                throw new Exception($"Invalid roomId. {roomId}");

            var userData = response.Items[0];

            room.Name = userData["name"].S;
            room.UserIds = userData["userIds"].SS.Select(id => new Guid(id)).ToList();
            room.LatestMessageDateTime = DateTimeHelper.FromUnixTime(long.Parse(userData["latestMessageDateTime"].S));

            return room;
        }
    }
}
