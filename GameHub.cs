using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace itr5.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendMessage(string message, string connectionId)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, connectionId);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}