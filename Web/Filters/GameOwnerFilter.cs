using System.Reflection;
using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.SignalR;
using Web.Attributes;

namespace Web.Filters;

public class GameOwnerFilter(GameService gameService) : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context,
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var attr = context.HubMethod.GetCustomAttribute<RequireOwnerAttribute>();
        if (attr is null)
            return await next(context);

        var userId = Guid.Parse(context.Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var methodParams = context.HubMethod.GetParameters();
        var paramIndex = Array.FindIndex(methodParams, p => p.Name == attr.CodeParamName);
        var arg = context.HubMethodArguments[paramIndex];
        var code = arg is string s ? s : (string)arg.GetType().GetProperty("Code")!.GetValue(arg)!;

        await gameService.VerifyAndCacheGame(code, userId);
        return await next(context);
    }
}
