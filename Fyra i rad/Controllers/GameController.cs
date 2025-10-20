using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Fyra_i_rad.Models;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fyra_i_rad.Controllers
{

    public class GameController : Controller
    {
        private readonly string _connectionString;
        public GameController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private int CurrentUserID => HttpContext.Session.GetInt32("UserID") ?? throw new UnauthorizedAccessException();

        //READ - Lista aktiva spel
        public async Task <IActionResult> Index()
        {
            var list = new List<GameModel>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT SpelID, Status, Skapad, Avslutad, VinnarID
                FROM Spel
                WHERE Status = 'Active'
                  AND EXISTS (SELECT 1 FROM Spelare WHERE Spelare.SpelID = Spel.SpelID AND Spelare.AnvandareID = @AnvandareID)
                ORDER BY Skapad DESC";

            await using var command = new SqlCommand(sql, connection); 
            command.Parameters.AddWithValue("@AnvandareID", CurrentUserID);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new GameModel
                {
                    SpelID = reader.GetInt32(0),
                    Status = Enum.Parse<GameStatus>(reader.GetString(1)),
                    Skapad = reader.GetDateTime(2),
                    Avslutad = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    VinnarID = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                });
            }

            return View(list);
        }

        //CREATE - Starta nytt spel
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var items = new List<SelectListItem>();

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"SELECT SpelarID, Username FROM Spelare WHERE SpelarID <> @me ORDER BY Username";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@me", CurrentUserID);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new SelectListItem
                {
                    Text = reader.GetString(1),
                    Value = reader.GetInt32(0).ToString()
                });
            }


            ViewBag.Spelare = items;
                       return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int opponentID)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var dbTrans = await connection.BeginTransactionAsync(); // Use SqlTransaction

            try
            {
                var insertGameSql = @"
                    INSERT INTO Spel(Status, VinnarID, Skapad) VALUES ('Active', NULL, SYSUTCDATETIME());
                    SELECT SCOPE_IDENTITY();";

                var command1 = new SqlCommand(insertGameSql, connection, (SqlTransaction)dbTrans); // Cast to SqlTransaction
                var newID = Convert.ToInt32(await command1.ExecuteScalarAsync());

                var insertPlayersSql = @"
                    INSERT INTO Speldeltagare(SpelID, SpelarID, SpelarRoll)
                    VALUES (@id, @me, 'P1'), (@id, @opp, 'P2');";

                var command2 = new SqlCommand(insertPlayersSql, connection, (SqlTransaction)dbTrans); // Cast to SqlTransaction
                command2.Parameters.AddWithValue("@id", newID);
                command2.Parameters.AddWithValue("@me", CurrentUserID);
                command2.Parameters.AddWithValue("@opp", opponentID);
                await command2.ExecuteNonQueryAsync();

                await dbTrans.CommitAsync();
                return RedirectToAction(nameof(Details), new { id = newID });
            }
            catch
            {
                await dbTrans.RollbackAsync();
                TempData["ErrorMessage"] = "Något gick fel, försök igen!";
                return RedirectToAction(nameof(Index));
            }
        }
        //Details - enkel sida med avsluta knapp

        public async Task<IActionResult> Details(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Kontrollera att användaren är med i spelet
            var okCommand = new SqlCommand(
                "SELECT COUNT(1) FROM Speldeltagare WHERE SpelID=@id AND SpelarID=@uid", connection);
            okCommand.Parameters.AddWithValue("@id", id);
            okCommand.Parameters.AddWithValue("@uid", CurrentUserID);
            var allowed = (int)await okCommand.ExecuteScalarAsync() > 0;
            if (!allowed) return Forbid();

            var infoCommand = new SqlCommand(
                @"SELECT SpelID, Status, Skapad, Avslutad, VinnarID
                  FROM Spel WHERE SpelID=@id", connection);
            infoCommand.Parameters.AddWithValue("@id", id);

            await using var reader = await infoCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var model = new GameModel
                {
                    SpelID = reader.GetInt32(0),
                    Status = Enum.Parse<GameStatus>(reader.GetString(1)),
                    Skapad = reader.GetDateTime(2),
                    Avslutad = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    VinnarID = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                };
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        //End - markera spelet som avslutat
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> End(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var okCommand = new SqlCommand(
                "SELECT COUNT(1) FROM Speldeltagare WHERE SpelID=@id AND SpelarID=@uid", connection);
            okCommand.Parameters.AddWithValue("@id", id);
            okCommand.Parameters.AddWithValue("@uid", CurrentUserID);
            var allowed = (int)await okCommand.ExecuteScalarAsync() > 0;
            if (!allowed) return Forbid();

            var command = new SqlCommand(
                "UPDATE Spel SET Status='Finished', Avslutad=SYSUTDATETIME() WHERE SpelD=@id", connection);
            command.Parameters.AddWithValue("@id", id); 
            await command.ExecuteNonQueryAsync();

            TempData["Msg"] = $"Spelet {id} avslutat!";
            return RedirectToAction(nameof(Index));
        }

        private static GameStatus ParseStatus(string status) =>
            status.ToLower() switch
            {
                "pending"  => GameStatus.Pending,
                "active"   => GameStatus.Active,
                "finished" => GameStatus.Finished,
                 _         => GameStatus.Active  
            };
    }
}


