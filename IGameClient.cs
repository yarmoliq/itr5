using System.Threading.Tasks;

namespace itr5.Hubs
{
    public interface IGameClient
    {
        Task GameUpdate(string[] board); // there is 9 cells

        Task GameWon(string message);

        Task Redirect(string controller, string action, string message);
    }
}