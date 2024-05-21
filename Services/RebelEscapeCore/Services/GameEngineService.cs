using Microsoft.AspNetCore.SignalR;
using RebelEscapeCore.DataStore;
using RebelEscapeCore.Hubs;
using RebelEscapeCore.Models;

namespace RebelEscapeCore.Services
{
    public class GameEngineService : IGameEngineService
    {
        private readonly IHubContext<ConnectionHub> _connectionHub;
        private readonly IDataStore _dataStore;

        public GameEngineService(IHubContext<ConnectionHub> connectionHub, IDataStore dataStore)
        {
            _connectionHub = connectionHub;
            _dataStore = dataStore;
        }

        public async Task<int> GetGameRequestConfirmation(GameRequestParameters parameters)
        {
            var client = _connectionHub.Clients.Client(parameters.TargetClientId);
            var status = await client
                .InvokeAsync<int>("GameRequestConfirmation", parameters.SenderUserName, CancellationToken.None);
            return status;
        }

        public void PlayerMove(MoveDetails moveDetails)
        {
            var gameDetails = _dataStore.GetGameById(moveDetails.GameId);
            if (gameDetails == null)
            {
                throw new Exception("Game not available");
            }

            bool performLogic = gameDetails.UpdateMove(moveDetails);
            _dataStore.UpdateGame(gameDetails);

            if (performLogic)
            {
                Task.Run(() => PerformGameLogic(moveDetails));
            }
        }

        public async Task StartGame(GameStartParameters parameters)
        {
            Random random = new Random();
            PlayerTypes senderPlayerType = random.Next(2) == 0 ? PlayerTypes.Soldier : PlayerTypes.Rebel;
            PlayerTypes targetPlayerType = senderPlayerType == PlayerTypes.Soldier ? PlayerTypes.Rebel : PlayerTypes.Soldier;

            string gameId = Guid.NewGuid().ToString();

            GameStartedParameters senderPlayerParams = new GameStartedParameters();
            senderPlayerParams.PlayerType = senderPlayerType;
            senderPlayerParams.PlayerId = parameters.SenderClientId;
            senderPlayerParams.GameId = gameId;

            GameStartedParameters targetPlayerParams = new GameStartedParameters();
            targetPlayerParams.PlayerType = targetPlayerType;
            targetPlayerParams.PlayerId = parameters.TargetClientId;
            targetPlayerParams.GameId = gameId;

            GameDetails gameData = new GameDetails();
            gameData.GameId = gameId;
            if (senderPlayerParams.PlayerType == PlayerTypes.Rebel)
            {
                gameData.RebelPlayerId = parameters.SenderClientId;
                gameData.SoldierPlayerId = parameters.TargetClientId;
            }
            else
            {
                gameData.RebelPlayerId = parameters.TargetClientId;
                gameData.SoldierPlayerId = parameters.SenderClientId;
            }

            _dataStore.CreateGame(gameData);

            var task1 = _connectionHub.Clients.Client(parameters.SenderClientId).SendAsync("GameStarted", senderPlayerParams);
            var task2 = _connectionHub.Clients.Client(parameters.TargetClientId).SendAsync("GameStarted", targetPlayerParams);

            await Task.WhenAll(task1, task2);
        }

        private void PerformGameLogic(MoveDetails moveDetails)
        {
            var gameDetails = _dataStore.GetGameById(moveDetails.GameId);
            string[] clientIds = new string[] { gameDetails.RebelPlayerId, gameDetails.SoldierPlayerId };
            if (gameDetails.SoldierMove == gameDetails.RebelMove)
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = true;
                moveResult.WinnerPlayerId = gameDetails.SoldierPlayerId;
                _dataStore.RemoveGame(moveDetails.GameId);
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
            else
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = false;
                moveResult.WinnerPlayerId = string.Empty;
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
        }
    }
}
