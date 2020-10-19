using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Collections.Concurrent;

namespace itr5.Models
{
    public class GameModel
    {
        public string Id { get; }

        public string Name;
        public PlayerModel player1 { get; set; }
        public PlayerModel player2 { get; set; }

        public List<PlayerModel> watchers = new List<PlayerModel>();

        public GameModel(PlayerModel creator)
        {
            Id = creator.Id;
            player1 = creator;
        }
    }
}