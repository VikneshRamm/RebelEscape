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
            if (userName == null)
            {
                return Task.CompletedTask;
            }

            if (_dataStore.IsUserExists(userName))
            {
                throw new Exception("User already exists");
            }

            Console.WriteLine("Connection opened");
            Console.WriteLine($"{connectionId} -- {userName}");
            _dataStore.StoreConnectionId(userName, connectionId);
            Clients.All.SendAsync("RefreshConnectedUserList", _dataStore.GetConnectionIds());
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
            _dataStore.RemoveConnectionId(userName);
            Clients.All.SendAsync("RefreshConnectedUserList", _dataStore.GetConnectionIds());
            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("SendMessage")]
        public void SendMessage(object message)
        {
            Console.WriteLine("namaskaram");
            Console.WriteLine($"From client {Context.User?.Identity?.Name}. Message: {message}");
        }

        [HubMethodName("GetResult")]
        public async Task<ClientStatus> GetResultAsync(string clientId)
        {
            var status = await Clients.Client(clientId).InvokeAsync<ClientStatus>("GetCurrentStatus", CancellationToken.None);
            Console.WriteLine($"{status.Status} -- {status.Id}");
            return status;
        }

        [HubMethodName("RequestGame")]
        public async Task<int> GetGameRequestConfirmation(GameRequestParameters parameters)
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
    }
}
