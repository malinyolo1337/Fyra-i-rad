
using Microsoft.Data.SqlClient;
using System.Data;
using Fyra_i_rad.Models;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc;
using Fyra_i_rad.Models;

namespace Fyra_i_rad.Controllers
{
    public class GameController : Controller
    {
        private readonly GameMethods _gameMethods;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public GameController(IConfiguration configuration)
        {
            _gameMethods = new GameMethods(configuration);
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult StartaSpel()
        {
            int? spelarID1 = HttpContext.Session.GetInt32("SpelarID1");
            int? spelarID2 = HttpContext.Session.GetInt32("SpelarID2");

            if (spelarID1 == null || spelarID2 == null)
            {
                TempData["Felmeddelande"] = "Du måste logga in två spelare först.";
                return RedirectToAction("Login", "Spelar");
            }

            int spelID = _gameMethods.SkapaNyttSpel();
            _gameMethods.LäggTillSpeldeltagare(spelID, spelarID1.Value, "Röd");
            _gameMethods.LäggTillSpeldeltagare(spelID, spelarID2.Value, "Blå");

            if (!_gameMethods.HarExaktTvåDeltagare(spelID))
            {
                TempData["Felmeddelande"] = "Spelet saknar två deltagare.";
                return RedirectToAction("Login", "Spelar");
            }

            return RedirectToAction("VisaBräde", "Spelrunda", new { spelID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult End(int id)
        {
            // Markera spelet som avslutat (utan vinnare)
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using (var cmd = new SqlCommand(
                "UPDATE Spel SET Status = 'Avslutad' WHERE SpelID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            TempData["Msg"] = "Spelet har avslutats.";
            // Tillbaka till listan över spel eller där du vill landa
            //return RedirectToAction("AktivaSpel", "Game");
            return RedirectToAction("VisaBräde", "SpelRunda", new {spelID = id});
        }
    }
}




