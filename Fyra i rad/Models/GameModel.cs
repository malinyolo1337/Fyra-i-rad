namespace Fyra_i_rad.Models
{
   
    public class GameModel
    {
        public int SpelID { get; set; }
        public string Status { get; set; } //pågår eller avslutat

        
        public int? VinnarID { get; set; }
    }
}
