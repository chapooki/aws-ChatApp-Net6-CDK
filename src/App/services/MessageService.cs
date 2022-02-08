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
    internal class MessageService: IMessageService
    {
        private readonly IRoomService _roomService;

        public MessageService(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task<Guid> SendMessage(ChatMessage message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Content))
                return default;

            var client = new AmazonDynamoDBClient();

            if (message.Id == new Guid())
                message.Id = Guid.NewGuid();

            var item = new Dictionary<string, AttributeValue>();
            item.Add(Constants.PartitionKeyField, new AttributeValue { S = $"{Constants.MessagesTableName}#{message.RoomId}" });
            item.Add(Constants.SortKeyField, new AttributeValue { S = message.Id.ToString() });
            item.Add(Constants.SecondaryIndexField, new AttributeValue { N = message.SendDateTime.ToUnixTime().ToString() });
            item.Add("sender", new AttributeValue { S = message.SenderUserId.ToString() });
            item.Add("content", new AttributeValue { S = message.Content });

            var request = new PutItemRequest
            {
                TableName = Shared.Constants.MainTableName,
                Item = item
            };

            var response = await client.PutItemAsync(request);

            await _roomService.ReceiveANewMessage(message);

            return message.Id;
        }
    }
}
