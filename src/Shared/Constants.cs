using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Constants
    {
        public static readonly string Region = "ap-southeat-2";

        public static readonly string AwsResourcesPrefix = "chatApp_";
        public static readonly string SSMRoot = "chatApp";
        public static readonly string SSMCognitoUserPoolId = $"/{SSMRoot}/auth/UserPoolId";
        public static readonly string SSMCognitoUserPoolArn = $"/{SSMRoot}/auth/UserPoolArn";
        public static readonly string SSMCognitoClientId = $"/{SSMRoot}/auth/ClientId";

        public static readonly string MainTableName = "chatTable";
        public static readonly string UsersTableName = "users";
        public static readonly string RoomsTableName = "rooms";
        public static readonly string messagesTableName = "messages";
        
        public static readonly string partitionKeyField = "partitionKey";
        public static readonly string sortKeyField = "sortKey";
        public static readonly string secondaryIndexField = "secondaryIndex";

    }
}
