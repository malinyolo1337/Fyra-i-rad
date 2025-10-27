//using Fyra_i_rad.Models;
//using System.Data;
//using Microsoft.Data.SqlClient;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;

//namespace Fyra_i_rad.Models
//{
//    public static class SpelrundaMethods


//    {
// Bygger spelbrädet utifrån databasen
//public static int[,] ByggSpelbräde(string connectionString, int spelID)
//{
//    int[,] bräde = new int[6, 7];

//    using (SqlConnection conn = new SqlConnection(connectionString))
//    {
//        conn.Open();
//        string query = "SELECT Kolumn, SpelarID FROM Spelrunda WHERE SpelID = @SpelID ORDER BY Dragsekvens";

//        using (SqlCommand cmd = new SqlCommand(query, conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);

//            using (SqlDataReader reader = cmd.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    int kolumn = reader.GetInt32(0);
//                    int spelarID = reader.GetInt32(1);

//                    for (int rad = 5; rad >= 0; rad--)
//                    {
//                        if (bräde[rad, kolumn] == 0)
//                        {
//                            bräde[rad, kolumn] = spelarID;
//                            break;
//                        }
//                    }
//                }
//            }
//        }
//    }

//    return bräde;
//}
//public static int[,] ByggSpelbräde(string connectionString, int spelID)
//{
//    int rader = 6, kolumner = 7;
//    int[,] bräde = new int[rader, kolumner];

//    using var conn = new SqlConnection(connectionString);
//    conn.Open();

//    var cmd = new SqlCommand(
//        "SELECT Kolumn, SpelarID FROM Spelrunda WHERE SpelID = @SpelID ORDER BY DragSekvens", conn);
//    cmd.Parameters.AddWithValue("@SpelID", spelID);

//    using var reader = cmd.ExecuteReader();
//    while (reader.Read())
//    {
//int kol = reader.GetInt32(0) - 1;

//        int spelare = reader.GetInt32(1);

//        if (kol is < 0 or >= 7 || spelare <= 0) continue;

//        for (int rad = rader - 1; rad >= 0; rad--)
//        {
//            if (bräde[rad, kol] == 0)
//            {
//                bräde[rad, kol] = spelare;
//                break;
//            }
//        }
//    }

//    return bräde;
//}


//public static int VemsTur(string connectionString, int spelID)
//{
//    int dragAntal = 0;
//    List<int> speldeltagare = new List<int>();

//    using (SqlConnection conn = new SqlConnection(connectionString))
//    {
//        conn.Open();

//        // Räkna antal drag
//        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            dragAntal = (int)cmd.ExecuteScalar();
//        }

//        // Hämta deltagare i turordning
//        using (SqlCommand cmd = new SqlCommand("SELECT SpelarID FROM Speldeltagare WHERE SpelID = @SpelID ORDER BY SpelarRoll", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            using (SqlDataReader reader = cmd.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    speldeltagare.Add(reader.GetInt32(0));
//                }
//            }
//        }
//    }
//    if (speldeltagare.Count == 0)
//        throw new InvalidOperationException("Inga deltagare kopplade till spelet.");

//    // Returnera spelarID för den som har turen
//    return speldeltagare[dragAntal % speldeltagare.Count];

//}
//public static bool GiltigtDrag(string connectionString, int spelID, int kolumn, int spelarID)
//{
//    // Skydd mot ogiltigt kolumnindex
//    if (kolumn < 0 || kolumn >= 7)
//        return false;

//    List<int> speldeltagare = new List<int>();
//    int dragAntal = 0;

//    using (SqlConnection conn = new SqlConnection(connectionString))
//    {
//        conn.Open();

//        // Räkna antal drag
//        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            dragAntal = (int)cmd.ExecuteScalar();
//        }

//        // Hämta deltagare i turordning
//        using (var cmd = new SqlCommand("SELECT SpelarID FROM Speldeltagare WHERE SpelID = @SpelID ORDER BY SpelarRoll", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            using (var reader = cmd.ExecuteReader())
//            {
//                while (reader.Read())
//                    speldeltagare.Add(reader.GetInt32(0));
//            }
//        }
//    }

//    // Kontrollera att deltagare finns
//    if (speldeltagare.Count == 0)
//        return false;

//    // Vem har turen?
//    int turSpelare = speldeltagare[dragAntal % speldeltagare.Count];

//    // Kontrollera om kolumnen är full
//    if (KolumnFull(connectionString, spelID, kolumn))
//        return false;

//    // Är det spelarens tur?
//    return spelarID == turSpelare;
//}


//// Kontrollera om draget är giltigt
//public static bool GiltigtDrag(string connectionString, int spelID, int kolumn, int spelarID)
//{
//    int tur = 0;
//    List<int> speldeltagare = new List<int>();

//    using (SqlConnection conn = new SqlConnection(connectionString))
//    {
//        conn.Open();

//        // Räkna antal drag
//        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            tur = (int)cmd.ExecuteScalar();
//        }

//        // Hämta speldeltagare
//        using (SqlCommand cmd = new SqlCommand("SELECT SpelarID FROM Speldeltagare WHERE SpelID = @SpelID ORDER BY SpelarRoll", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            using (SqlDataReader reader = cmd.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    speldeltagare.Add(reader.GetInt32(0));
//                }
//            }
//        }
//    }

//private static bool KolumnFull(string connectionString, int spelID, int kolumn)
//{
//    var bräde = ByggSpelbräde(connectionString, spelID);

//    // Skydd mot ogiltiga kolumnindex
//    if (kolumn < 0 || kolumn >= bräde.GetLength(1))
//        return true;

//    // Om översta raden i kolumnen är upptagen, är kolumnen full
//    return bräde[0, kolumn] != 0;
//}


//private static bool KolumnFull(string connectionString, int spelID, int kolumn)
//{
//    var bräde = ByggSpelbräde(connectionString, spelID);
//    return bräde[0, kolumn] != 0;
//}

//// Spara ett nytt drag
//public static void SparaDrag(string connectionString, int spelID, int kolumn, int spelarID)
//{
//    if (kolumn < 0 || kolumn >= 7) throw new ArgumentOutOfRangeException(nameof(kolumn));
//    int dragsekvens;
//    using var conn = new SqlConnection(connectionString);
//    conn.Open();

//    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
//    {
//        cmd.Parameters.AddWithValue("@SpelID", spelID);
//        dragsekvens = (int)cmd.ExecuteScalar();
//    }

//    // Om databasen använder 1-baserade kolumnnummer, skifta:
//    int dbKolumn = kolumn + 1; // eller remove +1 om DB förväntar sig 0-baserat

//    using (var cmd = new SqlCommand(
//        "INSERT INTO Spelrunda (SpelID, SpelarID, Kolumn, DragSekvens) VALUES (@SpelID, @SpelarID, @Kolumn, @DragSekvens)", conn))
//    {
//        cmd.Parameters.AddWithValue("@SpelID", spelID);
//        cmd.Parameters.AddWithValue("@SpelarID", spelarID);
//        cmd.Parameters.AddWithValue("@Kolumn", dbKolumn);
//        cmd.Parameters.AddWithValue("@DragSekvens", dragsekvens);
//        cmd.ExecuteNonQuery();
//    }
//}

// Kontrollera vinst
//public static bool KontrolleraVinst(string connectionString, int spelID, int kolumn, int spelarID)
//{
//    var bräde = ByggSpelbräde(connectionString, spelID);
//    return HarFyraIRad(bräde, kolumn, spelarID);
//}
//private static bool HarFyraIRad(int[,] bräde, int kolumn, int spelarID)
//{
//    int rader = bräde.GetLength(0);
//    int rad = -1;

//    // Hitta raden där senaste brickan hamnade
//    for (int r = rader - 1; r >= 0; r--)
//    {
//        if (bräde[r, kolumn] == spelarID)
//        {
//            rad = r;
//            break;
//        }
//    }

//    if (rad == -1) return false; // Ingen bricka hittad

//    // Hjälpmetod för att räkna i en riktning
//    int Räkna(int dr, int dk)
//    {
//        int count = 0;
//        for (int i = 1; i < 4; i++)
//        {
//            int nr = rad + dr * i;
//            int nk = kolumn + dk * i;
//            if (nr < 0 || nr >= rader || nk < 0 || nk >= bräde.GetLength(1)) break;
//            if (bräde[nr, nk] == spelarID) count++;
//            else break;
//        }
//        return count;
//    }

//    // Kolla alla riktningar: horisontellt, vertikalt, diagonalt ↘ och ↙
//    return
//        Räkna(0, 1) + Räkna(0, -1) + 1 >= 4 || // horisontellt
//        Räkna(1, 0) + Räkna(-1, 0) + 1 >= 4 || // vertikalt
//        Räkna(1, 1) + Räkna(-1, -1) + 1 >= 4 || // diagonal ↘
//        Räkna(1, -1) + Räkna(-1, 1) + 1 >= 4;   // diagonal ↙
//}

//private static bool HarFyraIRad(int[,] bräde, int kolumn, int spelarID)
//{
//    int rader = bräde.GetLength(0);
//    int rad = -1;

//    for (int r = rader - 1; r >= 0; r--)
//    {
//        if (bräde[r, kolumn] == spelarID)
//        {
//            rad = r;
//            break;
//        }
//    }
//    if (kolumn < 0 || kolumn >= bräde.GetLength(1)) return false;


//    if (rad == -1) return false;

//int Räkna(int dr, int dk)
//{
//    int count = 1;
//    for (int i = 1; i < 4; i++)
//    {
//        int nr = rad + dr * i;
//        int nk = kolumn + dk * i;
//        if (nr < 0 || nr >= rader || nk < 0 || nk >= bräde.GetLength(1)) break;
//        if (bräde[nr, nk] == spelarID) count++;
//        else break;
//    }
//    return count;
//}

//return Räkna(0, 1) + Räkna(0, -1) >= 4 ||
//       Räkna(1, 0) + Räkna(-1, 0) >= 4 ||
//       Räkna(1, 1) + Räkna(-1, -1) >= 4 ||
//       Räkna(1, -1) + Räkna(-1, 1) >= 4;
//    int Räkna(int dr, int dk)
//    {
//        int count = 0;
//        for (int i = 1; i < 4; i++)
//        {
//            int nr = rad + dr * i;
//            int nk = kolumn + dk * i;
//            if (nr < 0 || nr >= rader || nk < 0 || nk >= bräde.GetLength(1)) break;
//            if (bräde[nr, nk] == spelarID) count++;
//            else break;
//        }
//        return count;
//    }

//    return Räkna(0, 1) + Räkna(0, -1) + 1 >= 4 || ...

//}

// Uppdaterar spelstatus vid vinst
//public static void UppdateraSpelTillVinst(string connectionString, int spelID, int spelarID)
//{
//    using (SqlConnection conn = new SqlConnection(connectionString))
//    {
//        conn.Open();

//        using (SqlCommand cmd = new SqlCommand("UPDATE Spel SET Status = 'Avslutad', VinnarID = @SpelarID WHERE SpelID = @SpelID", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            cmd.Parameters.AddWithValue("@SpelarID", spelarID);
//            cmd.ExecuteNonQuery();
//        }

//        using (SqlCommand cmd = new SqlCommand("UPDATE Spelare SET AntalVinster = AntalVinster + 1 WHERE SpelarID = @SpelarID", conn))
//        {
//            cmd.Parameters.AddWithValue("@SpelarID", spelarID);
//            cmd.ExecuteNonQuery();
//        }
//    }
//}
//public static int? HämtaVinnare(string connectionString, int spelID)
//{
//    using var conn = new SqlConnection(connectionString);
//    conn.Open();

//    var cmd = new SqlCommand("SELECT VinnarID FROM Spel WHERE SpelID = @SpelID", conn);
//    cmd.Parameters.AddWithValue("@SpelID", spelID);
//    var result = cmd.ExecuteScalar();
//    return result != DBNull.Value ? (int?)Convert.ToInt32(result) : null;
//}

//public static bool AvslutatSpel(string connectionString, int spelID)
//{
//    using var conn = new SqlConnection(connectionString);
//    conn.Open();

//    var cmd = new SqlCommand("SELECT Status FROM Spel WHERE SpelID = @SpelID", conn);
//    cmd.Parameters.AddWithValue("@SpelID", spelID);
//    string status = cmd.ExecuteScalar()?.ToString();

//    return status == "Avslutad" ;
//}
//        public static void UppdateraSpelTillVinst(string connectionString, int spelID, int spelarID)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                using (SqlCommand cmd = new SqlCommand(
//                    "UPDATE Spel SET Status = 'Avslutad', VinnarID = @SpelarID WHERE SpelID = @SpelID", conn))
//                {
//                    cmd.Parameters.AddWithValue("@SpelID", spelID);
//                    cmd.Parameters.AddWithValue("@SpelarID", spelarID);
//                    cmd.ExecuteNonQuery();
//                }

//                using (SqlCommand cmd = new SqlCommand(
//                    "UPDATE Spelare SET AntalVinster = AntalVinster + 1 WHERE SpelarID = @SpelarID", conn))
//                {
//                    cmd.Parameters.AddWithValue("@SpelarID", spelarID);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }

//        public static bool AvslutatSpel(string connectionString, int spelID)
//        {
//            using var conn = new SqlConnection(connectionString);
//            conn.Open();

//            var cmd = new SqlCommand("SELECT Status FROM Spel WHERE SpelID = @SpelID", conn);
//            cmd.Parameters.AddWithValue("@SpelID", spelID);
//            string status = cmd.ExecuteScalar()?.ToString();

//            return status == "Avslutad";
//        }

//    }
//}
using System.Data;
using Microsoft.Data.SqlClient;

namespace Fyra_i_rad.Models
{
    //Spelregler
    public static class SpelrundaMethods
    {
        // Bygger spelbrädet från databasen
        public static int[,] ByggSpelbräde(string connectionString, int spelID)
        {
            int rader = 6, kolumner = 7;
            int[,] bräde = new int[rader, kolumner];

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT Kolumn, SpelarID FROM Spelrunda WHERE SpelID = @SpelID ORDER BY DragSekvens", conn);
            cmd.Parameters.AddWithValue("@SpelID", spelID);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int kol = reader.GetInt32(0) - 1; // justera från 1–7 till 0–6
                int spelare = reader.GetInt32(1);

                if (kol < 0 || kol >= kolumner || spelare <= 0) continue;

                for (int rad = rader - 1; rad >= 0; rad--)
                {
                    if (bräde[rad, kol] == 0)
                    {
                        bräde[rad, kol] = spelare;
                        break;
                    }
                }
            }

            return bräde;
        }

        // Returnerar spelarID för den som har turen
        public static int VemsTur(string connectionString, int spelID)
        {
            int dragAntal = 0;
            List<int> deltagare = new();

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                dragAntal = (int)cmd.ExecuteScalar();
            }

            using (var cmd = new SqlCommand("SELECT SpelarID FROM Speldeltagare WHERE SpelID = @SpelID ORDER BY SpelarRoll", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    deltagare.Add(reader.GetInt32(0));
            }

            if (deltagare.Count == 0)
                throw new InvalidOperationException("Inga deltagare kopplade till spelet.");

            return deltagare[dragAntal % deltagare.Count];
        }

        // Kontrollera om draget är giltigt
        public static bool GiltigtDrag(string connectionString, int spelID, int kolumn, int spelarID)
        {
            if (kolumn < 0 || kolumn >= 7)
                return false;

            var bräde = ByggSpelbräde(connectionString, spelID);
            if (bräde[0, kolumn] != 0)
                return false;

            return spelarID == VemsTur(connectionString, spelID);
        }

        // Spara ett nytt drag
        public static void SparaDrag(string connectionString, int spelID, int kolumn, int spelarID)
        {
            if (kolumn < 0 || kolumn >= 7)
                throw new ArgumentOutOfRangeException(nameof(kolumn));

            int dragsekvens;
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Spelrunda WHERE SpelID = @SpelID", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                dragsekvens = (int)cmd.ExecuteScalar();
            }

            int dbKolumn = kolumn + 1;

            using (var cmd = new SqlCommand(
                "INSERT INTO Spelrunda (SpelID, SpelarID, Kolumn, DragSekvens) VALUES (@SpelID, @SpelarID, @Kolumn, @DragSekvens)", conn))
            {
                cmd.Parameters.AddWithValue("@SpelID", spelID);
                cmd.Parameters.AddWithValue("@SpelarID", spelarID);
                cmd.Parameters.AddWithValue("@Kolumn", dbKolumn);
                cmd.Parameters.AddWithValue("@DragSekvens", dragsekvens);
                cmd.ExecuteNonQuery();
            }
        }

        // Kontrollera om senaste draget gav vinst
        public static bool KontrolleraVinst(string connectionString, int spelID, int kolumn, int spelarID)
        {
            var bräde = ByggSpelbräde(connectionString, spelID);
            return HarFyraIRad(bräde, kolumn, spelarID);
        }

        // Vinstlogik: fyra i rad
        private static bool HarFyraIRad(int[,] bräde, int kolumn, int spelarID)
        {
            int rader = bräde.GetLength(0);
            if (kolumn < 0 || kolumn >= bräde.GetLength(1)) return false;

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
                int count = 0;
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

            return Räkna(0, 1) + Räkna(0, -1) + 1 >= 4 || // horisontellt
                   Räkna(1, 0) + Räkna(-1, 0) + 1 >= 4 || // vertikalt
                   Räkna(1, 1) + Räkna(-1, -1) + 1 >= 4 || // diagonal ↘
                   Räkna(1, -1) + Räkna(-1, 1) + 1 >= 4;   // diagonal ↙
        }

        //// Uppdatera spelstatus vid vinst
        //public static void UppdateraSpelTillVinst(string connectionString, int spelID, int spelarID)
        //{
        //    using var conn = new SqlConnection(connectionString);
        //    conn.Open();

        //    using (var cmd = new SqlCommand("UPDATE Spel SET Status = 'Avslutad', VinnarID = @SpelarID WHERE SpelID = @SpelID", conn))
        //    {
        //        cmd.Parameters.AddWithValue("@SpelID", spelID);
        //        cmd.Parameters.AddWithValue("@SpelarID", spelarID);
        //        cmd.ExecuteNonQuery();
        //    }

        //    using (var cmd = new SqlCommand("UPDATE Spelare SET AntalVinster = AntalVinster + 1 WHERE SpelarID = @SpelarID", conn))
        //    {
        //        cmd.Parameters.AddWithValue("@SpelarID", spelarID);
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        // Hämta vinnare
        //public static int? HämtaVinnare(string connectionString, int spelID)
        //{
        //    using var conn = new SqlConnection(connectionString);
        //    conn.Open();

        //    var cmd = new SqlCommand("SELECT VinnarID FROM Spel WHERE SpelID = @SpelID", conn);
        //    cmd.Parameters.AddWithValue("@SpelID", spelID);
        //    var result = cmd.ExecuteScalar();
        //    return result != DBNull.Value ? (int?)Convert.ToInt32(result) : null;
        //}

        //// Kontrollera om spelet är avslutat
        //public static bool AvslutatSpel(string connectionString, int spelID)
        //{
        //    using var conn = new SqlConnection(connectionString);
        //    conn.Open();

        //    var cmd = new SqlCommand("SELECT Status FROM Spel WHERE SpelID = @SpelID", conn);
        //    cmd.Parameters.AddWithValue("@SpelID", spelID);
        //    string status = cmd.ExecuteScalar()?.ToString();

        //    return status == "Avslutad";
        //}
    }
}
