using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace itr5.Hubs
{
    public class GameHub : Hub
    {
        // private static string waitingPlayer;
        public static ConcurrentDictionary<string, string[]> availableGames = new ConcurrentDictionary<string, string[]>();
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

        public async Task NewGame()
        {
            // _logger.LogInformation("New connection: " + Context.ConnectionId);
            availableGames[Context.ConnectionId] = new string[] { };
        }

        public string[] GetAllGames()
        {
            var keys = availableGames.Keys;
            // _logger.LogInformation(Convert.ToString(keys.Count));
            string[] array = new string[keys.Count];
            keys.CopyTo(array, 0);
            return array;
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string[] res;
            availableGames.TryRemove(Context.ConnectionId, out res);

            await base.OnDisconnectedAsync(exception);
        }
    }
}