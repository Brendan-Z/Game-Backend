namespace A2.Dtos
{
    public class GameRecordOutDto
    {
        public string GameId { get; set; }
        public string State { get; set; }
        public string Player1 { get; set; }
        public string? Player2 { get; set; }
        public string? LastMovePlayer1 { get; set; }
        public string? LastMovePlayer2 { get; set; }
    }

}
