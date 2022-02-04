using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services
{
    internal class MessageService
    {
        //public async List<ChatMessage> GetMessages(string sender, string receiver, 
        //    DateTime? fromDateTime, bool onlyUnreadMessages)
        //{
        //    List<ChatMessage> messages = null;

        //    AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        //    var request = new QueryRequest
        //    {
        //        TableName = Shared.Constants.MessageTableName,
        //        KeyConditionExpression = "Id = :v_Id",
        //        ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
        //{":v_Id", new AttributeValue { S =  "Amazon DynamoDB#DynamoDB Thread 1" }}}
        //    };

        //    var response = client.Query(request);

        //    foreach (Dictionary<string, AttributeValue> item in response.Items)
        //    {
        //        // Process the result.
        //        PrintItem(item);
        //    }

        //    return messages;
        //}
    }
}
