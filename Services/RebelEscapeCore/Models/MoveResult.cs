namespace RebelEscapeCore.Models
{
    public class MoveResult
    {
        public bool IsGameOver { get; set; }

        public string WinnerPlayerId { get; set; } = string.Empty;
    }
}
