﻿using Amazon.CDK;
using static Cdk.CognitoStack;
using static Cdk.LambdasStack;

namespace Cdk
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new Amazon.CDK.App();

            var env = new Amazon.CDK.Environment
            {
                Account = "694976553616", // "379268798630"
                Region = "ap-southeast-2",
            };

            var dynamoDBStack = new DynamoDBStack(app, "ChatAppDynamoDBStack", new StackProps
            {
                Env = env
            });

            var cognitoStack = new CognitoStack(app, "ChatAppCognitoStack", new CognitoStackProps
            {
                Env = env,
                ChatTable = dynamoDBStack.ChatTable,
            });

            var lambdasStack = new LambdasStack(app, "ChatAppLambdasStack", new LambdasStackProps
            {
                Env = env,
                ChatTable = dynamoDBStack.ChatTable,
                UserPool = cognitoStack.UserPool
            });

            app.Synth();
        }
    }
}
