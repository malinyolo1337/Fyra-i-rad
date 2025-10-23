
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Configuration;

namespace Fyra_i_rad.Models
{
    public class GameMethods
    {
        private readonly string _connectionString;

        public GameMethods(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public GameMethods(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int SkapaNyttSpel()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Spel (Status, VinnarID) OUTPUT INSERTED.SpelID VALUES ('Pågår', NULL)", conn);
            return (int)cmd.ExecuteScalar();
        }

        public void LäggTillSpeldeltagare(int spelID, int spelarID, string spelarRoll)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "INSERT INTO Speldeltagare (SpelID, SpelarID, SpelarRoll) VALUES (@spelID, @spelarID, @spelarRoll)", conn);
            cmd.Parameters.AddWithValue("@spelID", spelID);
            cmd.Parameters.AddWithValue("@spelarID", spelarID);
            cmd.Parameters.AddWithValue("@spelarRoll", spelarRoll);
            cmd.ExecuteNonQuery();
        }

        public bool HarExaktTvåDeltagare(int spelID)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT COUNT(*) FROM Speldeltagare WHERE SpelID = @SpelID", conn);
            cmd.Parameters.AddWithValue("@SpelID", spelID);
            int antal = (int)cmd.ExecuteScalar();

            return antal == 2;
        }

        public List<int> HämtaDeltagare(int spelID)
        {
            var deltagare = new List<int>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT SpelarID FROM Speldeltagare WHERE SpelID = @SpelID ORDER BY SpelarRoll", conn);
            cmd.Parameters.AddWithValue("@SpelID", spelID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                deltagare.Add(reader.GetInt32(0));
            }

            return deltagare;
        }

        public void UppdateraSpelTillVinst(int spelID, int spelarID)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
           

            // Uppdatera spelstatus och vinnare
            using (var cmd = new SqlCommand(
                "UPDATE Spel SET Status = 'Avslutad', VinnarID = @SpelarID WHERE SpelID = @SpelID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                cmd.ExecuteNonQuery();
            }

            // Uppdatera spelarens vinstantal
            using (var cmd = new SqlCommand(
                "UPDATE Spelare SET AntalVinster = AntalVinster + 1 WHERE SpelarID = @SpelarID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                cmd.ExecuteNonQuery();
            }
        }



        public bool AvslutatSpel(int spelID)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT Status FROM Spel WHERE SpelID = @SpelID", conn);
            cmd.Parameters.AddWithValue("@SpelID", spelID);
            string status = cmd.ExecuteScalar()?.ToString();

            return status == "Avslutad";
        }

        public int? HämtaVinnare(int spelID)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT VinnarID FROM Spel WHERE SpelID = @SpelID", conn);
            cmd.Parameters.AddWithValue("@SpelID", spelID);
            var result = cmd.ExecuteScalar();

            return result != DBNull.Value ? (int?)Convert.ToInt32(result) : null;
        }
    }
}
