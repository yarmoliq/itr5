using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace itr5.Models
{
    public class PlayerModel
    {
        public string Id { get; }

        public string GameId { get; set; }
        public PlayerModel(string playerId)
        {
            Id = playerId;
        }
    }
}