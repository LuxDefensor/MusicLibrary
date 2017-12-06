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
    }
}
