using System.ComponentModel.DataAnnotations;

namespace FyraIRad.Models
{
    public class SpelarModel
    {
        public int SpelarID { get; set; }

        [Required(ErrorMessage = "Ange användarnamn")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Ange lösenord")]
        public string Password { get; set; }

        public string Markör { get; set; } = "röd";
        public int AntalVinster { get; set; } = 0;
        public int AntalFörluster { get; set; } = 0;
    }
}
