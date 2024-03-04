// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

#region Namespace
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
#endregion



namespace QM.UMS.Repository.Repository
{
    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ActionRepository>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------
    public class ActionRepository : IActionRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        public string UserIPAddress { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get Action Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of Action</returns>
        public List<ActionModel> GetActions(string code)
        {
            ActionModel actionModel = new ActionModel();
            List<ActionModel> actionModels = new List<ActionModel>();
            try
            {

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    String query = QueryConfig.ActionQuerySettings["GetAllActionInfo"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Status", UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        actionModel = new ActionModel();
                        actionModel.Id = Convert.ToInt32(dr["Id"]);
                        actionModel.ActionCode = dr["ActionCode"].ToString();
                        actionModel.Description = dr["Description"].ToString();
                        actionModel.Status = Convert.ToString(dr["Status"]);
                        actionModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTimeOffset(DateTime.Now) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        actionModel.CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? (int?)null : Convert.ToInt32(dr["CreatedBy"]);
                        actionModels.Add(actionModel);
                    }
                    return actionModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ACTIONFOUNDFAILED", MessageConfig.MessageSettings["ACTIONFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ACTIONFOUNDFAILED", MessageConfig.MessageSettings["ACTIONFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                actionModel = null;
            }
        }

        /// <summary>Get Action Details by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="actionId">Action Id</param>
        /// <returns>returns single entity of Action</returns>
        public ActionModel GetActionById(string code, int actionId)
        {
            ActionModel actionModel = new ActionModel();
            try
            {
                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ActionQuerySettings["GetAllActionInfoById"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@ActionId", actionId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        actionModel = new ActionModel();
                        actionModel.Id = Convert.ToInt32(dr["Id"]);
                        actionModel.ActionCode = dr["ActionCode"].ToString();
                        actionModel.Description = dr["Description"].ToString();
                        actionModel.Status = Convert.ToString(dr["Status"]);
                        actionModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTimeOffset(DateTime.Now) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        actionModel.CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? (int?)null : Convert.ToInt32(dr["CreatedBy"]);
                    }
                    return actionModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ACTIONFOUNDFAILED", MessageConfig.MessageSettings["ACTIONFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ACTIONFOUNDFAILED", MessageConfig.MessageSettings["ACTIONFOUNDFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Get Action Details by UserId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Action</returns>
        public List<ActionModel> GetActionsByUserId(string code, int userId)
        {
            ActionModel actionModel = new ActionModel();
            List<ActionModel> actionModels = new List<ActionModel>();
            try
            {
               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ActionQuerySettings["GetAllActionInfoByUserId"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", userId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        actionModel = new ActionModel();
                        actionModel.Id = Convert.ToInt32(dr["Id"]);
                        actionModel.ActionCode = dr["ActionCode"].ToString();
                        actionModel.Description = dr["Description"].ToString();
                        actionModel.Status = Convert.ToString(dr["Status"]);
                        actionModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTimeOffset(DateTime.Now) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        actionModel.CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? (int?)null : Convert.ToInt32(dr["CreatedBy"]);
                        actionModels.Add(actionModel);
                    }
                    return actionModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETACTIONBYUSERFAILED", MessageConfig.MessageSettings["GETACTIONBYUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETACTIONBYUSERFAILED", MessageConfig.MessageSettings["GETACTIONBYUSERFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                actionModel = null;
            }
        }

        /// <summary>Get Action Details by UserId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Action</returns>
        public List<ActionModel> GetActionsByRoleId(string code, int RoleId)
        {
            ActionModel actionModel = new ActionModel();
            List<ActionModel> actionModels = new List<ActionModel>();
            Item role = new Item();
            try
            {
                string query = string.Empty;
                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    query = QueryConfig.ActionQuerySettings["GetAllActioninfoByRoleId"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@RoleId", RoleId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        actionModel = new ActionModel();
                        actionModel.Id = Convert.ToInt32(dr["Id"]);
                        actionModel.ActionCode = dr["ActionCode"].ToString();
                        actionModel.Description = dr["Description"].ToString();
                        actionModel.Status = string.IsNullOrEmpty(dr["FlagId"].ToString()) ? "InActive" : "Active";
                        actionModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTimeOffset(DateTime.Now) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        actionModel.CreatedBy = string.IsNullOrEmpty(dr["CreatedBy"].ToString()) ? (int?)null : Convert.ToInt32(dr["CreatedBy"]);
                        actionModel.ModuleId = Convert.ToInt32(dr["ModuleId"]);
                        actionModel.OrderIndex = string.IsNullOrEmpty(dr["OrderIndex"].ToString()) ? (int?)null : Convert.ToInt32(dr["OrderIndex"]);
                        actionModel.ParentId = string.IsNullOrEmpty(dr["ParentId"].ToString()) ? (int?)null : Convert.ToInt32(dr["ParentId"]);
                        actionModels.Add(actionModel);
                    }
                    return actionModels.OrderBy(x => x.OrderIndex).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETACTIONBYROLEFAILED", MessageConfig.MessageSettings["GETACTIONBYROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETACTIONBYROLEFAILED", MessageConfig.MessageSettings["GETACTIONBYROLEFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                actionModel = null;
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return add status</returns>
        public bool AddAction(string code, ActionModel action)
        {
            try
            {
                bool status = false;
                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ActionQuerySettings["AddActionDetail"].ToString();
                    dbManager.CreateParameters(6);
                    dbManager.AddParameters(0, "@ActionCode", action.ActionCode);
                    dbManager.AddParameters(1, "@Description", action.Description);
                    dbManager.AddParameters(2, "@ModuleId", action.ModuleId);
                    dbManager.AddParameters(3, "@CreatedOn", DateTime.Now);
                    dbManager.AddParameters(4, "@CreatedBy", Convert.ToInt64(action.CreatedBy));
                    dbManager.AddParameters(5, "@Status", UsersStatus.Active.ToString());
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDACTIONFAILED", MessageConfig.MessageSettings["ADDACTIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDACTIONFAILED", MessageConfig.MessageSettings["ADDACTIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Assign Action To Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Common Model</param>
        /// <returns>return assign status</returns>
        public bool AssignActionToRole(string code, CommonModel action)
        {
            try
            {
                bool status = false;
                int count = 0;
                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string getQuery = QueryConfig.ActionQuerySettings["GetCountbyRoleAndAction"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", action.RelationalId);
                    dbManager.AddParameters(1, "@ActionId", action.Items.Id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, getQuery);
                    while (dr.Read())
                    {
                        count = Convert.ToInt32(dr["CountRecord"]);
                    }
                    dr.Close();
                    if (count > 0)
                    {
                        return status;
                    }                    
                    else
                    {
                        string query = QueryConfig.ActionQuerySettings["AssignActionToRole"].ToString();
                        dbManager.CreateParameters(4);
                        dbManager.AddParameters(0, "@RoleId", action.RelationalId);
                        dbManager.AddParameters(1, "@ActionId", action.Items.Id);
                        dbManager.AddParameters(2, "@CreatedOn", DateTimeOffset.Now);
                        dbManager.AddParameters(3, "@CreatedBy", action.CreatedBy);
                        int ReturnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                        if (ReturnId > 0)
                        {
                            status = true;
                        }
                    }                    
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ASSIGNACTIONTOROLEFAILED", MessageConfig.MessageSettings["ASSIGNACTIONTOROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ASSIGNACTIONTOROLEFAILED", MessageConfig.MessageSettings["ASSIGNACTIONTOROLEFAILED"].ToString(), ex.StackTrace);
            }
        }

        public List<AcessPermissionModel> GetActionPermission(string Code, int userId, List<string> CodeList)
        {
            AcessPermissionModel actionModel = new AcessPermissionModel();
            List<AcessPermissionModel> actionModels = new List<AcessPermissionModel>();
            Item role = new Item();
            try
            {
             
                string query = string.Empty;
               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    query = QueryConfig.ActionQuerySettings["GetAllActionCodePermission"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@UserId", userId);
                    StringBuilder appendcode = new StringBuilder();

                    for (int i = 0; i < CodeList.Count; i++)
                    {
                        if (i > 0)
                        {
                            appendcode.Append("," + "'" + CodeList[i] + "'");
                        }
                        else
                        {
                            appendcode.Append("'" + CodeList[i] + "'");
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        query = query + appendcode + ")";
                    }
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        actionModel = new AcessPermissionModel();
                        actionModel.Code = dr["ActionCode"].ToString();
                        actionModel.Status = true;
                        actionModels.Add(actionModel);
                    }
                    if (actionModels.Count <= 0)
                    {
                        foreach (var item in CodeList)
                        {
                            actionModel = new AcessPermissionModel();
                            actionModel.Code = item;
                            actionModel.Status = false;
                            actionModels.Add(actionModel);
                        }
                    }
                    return actionModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ASSIGNACTIONTOROLEFAILED", MessageConfig.MessageSettings["ASSIGNACTIONTOROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDACTIONFAILED", MessageConfig.MessageSettings["ASSIGNACTIONTOROLEFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return update status</returns>
        public bool UpdateAction(string code, ActionModel action)
        {
            try
            {
                bool status = false;
               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ActionQuerySettings["UpdateActionDetail"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@ActionId", action.Id);
                    dbManager.AddParameters(1, "@Description", action.Description);
                    dbManager.AddParameters(2, "@Status", action.Status);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        status = true;
                    }
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEACTIONFAILED", MessageConfig.MessageSettings["UPDATEACTIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEACTIONFAILED", MessageConfig.MessageSettings["UPDATEACTIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Action Detail By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="actionId">Action Id</param>
        /// <returns>return delete status</returns>
        public bool RemoveAction(string code, int actionId)
        {
            try
            {
                bool status = false;
                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ActionQuerySettings["RemoveActionInfo"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@ActionId", actionId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Deleted.ToString());
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEACTIONFAILED", MessageConfig.MessageSettings["DELETEACTIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEACTIONFAILED", MessageConfig.MessageSettings["DELETEACTIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Remove Action From Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Common Model</param>
        /// <returns>return remove status</returns>
        public bool RemoveActionFromRole(string code, CommonModel action)
        {
            try
            {
                bool status = false;
               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ActionQuerySettings["RemoveActionFromRole"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", action.RelationalId);
                    dbManager.AddParameters(1, "@ActionId", action.Items.Id);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        status = true;
                    }
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("REMOVEACTIONFROMROLEFAILED", MessageConfig.MessageSettings["REMOVEACTIONFROMROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("REMOVEACTIONFROMROLEFAILED", MessageConfig.MessageSettings["REMOVEACTIONFROMROLEFAILED"].ToString(), ex.StackTrace);
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
