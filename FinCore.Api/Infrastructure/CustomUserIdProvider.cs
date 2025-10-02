using Microsoft.AspNetCore.SignalR;

namespace FinCore.Api.Infrastructure;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("sub")?.Value;
    }
}