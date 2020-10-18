using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace itr5.Hubs
{
    public class GameHub : Hub
    {
        private static string waitingPlayer;
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> l)
        {
            _logger = l;
        }
        public async Task SendMessage(string message, string connectionId)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, connectionId);
        }

        public async Task MakeMove(string opponentConnectionId, string move)
        {
            await Clients.User(opponentConnectionId).SendAsync("OpponentsMove", move);
        }

        public string GetConnectionId() => Context.ConnectionId;

        public string NewConnection()
        {
            _logger.LogInformation(waitingPlayer);

            if(waitingPlayer == null)
            {
                waitingPlayer = Context.ConnectionId;
                return null;
            }

            string temp = waitingPlayer;
            waitingPlayer = null;
            
            Clients.User(waitingPlayer).SendAsync("AssignOpponent", Context.ConnectionId);

            return temp;
        }
    }
}