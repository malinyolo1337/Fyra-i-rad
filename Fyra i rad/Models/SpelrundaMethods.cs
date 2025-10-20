using Fyra_i_rad.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Fyra_i_rad.Models
{
    public static class SpelrundaMethods


    {
        // Bygger spelbrädet utifrån databasen
        public static int[,] ByggSpelbräde(string connectionString, int spelID)
        {
            int[,] bräde = new int[6, 7];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Kolumn, SpelarID FROM Spelrunda WHERE SpelID = @SpelID ORDER BY Dragsekvens";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SpelID", spelID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int kolumn = reader.GetInt32(0);
                            int spelarID = reader.GetInt32(1);

                            for (int rad = 5; rad >= 0; rad--)
                            {
                                if (bräde[rad, kolumn] == 0)
                                {
                                    bräde[rad, kolumn] = spelarID;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return bräde;
        }

        // Kontrollera om draget är giltigt
        public static bool ÄrDragGiltigt(string connectionString, int spelID, int kolumn, int spelarID)
        {
            int tur = 0;
            List<int> deltagare = new List<int>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Räkna antal drag
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
                {
                    cmd.Parameters.AddWithValue("@SpelID", spelID);
                    tur = (int)cmd.ExecuteScalar();
                }

                // Hämta deltagare
                using (SqlCommand cmd = new SqlCommand("SELECT SpelarID FROM Speldeltagare WHERE SpelID = @SpelID ORDER BY SpelarRoll", conn))
                {
                    cmd.Parameters.AddWithValue("@SpelID", spelID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            deltagare.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            int rättSpelare = deltagare[tur % deltagare.Count];
            return spelarID == rättSpelare && !KolumnFull(connectionString, spelID, kolumn);
        }

        private static bool KolumnFull(string connectionString, int spelID, int kolumn)
        {
            var bräde = ByggSpelbräde(connectionString, spelID);
            return bräde[0, kolumn] != 0;
        }

        // Spara ett nytt drag
        public static void SparaDrag(string connectionString, int spelID, int kolumn, int spelarID)
        {
            int dragsekvens = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
                {
                    cmd.Parameters.AddWithValue("@SpelID", spelID);
                    dragsekvens = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Spelrunda (SpelID, SpelarID, Kolumn, Dragsekvens) VALUES (@SpelID, @SpelarID, @Kolumn, @Dragsekvens)", conn))
                {
                    cmd.Parameters.AddWithValue("@SpelID", spelID);
                    cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                    cmd.Parameters.AddWithValue("@Kolumn", kolumn);
                    cmd.Parameters.AddWithValue("@Dragsekvens", dragsekvens);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Kontrollera vinst
        public static bool KontrolleraVinst(string connectionString, int spelID, int kolumn, int spelarID)
        {
            var bräde = ByggSpelbräde(connectionString, spelID);
            return HarFyraIRad(bräde, kolumn, spelarID);
        }

        private static bool HarFyraIRad(int[,] bräde, int kolumn, int spelarID)
        {
            int rader = bräde.GetLength(0);
            int rad = -1;

            for (int r = rader - 1; r >= 0; r--)
            {
                if (bräde[r, kolumn] == spelarID)
                {
                    rad = r;
                    break;
                }
            }

            if (rad == -1) return false;

            int Räkna(int dr, int dk)
            {
                int count = 1;
                for (int i = 1; i < 4; i++)
                {
                    int nr = rad + dr * i;
                    int nk = kolumn + dk * i;
                    if (nr < 0 || nr >= rader || nk < 0 || nk >= bräde.GetLength(1)) break;
                    if (bräde[nr, nk] == spelarID) count++;
                    else break;
                }
                return count;
            }

            return Räkna(0, 1) + Räkna(0, -1) >= 4 ||
                   Räkna(1, 0) + Räkna(-1, 0) >= 4 ||
                   Räkna(1, 1) + Räkna(-1, -1) >= 4 ||
                   Räkna(1, -1) + Räkna(-1, 1) >= 4;
        }

        // Uppdaterar spelstatus vid vinst
        public static void UppdateraSpelTillVinst(string connectionString, int spelID, int spelarID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE Spel SET Status = 'Slutfört', VinnarID = @SpelarID WHERE SpelID = @SpelID", conn))
                {
                    cmd.Parameters.AddWithValue("@SpelID", spelID);
                    cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand("UPDATE Spelare SET AntalVinster = AntalVinster + 1 WHERE SpelarID = @SpelareID", conn))
                {
                    cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
