using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace OilCaseApi.hub
{
    public class MouseHub : Hub
    {
        public static bool CorsCheck(string origin)
        {
            Console.WriteLine(origin);
            return true;
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("SendMessage", message);
        }

        public async Task SendUserMessage(string message, string userName)
        {
            await Clients.All.SendAsync("SendUserMessage", message, userName);
        }

        public async Task SendTeamCoord(string message, string userName)
        {
            await Clients.All.SendAsync("SendTeamCoord", "SendTeamCoord " + message, userName);
        }

        public async Task SendTeamCoordxy(int x, int y, string userName, string groupname)
        {
            await Clients.Group(groupname).SendAsync("SendTeamCoordxy", x, y, userName);
        }

        public async Task SendTeamCoordScroll(int scrolly, string userName, string groupname)
        {
            await Clients.Group(groupname).SendAsync("SendTeamCoordScroll", scrolly, userName);
        }

        public async Task SendTeamMessage(string userName, string groupname, string message)
        {
            await Clients.Group(groupname).SendAsync("SendTeamMessage", message, userName);
        }
        
        public async Task SendUserEnter(string group, string userName)
        {
            Debug.WriteLine($"-{group}-");
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Caller.SendAsync("SendMessage", "UserEntered to hub  " + group);
        }

        public async Task OnConnected()
        {
            var dsdf = Context.ConnectionId;
            var sdf = 345;
        }
    }
}