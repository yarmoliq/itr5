using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using itr5.Models;

namespace itr5.Hubs
{
    public class GameHub : Hub
    {
        // private static string waitingPlayer;
        public static ConcurrentDictionary<string, GameModel> availableGames = new ConcurrentDictionary<string, GameModel>();
        public static ConcurrentDictionary<string, PlayerModel> players = new ConcurrentDictionary<string, PlayerModel>();
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> l)
        {
            _logger = l;
        }

        public override async Task OnConnectedAsync()
        {
            PlayerModel newPlayer = new PlayerModel(Context.ConnectionId);
            players[newPlayer.Id] = newPlayer;

            await base.OnConnectedAsync();
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
            PlayerModel player;
            if(players.TryGetValue(Context.ConnectionId, out player))
            {
                availableGames[player.Id] = new GameModel(player);
                player.GameId = player.Id; // see: GameModel constructor
            }
        }

        public string[] GetAllGames()
        {
            var keys = availableGames.Keys;
            string[] array = new string[keys.Count];
            keys.CopyTo(array, 0);
            return array;
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            PlayerModel disconnectedPlayer;
            if (players.TryGetValue(Context.ConnectionId, out disconnectedPlayer))
            {
                GameModel game;
                if (availableGames.TryGetValue(disconnectedPlayer.GameId, out game))
                {
                    if (game.player1 == disconnectedPlayer ||
                        game.player2 == disconnectedPlayer)
                    {
                        // kill game
                        GameModel removeGame;
                        availableGames.TryRemove(game.Id, out removeGame);
                    }
                    else
                    {
                        game.watchers.Remove(disconnectedPlayer);
                    }
                }

                PlayerModel removePlayer;
                players.TryRemove(disconnectedPlayer.Id, out removePlayer);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}