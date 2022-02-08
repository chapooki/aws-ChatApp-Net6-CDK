# Empty AWS Serverless Application Project

This starter project consists of:
* serverless.template - An AWS CloudFormation Serverless Application Model template file for declaring your Serverless functions and other AWS resources
* Function.cs - Class file containing the C# method mapped to the single function declared in the template file
* aws-lambda-tools-defaults.json - Default argument settings for use within Visual Studio and command line deployment tools for AWS

You may also have a test project depending on the options selected.

The generated project contains a Serverless template declaration for a single AWS Lambda function that will be exposed through Amazon API Gateway as a HTTP *Get* operation. Edit the template to customize the function or add more functions and other resources needed by your application, and edit the function code in Function.cs. You can then deploy your Serverless application.

## Packaging as a Docker image.

This project is configured to package the Lambda function as a Docker image. The default configuration for the project and the Dockerfile is to build 
the .NET project on the host machine and then execute the `docker build` command which copies the .NET build artifacts from the host machine into 
the Docker image. 

The `--docker-host-build-output-dir` switch, which is set in the `aws-lambda-tools-defaults.json`, triggers the 
AWS .NET Lambda tooling to build the .NET project into the directory indicated by `--docker-host-build-output-dir`. The Dockerfile 
has a **COPY** command which copies the value from the directory pointed to by `--docker-host-build-output-dir` to the `/var/task` directory inside of the 
image.

Alternatively the Docker file could be written to use [multi-stage](https://docs.docker.com/develop/develop-images/multistage-build/) builds and 
have the .NET project built inside the container. Below is an example of building the .NET project inside the image.

```dockerfile
FROM public.ecr.aws/lambda/dotnet:6 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim as build
WORKDIR /src
COPY ["App.csproj", "App/"]
RUN dotnet restore "App/App.csproj"

WORKDIR "/src/App"
COPY . .
RUN dotnet build "App.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "App.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
```

When building the .NET project inside the image you must be sure to copy all of the class libraries the .NET Lambda project is depending on 
as well before the `dotnet build` step. The final published artifacts of the .NET project must be copied to the `/var/task` directory. 
The `--docker-host-build-output-dir` switch can also be removed from the `aws-lambda-tools-defaults.json` to avoid the 
.NET project from being built on the host machine before calling `docker build`.

## Here are some steps to follow from Visual Studio:

To deploy your Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view your deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "App/test/App.Tests"
    dotnet test
```

Deploy application
```
    cd "App/src/App"
    dotnet lambda deploy-serverless
```


--------------------------------------------------
To get a bearer token from cognito:

Step 1 - Sign into Cognito: https://naz-chat-app-auth.auth.ap-southeast-2.amazoncognito.com/login?client_id={clientId}&response_type=token&scope=aws.cognito.signin.user.admin+email+openid+phone+profile&redirect_uri={redirectUrl}
e.g.
https://naz-chat-app-auth.auth.ap-southeast-2.amazoncognito.com/login?client_id=5u58t4rp2n0dpgntk3tdokgj1q&response_type=token&scope=aws.cognito.signin.user.admin+email+openid+phone+profile&redirect_uri=https://example.com

Step 2 - get the access token from the Cognito redirect Url:
e.g.
e.g.
https://example.com/#id_token=eyJraWQiOiJpaWxFZ0NOcnhZZU5LZ2JYUnRjM0VKM1JTOW5LUzRPa3I3RFErTTB0cHJZPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiYzhvQTVxa0FRWnRPdTBSRUxmaDBsZyIsInN1YiI6IjJiMDVmM2VjLWUyNWItNDk0OC1iYzhjLTc5YWNjNmE0YTY1NSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtc291dGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtc291dGhlYXN0LTJfR09MZnVyNFl5IiwiY29nbml0bzp1c2VybmFtZSI6InVzZXIxIiwiYXVkIjoiNXU1OHQ0cnAybjBkcGdudGszdGRva2dqMXEiLCJldmVudF9pZCI6IjhiN2MyMzlmLWI3YjAtNDVkOS05NThjLTg5YjE0N2QxNTNkNCIsInRva2VuX3VzZSI6ImlkIiwiYXV0aF90aW1lIjoxNjQzOTMzNzg4LCJleHAiOjE2NDM5MzczODgsImlhdCI6MTY0MzkzMzc4OCwianRpIjoiMGExZjgyZDItZGRlZC00MGMzLTk1OTktNDE0MGI1M2FjZDBlIiwiZW1haWwiOiJuYXoucmFkbWFuZEBwdXJwbGUudGVsc3RyYS5jb20ifQ.KelP17bd1q2AIDyL837WC22Ja6tich127NQUa-BpIf5lZ14GWSgPLO0ZLoK-yAEVJ2rKUEwUpICg_udlCliXzahFdhx9pG1belckbLj733IlyY6Jqdl35jXNAvznWHrdv9Y0YXZ17fcJqvaC8QJ3IVsPTEObyNcSt1wrqRRBh0r6ZhXBOYXs5Bbnf5Ue1p1aol_a-KFWhPcUzfhbHtbPCt8Sz1VgD-f-yEA8NlRqi8NBMxkAneFNIzJuH8LPrm0bVAn580TJs6AF7dUbv439VfpyCzYhbxjji8Tq8Q8MnGy6YjcRjHFfgOa_Ctu6StTf8xNtMKcp0nfSge2cz3EIxw&access_token=eyJraWQiOiJ4Wnc5VlI1XC9nMVZLMUNzQzkrSkszXC8xRXBRSDBXVG1zZ3hPYXJDQ0d5NkU9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiIyYjA1ZjNlYy1lMjViLTQ5NDgtYmM4Yy03OWFjYzZhNGE2NTUiLCJldmVudF9pZCI6IjhiN2MyMzlmLWI3YjAtNDVkOS05NThjLTg5YjE0N2QxNTNkNCIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4gcGhvbmUgb3BlbmlkIHByb2ZpbGUgZW1haWwiLCJhdXRoX3RpbWUiOjE2NDM5MzM3ODgsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5hcC1zb3V0aGVhc3QtMi5hbWF6b25hd3MuY29tXC9hcC1zb3V0aGVhc3QtMl9HT0xmdXI0WXkiLCJleHAiOjE2NDM5MzczODgsImlhdCI6MTY0MzkzMzc4OCwidmVyc2lvbiI6MiwianRpIjoiZGZhNDk5NGMtNGQ4ZC00MWRlLThlMTAtNzQ4NTkyM2EyOWI0IiwiY2xpZW50X2lkIjoiNXU1OHQ0cnAybjBkcGdudGszdGRva2dqMXEiLCJ1c2VybmFtZSI6InVzZXIxIn0.PusYHV3Z3lfCWCHppfCPeJ4w3O_cKDGhH2M2l7-UNti2faqdvHqFS-GV0E27pj6rPSpuiXoBIpZbJc77O-YL-GuNPdShBkP9ZK5yptMJLCkpG3B0ySgJc7er9klfFubeyaGe8h7FTIpXi7vPwfri0xFd-TRdwCIsIaPEyqrqzQ-xZZ4eStFDl-SuH02Lcfv7eoSWJXhh7WeowmtHE3g99ac0EdEiXYYSSQnlY-1QjroXwFClyrdab6FfpaVtXjj4hBthMKXV1QzSYZ5f0jxgInV4mcHhKTYAH9uYpQJmJMQ1TDiaAr6N7Hc649AyC-0LQ8t0C7u_J0RNBVQIGnDCsQ&expires_in=3600&token_type=Bearer

Id token:
eyJraWQiOiJpaWxFZ0NOcnhZZU5LZ2JYUnRjM0VKM1JTOW5LUzRPa3I3RFErTTB0cHJZPSIsImFsZyI6IlJTMjU2In0.eyJhdF9oYXNoIjoiYzhvQTVxa0FRWnRPdTBSRUxmaDBsZyIsInN1YiI6IjJiMDVmM2VjLWUyNWItNDk0OC1iYzhjLTc5YWNjNmE0YTY1NSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczpcL1wvY29nbml0by1pZHAuYXAtc291dGhlYXN0LTIuYW1hem9uYXdzLmNvbVwvYXAtc291dGhlYXN0LTJfR09MZnVyNFl5IiwiY29nbml0bzp1c2VybmFtZSI6InVzZXIxIiwiYXVkIjoiNXU1OHQ0cnAybjBkcGdudGszdGRva2dqMXEiLCJldmVudF9pZCI6IjhiN2MyMzlmLWI3YjAtNDVkOS05NThjLTg5YjE0N2QxNTNkNCIsInRva2VuX3VzZSI6ImlkIiwiYXV0aF90aW1lIjoxNjQzOTMzNzg4LCJleHAiOjE2NDM5MzczODgsImlhdCI6MTY0MzkzMzc4OCwianRpIjoiMGExZjgyZDItZGRlZC00MGMzLTk1OTktNDE0MGI1M2FjZDBlIiwiZW1haWwiOiJuYXoucmFkbWFuZEBwdXJwbGUudGVsc3RyYS5jb20ifQ.KelP17bd1q2AIDyL837WC22Ja6tich127NQUa-BpIf5lZ14GWSgPLO0ZLoK-yAEVJ2rKUEwUpICg_udlCliXzahFdhx9pG1belckbLj733IlyY6Jqdl35jXNAvznWHrdv9Y0YXZ17fcJqvaC8QJ3IVsPTEObyNcSt1wrqRRBh0r6ZhXBOYXs5Bbnf5Ue1p1aol_a-KFWhPcUzfhbHtbPCt8Sz1VgD-f-yEA8NlRqi8NBMxkAneFNIzJuH8LPrm0bVAn580TJs6AF7dUbv439VfpyCzYhbxjji8Tq8Q8MnGy6YjcRjHFfgOa_Ctu6StTf8xNtMKcp0nfSge2cz3EIxw

Access token:
eyJraWQiOiJ4Wnc5VlI1XC9nMVZLMUNzQzkrSkszXC8xRXBRSDBXVG1zZ3hPYXJDQ0d5NkU9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiIyYjA1ZjNlYy1lMjViLTQ5NDgtYmM4Yy03OWFjYzZhNGE2NTUiLCJldmVudF9pZCI6IjhiN2MyMzlmLWI3YjAtNDVkOS05NThjLTg5YjE0N2QxNTNkNCIsInRva2VuX3VzZSI6ImFjY2VzcyIsInNjb3BlIjoiYXdzLmNvZ25pdG8uc2lnbmluLnVzZXIuYWRtaW4gcGhvbmUgb3BlbmlkIHByb2ZpbGUgZW1haWwiLCJhdXRoX3RpbWUiOjE2NDM5MzM3ODgsImlzcyI6Imh0dHBzOlwvXC9jb2duaXRvLWlkcC5hcC1zb3V0aGVhc3QtMi5hbWF6b25hd3MuY29tXC9hcC1zb3V0aGVhc3QtMl9HT0xmdXI0WXkiLCJleHAiOjE2NDM5MzczODgsImlhdCI6MTY0MzkzMzc4OCwidmVyc2lvbiI6MiwianRpIjoiZGZhNDk5NGMtNGQ4ZC00MWRlLThlMTAtNzQ4NTkyM2EyOWI0IiwiY2xpZW50X2lkIjoiNXU1OHQ0cnAybjBkcGdudGszdGRva2dqMXEiLCJ1c2VybmFtZSI6InVzZXIxIn0.PusYHV3Z3lfCWCHppfCPeJ4w3O_cKDGhH2M2l7-UNti2faqdvHqFS-GV0E27pj6rPSpuiXoBIpZbJc77O-YL-GuNPdShBkP9ZK5yptMJLCkpG3B0ySgJc7er9klfFubeyaGe8h7FTIpXi7vPwfri0xFd-TRdwCIsIaPEyqrqzQ-xZZ4eStFDl-SuH02Lcfv7eoSWJXhh7WeowmtHE3g99ac0EdEiXYYSSQnlY-1QjroXwFClyrdab6FfpaVtXjj4hBthMKXV1QzSYZ5f0jxgInV4mcHhKTYAH9uYpQJmJMQ1TDiaAr6N7Hc649AyC-0LQ8t0C7u_J0RNBVQIGnDCsQ

Step3 - Pass the access token in API call Header as bearer token

Step4 - 
use https://jwt.io/ to read the content of the JWT. 
    - if you use ID token: The "sub" field is userId and "cognito:username" is username
    - if you use access token: The "sub" field is userId and "username"is username


-----------------------------------------------------