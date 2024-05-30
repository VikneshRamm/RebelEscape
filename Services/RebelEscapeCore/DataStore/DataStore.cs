using RebelEscapeCore.Models;

namespace RebelEscapeCore.DataStore
{
    public class DataStore : IDataStore
    {
        private readonly Dictionary<string, ConnectedPlayerDetails> _connectedPlayers;
        private readonly Dictionary<string, GameDetails> _gameDetails;

        public DataStore()
        {
            _connectedPlayers = new Dictionary<string, ConnectedPlayerDetails>();
            _gameDetails = new Dictionary<string, GameDetails>();
        }

        public IEnumerable<ConnectedPlayerDetails> GetConnectedPlayers()
        {
            return _connectedPlayers.Values;
        }

        public bool IsPlayerAlreadyConnected(string playerId)
        {
            return _connectedPlayers.ContainsKey(playerId);
        }

        public void RemoveConnection(string connectionId)
        {
            string playerId = _connectedPlayers.FirstOrDefault(player => player.Value.ConnectionId == connectionId).Key;
            if (playerId != null)
            {
                _connectedPlayers.Remove(playerId);
            }
        }

        public string GetConnectionIdFromPlayerId(string playerId)
        {
            return _connectedPlayers[playerId].ConnectionId;
        }

        public string GetUserNameFromPlayerId(string playerId)
        {
            return _connectedPlayers[playerId].UserName;
        }

        public void StoreConnection(string userName, string playerId, string connectionId)
        {
            _connectedPlayers.Add(playerId, new ConnectedPlayerDetails()
            {
                UserName = userName,
                ConnectionId = connectionId,
                PlayerId = playerId,
            });
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
