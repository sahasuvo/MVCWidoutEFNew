using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

//Rakesh Jasu 5-May-2016
namespace DBCustomConnection
{
    public class DBCustomConnection
    {

        public SqlConnection GetConnection()
        {
            string cn = System.Configuration.ConfigurationManager.ConnectionStrings["GOPSConnectionString"].ToString();
            SqlConnection con = new SqlConnection(cn);
            con.Open();
            return con;
        }

        public DataTable ExecuteSP(string SpName, List<SqlParameter> plist)
        {
            DataTable dt = new DataTable();
            SqlConnection con = GetConnection();
            try
            {
                
                SqlCommand cmd = new SqlCommand(SpName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (plist != null)
                {
                    foreach (SqlParameter p in plist)
                    {
                        cmd.Parameters.Add(p);
                    }
                }
                cmd.CommandTimeout = 0;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                con.Close();
            }
            catch {
                con.Close();
            }
            return dt;
        }

        public DataSet ReturnDSWithSP(string SpName, List<SqlParameter> plist)
        {
            DataSet ds = new DataSet();
            SqlConnection con = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(SpName, con);
                cmd.Parameters.Clear(); 
                cmd.CommandType = CommandType.StoredProcedure;
                if (plist != null)
                {
                    foreach (SqlParameter p in plist)
                    {
                        cmd.Parameters.Add(p);
                    }
                }
                cmd.CommandTimeout = 0;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                con.Close();
            }
            catch {
                con.Close();
            }
            return ds;
        }


    }
}
