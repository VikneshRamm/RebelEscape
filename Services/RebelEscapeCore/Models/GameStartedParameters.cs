namespace RebelEscapeCore.Models
{
    public class GameStartedParameters
    {
        public string GameId { get; set; } = string.Empty;

        public string PlayerId { get; set; } = string.Empty;

        public PlayerTypes PlayerType { get; set; }
    }
}
