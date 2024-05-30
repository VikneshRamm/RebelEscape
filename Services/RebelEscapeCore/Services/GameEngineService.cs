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

        public async Task<GameConfirmationResult> GetGameRequestConfirmation(GameRequestParameters parameters)
        {
            var targetPlayerConnectionId = _dataStore.GetConnectionIdFromPlayerId(parameters.TargetPlayerId);
            var senderPlayerUserName = _dataStore.GetUserNameFromPlayerId(parameters.SenderPlayerId);
            var client = _connectionHub.Clients.Client(targetPlayerConnectionId);
            var status = await client
                .InvokeAsync<GameConfirmationResult>("GameRequestConfirmation", senderPlayerUserName, CancellationToken.None);
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
            var senderConnectionId = _dataStore.GetConnectionIdFromPlayerId(parameters.SenderPlayerId);
            var targetConnectionId = _dataStore.GetConnectionIdFromPlayerId(parameters.TargetPlayerId);

            string gameId = Guid.NewGuid().ToString();

            GameStartedParameters senderPlayerParams = new GameStartedParameters();
            senderPlayerParams.PlayerType = senderPlayerType;
            senderPlayerParams.PlayerId = parameters.SenderPlayerId;
            senderPlayerParams.GameId = gameId;

            GameStartedParameters targetPlayerParams = new GameStartedParameters();
            targetPlayerParams.PlayerType = targetPlayerType;
            targetPlayerParams.PlayerId = parameters.TargetPlayerId;
            targetPlayerParams.GameId = gameId;

            GameDetails gameData = new GameDetails();
            gameData.GameId = gameId;
            if (senderPlayerParams.PlayerType == PlayerTypes.Rebel)
            {
                gameData.RebelPlayerId = parameters.SenderPlayerId;
                gameData.SoldierPlayerId = parameters.TargetPlayerId;
            }
            else
            {
                gameData.RebelPlayerId = parameters.TargetPlayerId;
                gameData.SoldierPlayerId = parameters.SenderPlayerId;
            }

            _dataStore.CreateGame(gameData);

            var task1 = _connectionHub.Clients.Client(senderConnectionId).SendAsync("GameStarted", senderPlayerParams);
            var task2 = _connectionHub.Clients.Client(targetConnectionId).SendAsync("GameStarted", targetPlayerParams);

            await Task.WhenAll(task1, task2);
        }

        private void PerformGameLogic(MoveDetails moveDetails)
        {
            int finalStageNumber = 9;
            var gameDetails = _dataStore.GetGameById(moveDetails.GameId);
            var rebelPlayerConnectionId = _dataStore.GetConnectionIdFromPlayerId(gameDetails.RebelPlayerId);
            var soldierPlayerConnectionId = _dataStore.GetConnectionIdFromPlayerId(gameDetails.SoldierPlayerId);
            string[] clientIds = new string[] { rebelPlayerConnectionId, soldierPlayerConnectionId };
            if (gameDetails.SoldierMove == -1 && gameDetails.RebelMove == -1) // both timed out - no winner
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = true;
                moveResult.WinnerPlayerId = string.Empty;
                moveResult.SoldierMove = moveResult.RebelMove = -1;
                moveResult.SoldierScore = moveResult.RebelScore = 0;
                _dataStore.RemoveGame(moveDetails.GameId);
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
            else if (gameDetails.SoldierMove == -1) // soldier timeout
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = true;
                moveResult.WinnerPlayerId = gameDetails.RebelPlayerId;
                moveResult.SoldierMove = -1;
                moveResult.SoldierScore = 0;
                moveResult.RebelMove = gameDetails.RebelMove;
                moveResult.RebelScore = CalculateRebelScore(moveDetails.CurrentStage);
                _dataStore.RemoveGame(moveDetails.GameId);
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
            else if (gameDetails.RebelMove == -1) // rebel timeout
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = true;
                moveResult.WinnerPlayerId = gameDetails.SoldierPlayerId;
                moveResult.RebelMove = -1;
                moveResult.RebelScore = 0;
                moveResult.SoldierMove = gameDetails.SoldierMove;
                moveResult.SoldierScore = CalculateSoldierScore(moveDetails.CurrentStage);
                _dataStore.RemoveGame(moveDetails.GameId);
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
            else if (gameDetails.SoldierMove == gameDetails.RebelMove) // soldier hit
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = true;
                moveResult.WinnerPlayerId = gameDetails.SoldierPlayerId;
                moveResult.RebelMove = gameDetails.RebelMove;
                moveResult.RebelScore = CalculateRebelScore(moveDetails.CurrentStage);
                moveResult.SoldierMove = gameDetails.SoldierMove;
                moveResult.SoldierScore = CalculateSoldierScore(moveDetails.CurrentStage);
                _dataStore.RemoveGame(moveDetails.GameId);
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
            else if (moveDetails.CurrentStage == finalStageNumber) // rebel escaped
            {
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = true;
                moveResult.WinnerPlayerId = gameDetails.RebelPlayerId;
                moveResult.RebelMove = gameDetails.RebelMove;
                moveResult.RebelScore = CalculateRebelScore(moveDetails.CurrentStage);
                moveResult.SoldierMove = gameDetails.SoldierMove;
                moveResult.SoldierScore = CalculateSoldierScore(moveDetails.CurrentStage);
                _dataStore.RemoveGame(moveDetails.GameId);
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
            else
            {
                gameDetails.ResetMoves();
                MoveResult moveResult = new MoveResult();
                moveResult.IsGameOver = false;
                moveResult.WinnerPlayerId = string.Empty;
                _connectionHub.Clients.Clients(clientIds).SendAsync("MoveResult", moveResult);
            }
        }

        private int CalculateRebelScore(int currentStage)
        {
            if (currentStage == 9)
            {
                return 100;
            }

            return 10 * (currentStage - 1);
        }

        private int CalculateSoldierScore(int currentStage)
        {
            if (currentStage == 9)
            {
                return 10;
            }

            return 100 - ((currentStage - 1) * 10);
        }
    }
}
