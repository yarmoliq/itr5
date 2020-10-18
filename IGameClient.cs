using System.Threading.Tasks;

namespace itr5.Hubs
{
    public interface IGameClient
    {
        Task AssignOpponent(string opponentConnectionId);
        Task OpponentsMove(string move);
    }
}