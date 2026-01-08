using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SAT.API.Hubs;

[Authorize]
public class ExamHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task JoinTestGroup(Guid testId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetTestGroupName(testId));
    }

    public async Task LeaveTestGroup(Guid testId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetTestGroupName(testId));
    }

    public async Task BroadcastTimer(Guid testId, int remainingSeconds)
    {
        await Clients.Group(GetTestGroupName(testId))
            .SendAsync("timerUpdated", new { testId, remainingSeconds });
    }

    private static string GetTestGroupName(Guid testId) => $"test-{testId}";
}
