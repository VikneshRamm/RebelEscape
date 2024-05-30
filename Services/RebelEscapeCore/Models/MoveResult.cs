namespace RebelEscapeCore.Models
{
    public class MoveResult
    {
        public bool IsGameOver { get; set; }

        public string WinnerPlayerId { get; set; } = string.Empty;

        public int SoldierMove { get; set; }

        public int RebelMove { get; set; }

        public int SoldierScore { get; set; }

        public int RebelScore { get; set; }
    }
}
