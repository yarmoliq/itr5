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
        public static ConcurrentDictionary<string, GameModel> availableGames = new ConcurrentDictionary<string, GameModel>();
        public static ConcurrentDictionary<string, PlayerModel> players = new ConcurrentDictionary<string, PlayerModel>();
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> l)
        {
            _logger = l;
        }

        public string GetConnectionId() => Context.ConnectionId;

        public void NewGame()
        {
            string id = Context.ConnectionId;
            players.TryAdd(id, new PlayerModel(id));


            GameModel newGame = new GameModel(players[id]);
            if(availableGames.TryAdd(newGame.Id, newGame))
            {
                players[id].GameId = id;
            }
        }

        public void ConnectToGame(string gameId)
        {
            GameModel targetGame;
            if( availableGames.TryGetValue(gameId, out targetGame) )
            {
                string id = Context.ConnectionId;
                var x = players.TryAdd(id, new PlayerModel(id));

                players[id].GameId = gameId;

                // _logger.LogWarning("Connected " + id + " to " + targetGame.player1.Id);

                if(targetGame.player2 == null)
                {
                    targetGame.player2 = players[id];
                }
                else
                {
                    targetGame.watchers.Add(players[id]);
                }
            }
        }

        public string[] GetAllGames()
        {
            // _logger.LogInformation(Convert.ToString(availableGames.Count));

            var arr = Array.ConvertAll(availableGames.ToArray(),
                new Converter<KeyValuePair<string,GameModel>, string>((item) => 
                    item.Value.Name + "," + item.Value.Id + "," + Convert.ToString(
                                                                item.Value.watchers.Count
                                                                + Convert.ToInt16(item.Value.player1 != null)
                                                                + Convert.ToInt16(item.Value.player2 != null))));

            return arr;
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // _logger.LogWarning("disconnected: " + Context.ConnectionId);

            PlayerModel disconnectedPlayer;
            if( players.TryGetValue(Context.ConnectionId, out disconnectedPlayer) )
            {
                GameModel game;
                if( availableGames.TryGetValue(disconnectedPlayer.GameId, out game) )
                {
                    if(game.player1 == disconnectedPlayer || game.player2 == disconnectedPlayer)
                    {
                        // we cant skip player1 != null because we
                        // construct game object with player1 assignment
                        await this.Clients.Client(game.player1.Id).SendAsync("Redirect", "Home", "Index");
                        PlayerModel p;
                        players.TryRemove(game.player1.Id, out p);

                        if(game.player2 != null)
                        {
                            await this.Clients.Client(game.player2.Id).SendAsync("Redirect", "Home", "Index");
                            players.TryRemove(game.player2.Id, out p);
                        }

                        game.watchers.ForEach(async (watcher) =>
                        {
                            await this.Clients.Client(watcher.Id).SendAsync("Redirect", "Home", "Index");
                            PlayerModel t;
                            players.TryRemove(watcher.Id, out t);
                        });

                        GameModel g;
                        availableGames.TryRemove(game.Id, out g);
                    }
                    else
                    {
                        game.watchers.Remove(disconnectedPlayer);
                    }
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}