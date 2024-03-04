// ----------------------------------------------------------------------------------------------------------------
// <copyright file="RoleRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;    
    using System.Data.SqlClient;    
    using System.Data;   
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.Common;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Models;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <RoleRepository>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class RoleRepository : IRoleRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        public string UserIPAddress { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get role details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of roles</returns>
        public List<RoleModel> GetRoles(string code, string ModuleCode)
        {
            RoleModel roleModel = new RoleModel();
            List<RoleModel> roleModels = new List<RoleModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["GetAllRoleInfo"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Status", UsersStatus.Active.ToString());
                    dbManager.AddParameters(1, "@ModuleCode", ModuleCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        roleModel = new RoleModel();
                        roleModel.Id = Convert.ToInt32(dr["Id"]);
                        roleModel.Name = dr["Name"].ToString();
                        roleModel.Description = dr["Description"].ToString();
                        roleModel.Status = Convert.ToString(dr["Status"]);
                        roleModel.CreatedOn = DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        roleModel.Creator = dr["CreatedBy"].ToString();
                        roleModel.Module = !string.IsNullOrEmpty(dr["moduleId"].ToString()) ? new ItemCode() { Id = Convert.ToInt32(dr["moduleId"]), Value = dr["ModuleName"].ToString() } : new ItemCode() { Id = 0, Value = null };                       
                        roleModels.Add(roleModel);
                    }
                    return roleModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                roleModel = null;
            }
        }

        /// <summary>/// This method is used to get role details by code and Id/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="roleId">roleId is Parameter to fetch data</param>
        /// <returns>returns single entity of role</returns>
        public RoleModel GetRoleById(string code, int roleId)
        {
            RoleModel roleModel = new RoleModel();
            try
            {
               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["GetRoleInfoById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@RoleId", roleId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        roleModel.Id = Convert.ToInt32(dr["Id"]);
                        roleModel.Name = dr["Name"].ToString();
                        roleModel.Description = dr["Description"].ToString();
                        roleModel.Status = Convert.ToString(dr["Status"]);
                        roleModel.CreatedOn = DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        roleModel.Creator = dr["CreatedBy"].ToString();
                        roleModel.Module = !string.IsNullOrEmpty(dr["moduleId"].ToString()) ? new ItemCode() { Id = Convert.ToInt32(dr["moduleId"]), Value = dr["ModuleName"].ToString() } : new ItemCode() { Id = 0, Value = null };
                       
                    }
                    return roleModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>/// This method is used to get list of roles assigned to group/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>returns list of roles</returns>
        public List<RoleModel> GetRolesAssignedToRolesGroup(string code, int groupId)
        {
            RoleModel roleModel = new RoleModel();
            List<RoleModel> roleModels = new List<RoleModel>();
            try
            {
               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["GetRolesAssignedToRolesGroup"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@GroupId", groupId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        roleModel = new RoleModel();
                        roleModel.Id = Convert.ToInt32(dr["Id"]);
                        roleModel.Name = dr["Name"].ToString();
                        roleModel.Description = dr["Description"].ToString();
                        roleModel.Status = Convert.ToString(dr["Status"]);
                        roleModel.Creator = dr["CreatedBy"].ToString();
                        roleModels.Add(roleModel);
                    }
                    return roleModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                roleModel = null;
            }
        }

        /// <summary>Get Roles By Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="status">Role Status</param>
        /// <returns>returns list of roles</returns>
        public List<RoleModel> GetRolesByStatus(string code, string status)
        {
            RoleModel roleModel = new RoleModel();
            List<RoleModel> roleModels = new List<RoleModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["GetRolesByStatus"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Status", status);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        roleModel = new RoleModel();
                        roleModel.Id = Convert.ToInt32(dr["Id"]);
                        roleModel.Name = dr["Name"].ToString();
                        roleModel.Description = dr["Description"].ToString();
                        roleModel.Status = Convert.ToString(dr["Status"]);
                      
                        roleModels.Add(roleModel);
                    }
                    return roleModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ROLEFOUNDFAILED", MessageConfig.MessageSettings["ROLEFOUNDFAILED"].ToString(), ex.StackTrace);
            }
        }
        
        #endregion

        #region POST Methods

        /// <summary>/// This method is used to create role by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">role is a object of RoleModel</param>
        /// <returns>returns bool value</returns>
        public bool AddRole(string code, RoleModel role)
        {
            try
            {
                bool status = false;                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["AddRoleDetail"].ToString();
                    dbManager.CreateParameters(6);
                    dbManager.AddParameters(0, "@Name", role.Name);
                    dbManager.AddParameters(1, "@Description", role.Description);
                    dbManager.AddParameters(2, "@Status", role.Status);
                    dbManager.AddParameters(3, "@CreatedOn", DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(4, "@CreatedBy", Convert.ToInt64(role.CreatedBy));
                    dbManager.AddParameters(5, "@Module", role.Module.Code);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDROLEFAILED", MessageConfig.MessageSettings["ADDROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDROLEFAILED", MessageConfig.MessageSettings["ADDROLEFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>This method is used to create permission of role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="id">Role Createdby Id</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="permissions">permission is object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        public bool AccessPermissionToRole(string code, CommonModel permission)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    //dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //List<int> allPermission = permission.Items.Select(x=>x.Id).ToList();
                    string query = QueryConfig.RoleQuerySettings["AccessPermissionToRole"].ToString();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@RoleId", permission.RelationalId);
                    dbManager.AddParameters(1, "@PermissionId", permission.Items.Id);
                    dbManager.AddParameters(2, "@CreatedOn", DateTime.Now);
                    dbManager.AddParameters(3, "@CreatedBy", permission.CreatedBy);                   
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        //dbManager.Transaction.Commit();
                        status = true;
                    }
                    //else
                    //{
                    //    dbManager.Transaction.Rollback();
                    //    return false;
                    //}
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                //if (dbManager.Transaction != null)
                //    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ACCESSPERMISSIONTOROLEFAILED", MessageConfig.MessageSettings["ACCESSPERMISSIONTOROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                //if (dbManager.Transaction != null)
                //    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ACCESSPERMISSIONTOROLEFAILED", MessageConfig.MessageSettings["ACCESSPERMISSIONTOROLEFAILED"].ToString(), ex.StackTrace);
            }
        }
                
        #endregion

        #region PUT Methods

        /// <summary>/// This method is used to update role by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">role is a object of RoleModel</param>
        /// <returns>returns bool value</returns>
        public bool UpdateRole(string code, RoleModel role)
        {
            try
            {
                bool status = false;               
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["UpdateRoleDetails"].ToString();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@Name", role.Name);
                    dbManager.AddParameters(1, "@Description", role.Description);
                    dbManager.AddParameters(2, "@Status", role.Status);
                    dbManager.AddParameters(3, "@RoleId", role.Id);
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
                throw new RepositoryException("UPDATEROLEFAILED", MessageConfig.MessageSettings["UPDATEROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEROLEFAILED", MessageConfig.MessageSettings["UPDATEROLEFAILED"].ToString(), ex.StackTrace);
            }
        }
        
        #endregion

        #region DELETE Methods

        /// <summary>/// This method is used to delete role by Id/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Id is Parameter to delete data</param>
        /// <returns>returns bool value</returns>
        public bool RemoveRoleInfo(string code, int roleId)
        {
            try
            {
                bool status = false;                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.RoleQuerySettings["RemoveRoleInfo"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", roleId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Deleted.ToString());
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
                throw new RepositoryException("DELETEROLEFAILED", MessageConfig.MessageSettings["DELETEROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEROLEFAILED", MessageConfig.MessageSettings["DELETEROLEFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>/// This method is used to delete permission of role/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="id">Role createdby id</param>
        /// <param name="roleId">Role id</param>
        /// <param name="permissions">permission is object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        public bool RemovePermissionFromRole(string code, CommonModel permission)
        {
            try
            {
                bool status = false;                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    //dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //List<int> allPermission = permission.Items.Select(x => x.Id).ToList();
                    string query = QueryConfig.RoleQuerySettings["RemovePermissionFromRole"].ToString();
                    dbManager.CreateParameters(6);
                    dbManager.AddParameters(0, "@RoleId", permission.RelationalId);
                    dbManager.AddParameters(1, "@PermissionId", permission.Items.Id);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        //dbManager.Transaction.Commit();
                        status = true;
                    }
                    //else
                    //{
                    //    dbManager.Transaction.Rollback();
                    //    return false;
                    //}
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                //if (dbManager.Transaction != null)
                //    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("REMOVEPERMISSIONTOROLEFAILED", MessageConfig.MessageSettings["REMOVEPERMISSIONTOROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                //if (dbManager.Transaction != null)
                //    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("REMOVEPERMISSIONTOROLEFAILED", MessageConfig.MessageSettings["REMOVEPERMISSIONTOROLEFAILED"].ToString(), ex.StackTrace);
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