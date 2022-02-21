using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cdk
{
    internal class PermissionHelper
    {
        public static void GiveAccessToChatTable(Table chatTable, Function lambda)
        {
            chatTable.GrantReadWriteData(lambda);
        }

        public static void GiveAccessToChatAppSSM(Function lambda)
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
