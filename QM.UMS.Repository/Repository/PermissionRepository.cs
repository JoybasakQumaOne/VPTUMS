// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.Repository
{
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

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <PermissionRepository>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------      
    public class PermissionRepository : IPermissionRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        public string UserIPAddress { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get Permission details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of Permission</returns>
        public List<PermissionModel> GetPermission(string code)
        {
            PermissionModel permissionModel = new PermissionModel();
            List<PermissionModel> permissionModels = new List<PermissionModel>();
            try
            {
                String connectionString = GetConnectionString.GetADOConnectionString(code);
                using (dbManager = new DBManager(DataProvider.SqlServer, connectionString))
                {
                    dbManager.Open();
                    string query = QueryBuilder.GetAllPermissionInfo();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        permissionModel = new PermissionModel();
                        permissionModel.Id = Convert.ToInt32(dr["Id"]);
                        permissionModel.AccessRights = dr["Rights"].ToString();
                        permissionModel.Description = dr["Description"].ToString();
                        permissionModel.Status = Convert.ToString(dr["Status"]);
                        permissionModel.CreatedOn = DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        permissionModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"].ToString());
                        permissionModels.Add(permissionModel);
                    }
                    return permissionModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("PERMISSIONFOUNDFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("PERMISSIONFOUNDFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                permissionModel = null;
            }
        }

        /// <summary>Get Permission details by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Permission Id</param>
        /// <returns>returns single entity of permission</returns>
        public PermissionModel GetPermissionById(string code, int permissionId)
        {
            PermissionModel permissionModel = new PermissionModel();
            try
            {
                String connectionString = GetConnectionString.GetADOConnectionString(code);
                using (dbManager = new DBManager(DataProvider.SqlServer, connectionString))
                {
                    dbManager.Open();
                    string query = QueryBuilder.GetPermissionInfoById(permissionId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        permissionModel = new PermissionModel();
                        permissionModel.Id = Convert.ToInt32(dr["Id"]);
                        permissionModel.AccessRights = dr["Rights"].ToString();
                        permissionModel.Description = dr["Description"].ToString();
                        permissionModel.Status = Convert.ToString(dr["Status"]);
                        permissionModel.CreatedOn = DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        permissionModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"].ToString());
                    }
                    return permissionModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("PERMISSIONFOUNDFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("PERMISSIONFOUNDFAILED", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>Get permission by Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="roleId">Role Id</param>        
        /// <returns>returns list of permission</returns>
        public List<PermissionModel> GetPermissionByRole(string code, int roleId)
        {
            PermissionModel permissionModel = new PermissionModel();
            List<PermissionModel> permissionModels = new List<PermissionModel>();
            try
            {
                String connectionString = GetConnectionString.GetADOConnectionString(code);
                using (dbManager = new DBManager(DataProvider.SqlServer, connectionString))
                {
                    dbManager.Open();
                    string query = QueryBuilder.GetPermissionByRole(roleId, UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        permissionModel = new PermissionModel();
                        permissionModel.Id = Convert.ToInt32(dr["Id"]);
                        permissionModel.AccessRights = dr["Rights"].ToString();
                        permissionModel.Description = dr["Description"].ToString();
                        permissionModel.Status = Convert.ToString(dr["Status"]);
                        permissionModels.Add(permissionModel);
                    }
                    return permissionModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("PERMISSIONFOUNDFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("PERMISSIONFOUNDFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                permissionModels = null;
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Permission</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permission">Object of Permission Model</param>
        /// <returns>returns bool value</returns>
        public bool AddPermission(string code, PermissionModel permission)
        {
            try
            {
                bool status = false;
                String connectionString = GetConnectionString.GetADOConnectionString(code);
                using (dbManager = new DBManager(DataProvider.SqlServer, connectionString))
                {
                    dbManager.Open();
                    string query = QueryBuilder.AddPermissionDetail(permission);
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
                throw new RepositoryException("ADDPERMISSIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDPERMISSIONFAILED", ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update permission</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permission">Object of Permission Model</param>
        /// <returns>returns bool value</returns>
        public bool UpdatePermission(string code, PermissionModel permission)
        {
            try
            {
                bool status = false;
                String connectionString = GetConnectionString.GetADOConnectionString(code);
                using (dbManager = new DBManager(DataProvider.SqlServer, connectionString))
                {
                    dbManager.Open();
                    string query = QueryBuilder.UpdatePermissionDetail(permission);
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
                throw new RepositoryException("UPDATEPERMISSIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEPERMISSIONFAILED", ex.Message, ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete permission by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Permission Id</param>
        /// <returns>returns bool value</returns>
        public bool RemovePermissionInfo(string code, int permissionId)
        {
            try
            {
                bool status = false;
                String connectionString = GetConnectionString.GetADOConnectionString(code);
                using (dbManager = new DBManager(DataProvider.SqlServer, connectionString))
                {
                    dbManager.Open();
                    string query = QueryBuilder.RemovePermissionInfo(permissionId);
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
                throw new RepositoryException("DELETEPERMISSIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEPERMISSIONFAILED", ex.Message, ex.StackTrace);
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