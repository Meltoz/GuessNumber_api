using Application.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;
using Web.Hubs.Interfaces;

namespace Web.Filters;
public class HubExceptionFilter : IHubFilter
{
    private readonly ILogger<HubExceptionFilter> _logger;
    private static string Critical = "critical";
    private static string Error = "error";

    public HubExceptionFilter(ILogger<HubExceptionFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (ArgumentEmptyException ex)
        {
            _logger.LogWarning(ex, "Argument empty: {Message}", ex.Message);
            await SendError(invocationContext, Error);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex, "Entity Not Found: {Message}", ex.Message);
            await SendError(invocationContext, Error);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            await SendError(invocationContext, Error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled hub exception in {HubMethod}", invocationContext.HubMethodName);
            await SendError(invocationContext, Critical);
        }
        
        return null;
    }

    private async Task SendError(HubInvocationContext context, string typeErreur)
    {
            var caller = (context.Hub as Hub<IGameHubClient>)?.Clients.Caller;
            if (caller is not null)
                await caller.ReceiveError(typeErreur, "Une erreur est survenue");
    }
}


