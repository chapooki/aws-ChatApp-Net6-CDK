using Amazon.CDK;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SSM;
using Shared;
using System.Collections.Generic;

namespace Cdk
{
    public class CognitoStack : Stack
    {
        public class CognitoStackProps : StackProps
        {
            public Table ChatTable { get; set; }

        }

        public UserPool UserPool { get; private set; }

        internal CognitoStack(Construct scope, string id, CognitoStackProps props = null) : base(scope, id, props)
        {
            var postSignUpFunction = new Function(this, $"{Constants.AwsResourcesPrefix}userPoolTrigger_registerUser", new FunctionProps
            {
                Runtime = Runtime.FROM_IMAGE,
                Code = Code.FromAssetImage("./src/app", new AssetImageCodeProps
                {
                    Cmd = new string[] { "App::App.Lambdas.UserLambdas::RegisterUser" }
                }),
                Handler = Handler.FROM_IMAGE,
                Timeout = Duration.Minutes(5)
            });
            PermissionHelper.GiveAccessToChatTable(props.ChatTable, postSignUpFunction);
            PermissionHelper.GiveAccessToChatAppSSM(postSignUpFunction);

            var userPool = new UserPool(this, $"{Constants.AwsResourcesPrefix}UserPool", new UserPoolProps
            {
                UserPoolName = $"{Constants.AwsResourcesPrefix}UserPool",
                SelfSignUpEnabled = true,
                UserVerification = new UserVerificationConfig
                {
                    EmailSubject = "Verify your email for our awesome Chat app!",
                    EmailBody = "Thanks for signing up to our awesome Chat app! Your verification code is {####}",
                    EmailStyle = VerificationEmailStyle.CODE,
                    SmsMessage = "Thanks for signing up to our awesome Chat app! Your verification code is {####}"
                },
                UserInvitation = new UserInvitationConfig
                {
                    EmailSubject = "Invite to join our awesome Chat app!",
                    EmailBody = "Hello {username}, you have been invited to join our awesome Chat app! Your temporary password is {####}",
                    SmsMessage = "Hello {username}, your temporary password for our awesome Chat app is {####}"
                },
                SignInAliases = new SignInAliases // so that user can sign in with either their username or their email address 
                {
                    Username = true,
                    Email = true
                },
                StandardAttributes = new StandardAttributes
                {
                    Email = new StandardAttribute
                    {
                        Required = true,
                        Mutable = false
                    }
                },
                //Mfa = Mfa.REQUIRED,
                //MfaSecondFactor = new MfaSecondFactor
                //{
                //    Sms = true,
                //    Otp = true
                //},
                //PasswordPolicy = new PasswordPolicy
                //{
                //    MinLength = 12,
                //    RequireLowercase = true,
                //    RequireUppercase = true,
                //    RequireDigits = true,
                //    RequireSymbols = true,
                //    TempPasswordValidity = Duration.Days(3)
                //}
                Email = UserPoolEmail.WithCognito("support@chatapp.com"), // REPLY-TO email address
                // For typical production environments, the default email limit is below the required delivery volume.
                // To enable a higher delivery volume, you can configure the UserPool to send emails through Amazon SES.
                //Email = UserPoolEmail.WithSES(new UserPoolSESOptions
                //{
                //    SesRegion = "us-east-1",
                //    FromEmail = "noreply@myawesomeapp.com",
                //    FromName = "Awesome App",
                //    ReplyTo = "support@myawesomeapp.com"
                //})
                LambdaTriggers = new UserPoolTriggers
                {
                    PostConfirmation = postSignUpFunction
                }
            });
            
            userPool.AddDomain($"{Constants.AwsResourcesPrefix}userPoolDomain", new UserPoolDomainOptions
            {
                CognitoDomain = new CognitoDomainOptions
                {
                    DomainPrefix = "naz-chat-app-auth"
                }
            });

            var client = userPool.AddClient($"{Constants.AwsResourcesPrefix}userPool-client", new UserPoolClientOptions
            {
                AuthFlows = new AuthFlow
                {
                    UserPassword = true,
                    UserSrp = true
                },
                IdTokenValidity = Duration.Days(1),
                RefreshTokenValidity = Duration.Days(1),
                AccessTokenValidity = Duration.Days(1),
                PreventUserExistenceErrors = true, //to return generic authentication failure responses instead of an UserNotFoundException
            });

            new CfnOutput(this, "chatApp_userpoolId", new CfnOutputProps { Value = userPool.UserPoolId });
            new CfnOutput(this, "chatApp_userpoolClientId", new CfnOutputProps { Value = client.UserPoolClientId });

            new StringParameter(this, $"{Constants.AwsResourcesPrefix}ssm-userPoolId", new StringParameterProps
            {
                ParameterName = Constants.SSMCognitoUserPoolId,
                StringValue = userPool.UserPoolId
            }); 
            
            new StringParameter(this, $"{Constants.AwsResourcesPrefix}ssm-userPoolArn", new StringParameterProps
            {
                ParameterName = Constants.SSMCognitoUserPoolArn,
                StringValue = userPool.UserPoolArn
            });

            new StringParameter(this, $"{Constants.AwsResourcesPrefix}ssm-clientId", new StringParameterProps
            {
                ParameterName = Constants.SSMCognitoClientId,
                StringValue = userPool.UserPoolId
            });

            this.UserPool = userPool;
        }
    }
}
