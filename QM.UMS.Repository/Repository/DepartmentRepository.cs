using CommonApplicationFramework.Common;
using CommonApplicationFramework.ConfigurationHandling;
using CommonApplicationFramework.DataHandling;
using CommonApplicationFramework.ExceptionHandling;
using CommonApplicationFramework.Logging;
using QM.UMS.Models;
using QM.UMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.Repository
{
    public class DepartmentRepository : IDepartmentRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get list of Department Details</summary>
        /// <param name="code">code in Entity</param>
        /// <returns>Returns List of Department Details</returns>        
        public List<DepartmentModel> GetAllDepartment()
        {
            DepartmentModel DepartmentModel = new DepartmentModel();
            List<DepartmentModel> DepartmentModels = new List<DepartmentModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DepartmentQuerySettings["GetAllDepartment"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Status", true);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        DepartmentModel = new DepartmentModel();
                        DepartmentModel.DepartmentId = Convert.ToInt32(dr["Id"]);
                        DepartmentModel.DepartmentName = dr["Name"].ToString();
                        DepartmentModel.Status = Convert.ToBoolean(dr["Status"]);
                        DepartmentModel.Module = !string.IsNullOrEmpty(dr["ModuleId"].ToString()) ? new ItemCode() { Id = Convert.ToInt32(dr["ModuleId"]), Value = dr["Name"].ToString(), Code = dr["Code"].ToString() } : new ItemCode() { Id = 0, Value = null, Code = null };
                        DepartmentModels.Add(DepartmentModel);
                    }
                    this.dbManager.CloseReader();
                    return DepartmentModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETDEPARTMENTFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETDEPARTMENTFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                DepartmentModel = null;
            }
        }

        /// <summary>Get Department Details by Id</summary>
        /// <param name="code">code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>Returns single entity of Department</returns>
        public DepartmentModel GetDepartment(string code, int departmentId)
        {
            DepartmentModel DepartmentModel = new DepartmentModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DepartmentQuerySettings["GetDepartment"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@DepartmentId", departmentId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        DepartmentModel = new DepartmentModel();
                        DepartmentModel.DepartmentId = Convert.ToInt32(dr["Id"]);
                        DepartmentModel.DepartmentName = dr["Name"].ToString();
                        DepartmentModel.Status = Convert.ToBoolean(dr["Status"]);
                        DepartmentModel.Module = !string.IsNullOrEmpty(dr["ModuleId"].ToString()) ? new ItemCode() { Id = Convert.ToInt32(dr["ModuleId"]), Value = dr["Name"].ToString(), Code = dr["Code"].ToString() } : new ItemCode() { Id = 0, Value = null, Code = null };
                    }
                    this.dbManager.CloseReader();
                    return DepartmentModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETDEPARTMENTFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETDEPARTMENTFAILED", ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="dept">dept is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>        
        public int AddDepartment(string code, DepartmentModel department)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string queryCheckDuplicate = QueryConfig.DepartmentQuerySettings["DepartmentCheckDuplicate"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@DepartmentName", department.DepartmentName);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, queryCheckDuplicate);
                    if (dr.Read())
                        throw new DuplicateException();
                    else
                    {
                        dbManager.CloseReader();
                        string query = QueryConfig.DepartmentQuerySettings["AddDepartment"].ToString();
                        dbManager.CreateParameters(4);
                        dbManager.AddParameters(0, "@DepartmentName", department.DepartmentName);
                        dbManager.AddParameters(1, "@Status", true);
                        dbManager.AddParameters(2, "@CreatedBy", department.CreatedBy);
                        dbManager.AddParameters(3, "@CreatedOn", DateTimeOffset.Now);
                        object returnId = dbManager.ExecuteScalar(CommandType.Text, query);
                        return Convert.ToInt32(returnId);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDDEPARTMENTFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("ADDDEPARTMENTNOTSUCCESS", department.DepartmentName + MessageConfig.MessageSettings["ADDDEPARTMENTNOTSUCCESS"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDDEPARTMENTFAILED", ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="department">department is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>        
        public bool UpdateDepartment(string code, DepartmentModel department)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DepartmentQuerySettings["UpdateDepartment"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@DepartmentId", department.DepartmentId);
                    dbManager.AddParameters(1, "@DepartmentName", department.DepartmentName);
                    dbManager.AddParameters(2, "@Status", department.Status);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEBRANDFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEBRANDFAILED", ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>returns bool value</returns>        
        public bool DeleteDepartment(string code, int departmentId)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DepartmentQuerySettings["DeleteDepartment"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@DepartmentId", departmentId);
                    dbManager.AddParameters(1, "@Status", false);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                if (sqlEx.Number == 547)
                    throw new ConstraintsException("FOREIGNKEYCONSTRAINTS", sqlEx.Message, sqlEx.StackTrace);
                else
                    throw new RepositoryException("DELETEDEPARTMENTFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEDEPARTMENTFAILED", ex.Message, ex.StackTrace);
            }
        }

        #endregion

        /// <summary>
        /// Register Void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
