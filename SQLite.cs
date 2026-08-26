using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;

namespace VaderConsulting.Database
{
    public class SQLite
    {
        private string _ConnectionString = "";
        private System.Data.SQLite.SQLiteConnection _Connection = null;
        private bool _ConnectionResult = false;
        private int _QueryTimeout = 5;

        public SQLite(int QueryTimeout)
        {
            _QueryTimeout = QueryTimeout;
        }

        #region Properties

        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        public System.Data.SQLite.SQLiteConnection Connection
        {
            get
            {
                return _Connection;
            }
            set
            {
                _Connection = value;
            }
        }

        public bool ConnectionResult
        {
            get
            {
                return _ConnectionResult;
            }
            set
            {
                _ConnectionResult = value;
            }
        }

        #endregion

        public MemoryStream GetImage(string ObjectId)
        {
            string Query = "SELECT PagePreviewImage, Width, Height FROM VisioPagePreviews WHERE ObjectID = '" + ObjectId + "'";

            //DataTable Data = PopulateDataTable(Query, _Connection);
            DataTable Data = Execute(Query);

            MemoryStream ms = new MemoryStream((byte[])Data.Rows[0][0]);

            return ms;
        }

        public bool OpenConnection()
        {
            return OpenConnection(_ConnectionString);
        }

        public bool OpenConnection(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
            _Connection = new System.Data.SQLite.SQLiteConnection(_ConnectionString);

            if (_Connection.State == ConnectionState.Closed)
            {
                try
                {
                    _Connection.Open();
                    _ConnectionResult = true;
                    return true;
                }
                catch (System.Data.SQLite.SQLiteException e)
                {
                    Debug.Print("[ERR1218] Unexpected SqlLite Exception: " + e.ToString());
                    _ConnectionResult = false;
                    return false;
                }
                catch (Exception e)
                {
                    Debug.Print("[ERR1012] Unexpected error: " + e.ToString());
                    _ConnectionResult = false;
                    return false;
                }
            }
            return true;
        }

        public void CloseConnection()
        {
            if (_Connection != null)
            {
                _Connection.Close();
                _Connection.Dispose();
            }

            _ConnectionResult = false;
        }

        public DataTable Execute(string Query)
        {
            DataSet Results = new DataSet("Results");
            DataTable Table = new DataTable("Table1");
            System.Data.SQLite.SQLiteCommand Command = null;
            System.Data.SQLite.SQLiteDataReader Reader = null;

            if (_Connection == null)
            {
                _Connection.Open();
            }

            if (_Connection.State == ConnectionState.Closed)
            {
                try
                {
                    _Connection.Open();
                }
                catch (Exception e)
                {
                    Debug.Print("[ERR1220] SQLite Exception: " + e.ToString());
                    return null;
                }
            }

            Command = new System.Data.SQLite.SQLiteCommand(Query, _Connection);
            Command.CommandTimeout = _QueryTimeout;

            int AttemptCount = 1;

            while (AttemptCount < 4 )
            {
                try
                {
                    Reader = Command.ExecuteReader();

                    if (Reader.HasRows)
                    {
                        Table.Load(Reader);
                    }

                    Command.Dispose();

                    Reader.Close();

                    return Table;

                }
                catch (Exception e)
                {
                    if (AttemptCount < 4)
                    {
                        Debug.Print("[ERR1221] SQL error filling table (Attempt #" + AttemptCount + ").  Retrying...");

                        AttemptCount++;
                    }
                    else
                    {
                        Debug.Print("[ERR1222] SQL Exception: " + e.ToString());
                        return null;
                    }
                    
                }
            }

            return null;
        }

        public void ExecuteNonQuery(string Query)
        {
            System.Data.SQLite.SQLiteCommand Command = null;

            if (_Connection.State == ConnectionState.Closed)
            {
                try
                {
                    _Connection.Open();
                }
                catch (Exception e)
                {
                    Debug.Print("[ERR1223] SQL Exception: " + e.ToString());
                    return;
                }
            }

            Command = new System.Data.SQLite.SQLiteCommand(Query, _Connection);
            Command.CommandTimeout = _QueryTimeout;

            try
            {
                Command.ExecuteNonQuery();
                Command.Dispose();
            }
            catch (Exception e)
            {
                Debug.Print("[ERR1224] SQL Exception: " + e.ToString());
                return;
            }
        }
    }
}
