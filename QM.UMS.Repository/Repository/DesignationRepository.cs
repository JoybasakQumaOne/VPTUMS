using CommonApplicationFramework.Common;
using CommonApplicationFramework.ConfigurationHandling;
using CommonApplicationFramework.DataHandling;
// -----------------------------------------------------------------------------------------------------------------
// <copyright file="DepartmentRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>20-06-2017</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------
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
      /// -----------------------------------------------------------------
    ///   Namespace:    <MFL>
    ///   Class:        <Department>
    ///   Description:  <> 
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------
    /// 


    public class DesignationRepository : IDesignationRepository, IDisposable
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
        public List<DesignationModel> GetAllDesignation()
        {
            DesignationModel DepartmentModel = new DesignationModel();
            List<DesignationModel> DepartmentModels = new List<DesignationModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DesignationQuerySettings["GetAllDesignation"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Status", true);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        DepartmentModel = new DesignationModel();
                        DepartmentModel.DesignationId = Convert.ToInt32(dr["Id"]);
                        DepartmentModel.DesignationName = dr["Designation"].ToString();
                        DepartmentModel.Status = Convert.ToBoolean(dr["Status"]);
                        DepartmentModel.Module = !string.IsNullOrEmpty(dr["ModuleId"].ToString()) ? new ItemCode() { Id = Convert.ToInt32(dr["ModuleId"]), Value = dr["Name"].ToString(), Code = dr["Code"].ToString() } : new ItemCode() { Id = 0, Value = null,Code=null };
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
        public DesignationModel GetDesignation(string code, int DesignationId)
        {
            DesignationModel DepartmentModel = new DesignationModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DesignationQuerySettings["GetDesignation"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@DesignationId", DesignationId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        DepartmentModel = new DesignationModel();
                        DepartmentModel.DesignationId = Convert.ToInt32(dr["Id"]);
                        DepartmentModel.DesignationName = dr["Name"].ToString();
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
        public int AddDesignation(string code, DesignationModel department)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string queryCheckDuplicate = QueryConfig.DesignationQuerySettings["DesignationCheckDuplicate"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@DesignationName", department.DesignationName);
                    dbManager.AddParameters(1, "@ModuleCode", department.Module.Id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, queryCheckDuplicate);
                    if (dr.Read())
                        throw new DuplicateException();
                    else
                    {
                        dbManager.CloseReader();
                        string query = QueryConfig.DesignationQuerySettings["AddDesignation"].ToString();
                        dbManager.CreateParameters(5);
                        dbManager.AddParameters(0, "@Designation", department.DesignationName);
                        dbManager.AddParameters(1, "@Status", true);
                        dbManager.AddParameters(2, "@CreatedBy", department.CreatedBy);
                        dbManager.AddParameters(3, "@CreatedOn", DateTimeOffset.Now);
                        dbManager.AddParameters(4, "@ModuleCode", department.Module.Code);
                       // dbManager.AddParameters(
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
                throw new DuplicateException("ADDDEPARTMENTNOTSUCCESS", department.DesignationName + MessageConfig.MessageSettings["ADDDEPARTMENTNOTSUCCESS"], "");
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
        public bool UpdateDesignation(string code, DesignationModel designation)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DesignationQuerySettings["UpdateDesignation"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@DepartmentId", designation.DesignationId);
                    dbManager.AddParameters(1, "@DepartmentName", designation.DesignationName);
                    dbManager.AddParameters(2, "@Status", designation.Status);
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
        public bool DeleteDesignation(string code, int departmentId)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.DesignationQuerySettings["DeleteDesignation"].ToString();
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

    

