using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RebelEscapeCore.DataStore;
using RebelEscapeCore.Models;
using RebelEscapeCore.Services;

namespace RebelEscapeCore.Hubs
{
    [Authorize]
    public class ConnectionHub : Hub
    {
        private readonly IDataStore _dataStore;
        private readonly IGameEngineService _gameEngineService;

        public ConnectionHub(IDataStore dataStore, IGameEngineService gameEngineService)
        {
            _dataStore = dataStore;
            _gameEngineService = gameEngineService;
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userName = Context?.User?.Identity?.Name;
            var identity = Context?.User?.Identities.FirstOrDefault();
            var claim = identity?.Claims.First(claim => claim.Type == "userId");
            string? playerId = claim?.Value;
            if (userName == null || playerId == null)
            {
                return Task.CompletedTask;
            }

            if (_dataStore.IsPlayerAlreadyConnected(playerId))
            {
                throw new Exception("Player Already connected");
            }

            Console.WriteLine("Connection opened");
            Console.WriteLine($"{connectionId} -- {userName}");
            _dataStore.StoreConnection(userName, playerId, connectionId);
            RefershConnectedPlayers();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userName = Context.User?.Identity?.Name;
            if (userName == null)
            {
                return Task.CompletedTask;
            }

            Console.WriteLine("Connection closed");
            Console.WriteLine($"{connectionId} -- {userName}");
            _dataStore.RemoveConnection(connectionId);
            RefershConnectedPlayers();
            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("SendMessage")]
        public void SendMessage(object message)
        {
            Console.WriteLine("namaskaram");
            Console.WriteLine($"From client {Context.User?.Identity?.Name}. Message: {message}");
        }

        [HubMethodName("GetResult")]
        public async Task<ClientStatus> GetResultAsync(string targetPlayerId)
        {
            var targetPlayerConnectionId = _dataStore.GetConnectionIdFromPlayerId(targetPlayerId);
            var status = await Clients.Client(targetPlayerConnectionId).InvokeAsync<ClientStatus>("GetCurrentStatus", CancellationToken.None);
            Console.WriteLine($"{status.Status} -- {status.Id}");
            return status;
        }

        [HubMethodName("RequestGame")]
        public async Task<GameConfirmationResult> GetGameRequestConfirmation(GameRequestParameters parameters)
        {
            return await _gameEngineService.GetGameRequestConfirmation(parameters);
        }

        [HubMethodName("StartGame")]
        public async Task StartGame(GameStartParameters parameters)
        {
            await _gameEngineService.StartGame(parameters);
        }

        [HubMethodName("PlayerMove")]
        public void PlayerMove(MoveDetails moveDetails)
        {
            _gameEngineService.PlayerMove(moveDetails);
        }

        [HubMethodName("GetOnlinePlayers")]
        public IEnumerable<PlayerDetails> GetOnlinePlayersList()
        {
            var connectedPlayersWithConnectionId = _dataStore.GetConnectedPlayers();
            var connectedPlayersWithoutConnectionId = GetConnectedPlayers(connectedPlayersWithConnectionId);
            return connectedPlayersWithoutConnectionId;
        }

        private void RefershConnectedPlayers()
        {
            var connectedPlayersWithConnectionId = _dataStore.GetConnectedPlayers();
            var connectedPlayersWithoutConnectionId = GetConnectedPlayers(connectedPlayersWithConnectionId);
            Clients.All.SendAsync("RefreshConnectedUserList", connectedPlayersWithoutConnectionId);
        }

        private IEnumerable<PlayerDetails> GetConnectedPlayers(IEnumerable<ConnectedPlayerDetails> connectedPlayerDetails)
        {
            return connectedPlayerDetails.Select(player => new PlayerDetails()
            {
                PlayerId = player.PlayerId,
                UserName = player.UserName,
            });
        }
    }
}
