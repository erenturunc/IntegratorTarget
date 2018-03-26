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
    class ProductDataProvider
    {
        public static Dictionary<long, Product> GetProducts(int MemberID, int ProviderID)
        {
            Dictionary<long, Product> Result = new Dictionary<long, Product>();
            string ConnectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection conn;
            SqlCommand cmd;
            SqlDataReader reader;
            string query = "Global_Get_Products";

            conn = new SqlConnection(ConnectionString);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                cmd = new SqlCommand(query, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("MemberID", SqlDbType.Int).Value = MemberID;
                cmd.Parameters.Add("ProviderID", SqlDbType.Int).Value = ProviderID;

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product p = new Product();
                    foreach (var attr in p.GetType().GetFields())
                    {
                        if (reader[attr.Name] == DBNull.Value)
                            attr.SetValue(p, null);
                        else
                            attr.SetValue(p, reader[attr.Name]);
                    }
                    Result.Add(p.ProductID, p);
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
