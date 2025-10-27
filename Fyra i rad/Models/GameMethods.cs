
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Configuration;
using static Fyra_i_rad.Models.SpeldeltagareModel;

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

        
        public List<SpelDeltagareModel> HämtaDeltagare(int spelID)
        {
            var lista = new List<SpelDeltagareModel>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT SpelID, SpelarID, SpelarRoll FROM SpelDeltagare WHERE SpelID = @spelid", conn);
            cmd.Parameters.AddWithValue("@spelid", spelID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new SpelDeltagareModel
                {
                    SpelID = reader.GetInt32(0),
                    SpelarID = reader.GetInt32(1),
                    SpelarRoll = reader.GetString(2)
                });
            }

            return lista;
        }
        public string HämtaUsername(int spelarID)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT Username FROM Spelare WHERE SpelarID = @namn", conn);
            cmd.Parameters.AddWithValue("@namn", spelarID);

            var result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : $"Spelare {spelarID}";
        }


        public void UppdateraVinnareOchFörlorare(int spelID, int spelarID)
   
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // 1. Uppdatera spelet
            using (var cmd = new SqlCommand(
                "UPDATE Spel SET Status = 'Avslutad', VinnarID = @SpelarID WHERE SpelID = @SpelID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                cmd.ExecuteNonQuery();
            }

            // 2. Uppdatera VINNARE med +1 vinst
            using (var cmd = new SqlCommand(
                "UPDATE Spelare SET AntalVinster = AntalVinster + 1 WHERE SpelarID = @SpelarID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                cmd.ExecuteNonQuery();
            }

            // 3. Hämta FÖRLORARE (den andra spelaren)
            int förlorareID = 0;
            using (var cmd = new SqlCommand(
                "SELECT SpelarID FROM SpelDeltagare WHERE SpelID = @SpelID AND SpelarID != @VinnareID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                cmd.Parameters.AddWithValue("@VinnareID", spelarID);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    förlorareID = reader.GetInt32(0);
                }
            }

            // 4. Uppdatera FÖRLORARE med +1 förlust
            if (förlorareID != 0)
            {
                using var cmd = new SqlCommand(
                    "UPDATE Spelare SET AntalFörluster = AntalFörluster + 1 WHERE SpelarID = @FörlorareID", conn);
                cmd.Parameters.AddWithValue("@FörlorareID", förlorareID);
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
