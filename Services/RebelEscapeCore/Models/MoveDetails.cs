namespace RebelEscapeCore.Models
{
    public class MoveDetails
    {
        public string GameId { get; set; } = string.Empty;

        public string PlayerId { get; set; } = string.Empty;

        public PlayerTypes PlayerType { get; set; }

        public int Move { get; set; }
    }
}
