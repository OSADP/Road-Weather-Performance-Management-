using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data;

namespace INFLOClassLib
{
    public class clsDatabase
    {
        public enum enDBInterfaceType
        {
            enNA = 0,
            enSQLServer = 1,
            enODBC = 2,
            enOLEDB = 3
        };
        
        private enDBInterfaceType m_DBInterfaceType;
        public enDBInterfaceType DBInterfaceType
        {
            get { return m_DBInterfaceType; }
            set { m_DBInterfaceType = value; }
        }
        
        private string m_ConnectionStr;
        public string ConnectionStr
        {
            get { return m_ConnectionStr; }
            set { m_ConnectionStr = value; }
        }

        private string m_OLEMSAccessFileName;
        public string OLEMSAccessFileName
        {
            get { return m_OLEMSAccessFileName; }
            set { m_OLEMSAccessFileName = value; }
        }
        
        private string m_ODBCDSNName;
        public string ODBCDSNName
        {
            get { return m_ODBCDSNName; }
            set { m_ODBCDSNName = value; }
        }

        private string m_SQLServerConnection;
        public string SQLServerConnection
        {
            get { return m_SQLServerConnection; }
            set { m_SQLServerConnection = value; }
        }

        private bool m_WasDone;
        public bool WasDone
        {
            get { return m_WasDone; }
        }

        private DataSet m_DataSet;

        private OleDbDataAdapter m_OLEDBDataAdapter;
        private OleDbConnection m_OLEDBConnection;
        public OleDbConnection OLEDBConnection
        {
            get { return m_OLEDBConnection; }
            set { m_OLEDBConnection = value; }
        }

        private SqlDataAdapter m_SQLDBDataAdapter;
        private SqlConnection m_SQLDBConnection;
        public SqlConnection SQLDBConnection
        {
            get { return m_SQLDBConnection; }
            set { m_SQLDBConnection = value; }
        }

        private OdbcDataAdapter m_ODBCDBDataAdapter;
        private OdbcConnection m_ODBCDBConnection;
        public OdbcConnection ODBCDBConnection
        {
            get { return m_ODBCDBConnection; }
            set { m_ODBCDBConnection = value; }
        }

        public clsDatabase()
        {
        }
        public string OpenDBConnection(string p_DBInterfaceType, string p_ConnectionStr)
        {
            string retValue = string.Empty;
            //string ConnectionStr = string.Empty;

            if (p_DBInterfaceType.ToLower() == "oledb")
            {
                m_DBInterfaceType = enDBInterfaceType.enOLEDB;
                m_OLEMSAccessFileName = clsGlobalVars.AccessDBFileName;
                if (m_OLEMSAccessFileName.Length > 0)
                {
                    m_ConnectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + m_OLEMSAccessFileName;
                }
                else
                {
                    retValue = "\tError in establishing connection to INFLO database. " +
                               "\r\n\t\tA Valid MS Access database file name must be specified. " +
                               "\r\n\t\tInvalid MS Access db file specified:= " + m_OLEMSAccessFileName;
                    return retValue;
                }
            }
            else if (p_DBInterfaceType.ToLower() == "odbc")
            {
                m_DBInterfaceType = enDBInterfaceType.enODBC;
                m_ODBCDSNName = clsGlobalVars.DSNName;
                if (m_ODBCDSNName.Length > 0)
                {
                    m_ConnectionStr = "DSN=" + m_ODBCDSNName;
                }
                else
                {
                    retValue = "\tError in establishing connection to INFLO database. " +
                               "\r\n\t\tA Valid ODBC DSN name must be specified. " + 
                               "\r\n\t\tInvalid DSN name specified:= " + m_ODBCDSNName;
                    return retValue;
                }
            }
            else if (p_DBInterfaceType.ToLower() == "sqlserver")
            {
                m_DBInterfaceType = enDBInterfaceType.enSQLServer;
                
                if ((clsGlobalVars.SqlServer.Length > 0) && (clsGlobalVars.SqlServerDatabase.Length > 0) && (clsGlobalVars.SqlServerUserId.Length > 0) && (clsGlobalVars.SqlServerPassword.Length > 0))
                {
                    System.Data.SqlClient.SqlConnectionStringBuilder tmpBuilder = new SqlConnectionStringBuilder();
                    tmpBuilder["Server"] = clsGlobalVars.SqlServer;
                    //tmpBuilder["Database"] = "(inflodev-vm-sql)";
                    tmpBuilder.InitialCatalog = clsGlobalVars.SqlServerDatabase;
                    tmpBuilder.UserID = clsGlobalVars.SqlServerUserId;
                    tmpBuilder.Password = clsGlobalVars.SqlServerPassword;


                    m_SQLServerConnection = tmpBuilder.ConnectionString;
                }
                else if (clsGlobalVars.SqlStrConnection.Length > 0)
                {
                    m_SQLServerConnection = clsGlobalVars.SqlStrConnection;
                }
                if (m_SQLServerConnection.Length > 0)
                {
                    m_ConnectionStr = m_SQLServerConnection;
                }
                else
                {
                    retValue = "\tError in establishing connection to INFLO database. " +
                               "\r\n\t\tA Valid ODBC SQL Server Connection string must be specified. " +
                               "\r\n\t\tInvalid SQL Server Connection specified:= " + m_SQLServerConnection;
                    return retValue;
                }
            }

            if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                try
                {
                    m_OLEDBDataAdapter = new OleDbDataAdapter();
                    m_OLEDBConnection = new OleDbConnection(m_ConnectionStr);
                    m_OLEDBDataAdapter.SelectCommand = new OleDbCommand();
                    m_OLEDBDataAdapter.SelectCommand.Connection = m_OLEDBConnection;
                    m_OLEDBConnection.Open();
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = "\tOpenDBConnection Error - Connection to MS Access database: " + m_OLEMSAccessFileName + " failed. \r\n\t" + e.Message;
                    m_WasDone = false;
                }

            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                try
                {
                    m_ODBCDBDataAdapter = new OdbcDataAdapter();
                    m_ODBCDBConnection = new OdbcConnection(m_ConnectionStr);
                    m_ODBCDBDataAdapter.SelectCommand = new OdbcCommand();
                    m_ODBCDBDataAdapter.SelectCommand.Connection = m_ODBCDBConnection;
                    m_ODBCDBConnection.Open();
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = "\tOpenDBConnection Error - Connection to ODBC DSN: " + m_ODBCDSNName + " failed. \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                try
                {
                    m_SQLDBDataAdapter = new SqlDataAdapter();
                    m_SQLDBConnection = new SqlConnection(m_ConnectionStr);
                    m_SQLDBDataAdapter.SelectCommand = new SqlCommand();
                    m_SQLDBDataAdapter.SelectCommand.Connection = m_SQLDBConnection;
                    m_SQLDBConnection.Open();
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = "\tOpenDBConnection Error - Connection to SQL Server: " + m_SQLServerConnection + " failed. \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            return retValue;
        }

        public  clsDatabase(string p_DBInterfaceType)
        {
            string retValue = string.Empty;

            switch (p_DBInterfaceType.ToLower())
            {
                case "sqlserver":
                    m_DBInterfaceType = enDBInterfaceType.enSQLServer;
                    if ((clsGlobalVars.SqlServer.Length > 0) && (clsGlobalVars.SqlServerDatabase.Length > 0) && (clsGlobalVars.SqlServerUserId.Length > 0) && (clsGlobalVars.SqlServerPassword.Length > 0))
                    {
                        System.Data.SqlClient.SqlConnectionStringBuilder tmpBuilder = new SqlConnectionStringBuilder();
                        tmpBuilder["Server"] = clsGlobalVars.SqlServer;
                        //tmpBuilder["Database"] = "(inflodev-vm-sql)";
                        tmpBuilder.InitialCatalog = clsGlobalVars.SqlServerDatabase;
                        tmpBuilder.UserID = clsGlobalVars.SqlServerUserId;
                        tmpBuilder.Password = clsGlobalVars.SqlServerPassword;


                        m_SQLServerConnection = tmpBuilder.ConnectionString;
                    }
                    else if (clsGlobalVars.SqlStrConnection.Length > 0)
                    {
                        m_SQLServerConnection = clsGlobalVars.SqlStrConnection;
                    }
                    if (m_SQLServerConnection.Length > 0)
                    {
                        m_ConnectionStr = m_SQLServerConnection;
                    }
                    //else
                    //{
                    //    retValue = "\tError in generating connection to INFLO database. " +
                    //               "\r\n\t\tA Valid ODBC SQL Server Connection string must be specified. " +
                    //               "\r\n\t\tInvalid SQL Server Connection specified:= " + m_SQLServerConnection;
                    //    return retValue;
                    //}
                    break;
                case "odbc":
                    m_DBInterfaceType = enDBInterfaceType.enODBC;
                    m_ODBCDSNName = clsGlobalVars.DSNName;
                    if (m_ODBCDSNName.Length > 0)
                    {
                        m_ConnectionStr = "DRIVER={MySQL ODBC Driver};DSN=" + m_ODBCDSNName + ";UID=inflo;PWD=B@ttelle;";
                        m_ConnectionStr = "DRIVER={MySQL ODBC Driver};Server=172.16.7.100;UID=inflo;PWD=B@ttelle;";
                    }
                    //else
                    //{
                    //    retValue = "\tError in generating connection to INFLO database. " +
                    //               "\r\n\t\tA Valid ODBC DSN name must be specified. " + 
                    //               "\r\n\t\tInvalid DSN name specified:= " + m_ODBCDSNName;
                    //    return retValue;
                    //}
                    break;
                case "oledb":
                    m_DBInterfaceType = enDBInterfaceType.enOLEDB;
                    m_OLEMSAccessFileName = clsGlobalVars.AccessDBFileName;
                    if (m_OLEMSAccessFileName.Length > 0)
                    {
                        m_ConnectionStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + m_OLEMSAccessFileName;
                    }
                    //else
                    //{
                    //    retValue = "\tError in generating connection to INFLO database. " +
                    //               "\r\n\t\tA Valid MS Access database file name must be specified. " +
                    //               "\r\n\t\tInvalid MS Access db file specified:= " + m_OLEMSAccessFileName;
                    //    return retValue;
                    //}
                    break;
            }
        }
        public string OpenDBConnection()
        {
            string retValue = string.Empty;
            string ConnectionStr = string.Empty;

            if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                try
                {
                    m_OLEDBDataAdapter = new OleDbDataAdapter();
                    m_OLEDBConnection = new OleDbConnection(m_ConnectionStr);
                    m_OLEDBDataAdapter.SelectCommand = new OleDbCommand();
                    m_OLEDBDataAdapter.SelectCommand.Connection = m_OLEDBConnection;
                    m_OLEDBConnection.Open();
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = "\tOpenDBConnection Error - Connection to MS Access database: " + m_OLEMSAccessFileName + " failed. \r\n\t" + e.Message;
                    m_WasDone = false;
                }

            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                try
                {
                    m_ODBCDBDataAdapter = new OdbcDataAdapter();
                    m_ODBCDBConnection = new OdbcConnection(m_ConnectionStr);
                    m_ODBCDBDataAdapter.SelectCommand = new OdbcCommand();
                    m_ODBCDBDataAdapter.SelectCommand.Connection = m_ODBCDBConnection;
                    m_ODBCDBConnection.Open();
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = "\tOpenDBConnection Error - Connection to ODBC DSN: " + m_ODBCDSNName + " failed. \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                try
                {
                    m_SQLDBDataAdapter = new SqlDataAdapter();
                    m_SQLDBConnection = new SqlConnection(m_ConnectionStr);
                    m_SQLDBDataAdapter.SelectCommand = new SqlCommand();
                    m_SQLDBDataAdapter.SelectCommand.Connection = m_SQLDBConnection;
                    m_SQLDBConnection.Open();
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = "\tOpenDBConnection Error - Connection to SQL Server: " + m_SQLServerConnection + " failed. \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            return retValue;
        }

        public string FillDataSet(string strSql, ref DataSet p_DataSet)
        {
            string retValue = string.Empty;

            if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                try
                {
                    m_SQLDBDataAdapter.SelectCommand.CommandText = strSql;
                    m_SQLDBDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    m_SQLDBDataAdapter.SelectCommand.ExecuteNonQuery();
                    m_SQLDBDataAdapter.Fill(p_DataSet);
                    m_WasDone = true;
                }
                catch (System.StackOverflowException exc)
                {
                    retValue = " FillDataSet SQL-Server-Database Error - Fill data set attempt failed: \r\n\t" + strSql + "\r\n\t" + exc.Message;
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " FillDataSet SQL-Server-Database Error - Fill data set attempt failed: \r\n\t" + strSql + " \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                try
                {
                    m_ODBCDBDataAdapter.SelectCommand.CommandText = strSql;
                    m_ODBCDBDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    m_ODBCDBDataAdapter.SelectCommand.ExecuteNonQuery();
                    m_ODBCDBDataAdapter.Fill(p_DataSet);
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = " FillDataSet ODBC-Database Error - Fill data set attempt failed: \r\n\t" + strSql + " \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                try
                {
                    m_OLEDBDataAdapter.SelectCommand.CommandText = strSql;
                    m_OLEDBDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    try
                    {
                        m_OLEDBDataAdapter.SelectCommand.ExecuteNonQuery();
                        m_OLEDBDataAdapter.Fill(p_DataSet);
                    }
                    catch (System.StackOverflowException exc)
                    {
                        retValue = " FillDataSet OLEDB-Database Error - Fill data set attempt failed: \r\n\t" + strSql + " \r\n\t" + exc.Message;
                        m_WasDone = false;
                    }
                    m_WasDone = true;
                }
                catch (System.StackOverflowException exc)
                {
                    retValue = " FillDataSet OLEDB-Database Error - Fill data set attempt failed: \r\n\t" + strSql + " \r\n\t" + exc.Message;
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " FillDataSet OLEDB-Database Error - Fill data set attempt failed: \r\n\t" + strSql + " \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            return retValue;
        }
        public string FillDataSet(string strTable, string strSql, ref DataSet FilledDataSet)
        {
            string retValue = string.Empty;

            if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                try
                {
                    m_SQLDBDataAdapter.SelectCommand.CommandText = strSql;
                    m_SQLDBDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    m_SQLDBDataAdapter.SelectCommand.ExecuteNonQuery();
                    m_SQLDBDataAdapter.Fill(m_DataSet);
                    FilledDataSet = m_DataSet;
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = " FillDataSet SQL-Server-Database Error - Fill data set attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                try
                {
                    m_ODBCDBDataAdapter.SelectCommand.CommandText = strSql;
                    m_ODBCDBDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    m_ODBCDBDataAdapter.SelectCommand.ExecuteNonQuery();
                    m_ODBCDBDataAdapter.Fill(m_DataSet, strTable);
                    FilledDataSet = m_DataSet;
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = " FillDataSet ODBC-Database Error - Fill data set attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                try
                {
                    m_OLEDBDataAdapter.SelectCommand.CommandText = strSql;
                    m_OLEDBDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    m_OLEDBDataAdapter.SelectCommand.ExecuteNonQuery();
                    m_OLEDBDataAdapter.Fill(m_DataSet, strTable);
                    FilledDataSet = m_DataSet;
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    retValue = " FillDataSet OLEDB-Database Error - Fill data set attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            return retValue;
        }

        public string InsertRow(string strCmd)
        {
            string retValue = string.Empty;
            retValue = string.Empty;

            if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                SqlCommand objsqlCommand = new SqlCommand();
                try
                {
                    objsqlCommand = new SqlCommand(strCmd, m_SQLDBConnection);
                    objsqlCommand.ExecuteNonQuery();
                    m_WasDone = true;
                }
                catch (SqlException dbException)
                {
                    retValue = " InsertRow SQL-Server-Database Error - insert new row attempt failed: \r\n\t" + dbException.Message;
                    retValue = dbException.ToString();
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " InsertRow SQL-Server-Database Error - insert new row attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
                finally
                {
                    if (!(objsqlCommand == null))
                    {
                        objsqlCommand = null;
                    }
                } 
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                OdbcCommand objODBCCommand = new OdbcCommand();
                try
                {
                    objODBCCommand = new OdbcCommand(strCmd, m_ODBCDBConnection);
                    objODBCCommand.ExecuteNonQuery();
                    objODBCCommand = null;
                    m_WasDone = true;
                }
                catch (OdbcException dbException)
                {
                    retValue = " InsertRow ODBC Database Error - insert new row attempt failed: \r\n\t" + dbException.Message;
                    retValue = dbException.ToString();
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " InsertRow ODBC Database Error - insert new row attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
                finally
                {
                    if (!(objODBCCommand == null))
                    {
                        objODBCCommand = null;
                    }
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                //OleDbCommand objOledbCommand = new OleDbCommand();
                string sqlStr = string.Empty;
                try 
                {

                    //sqlStr = "INSERT INTO TMEInput_CVData(NomadicDeviceID, Timestamp, Speed, MileMarkerLocation, Queued) " +
                    //           "Values(?, ?, ?, ?, ?)";
                    //sqlStr = "INSERT INTO TMEInput_CVData(Timestamp) " +
                    //           "Values(?)";
                    OleDbCommand objOledbCommand = new OleDbCommand(strCmd, m_OLEDBConnection);
                    //objOledbCommand.Parameters.Add("NomadicDeviceID", OleDbType.Char).Value = CVID;
                    //objOledbCommand.Parameters.Add("Timestamp", OleDbType.Date).Value = "#" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + "#";
                    //objOledbCommand.Parameters.Add("Speed", OleDbType.Double).Value = speed;
                    //objOledbCommand.Parameters.Add("MileMarkerLocation", OleDbType.Double).Value = mmlocation;
                    //objOledbCommand.Parameters.Add("Queued", OleDbType.Char).Value = queued;

                    //myCommand.Parameters.AddWithValue("@MovieProducer", txtMovieProducer.Text)
                    //myCommand.Parameters.AddWithValue("@MovieWriter", txtMovieWriter.Text)
                    //myCommand.Parameters.AddWithValue("@MovieComposer", txtMovieComposer.Text)
                    //myCommand.Parameters.AddWithValue("@MovieActors", txtMovieActors.Text)

                    objOledbCommand.ExecuteNonQuery();
                    objOledbCommand = null;
                    m_WasDone = true;
                }
                catch (OleDbException dbException)
                {
                    retValue = " InsertRow OLEDB Database Error - insert new row attempt failed: \r\n\t" + dbException.Message;
                    retValue = dbException.ToString();
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " InsertRow OLEDB Database Error - insert new row attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
                finally
                {
                    //if (!(objOledbCommand == null))
                    //{
                    //    objOledbCommand = null;
                    //}
                }       
            }
            return retValue;
        }

        /*public string  AddRecordto()
        {

        if (true)
        {
            string CmndString = "INSERT INTO BT_Link_Travel_Time(BT_Station_ID, TS, Link_Travel_TM, Link_Speed, Summary_Minutes, Summary_Samples) " + 
                            "Values(" + BTLinkID + ", " + Chr(34) + BtMsg.timestamp +  Chr(34) + ", " + BtMsg.travel_time + ", " +
                             BtMsg.speed_mph + ", " + BtMsg.summary_mins + ", " + BtMsg.summary_samples + ")"

            Me.OleBTNewMsgCmnd = New OleDbCommand(CmndString, OleBTNewMsgConn)

            Try
                OleBTNewMsgConn.Open()
                OleBTNewMsgCmnd.ExecuteReader()
                LogTxtMsg(txtLog, "A new BT record was added to the database: " & BtMsg.BTSegmentDBID & ", " & BtMsg.timestamp & ", " & BtMsg.travel_time & ", " & BtMsg.speed_mph & ", " & BtMsg.summary_mins & ", " & BtMsg.summary_samples)
            Catch ex As Exception
                MessageBox.Show("Could not add record: " & BtMsg.BTSegmentDBID & ", " & BtMsg.timestamp & ", " & BtMsg.travel_time & ", " & BtMsg.speed_mph & ", " & BtMsg.summary_mins & ", " & BtMsg.summary_samples & ControlChars.CrLf & ex.Message)
            Finally
                OleBTNewMsgConn.Close()
                OleBTNewMsgConn = Nothing
                OleBTNewMsgCmnd = Nothing
            End Try
        }
        }
        */

        public string UpdateRow(string strCmd)
        {
            string retValue = string.Empty;
            retValue = "Record was updated.";

            if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                SqlCommand objsqlCommand;
                objsqlCommand = new SqlCommand(strCmd, m_SQLDBConnection);
                try
                {
                    objsqlCommand.ExecuteNonQuery();
                }
                catch (SqlException dbException)
                {
                    retValue = " UpdateRow SQL-Server-Database Error - Update row attempt failed: \r\n\t" + dbException.Message;
                }
                catch (Exception e)
                {
                    retValue = " UpdateRow SQL-Server-Database Error - Update row attempt failed: \r\n\t" + e.Message;
                }
                finally
                {
                    if (!(objsqlCommand == null))
                    {
                        objsqlCommand = null;
                    }
                    else
                    {
                        retValue = "Record was not updated.";
                    }
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                OdbcCommand objODBCCommand;
                objODBCCommand = new OdbcCommand(strCmd, m_ODBCDBConnection);
                try
                {
                    objODBCCommand.ExecuteNonQuery();
                }
                catch(OdbcException dbException)
                {
                    retValue = " UpdateRow ODBC Database Error - Update row attempt failed: \r\n\t" + dbException.Message;
                }
                catch (Exception e)
                {
                    retValue = " UpdateRow ODBC Database Error - Update row attempt failed: \r\n\t" + e.Message;
                }
                finally
                {
                    if (!(objODBCCommand == null))
                    {
                        objODBCCommand = null;
                    }
                    else
                    {
                        retValue = "Record was not updated.";
                    }
                }
            }
            else if(m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                OleDbCommand objOledbCommand;
                objOledbCommand = new OleDbCommand(strCmd, m_OLEDBConnection);
                try
                {
                    objOledbCommand.ExecuteNonQuery();
                }
                catch (OleDbException dbException)
                {
                    retValue = " UpdateRow OLEDB Database Error - Update row attempt failed: \r\n\t" + dbException.Message;
                }
                catch (Exception e)
                {
                    retValue = " UpdateRow OLEDB Database Error - Update row attempt failed: \r\n\t" + e.Message;
                }
                finally
                {
                    if (!(objOledbCommand == null))
                    {
                        objOledbCommand = null;
                    }
                    else
                    {
                        retValue = "Record was not updated.";
                    }
                }
            }
            return retValue;
        }

        public string DeleteRow(string strCmd)
        {
            string retValue = string.Empty;
        
            if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                SqlCommand objsqlCommand = new SqlCommand(strCmd, m_SQLDBConnection);
                try
                {
                    objsqlCommand.ExecuteNonQuery();
                    objsqlCommand = null;
                    m_WasDone = true;
                }
                catch (SqlException dbException)
                {
                    retValue = " Delete rows SQL Server Database Error - Delete old rows attempt failed: \r\n\t" + dbException.Message;
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " Delete rows SQL Server Database Error - Delete old rows attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                OdbcCommand objODBCCommand = new OdbcCommand(strCmd, m_ODBCDBConnection);
                try
                {
                    objODBCCommand.ExecuteNonQuery();
                    objODBCCommand = null;
                    m_WasDone = true;
                }
                catch (OdbcException dbException)
                {
                    retValue = " Delete rows ODBC Database Error - Delete old rows attempt failed: \r\n\t" + dbException.Message;
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " Delete rows ODBC Database Error - Delete old rows attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {

                OleDbCommand objOledbCommand = new OleDbCommand(strCmd, m_OLEDBConnection);
                try
                {
                    objOledbCommand.ExecuteNonQuery();
                    objOledbCommand = null;
                    m_WasDone = true;
                }
                catch (OleDbException dbException)
                {
                    retValue = " Delete rows OLE Database Error - Delete old rows attempt failed: \r\n\t" + dbException.Message;
                    m_WasDone = false;
                }
                catch (Exception e)
                {
                    retValue = " Delete rows OLE Database Error - Delete old rows attempt failed: \r\n\t" + e.Message;
                    m_WasDone = false;
                }
            }
            return retValue;
        }

        public string BulkCopyCVData(DataTable CVDataTable, string p_DBInterfaceType)
        {
            string retValue = string.Empty;

            if (p_DBInterfaceType.ToLower() == "sqlserver")
            {
                m_DBInterfaceType = enDBInterfaceType.enSQLServer;
                if ((clsGlobalVars.SqlServer.Length > 0) && (clsGlobalVars.SqlServerDatabase.Length > 0) && (clsGlobalVars.SqlServerUserId.Length > 0) && (clsGlobalVars.SqlServerPassword.Length > 0))
                {
                    SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
                    cb["Server"] = clsGlobalVars.SqlServer;
                    //tmpBuilder["Database"] = "(inflodev-vm-sql)";
                    cb.InitialCatalog = clsGlobalVars.SqlServerDatabase;
                    cb.UserID = clsGlobalVars.SqlServerUserId;
                    cb.Password = clsGlobalVars.SqlServerPassword;

                    m_SQLServerConnection = cb.ConnectionString;
                }
                else if (clsGlobalVars.SqlStrConnection.Length > 0)
                {
                    m_SQLServerConnection = clsGlobalVars.SqlStrConnection;
                }
                if (m_SQLServerConnection.Length > 0)
                {
                    m_ConnectionStr = m_SQLServerConnection;
                }
                else
                {
                    retValue = "\tCVData BulkCopy: Error in generating connection to INFLO database. " +
                               "\r\n\t\tA Valid ODBC SQL Server Connection string must be specified. " +
                               "\r\n\t\tInvalid SQL Server Connection specified:= " + m_SQLServerConnection;
                    return retValue;
                }
            }

            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(m_ConnectionStr);
                sbc.DestinationTableName = "TME_CVData_Input";
                sbc.WriteToServer(CVDataTable);
                sbc.Close();
            }
            catch (Exception ex)
            {
                retValue = "Error in BulK copy into INFLO database of CV Data: " + ex.Message;
                return retValue;
            }

            return retValue;
        }

        public string BulkCopyDZData(DataTable DZDataTable, string p_DBInterfaceType)
        {
            string retValue = string.Empty;

            if (p_DBInterfaceType.ToLower() == "sqlserver")
            {
                m_DBInterfaceType = enDBInterfaceType.enSQLServer;
                if ((clsGlobalVars.SqlServer.Length > 0) && (clsGlobalVars.SqlServerDatabase.Length > 0) && (clsGlobalVars.SqlServerUserId.Length > 0) && (clsGlobalVars.SqlServerPassword.Length > 0))
                {
                    SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
                    cb["Server"] = clsGlobalVars.SqlServer;
                    //tmpBuilder["Database"] = "(inflodev-vm-sql)";
                    cb.InitialCatalog = clsGlobalVars.SqlServerDatabase;
                    cb.UserID = clsGlobalVars.SqlServerUserId;
                    cb.Password = clsGlobalVars.SqlServerPassword;

                    m_SQLServerConnection = cb.ConnectionString;
                }
                else if (clsGlobalVars.SqlStrConnection.Length > 0)
                {
                    m_SQLServerConnection = clsGlobalVars.SqlStrConnection;
                }
                if (m_SQLServerConnection.Length > 0)
                {
                    m_ConnectionStr = m_SQLServerConnection;
                }
                else
                {
                    retValue = "\tDetection Zone Data BulkCopy: Error in generating connection to INFLO database. " +
                               "\r\n\t\tA Valid ODBC SQL Server Connection string must be specified. " +
                               "\r\n\t\tInvalid SQL Server Connection specified:= " + m_SQLServerConnection;
                    return retValue;
                }
            }

            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(m_ConnectionStr);
                sbc.DestinationTableName = "TME_TSSData_Input";
                sbc.WriteToServer(DZDataTable);
                sbc.Close();
            }
            catch (Exception ex)
            {
                retValue = "Error in BulK copy into INFLO database of TSS Detection Zone Data: " + ex.Message;
                return retValue;
            }

            return retValue;
        }

        ~clsDatabase()  
        {
            if (m_DBInterfaceType == enDBInterfaceType.enSQLServer)
            {
                try
                {
                    m_SQLDBConnection.Close();
                    if (!(m_DataSet == null))
                    {
                        m_DataSet = null;
                    }
                    if (!(m_SQLDBDataAdapter == null))
                    {
                        m_SQLDBDataAdapter = null;
                    }
                    if (!(m_SQLDBConnection == null))
                    {
                        m_SQLDBConnection = null;
                    }
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    string retValue = "SQLServer-Database Message - dispose attempt failed: " + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enODBC)
            {
                try
                {
                    if (!(m_DataSet == null))
                    {
                        m_DataSet = null;
                    }
                    if (!(m_ODBCDBDataAdapter == null))
                    {
                        m_ODBCDBDataAdapter = null;
                    }
                    if (!(m_ODBCDBConnection == null))
                    {
                        m_ODBCDBConnection = null;
                    }
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    string retValue = "ODBC-Database Message - dispose attempt failed: " + e.Message;
                    m_WasDone = false;
                }
            }
            else if (m_DBInterfaceType == enDBInterfaceType.enOLEDB)
            {
                try
                {
                    if (!(m_DataSet == null))
                    {
                        m_DataSet = null;
                    }
                    if (!(m_OLEDBDataAdapter == null))
                    {
                        m_OLEDBDataAdapter = null;
                    }
                    if (!(m_OLEDBConnection == null))
                    {
                        m_OLEDBConnection = null;
                    }
                    m_WasDone = true;
                }
                catch (Exception e)
                {
                    string retValue = "OLEDB-Database Message - dispose attempt failed: " + e.Message;
                    m_WasDone = false;
                }
            }
        }
    }
}
