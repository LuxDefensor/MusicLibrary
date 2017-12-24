using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settings;
using System.Data.SqlClient;
using System.Data;

namespace MusicLibrary
{
    static class DataManager
    {
        private static string cs;

        static DataManager()
        {
            SettingsManager settings = new SettingsManager();
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = settings["Server"];
            csb.InitialCatalog = settings["Database"];
            csb.UserID = settings["User"];
            csb.Password = settings["Password"];
            cs = csb.ConnectionString;
        }

        public static void ExecuteQuery(string sql, int timeout = 30)
        {
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandTimeout = timeout;
                cmd.CommandText = sql;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(sql, ex);
                }
            }
        }

        public static int GetIntValue(string sql, int timeout)
        {
            object returnValue;
            int result;
            using (SqlConnection cn = new SqlConnection(cs))
                returnValue = GetSingleObjectValue(sql, timeout);
            if (!int.TryParse(returnValue.ToString(), out result))
                throw new Exception("Returned value cannot be parsed as int: " + returnValue.ToString() + Environment.NewLine + sql);
            else
                return result;
        }

        public static double GetDoubleValue(string sql, int timeout)
        {
            object returnValue;
            double result;
            using (SqlConnection cn = new SqlConnection(cs))
                returnValue = GetSingleObjectValue(sql, timeout);
            if (!double.TryParse(returnValue.ToString(), out result))
                throw new Exception("Returned value cannot be parsed as double: " + returnValue.ToString() + Environment.NewLine + sql);
            else
                return result;
        }

        public static DateTime GetDateValue(string sql, int timeout)
        {
            object returnValue;
            DateTime result;
            using (SqlConnection cn = new SqlConnection(cs))
                returnValue = GetSingleObjectValue(sql, timeout);
            if (!DateTime.TryParse(returnValue.ToString(), out result))
                throw new Exception("Returned value cannot be parsed as date: " + returnValue.ToString() + Environment.NewLine + sql);
            else
                return result;
        }

        public static string GetStringValue(string sql, int timeout)
        {
            return GetSingleObjectValue(sql, timeout).ToString();
        }


        private static object GetSingleObjectValue(string sql, int timeout)
        {
            object returnValue;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandTimeout = timeout;
                cmd.CommandText = sql;
                try
                {
                    returnValue = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception(sql, ex);
                }
                if (returnValue == null || Convert.IsDBNull(returnValue))
                    throw new Exception("Query returned NULL" + Environment.NewLine + sql);
                else
                    return returnValue;
            }
        }

        public static DataTable GetData(string sql, int timeout)
        {
            DataTable result = new DataTable();
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandTimeout = timeout;
                cmd.CommandText = sql;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                try
                {
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    throw new Exception(sql, ex);
                }
            }
            return result;
        }

        #region Stored procedures

        #region Artist

        public static int AddArtist(string name, int country, int startingYear, int id)
        {
            int result = -100;
            SqlParameter par;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_ADD_Artist";
                cmd.Parameters.AddWithValue("@ArtistName", name);
                cmd.Parameters.AddWithValue("@Country", country);
                cmd.Parameters.AddWithValue("@StartingYear", startingYear);
                par = cmd.Parameters.AddWithValue("@ArtistID", result);
                par.Direction = ParameterDirection.InputOutput;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SP_ADD_Artist(" + name + "," + country + "," + startingYear + ")", ex);
                }
            }
            if (result > 0)
                return result;
            else
                throw new Exception("SP_ADD_Artist: something went wrong");
        }

        public static void ChangeArtist(int id, string newName = "",
            int newCountry = -1, int startingYear = -1)
        {
            SqlParameter par;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_Change_Artist";
                cmd.Parameters.AddWithValue("@ID", id);
                par = cmd.Parameters.Add("@NewName", SqlDbType.NVarChar, 255);
                par.SqlValue = newName == "" ? null : newName;
                par = cmd.Parameters.Add("@NewCountry", SqlDbType.Int);
                if (newCountry < 0)
                    par.SqlValue = null;
                else
                    par.SqlValue = newCountry;
                par = cmd.Parameters.Add("@StartingYear", SqlDbType.Int);
                if (startingYear < 0)
                    par.SqlValue = null;
                else
                    par.SqlValue = startingYear;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SP_ADD_Artist(" + newName + "," + newCountry + "," + startingYear + ")", ex);
                }
            }
        }

        #endregion
        
        #region Genre

        public static int AddGenre(int parentID, string genreName)
        {
            int result = -100;
            SqlParameter par;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_ADD_Genre";
                cmd.Parameters.AddWithValue("@ID_Parent", parentID);
                cmd.Parameters.AddWithValue("@GenreName", genreName);
                par = cmd.Parameters.AddWithValue("@ID_Genre", result);
                par.Direction = ParameterDirection.InputOutput;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SP_ADD_Genre(" + parentID + "," + genreName + ")", ex);
                }
            }
            if (result > 0)
                return result;
            else if (result == -1)
                throw new Exception("SP_ADD_Genre: No such parent genre " + parentID);
            else if (result == -2)
                throw new Exception("SP_ADD_Genre: " + genreName + " genre alredy exists");
            else
                throw new Exception("SP_ADD_Genre: something went wrong");
        }

        public static void DeleteGenre(int genreID)
        {
            int result = -100;
            SqlParameter par;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_DELETE_Genre";
                cmd.Parameters.AddWithValue("@ID_Genre", genreID);
                par = cmd.Parameters.AddWithValue("@Result", result);
                par.Direction = ParameterDirection.InputOutput;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("SP_DELETE_Genre(" + genreID + ")", ex);
                }
            }
            if (result == -1)
                throw new Exception("SP_DELETE_Genre: No such genre " + genreID);
            else if (result == -2)
                throw new Exception("SP_DELETE_Genre: " + genreID + " still has children." +
                    "It cannot be deleted before them");
            else if (result < 0)
                throw new Exception("SP_DELETE_Genre: something went wrong");
        }

        #endregion

        #endregion
    }
}
