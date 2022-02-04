using Amazon.CDK;
using Amazon.CDK.AWS.AppSync;
using Shared;

namespace Cdk
{
    public class AppsyncStack : Stack
    {
        internal AppsyncStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var api = new GraphqlApi(this, $"{Constants.AwsResourcesPrefix}GraphqlApi", new GraphqlApiProps
            {
                Name = "appsync-api",
                Schema = Schema.FromAsset("graphql/schema.graphql"),
                AuthorizationConfig = new AuthorizationConfig
                {
                    DefaultAuthorization = new AuthorizationMode
                    {
                        AuthorizationType = AuthorizationType.API_KEY,
                        ApiKeyConfig = new ApiKeyConfig
                        {
                            Expires = Expiration.After(Duration.Days(365))
                        }
                    },
                },
                XrayEnabled = true,
            });

            // Prints out the AppSync GraphQL endpoint to the terminal
            var graphQLAPIURLOutput = new CfnOutput(this, "GraphQLAPIURL", new CfnOutputProps
            {
                Value = api.GraphqlUrl
            });

            // Prints out the AppSync GraphQL API key to the terminal
            var graphQLAPIKey = new CfnOutput(this, "GraphQLAPIKey", new CfnOutputProps
            {
                Value = api.ApiKey ?? ""
            });
        }
    }
}
