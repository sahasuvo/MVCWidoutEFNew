using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections;

public enum DBOperation
{
    ViewAll = 1,
    ViewSingle = 2,
    Insert = 3,
    Update = 4,
    Delete = 5,
    CustomView1 = 6,
    CustomView2 = 7

}

//namespace nsDBConnection
//{
public class DBConnection
{
    SqlConnection sqlCon = null;
    SqlCommand sqlCmd = null;

    public string gConnectionString { get; set; }
    string database;


    public DBConnection()
    {
        //if (gConnectionString.Trim().Length <= 0 || gConnectionString== null)
        // {
        string windowsAuthentication = System.Configuration.ConfigurationSettings.AppSettings["UseWindowsAuthentication"].ToString();

        /***** Modify By Atanu For dynamic Update Connection String*/

        string conString = System.Configuration.ConfigurationManager.ConnectionStrings["GOPSConnectionString"].ToString();

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.ConnectionString = conString;
        string server = builder.DataSource;
        database = builder.InitialCatalog;
        string userName = builder.UserID;
        string password = builder.Password;
        /*************/


        //string server = System.Configuration.ConfigurationSettings.AppSettings["ServerName"].ToString();
        //database = System.Configuration.ConfigurationSettings.AppSettings["DatabaseName"].ToString();
        //string userName = System.Configuration.ConfigurationSettings.AppSettings["UserName"].ToString();
        //string password = System.Configuration.ConfigurationSettings.AppSettings["Password"].ToString();

        if (windowsAuthentication.Length > 0)
        {
            if (windowsAuthentication.ToUpper().Equals("TRUE"))
                gConnectionString = "Integrated Security=SSPI;trusted_connection=true" + ";database=" + database + ";server=" + server + ";pooling = false";
            else
                gConnectionString = "Server=" + server + ";Database=" + database + ";uid=" + userName + ";pwd=" + password + ";pooling = false";
        }
        //}
    }

    /// <summary>
    /// This function used to call any kind of procedure return dataset, return value and output values
    /// </summary>
    /// <param name="procName">This field will denote procedure name</param>
    /// <param name="values">This field gets the values in object array; for output parameter user must send the value </param>
    /// <param name="dOp">This variable denotes the type of Operation means insert,view,update,delete etc</param>
    /// <param name="dsResult">This is dataset as a return type </param>
    /// <returns></returns>
    /// 
    public int CallStoredProcedure(String procName, ref object[] values,
        DBOperation dOp, ref DataSet dsResult)
    {
        SqlDataReader rdr = null;
        int ReturnSuccess = 0;
        string stroutindex = string.Empty;
        using (sqlCon = new SqlConnection(gConnectionString))
        {
            sqlCmd = new SqlCommand("ProcGOPSFindSPParams", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter param = new SqlParameter();
            param = sqlCmd.Parameters.Add("@SPName", SqlDbType.VarChar, 250);
            param.Direction = ParameterDirection.Input;
            param.Value = procName;

            sqlCon.Open();
            rdr = sqlCmd.ExecuteReader();
            sqlCmd = new SqlCommand(procName, sqlCon);

            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandTimeout = 0;
            int i = 0;
            // for input and output parameter
            while (rdr.Read())
            {
                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = rdr["parameter_name"].ToString();
                param1.SqlDbType = getType(rdr["parameter_type"].ToString());
                param1.Size = Convert.ToInt32(rdr["max_length"]);
                param1.Direction = Convert.ToBoolean(rdr["is_output"]) == true ? ParameterDirection.InputOutput : ParameterDirection.Input;
                param1.Value = values[i];
                sqlCmd.Parameters.Add(param1);
                stroutindex += Convert.ToBoolean(rdr["is_output"]) == true ? rdr["parameter_id"].ToString() + "," : "";
                i++;
            }
            sqlCon.Close();
            // for Return parameter
            SqlParameter paramreturn = new SqlParameter();
            paramreturn.ParameterName = "@ReturnValue";
            paramreturn.SqlDbType = SqlDbType.Int;
            paramreturn.Direction = ParameterDirection.ReturnValue;
            paramreturn.Value = ReturnSuccess;
            sqlCmd.Parameters.Add(paramreturn);

            switch (dOp)
            {
                case DBOperation.ViewAll:
                case DBOperation.ViewSingle:
                case DBOperation.CustomView1:
                case DBOperation.CustomView2:
                    SqlDataAdapter DA = new SqlDataAdapter();
                    DA.SelectCommand = sqlCmd;
                    DA.Fill(dsResult);
                    if (stroutindex != string.Empty)
                    {
                        stroutindex = stroutindex.Substring(0, stroutindex.Length - 1);
                        string[] arroutindex = stroutindex.Split(',');
                        if (arroutindex.Length > 0)
                            for (int j = 0; j < arroutindex.Length; j++)
                                values[Convert.ToInt32(arroutindex[j].ToString()) - 1]
                                    = sqlCmd.Parameters[Convert.ToInt32(arroutindex[j].ToString()) - 1].Value;
                    }
                    ReturnSuccess = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
                    sqlCon.Close();
                    break;
                case DBOperation.Insert:
                case DBOperation.Update:
                case DBOperation.Delete:
                    sqlCon.Open();
                    sqlCmd.ExecuteNonQuery();
                    // geting output parameter dynamically
                    if (stroutindex != string.Empty)
                    {
                        stroutindex = stroutindex.Substring(0, stroutindex.Length - 1);
                        string[] arroutindex = stroutindex.Split(',');
                        if (arroutindex.Length > 0)
                            for (int j = 0; j < arroutindex.Length; j++)
                                values[Convert.ToInt32(arroutindex[j].ToString()) - 1]
                                    = sqlCmd.Parameters[Convert.ToInt32(arroutindex[j].ToString()) - 1].Value;
                    }
                    ReturnSuccess = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
                    sqlCon.Close();
                    break;
            }
        }
        return ReturnSuccess;
    }

    /// <summary>
    /// this function is new with hashtable 
    /// </summary>
    /// <param name="procName"></param>
    /// <param name="ht"></param>
    /// <param name="dOp"></param>
    /// <param name="dsResult"></param>
    /// <returns></returns>
    protected int CallStoredProcedureNew(string procName, ref Hashtable ht, DBOperation dOp,
        ref DataSet dsResult)
    {
        SqlDataReader rdr = null;
        int ReturnSuccess = 0;
        string stroutindex = string.Empty;
        using (sqlCon = new SqlConnection(gConnectionString))
        {
            sqlCmd = new SqlCommand("ProcGOPSFindSPParams", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            SqlParameter param = new SqlParameter();
            param = sqlCmd.Parameters.Add("@SPName", SqlDbType.VarChar, 250);
            param.Direction = ParameterDirection.Input;
            param.Value = procName;

            sqlCon.Open();
            rdr = sqlCmd.ExecuteReader();
            sqlCmd = new SqlCommand(procName, sqlCon);

            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandTimeout =3600;
            int i = 0;
            // for input and output parameter
            while (rdr.Read())
            {
                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = rdr["parameter_name"].ToString();
                param1.SqlDbType = getType(rdr["parameter_type"].ToString());
                param1.Size = Convert.ToInt32(rdr["max_length"]);
                param1.Direction = Convert.ToBoolean(rdr["is_output"]) == true ? ParameterDirection.InputOutput : ParameterDirection.Input;
                param1.Value = ht[i];
                sqlCmd.Parameters.Add(param1);
                stroutindex += Convert.ToBoolean(rdr["is_output"]) == true ? rdr["parameter_id"].ToString() + "," : "";
                i++;
            }
            sqlCon.Close();
            // for Return parameter
            SqlParameter paramreturn = new SqlParameter();
            paramreturn.ParameterName = "@ReturnValue";
            paramreturn.SqlDbType = SqlDbType.Int;
            paramreturn.Direction = ParameterDirection.ReturnValue;
            paramreturn.Value = ReturnSuccess;
            sqlCmd.Parameters.Add(paramreturn);

            switch (dOp)
            {
                case DBOperation.ViewAll:
                case DBOperation.ViewSingle:
                    SqlDataAdapter DA = new SqlDataAdapter();
                    DA.SelectCommand = sqlCmd;
                    DA.Fill(dsResult);
                    if (stroutindex != string.Empty)
                    {
                        stroutindex = stroutindex.Substring(0, stroutindex.Length - 1);
                        string[] arroutindex = stroutindex.Split(',');
                        if (arroutindex.Length > 0)
                            for (int j = 0; j < arroutindex.Length; j++)
                                ht[Convert.ToInt32(arroutindex[j].ToString()) - 1]
                                    = sqlCmd.Parameters[Convert.ToInt32(arroutindex[j].ToString()) - 1].Value;
                    }
                    ReturnSuccess = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
                    sqlCon.Close();
                    break;
                case DBOperation.Insert:
                case DBOperation.Update:
                case DBOperation.Delete:
                    sqlCon.Open();
                    sqlCmd.ExecuteNonQuery();
                    // geting output parameter dynamically
                    if (stroutindex != string.Empty)
                    {
                        stroutindex = stroutindex.Substring(0, stroutindex.Length - 1);
                        string[] arroutindex = stroutindex.Split(',');
                        if (arroutindex.Length > 0)
                            for (int j = 0; j < arroutindex.Length; j++)
                                ht[Convert.ToInt32(arroutindex[j].ToString()) - 1]
                                    = sqlCmd.Parameters[Convert.ToInt32(arroutindex[j].ToString()) - 1].Value;
                    }
                    ReturnSuccess = Convert.ToInt32(sqlCmd.Parameters["@ReturnValue"].Value);
                    sqlCon.Close();
                    break;
            }
        }
        return ReturnSuccess;
    }
    private SqlDbType getType(string type)
        {
            SqlDbType objDBtype = new SqlDbType();
            switch (type)
            {
                case "bit":
                    objDBtype = SqlDbType.Bit;
                    break;
                case "int":
                    objDBtype = SqlDbType.Int;
                    break;
                case "varchar":
                    objDBtype = SqlDbType.VarChar;
                    break;
                case "bigint":
                    objDBtype = SqlDbType.BigInt;
                    break;
                case "PayrollKeyID":
                    objDBtype = SqlDbType.BigInt;
                    break;
                case "datetime":
                    objDBtype = SqlDbType.DateTime;
                    break;
                case "float":
                    objDBtype = SqlDbType.Float;
                    break;
                case "char":
                    objDBtype = SqlDbType.Char;
                    break;
                case "nvarchar":
                    objDBtype = SqlDbType.NVarChar;
                    break;
                case "decimal":
                    objDBtype = SqlDbType.Decimal;
                    break;
                case "varbinary":
                    objDBtype = SqlDbType.VarBinary;
                    break;
                case "tblAttendance":
                case "AttInsert":
                case "tblDeviceMap":
                case "tblBulkExit":
                case "tblEmpGradeMap":
                case "BulkOfferletterTable": //BulkOfferletterTable
                case "TBLTYPEMPLOYEEDOCUMENTDETAILS":
                case "TBLTYPEMPLOYEEFAMILYDETAILS":
                case "TBLTYPEMPLOYEEEDUCATIONDETAILS":
                case "tblTypUserAccessList":
                case "TBLTYPEMPLOYEEPropertyDETAILS":
                case "tblTypUserBranchAccessList":
                case "tblTypUserLeevelAccessList":
                case "tblTypAllIndiaUserList":
                case "tblTypBranchWiseUserList":
                case "AttConsole":
                case "AttConsoleExit":
                case "particularDtls":
                case "ALLdata":
                case "TableData":
                case "DesignationAll":
                case "TBLTYPEMPLOYEEPropertyDETAILSDATEConfigur":
                case "udtEmpAppraisal":
                case "udtAppraisalDescDetails":
                case "TblJdInsert":
                objDBtype = SqlDbType.Structured;
                    break;
                default:
                    objDBtype = SqlDbType.VarChar;
                    break;
            }
            return objDBtype;
        }
}