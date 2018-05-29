using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratorTarget.Sql
{
    class AppDataProvider
    {
        public static Dictionary<string, string> Get_Configuration(string MemberCode, string ProviderCode, string TargetCode)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();
            string ConnectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;
            string query = "Target_Get_Configuration";

            conn = new SqlConnection(ConnectionString);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                cmd = new SqlCommand(query, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("MemberCode", SqlDbType.VarChar).Value = MemberCode;
                cmd.Parameters.Add("ProviderCode", SqlDbType.VarChar).Value = ProviderCode;
                cmd.Parameters.Add("TargetCode", SqlDbType.VarChar).Value = TargetCode;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string key = ((string)reader["ConfigurationKey"]).ToLowerInvariant();
                    string value = ((string)reader["ConfigurationValue"]).ToLowerInvariant();
                    if (!Result.ContainsKey(key))
                        Result.Add(key, value);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return Result;
        }

        public static Dictionary<MappingType, Dictionary<string, string>> Get_Mapping(int MemberID, int ProviderID, int TargetID)
        {
            Dictionary<MappingType, Dictionary<string, string>> Result = new Dictionary<MappingType, Dictionary<string, string>>();
            string ConnectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;
            string query = "Global_Get_Mapping";
            
            conn = new SqlConnection(ConnectionString);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                cmd = new SqlCommand(query, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("MemberID", SqlDbType.VarChar).Value = MemberID;
                cmd.Parameters.Add("ProviderID", SqlDbType.VarChar).Value = ProviderID;
                cmd.Parameters.Add("TargetID", SqlDbType.VarChar).Value = TargetID;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MappingType attribute = (MappingType)Enum.Parse(typeof(MappingType), ((string)reader["Attribute"]));
                    string key = ((string)reader["SourceValue"]);
                    string value = ((string)reader["TargetValue"]);
                    Dictionary<string, string> MapDic;
                    if (!Result.ContainsKey(attribute))
                    {
                        MapDic = new Dictionary<string, string>();
                        Result.Add(attribute, MapDic);
                    }
                    else
                        MapDic = Result[attribute];


                    if (!MapDic.ContainsKey(key))
                        MapDic.Add(key, value);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return Result;
        }

        public static Dictionary<string, Dictionary<string, string>> Get_Index(int MemberID, int TargetID)
        {
            Dictionary<string, Dictionary<string, string>> Result = new Dictionary<string, Dictionary<string, string>>();
            string ConnectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;
            string query = "Global_Get_Index";

            conn = new SqlConnection(ConnectionString);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                cmd = new SqlCommand(query, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("MemberID", SqlDbType.VarChar).Value = MemberID;
                cmd.Parameters.Add("TargetID", SqlDbType.VarChar).Value = TargetID;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string attribute = ((string)reader["Attribute"]);
                    string key = ((string)reader["Value"]);
                    Dictionary<string, string> MapDic;
                    if (!Result.ContainsKey(attribute))
                    {
                        MapDic = new Dictionary<string, string>();
                        Result.Add(attribute, MapDic);
                    }
                    else
                        MapDic = Result[attribute];


                    if (!MapDic.ContainsKey(key))
                        MapDic.Add(key, key);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return Result;
        }
    }
}
