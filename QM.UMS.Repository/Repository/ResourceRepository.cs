// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>05-04-2017</createdOn>
// <comment></comment>
// --------------------------------------------------------------------------------------------------------------------

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
    using System.Configuration;
    using System.IO;
    using System.Data.OleDb;
    using System.Text.RegularExpressions;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Repository.Helper;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ResourceRepository>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class ResourceRepository : RequestHeader,IResourceRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        public string UserIPAddress { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get Resource Details by UserId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Resource</returns>
        public List<ResourceModel> GetResourcesByUserId(string code, string Module, int userId)
        {
            ResourceModel resourceModel = new ResourceModel();
            List<ResourceModel> resourceModels = new List<ResourceModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["GetAllResouceByUserId"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Module", Module);
                    dbManager.AddParameters(1, "@UserId", userId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        resourceModel = new ResourceModel();
                        resourceModel.Id = Convert.ToInt32(dr["Id"]);
                        resourceModel.Module = Convert.ToInt32(dr["ModuleId"]);
                        resourceModel.ResourceCode = dr["ResourceCode"].ToString();
                        resourceModel.ResourceURL = dr["ResourceURL"].ToString();
                        resourceModel.ResourceType = !string.IsNullOrEmpty(dr["ResourceType"].ToString()) ? new Item() { Id = Convert.ToInt32(dr["ResourceType"]), Value = dr["ResourceTypeName"].ToString() } : new Item() { Id = 0, Value = null };
                        resourceModel.CreatedOn = DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        resourceModel.Creator = dr["Name"].ToString();
                        resourceModel.Description = dr["Description"].ToString();
                        resourceModels.Add(resourceModel);
                    }
                    return resourceModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETRESOURCEBYUSERFAILED", MessageConfig.MessageSettings["GETRESOURCEBYUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETACTIONBYUSERFAILED", MessageConfig.MessageSettings["GETRESOURCEBYUSERFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                resourceModel = null;
            }
        }

        public List<string> GetResourcesByUser(string uid)
        {
            string Str = string.Empty;
            List<string> stringlist = null;
            try
            {
                using (dbManager = new DBManager())
                {
                    stringlist = new List<string>();
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    LogManager.Log("UserId "+Context?.UserId.ToString());
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["GetResourceByUserId"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@UserId", uid);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        Str = dr["ResourceCode"].ToString();
                        stringlist.Add(Str);
                    }
                }
                return stringlist;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETRESOURCEBYUSERFAILED", "GETRESOURCEBYUSERFAILED", sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETRESOURCEBYUSERFAILED", "GETRESOURCEBYUSERFAILED", ex.StackTrace);
            }
            finally
            {
                Str = null;
            }
        }

        /// <summary>Get Resource Details by Module</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">Module</param>
        /// <returns>returns list of Resource</returns>
        public List<ResourceModel> GetAllResourcesByModuleId(string code, string Module)
        {
            ResourceModel resourceModel = new ResourceModel();
            List<ResourceModel> resourceModels = new List<ResourceModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["GetAllResourceByModule"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Module", Module);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        resourceModel = new ResourceModel();
                        resourceModel.Id = Convert.ToInt32(dr["Id"]);
                        resourceModel.Module = Convert.ToInt32(dr["ModuleId"]);
                        resourceModel.ResourceCode = dr["ResourceCode"].ToString();
                        resourceModel.ResourceURL = dr["ResourceURL"].ToString();
                        resourceModel.ResourceType = !string.IsNullOrEmpty(dr["ResourceType"].ToString()) ? new Item() { Id = Convert.ToInt32(dr["ResourceType"]), Value = dr["ResourceTypeName"].ToString() } : new Item() { Id = 0, Value = null };
                        resourceModel.CreatedOn = DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        resourceModel.Creator = dr["Name"].ToString();

                        resourceModels.Add(resourceModel);
                    }
                    return resourceModels;
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
                resourceModels = null;
            }
        }

        /// <summary>/// Get Resources By Id /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="Id">Resource Id</param>
        /// <returns>returns single entity of resource</returns>
        public ResourceModel GetResourceById(string Code, int Id)
        {
            ResourceModel resourceModel = new ResourceModel();
            Item resourceType = new Item();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.GetDBConnectionString(Code);
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["GetResourceInfoById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@ResourceId", Id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        resourceModel = new ResourceModel();
                        resourceModel.Id = Convert.ToInt32(dr["Id"]);
                        resourceModel.Module = Convert.ToInt32(dr["ModuleId"]);
                        resourceModel.ResourceCode = dr["ResourceCode"].ToString();
                        resourceType = new Item();
                        resourceType.Id = Convert.ToInt32(dr["ResourceTypeId"]);
                        resourceType.Value = dr["ResourceType"].ToString();
                        resourceModel.ResourceType = resourceType;
                        resourceModel.ResourceURL = dr["ResourceURL"].ToString();
                        resourceModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        resourceModel.Creator = dr["CreatedBy"].ToString();
                        resourceModel.Description = dr["Description"].ToString();
                    }
                    return resourceModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("RESOURCEFOUNDFAILED", MessageConfig.MessageSettings["RESOURCEFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("RESOURCEFOUNDFAILED", MessageConfig.MessageSettings["RESOURCEFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                resourceModel = null;
            }
        }

        /// <summary> /// Get resources by roleid and modulecode /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="roleId">roleId</param>
        /// <param name="moduleCode">moduleCode</param>
        /// <returns>returns list of resources</returns>
        public List<ResourceModel> GetResourcesByRole(string Code, int roleId, string moduleCode)
        {
            ResourceModel resourceModel = new ResourceModel();
            List<ResourceModel> resourceModels = new List<ResourceModel>();
            Item resourceType = new Item();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.GetDBConnectionString(Code);
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["GetResourcesByRole"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", roleId);
                    dbManager.AddParameters(1, "@Module", moduleCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        resourceModel = new ResourceModel();
                        resourceModel.Id = Convert.ToInt32(dr["Id"]);
                        resourceModel.Module = Convert.ToInt32(dr["ModuleId"]);
                        resourceModel.ResourceCode = dr["ResourceCode"].ToString();
                        resourceModel.Description = dr["Description"].ToString();
                        resourceType = new Item();
                        resourceType.Id = Convert.ToInt32(dr["ResourceTypeId"]);
                        resourceType.Value = dr["ResourceType"].ToString();
                        resourceModel.ResourceType = resourceType;
                        resourceModel.ResourceURL = dr["ResourceURL"].ToString();
                        resourceModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        //resourceModel.Creator = dr["CreatedBy"].ToString();
                        resourceModel.Description = dr["Description"].ToString();
                        resourceModel.Status = string.IsNullOrEmpty(dr["FlagId"].ToString()) ? "InActive" : "Active";
                        resourceModel.OrderIndex = string.IsNullOrEmpty(dr["OrderIndex"].ToString()) ? (int?)null : Convert.ToInt32(dr["OrderIndex"]);
                        resourceModel.ParentId = string.IsNullOrEmpty(dr["ParentId"].ToString()) ? (int?)null : Convert.ToInt32(dr["ParentId"]);
                        resourceModels.Add(resourceModel);
                    }                   
                    return resourceModels.OrderBy(x => x.Id).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("RESOURCEFOUNDFAILED", MessageConfig.MessageSettings["RESOURCEFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("RESOURCEFOUNDFAILED", MessageConfig.MessageSettings["RESOURCEFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                resourceModel = null;
            }
        }
        
        #endregion

        #region POST Methods

        /// <summary>/// Add Resources/// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of ResourceModel</param>
        /// <returns>returns true or false</returns>
        public bool AddResource(string code, ResourceModel resource)
        {
            try
            {
                bool status = false;              
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["AddResource"].ToString();
                    dbManager.CreateParameters(7);
                    dbManager.AddParameters(0, "@Module", resource.Module);
                    dbManager.AddParameters(1, "@ResourceCode", resource.ResourceCode);
                    dbManager.AddParameters(2, "@ResourceType", resource.ResourceType.Id);
                    dbManager.AddParameters(3, "@ResourceURL", resource.ResourceURL);
                    dbManager.AddParameters(4, "@CreatedOn", DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(5, "@CreatedBy", Convert.ToInt32(resource.CreatedBy));
                    dbManager.AddParameters(6, "@Description", Convert.ToInt32(resource.Description));
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDRESOURCEFAILED", MessageConfig.MessageSettings["ADDRESOURCEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDRESOURCEFAILED", MessageConfig.MessageSettings["ADDRESOURCEFAILED"].ToString(), ex.StackTrace);
            }

        }

        /// <summary>/// Add Resources to Role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of CommonModel</param>
        /// <returns>returns true or false</returns>
        public bool AddResourceToRole(string Code, CommonModel resource)
        {
            try
            {
                bool status = false;
                int count = 0;                
                using (dbManager = new DBManager())
                {
                    dbManager.GetDBConnectionString(Code);
                    dbManager.Open();
                    string getQuery = QueryConfig.ResourceQuerySettings["GetCountByRoleAndResource"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", resource.RelationalId);
                    dbManager.AddParameters(1, "@ResourceId", resource.Items.Id);
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
                        string query = QueryConfig.ResourceQuerySettings["AddResourceToRole"].ToString();
                        dbManager.CreateParameters(4);
                        dbManager.AddParameters(0, "@RoleId", resource.RelationalId);
                        dbManager.AddParameters(1, "@CreatedOn", DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                        dbManager.AddParameters(2, "@CreatedBy", Convert.ToInt64(resource.CreatedBy));
                        dbManager.AddParameters(3, "@ResourceId", resource.Items.Id);
                        int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                        if (returnId > 0)
                        {
                            status = true;
                        }
                    }
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDRESOURCESTOROLEFAILED", MessageConfig.MessageSettings["ADDRESOURCESTOROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDRESOURCESTOROLEFAILED", MessageConfig.MessageSettings["ADDRESOURCESTOROLEFAILED"].ToString(), ex.StackTrace);
            }
        }
        
        #endregion

        #region PUT Methods

        /// <summary>/// Update Resources/// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of ResourceModel</param>
        /// <returns>returns true or false</returns>
        public bool UpdateResource(string code, ResourceModel resource)
        {
            try
            {
                bool status = false;                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["UpdateResource"].ToString();
                    dbManager.CreateParameters(6);
                    dbManager.AddParameters(0, "@ResourceId", resource.Id);
                    dbManager.AddParameters(1, "@Module", resource.Module);
                    dbManager.AddParameters(2, "@ResourceCode", resource.ResourceCode);
                    dbManager.AddParameters(3, "@ResourceType", resource.ResourceType.Id);
                    dbManager.AddParameters(4, "@ResourceURL", resource.ResourceURL);                      
                    dbManager.AddParameters(5, "@Description", Convert.ToInt32(resource.Description));
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
                throw new RepositoryException("UPDATERESOURCEFAILED", MessageConfig.MessageSettings["UPDATERESOURCEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATERESOURCEFAILED", MessageConfig.MessageSettings["UPDATERESOURCEFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary> /// Remove resources from role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="resourceId">Resource Id</param>
        /// <returns>returns true or false</returns>
        public bool RemoveResourceFromRole(string Code, int roleId, int resourceId)
        {
            try
            {
                bool status = false;                
                using (dbManager = new DBManager())
                {
                    dbManager.GetDBConnectionString(Code);
                    dbManager.Open();
                    string query = QueryConfig.ResourceQuerySettings["RemoveResourceFromRole"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", roleId);
                    dbManager.AddParameters(1, "@ResourceId", resourceId);
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
                throw new RepositoryException("DELETERESOURCEFAILED", MessageConfig.MessageSettings["DELETERESOURCEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETERESOURCEFAILED", MessageConfig.MessageSettings["DELETERESOURCEFAILED"].ToString(), ex.StackTrace);
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