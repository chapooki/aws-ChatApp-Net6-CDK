using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Shared;

namespace Cdk
{
    public class LambdasStack : Stack
    {
        public class LambdasStackProps : StackProps
        {
            public Table ChatTable { get; set; }
            public UserPool UserPool { get; set; }

        }

        internal LambdasStack(Construct scope, string id, LambdasStackProps props) : base(scope, id, props)
        {
            var api = new RestApi(this, $"{Constants.AwsResourcesPrefix}gw");

            var auth = new CognitoUserPoolsAuthorizer(this, $"{Constants.AwsResourcesPrefix}gw-authorizer", new CognitoUserPoolsAuthorizerProps{
              CognitoUserPools = new IUserPool[] { props.UserPool }
            });

            var users = api.Root.AddResource("users");

            var user = users.AddResource("{userIds}");

            var userLambda = new Function(this, $"{Constants.AwsResourcesPrefix}api_getUser", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Code = Code.FromAssetImage("./src/app", new AssetImageCodeProps
                {
                    Cmd = new string[] { "App::App.Lambdas.UserLambdas::GetList" }
                }),
                Handler = Handler.FROM_IMAGE,
                Timeout = Duration.Minutes(5)
            });
            GiveAccessToChatTable(props.ChatTable, userLambda);
            GiveAccessToChatAppSSM(userLambda);

            user.AddMethod("GET", new LambdaIntegration(userLambda, 
                new LambdaIntegrationOptions { Proxy = true }), 
                new MethodOptions { 
                    AuthorizationType = AuthorizationType.COGNITO,
                    AuthorizationScopes = new string[] { "openid", "profile", "email" },
                    Authorizer = auth
                });


            var rooms = api.Root.AddResource("rooms");
            var room = rooms.AddResource("{roomIds}");

            var getRoomLambda = new Function(this, $"{Constants.AwsResourcesPrefix}api_getRooms", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Code = Code.FromAssetImage("./src/app", new AssetImageCodeProps
                {
                    Cmd = new string[] { "App::App.Lambdas.RoomLambdas::GetList" }
                }),
                Handler = Handler.FROM_IMAGE,
                Timeout = Duration.Minutes(5)
            });
            GiveAccessToChatTable(props.ChatTable, getRoomLambda);
            GiveAccessToChatAppSSM(getRoomLambda);

            room.AddMethod("GET", new LambdaIntegration(getRoomLambda,
                new LambdaIntegrationOptions { Proxy = true }),
                new MethodOptions
                {
                    AuthorizationType = AuthorizationType.COGNITO,
                    AuthorizationScopes = new string[] { "openid", "profile", "email" },
                    Authorizer = auth
                });


            var createRoomLambda = new Function(this, $"{Constants.AwsResourcesPrefix}api_createRoom", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Code = Code.FromAssetImage("./src/app", new AssetImageCodeProps
                {
                    Cmd = new string[] { "App::App.Lambdas.RoomLambdas::Put" }
                }),
                Handler = Handler.FROM_IMAGE,
                Timeout = Duration.Minutes(5)
            });
            GiveAccessToChatTable(props.ChatTable, createRoomLambda);
            GiveAccessToChatAppSSM(createRoomLambda);

            rooms.AddMethod("PUT", new LambdaIntegration(createRoomLambda,
                new LambdaIntegrationOptions { Proxy = true }),
                new MethodOptions
                {
                    AuthorizationType = AuthorizationType.COGNITO,
                    AuthorizationScopes = new string[] { "openid", "profile", "email" },
                    Authorizer = auth,
                });

            var messages = api.Root.AddResource("messages");

            var sendMessageLambda = new Function(this, $"{Constants.AwsResourcesPrefix}api_sendMessage", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Code = Code.FromAssetImage("./src/app", new AssetImageCodeProps
                {
                    Cmd = new string[] { "App::App.Lambdas.MessageLambdas::Put" }
                }),
                Handler = Handler.FROM_IMAGE,
                Timeout = Duration.Minutes(5)
            });
            GiveAccessToChatTable(props.ChatTable, sendMessageLambda);
            GiveAccessToChatAppSSM(sendMessageLambda);

            messages.AddMethod("PUT", new LambdaIntegration(sendMessageLambda,
                new LambdaIntegrationOptions { Proxy = true }),
                new MethodOptions
                {
                    AuthorizationType = AuthorizationType.COGNITO,
                    AuthorizationScopes = new string[] { "openid", "profile", "email" },
                    Authorizer = auth,
                });
        }

        private void GiveAccessToChatTable(Table chatTable, Function lambda)
        {
            chatTable.GrantReadWriteData(lambda);
        }

        private void GiveAccessToChatAppSSM(Function lambda)
        {
            lambda.AddToRolePolicy(
              new PolicyStatement(new PolicyStatementProps
              {
                  Effect = Effect.ALLOW,
                  Actions = new string[] { "ssm:GetParameter", "ssm:GetParameters", "ssm:GetParametersByPath" },
                  Resources = new string[] { "*" /* $"arn:aws:ssm:{Constants.Region}:*:parameter/{Constants.SSMRoot}/*" */ }  // NOTE: It didn't work until I set it to *            
              })
            );
        }
    }
}
