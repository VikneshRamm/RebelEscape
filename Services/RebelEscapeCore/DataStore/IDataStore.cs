using RebelEscapeCore.Models;

namespace RebelEscapeCore.DataStore
{
    public interface IDataStore
    {
        void StoreConnection(string userName, string playerId, string userId);

        IEnumerable<ConnectedPlayerDetails> GetConnectedPlayers();

        void RemoveConnection(string connectionId);

        bool IsPlayerAlreadyConnected(string playerId);

        string GetConnectionIdFromPlayerId(string playerId);

        string GetUserNameFromPlayerId(string playerId);

        void CreateGame(GameDetails gameDetails);

        void RemoveGame(string gameId);

        GameDetails GetGameById(string gameId);

        void UpdateGame(GameDetails gameDetails);
    }
}
