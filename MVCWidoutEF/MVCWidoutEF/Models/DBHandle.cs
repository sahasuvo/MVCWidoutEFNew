using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace MVCWidoutEF.Models
{
    public class DBHandle
    {
        private SqlConnection conn;
        private void SqlConn()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["studentconfig"].ToString());
        }

        private int SqlNonQuery(String StrStoredProcedure,object[] objparam)
        {
            int ireturn=0;
            SqlConn();
            SqlCommand cmd = new SqlCommand(StrStoredProcedure, conn);
            SqlCommandBuilder.DeriveParameters(cmd);
            cmd.CommandType = CommandType.StoredProcedure;

            //SqlParameterCollection sqlparams = new SqlParameterCollection();
            //sqlparams.
            for (int i=0;i<=cmd.Parameters.Count;i++)
            {
                cmd.Parameters[i].Value = objparam[i].ToString();
                
                
                //cmd.Parameters.Add(new SqlParameter().Value)
            }
            conn.Open();
            ireturn = cmd.ExecuteNonQuery();
            conn.Close();
            return ireturn;
        }

        dynamic datareturn { get; set; }
        private dynamic SqlGetData(String StrStoredProcedure,object[] objparam, int executiontype)
        {
            datareturn = null;
            SqlConn();
            SqlCommand cmd = new SqlCommand(StrStoredProcedure, conn);
            SqlCommandBuilder.DeriveParameters(cmd);
            cmd.CommandType = CommandType.StoredProcedure;
            
          
            if (objparam.Length > 0)
            {
                for (int i = 0; i <= cmd.Parameters.Count; i++)
                {
                    cmd.Parameters[i].Value = objparam[i].ToString();
                    //cmd.Parameters.Add(new SqlParameter().Value)
                }
            }

            conn.Open();
            switch (executiontype)
            {
                case 1: // Execute scalar
                    object objexecscalar = cmd.ExecuteScalar();
                    if (objexecscalar != null)
                        datareturn= objexecscalar.ToString();
                    break;
                case 2: //Execute Query
                    break;
            }

            conn.Close();
            return datareturn;
        }

        public bool AddStudent(StudentModel smodel)
        {
            
            object[] data = new object[3];
            data[0] = smodel.Name;
            data[1] = smodel.CityName;
            data[2] = smodel.Address;

            int sadd = SqlNonQuery("AddNewStudent",data);
            return sadd == 0 ? false : true;
        }
        public bool UpdateStudent(StudentModel smodel)
        {
            object[] data = new object[4];

            data[0] = smodel.ID;
            data[1] = smodel.Name;
            data[2] = smodel.CityName;
            data[3] = smodel.Address;

            int i = SqlNonQuery("UpdateStudent", data);
            return i == 0 ? false : true;
        }
        public bool DeleteStudent(int id)
        {
            object[] data = new object[1];

            data[0] = id;

            int i = SqlNonQuery("DeleteStudent", data);
            return i == 0 ? false : true;
        }
    }
}