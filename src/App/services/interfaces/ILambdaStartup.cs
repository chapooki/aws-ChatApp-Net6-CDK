using Microsoft.AspNetCore.Builder;

namespace App.interfaces
{
    public interface ILambdaStartup
    {
        WebApplication App { get; }
    }
}