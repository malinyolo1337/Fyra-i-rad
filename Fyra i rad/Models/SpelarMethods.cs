using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FyraIRad.Models
{
    public class SpelarMethods
    {
        private readonly string _connectionString;

        public SpelarMethods(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int InsertSpelar(SpelarModel spelar, out string errormsg)
        {
            errormsg = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = "INSERT INTO Spelare (Username, Password, AntalVinster, AntalFörluster) VALUES (@u, @p, @v, @f)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@u", spelar.Username);
                    cmd.Parameters.AddWithValue("@p", spelar.Password);
                    cmd.Parameters.AddWithValue("@v", spelar.AntalVinster);
                    cmd.Parameters.AddWithValue("@f", spelar.AntalFörluster);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return 0;
            }
        }

        public SpelarModel? Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Spelare WHERE Username=@u AND Password=@p";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SpelarModel
                        {
                            SpelarID = Convert.ToInt32(reader["SpelarID"]),
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            AntalVinster = Convert.ToInt32(reader["AntalVinster"]),
                            AntalFörluster = Convert.ToInt32(reader["AntalFörluster"])
                        };
                    }
                }
            }

            return null;
        }

        public SpelarModel GetSpelarById(int spelarID)
        {
            SpelarModel spelar = new SpelarModel();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM Spelare WHERE SpelarID = @SpelarID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@SpelarID", spelarID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    spelar.SpelarID = Convert.ToInt32(reader["SpelarID"]);
                    spelar.Username = reader["Username"].ToString();
                    spelar.Password = reader["Password"].ToString();
                    spelar.AntalVinster = Convert.ToInt32(reader["AntalVinster"]);
                    spelar.AntalFörluster = Convert.ToInt32(reader["AntalFörluster"]);
                }
            }

            return spelar;
        }

        public List<SpelarModel> GetSpelarModelList(out string errormsg)
        {
            errormsg = "";
            List<SpelarModel> spelarList = new List<SpelarModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = "SELECT * FROM Spelare";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    conn.Open();
                    adapter.Fill(ds, "Spelare");

                    foreach (DataRow row in ds.Tables["Spelare"].Rows)
                    {
                        spelarList.Add(new SpelarModel
                        {
                            SpelarID = Convert.ToInt32(row["SpelarID"]),
                            Username = row["Username"].ToString(),
                            Password = row["Password"].ToString(),
                            AntalVinster = Convert.ToInt32(row["AntalVinster"]),
                            AntalFörluster = Convert.ToInt32(row["AntalFörluster"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }

            return spelarList;
        }

        public int UpdateSpelar(SpelarModel spelar, out string errormsg)
        {
            errormsg = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = "UPDATE Spelare SET Username=@Username, Password=@Password, Markör=@Markör, AntalVinster=@Vinster, AntalFörluster=@Förluster WHERE SpelarID=@SpelarID";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Username", spelar.Username);
                    cmd.Parameters.AddWithValue("@Password", spelar.Password);
                 
                    cmd.Parameters.AddWithValue("@Vinster", spelar.AntalVinster);
                    cmd.Parameters.AddWithValue("@Förluster", spelar.AntalFörluster);
                    cmd.Parameters.AddWithValue("@SpelarID", spelar.SpelarID);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return 0;
            }
        }

        public int DeleteSpelar(int spelarID, out string errormsg)
        {
            errormsg = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string sql = "DELETE FROM Spelare WHERE SpelarID = @SpelarID";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@SpelarID", spelarID);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return 0;
            }
        }
    }
}

