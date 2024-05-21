namespace RebelEscapeCore.Models
{
    public class GameDetails
    {
        private object _lock = new object();
        private int _soldierMove = 0;
        private int _rebelMove = 0;

        public int SoldierMove
        {
            get
            {
                return _soldierMove;
            }
        }

        public int RebelMove
        {
            get
            {
                return _rebelMove;
            }
        }

        public string GameId { get; set; } = string.Empty;

        public string SoldierPlayerId { get; set; } = string.Empty;

        public string RebelPlayerId { get; set; } = string.Empty;

        public void ResetMoves()
        {
            _soldierMove = 0;
            _rebelMove = 0;
        }

        public bool UpdateMove(MoveDetails moveDetails)
        {
            lock (_lock)
            {
                if (moveDetails.PlayerType == PlayerTypes.Rebel)
                {
                    _rebelMove = moveDetails.Move;
                    return _soldierMove != 0;
                }

                _soldierMove = moveDetails.Move;
                return _rebelMove != 0;
            }
        }
    }
}
