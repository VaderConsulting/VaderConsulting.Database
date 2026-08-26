using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VaderConsulting.Database
{
    public class SQLServer
    {
        private string _ConnectionString = "";
        private SqlConnection _Connection = null;
        private bool _ConnectionResult = false;

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

        public SqlConnection Connection
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

        public DataTable PopulateDataTable(string Query, SqlConnection Connection)
        {
            SqlDataAdapter Adapter = null;
            DataSet QueryResults = null;
            DataTable QueryResultsTable = null;

            OpenConnection();

            Adapter = new SqlDataAdapter(Query, Connection);
            QueryResults = new DataSet("Results");

            if (QueryResults.Tables != null)
            {
                Adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                try
                {
                    Adapter.Fill(QueryResults, "Results");

                    QueryResultsTable = QueryResults.Tables["Results"];
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return QueryResultsTable;
        }

        public MemoryStream GetImage(string ObjectId)
        {
            string Query = "SELECT PagePreviewImage, Width, Height FROM VisioPagePreviews WHERE ObjectID = '" + ObjectId + "'";

            DataTable Data = PopulateDataTable(Query, _Connection);

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
            _Connection = new SqlConnection(_ConnectionString);

            if (_Connection.State == ConnectionState.Closed)
            {
                try
                {
                    _Connection.Open();
                    _ConnectionResult = true;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Print("Could not connect to SQL Server!");
                    Debug.Print(ex.InnerException.ToString());
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

        public DataSet Execute(string Query)
        {
            DataSet Results = new DataSet("Results");
            DataTable Table = new DataTable("Table1");
            SqlCommand Command = null;
            SqlDataReader Reader = null;

            if (_Connection.State == ConnectionState.Closed)
            {
                try
                {
                    _Connection.Open();
                }
                catch
                {
                    return null;
                }
            }

            Command = new SqlCommand(Query, _Connection);

            try
            {
                Reader = Command.ExecuteReader();

                if (Reader.HasRows)
                {
                    Table.Load(Reader);
                }

                Results.Tables.Add(Table);

                Command.Dispose();

                Reader.Close();
                
            }
            catch
            {
                return null;
            }

            return Results;
        }

        public void ExecuteNonQuery(string Query)
        {
            SqlCommand Command = null;

            if (_Connection.State == ConnectionState.Closed)
            {
                try
                {
                    _Connection.Open();
                }
                catch
                {
                    return;
                }
            }

            Command = new SqlCommand(Query, _Connection);

            try
            {
                Command.ExecuteNonQuery();
                Command.Dispose();
            }
            catch
            {
                return;
            }
        }
    }
}
