namespace Fyra_i_rad.Models
{
    public class AdminViewModel
    {
        public int AntalSpelare { get; set; }
        public int AntalSpel { get; set; }
        public int AntalVinster { get; set; }
        public int AntalFörluster { get; set; }

        // För framtida användning:
        public List<Spelare>? Spelare { get; set; }
        public List<Spel>? Spel { get; set; }
    }
}

