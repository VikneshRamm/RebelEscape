using RebelEscapeCore.Models;

namespace RebelEscapeCore.DataStore
{
    public class DataStore : IDataStore
    {
        private readonly Dictionary<string, string> _connectionIds;
        private readonly Dictionary<string, GameDetails> _gameDetails;

        public DataStore()
        {
            _connectionIds = new Dictionary<string, string>();
            _gameDetails = new Dictionary<string, GameDetails>();
        }

        public IEnumerable<ConnectedUserDetails> GetConnectionIds()
        {
            return _connectionIds.Select(k => new ConnectedUserDetails() { UserName = k.Key, ConnectionId = k.Value });
        }

        public bool IsUserExists(string userName)
        {
            return _connectionIds.ContainsKey(userName);
        }

        public void RemoveConnectionId(string userName)
        {
            _connectionIds.Remove(userName);
        }

        public void StoreConnectionId(string userName, string connectionId)
        {
            _connectionIds.Add(userName, connectionId);
        }

        public string GetUserNameByConnectionId(string connectionId)
        {
            return _connectionIds.First(item => item.Value == connectionId).Key;
        }

        public void CreateGame(GameDetails gameDetails)
        {
            _gameDetails.Add(gameDetails.GameId, gameDetails);
        }

        public void RemoveGame(string gameId)
        {
            _gameDetails.Remove(gameId);
        }

        public GameDetails GetGameById(string gameId)
        {
            return _gameDetails[gameId];
        }

        public void UpdateGame(GameDetails gameDetails)
        {
            _gameDetails[gameDetails.GameId] = gameDetails;
        }
    }
}
