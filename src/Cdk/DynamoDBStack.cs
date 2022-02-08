using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Shared;

namespace Cdk
{
    public class DynamoDBStack : Stack
    {
        public Table ChatTable { get; private set; }

        internal DynamoDBStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var table = new Table(this, $"{Constants.AwsResourcesPrefix}chatTable1", new TableProps {
                TableName = Shared.Constants.MainTableName,
                PartitionKey =  new Attribute { Name = Shared.Constants.PartitionKeyField, Type = AttributeType.STRING },
                SortKey = new Attribute { Name = Shared.Constants.SortKeyField, Type = AttributeType.STRING },
                BillingMode = BillingMode.PROVISIONED,
                ReadCapacity = 1,
                WriteCapacity = 1,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            table.AddLocalSecondaryIndex(new LocalSecondaryIndexProps { 
                IndexName = $"{Shared.Constants.SecondaryIndexField}Index",
                SortKey = new Attribute { Name = Shared.Constants.SecondaryIndexField, Type = AttributeType.NUMBER },
                ProjectionType = ProjectionType.ALL
            });

            ChatTable = table;
            new CfnOutput(this, "chatApp_chatTable", new CfnOutputProps { Value = table.TableArn });
        }
    }
}
