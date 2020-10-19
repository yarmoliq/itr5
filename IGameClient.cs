using System.Threading.Tasks;

namespace itr5.Hubs
{
    public interface IGameClient
    {
        Task Move(string move);

        Task Redirect(string controller, string action);

        Task RedirectToGame(string gameId);
    }
}