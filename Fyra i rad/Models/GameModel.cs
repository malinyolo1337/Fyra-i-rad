namespace Fyra_i_rad.Models
{
    public enum GameStatus { Pending, Active, Finished }
    public class GameModel
    {
        public int SpelID { get; set; }
        public GameStatus Status { get; set; }
        public DateTime Skapad { get; set; }
        public DateTime? Avslutad { get; set; }
        public int? VinnarID { get; set; }
    }
}
