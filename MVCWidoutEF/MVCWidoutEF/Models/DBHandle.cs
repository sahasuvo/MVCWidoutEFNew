using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Reflection;

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
        private dynamic SqlGetData(String StrStoredProcedure,object[] objparam, int? executiontype)
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
                default:
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adp.Fill(ds);
                    datareturn = ds;
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

        private List<T>ConvertDataTable<T>(DataTable dt)
        {
            ////http://www.c-sharpcorner.com/UploadFile/ee01e6/different-way-to-convert-datatable-to-list/
            //https://stackoverflow.com/questions/7794818/how-can-i-convert-a-datatable-into-a-dynamic-object

            List<T> dtlist = new List<T>();
            
            foreach (DataRow dr in dt.Rows)
            {
                Type tempcls = typeof(T);
                T objT = Activator.CreateInstance<T>();

                foreach (DataColumn dc in dt.Columns)
                {
                    foreach (PropertyInfo prop in tempcls.GetProperties())
                    {
                        if (prop.Name == dc.ColumnName)
                        {
                            prop.SetValue(objT, dr[dc.ColumnName], null);
                        }
                        else
                            continue;
                    }
                }
                dtlist.Add(objT);
            }

            return dtlist;
            

            //List<T>

            //
            //foreach(DataRow dr in dt.Rows)
            //{
            //    T item = 
            //}
        }
        //public dynamic GetStudent()
        //{
        //    DataTable dtStudent = SqlGetData("GetStudentDetails", null, null);
        //    //System.Collections.Generic.List<T objlst = ConvertDataTable(dtStudent);
        //    //SqlConn();
        //    //List<StudentModel> studentlist = new List<StudentModel>();
        //    //SqlCommand  cmd = new SqlCommand("")
        //}

    }
}