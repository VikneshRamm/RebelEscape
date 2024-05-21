using RebelEscapeCore.Models;

namespace RebelEscapeCore.DataStore
{
    public interface IDataStore
    {
        void StoreConnectionId(string userName, string connectionId);

        IEnumerable<ConnectedUserDetails> GetConnectionIds();

        void RemoveConnectionId(string connectionId);

        bool IsUserExists(string userName);

        string GetUserNameByConnectionId(string connectionId);

        void CreateGame(GameDetails gameDetails);

        void RemoveGame(string gameId);

        GameDetails GetGameById(string gameId);

        void UpdateGame(GameDetails gameDetails);
    }
}
