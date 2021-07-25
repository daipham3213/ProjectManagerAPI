using Microsoft.AspNetCore.SignalR;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Core.Resources;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagerAPI.SignalR
{
    public class RequestHub : Hub
    {
        public async Task SendSignal(string username, RequestResource request)
        {
            await Clients.All
                .SendAsync("ReceiveMessage", username, request);
        }
    }
}