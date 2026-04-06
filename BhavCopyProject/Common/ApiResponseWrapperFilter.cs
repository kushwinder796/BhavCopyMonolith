using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BhavCopyProject.Common;

public sealed class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
       
        if (context.HttpContext.Response.HasStarted)
        {
            await next();
            return;
        }

        
        if (context.Result is FileResult)
        {
            await next();
            return;
        }

        
        if (context.Result is ObjectResult obj)
        {
            if (obj.Value is ApiResponseBase)
            {
                await next();
                return;
            }

            var status = obj.StatusCode ?? context.HttpContext.Response.StatusCode;
            if (status == 0) status = StatusCodes.Status200OK;

            var success = status is >= 200 and <= 299;
            var message = ExtractMessage(obj.Value, success);

            var wrapped = success
                ? ApiResponse<object?>.Ok(obj.Value, status, message)
                : ApiResponse<object?>.Fail(obj.Value, status, message);

            obj.Value = wrapped;
            obj.StatusCode = status;

            await next();
            return;
        }

        
        if (context.Result is StatusCodeResult sc)
        {
            var success = sc.StatusCode is >= 200 and <= 299;
            var message = success ? "OK" : "Request failed";

            context.Result = new ObjectResult(success
                ? ApiResponse<object?>.Ok(null, sc.StatusCode, message)
                : ApiResponse<object?>.Fail(null, sc.StatusCode, message))
            { StatusCode = sc.StatusCode };

            await next();
            return;
        }

        await next();
    }

    private static string ExtractMessage(object? value, bool success)
    {
        if (success) return "OK";

        if (value is null) return "Request failed";

        
        var prop = value.GetType().GetProperty("message") ?? value.GetType().GetProperty("Message");
        if (prop?.GetValue(value) is string s && !string.IsNullOrWhiteSpace(s))
            return s;

        return "Request failed";
    }
}

