using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace QM.eBook.DMS.DBConnection
{
    public enum DataProvider
    {
        SqlServer, OleDb, Odbc, Oracle
    }

    public class DBhandler : IDisposable
    {
        #region DB Variables
        private IDbConnection idbConnection;
        private IDataReader idataReader;
        private IDbCommand idbCommand;
        private DataProvider providerType;
        private IDbTransaction idbTransaction = null;
        private IDbDataParameter[] idbParameters = null;
        private string strConnection;
        #endregion

        #region DB Paramters

        public DBhandler()
        {
            this.providerType = DataProvider.SqlServer;
            //this.strConnection = connectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                return this.idbConnection;
            }
        }

        public IDataReader DataReader
        {
            get
            {
                return this.idataReader;
            }

            set
            {
                this.idataReader = value;
            }
        }

        public DataProvider ProviderType
        {
            get
            {
                return this.providerType;
            }

            set
            {
                this.providerType = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.strConnection;
            }

            set
            {
                this.strConnection = value;
            }
        }

        public IDbCommand Command
        {
            get
            {
                return this.idbCommand;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return this.idbTransaction;
            }
            set
            {
                this.idbTransaction = value;
            }
        }

        public IDbDataParameter[] Parameters
        {
            get
            {
                return this.idbParameters;
            }
        }

        public void Open()
        {
            this.idbConnection = new SqlConnection();
            this.idbConnection.ConnectionString = this.ConnectionString;
            if (this.idbConnection.State != ConnectionState.Open)
            {
                this.idbConnection.Open();
            }
            this.idbCommand = new SqlCommand();
        }

        public void Close()
        {
            if (this.idbConnection != null)
            {
                if (this.idbConnection.State != ConnectionState.Closed)
                {
                    this.idbConnection.Close();
                }
            }
        }

        public void CreateParameters(int paramsCount)
        {
            this.idbParameters = new IDbDataParameter[paramsCount];
            IDbDataParameter[] idbParams = new IDbDataParameter[paramsCount];
            for (int i = 0; i < paramsCount; ++i)
            {
                idbParams[i] = new SqlParameter();
            }
            this.idbParameters = idbParams;
        }

        public void AddParameters(int index, string paramName, object objValue)
        {
            if (index < this.idbParameters.Length)
            {
                this.idbParameters[index].ParameterName = paramName;
                if (objValue != null)
                {
                    this.idbParameters[index].Value = objValue;
                }
                else
                {
                    this.idbParameters[index].Value = DBNull.Value;
                }
            }
        }

        public void BeginTransaction()
        {
            if (this.idbTransaction == null)
            {
                this.idbTransaction = new SqlConnection().BeginTransaction();
            }

            this.idbCommand.Transaction = this.idbTransaction;
        }

        public void CommitTransaction()
        {
            if (this.idbTransaction != null)
            {
                this.idbTransaction.Commit();
            }

            this.idbTransaction = null;
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            this.idbCommand = new SqlCommand();
            this.idbCommand.Connection = this.Connection;
            this.PrepareCommand(this.idbCommand, this.Connection, this.Transaction, commandType, commandText, this.Parameters);
            this.DataReader = this.idbCommand.ExecuteReader();
            this.idbCommand.Parameters.Clear();
            return this.DataReader;
        }

        public void CloseReader()
        {
            if (this.DataReader != null)
            {
                this.DataReader.Close();
            }
        }

        private void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDbDataParameter[] commandParameters)
        {
            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }
        }

        private void AttachParameters(IDbCommand command, IDbDataParameter[] commandParameters)
        {
            foreach (IDbDataParameter idbParameter in commandParameters)
            {
                if ((idbParameter.Direction == ParameterDirection.InputOutput) && (idbParameter.Value == null))
                {
                    idbParameter.Value = DBNull.Value;
                }

                command.Parameters.Add(idbParameter);
            }
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            this.idbCommand = new SqlCommand();
            this.PrepareCommand(this.idbCommand, this.Connection, this.Transaction, commandType, commandText, this.Parameters);
            int returnValue = this.idbCommand.ExecuteNonQuery();
            this.idbCommand.Parameters.Clear();
            return returnValue;
        }

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            this.idbCommand = new SqlCommand();
            this.PrepareCommand(this.idbCommand, this.Connection, this.Transaction, commandType, commandText, this.Parameters);
            object returnValue = this.idbCommand.ExecuteScalar();
            this.idbCommand.Parameters.Clear();
            return returnValue;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Close();
            this.idbCommand = null;
            this.idbTransaction = null;
            this.idbConnection = null;
        }

        //public IDbConnection Connection { get { return this.idbConnection; } }

        //public IDbCommand Command { get { return this.idbCommand; } }

        //private string connectionString = string.Empty;

        //public string connectionString1 = string.Empty;

        //private string queryContent = string.Empty;

        //private int parameterCount = 0;

        //private List<Parameter> listParameters;

        ////private IDbTransaction idbTransaction { get { return this.idbTransaction; } set { this.idbTransaction = value; } }
        //#endregion

        //public SqlConnection SqlConnection()
        //{
        //    SqlConnection sqlConnection = new SqlConnection();
        //    if (!string.IsNullOrEmpty(this.idbConnection.ConnectionString))
        //    {
        //        sqlConnection = new SqlConnection(this.idbConnection.ConnectionString);
        //    }
        //    return sqlConnection;
        //}

        //public SqlCommand sqlCommand(string queryString)
        //{
        //    SqlCommand command = new SqlCommand(queryString, this.SqlConnection());
        //    return command;
        //}

        //public void AddQueryParameter(List<Parameter> parameters, string pointer, object objectValue)
        //{
        //    var command = sqlCommand(null);
        //    if (parameters.Count > 0)
        //    {
        //        foreach(Parameter value in parameters)
        //        {
        //            command.Parameters.AddWithValue(value.Key, value.Value);
        //        }
        //    }
        //    //command.Parameters.AddWithValue(pointer, objectValue);
        //}

        //public void open()
        //{

        //    this.SqlConnection().Open();
        //}

        ////public void Transaction()
        ////{
        ////    if(this.idbTransaction == null)
        ////    {
        ////        this.idbTransaction = this.idbConnection.BeginTransaction();
        ////    }
        ////}

        //#region Dispose
        //public void Dispose()
        //{
        //    GC.SuppressFinalize(this);
        //}
        #endregion
    }
}
