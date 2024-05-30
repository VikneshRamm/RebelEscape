using RebelEscapeCore.Models;

namespace RebelEscapeCore.Services
{
    public interface IGameEngineService
    {
        Task<GameConfirmationResult> GetGameRequestConfirmation(GameRequestParameters parameters);

        Task StartGame(GameStartParameters parameters);

        void PlayerMove(MoveDetails moveDetails);
    }
}
