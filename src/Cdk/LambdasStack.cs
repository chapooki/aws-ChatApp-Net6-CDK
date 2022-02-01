using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;

namespace Cdk
{
    public class LambdasStack : Stack
    {
        public class LambdasStackProps : StackProps
        {
            public Table ChatTable { get; set; }
        }

        internal LambdasStack(Construct scope, string id, LambdasStackProps props) : base(scope, id, props)
        {
            var api = new RestApi(this, "chatApp-GW");            

            var users = api.Root.AddResource("users");
   
            var user = users.AddResource("{userId}");

            var userLambda = new Function(this, "chatApp_Api_GetUser", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Code = Code.FromAssetImage("./src/app", new AssetImageCodeProps
                {
                    Cmd = new string[] { "App::App.lambdas.UserLambdas::Get" }
                }),
                Handler = Handler.FROM_IMAGE,
                Timeout = Duration.Minutes(5)                
            });
            props.ChatTable.GrantReadWriteData(userLambda);

            user.AddMethod("GET", new LambdaIntegration (userLambda, new LambdaIntegrationOptions { Proxy = true }));
            
        }
}
}
