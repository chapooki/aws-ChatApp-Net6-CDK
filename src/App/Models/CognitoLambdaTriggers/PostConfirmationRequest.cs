using System.Collections.Generic;

namespace App.Models.CognitoLambdaTriggers
{
    public class PostConfirmationRequest : LambdaTriggerRequestBase
    {
        public class RequestData
        {
            public Dictionary<string, string> userAttributes { get; set; }
            public Dictionary<string, string> clientMetadata { get; set; }

        }

        public class ResponseData
        {
            public Dictionary<string, string> userAttributes { get; set; }
            public Dictionary<string, string> clientMetadata { get; set; }

        }

        public RequestData request { get; set; }
        public ResponseData response { get; set; }
    }
}
