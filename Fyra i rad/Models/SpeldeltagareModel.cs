namespace Fyra_i_rad.Models
{
    public class SpeldeltagareModel
    {
        public class SpelDeltagareModel
        {
            public int SpelID { get; set; }
            public int SpelarID { get; set; }
            public string SpelarRoll { get; set; } // t.ex. "Röd" eller "Blå"
        }

    }
}
