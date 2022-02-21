namespace App.Models.CognitoLambdaTriggers
{
    public class LambdaTriggerRequestBase
    {
        public class CallerContextData
        {
            public string awsSdkVersion { get; set; }
            public string clientId { get; set; }
        }

        public string version { get; set; }
        public string triggerSource { get; set; }
        public string region { get; set; }
        public string userPoolId { get; set; }
        public string userName { get; set; }
        public CallerContextData callerContext { get; set; }
    }
}
