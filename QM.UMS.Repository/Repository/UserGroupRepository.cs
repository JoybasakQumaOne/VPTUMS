// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="UserGroupRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Globalization;
    using System.Data;
    using System.Configuration;
    using System.Data.SqlClient;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using CommonApplicationFramework.ConfigurationHandling;
    using System.Text;
    using CommonApplicationFramework.Common;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Models;
    using CommonApplicationFramework.Caching;
    using Newtonsoft.Json;
    using QM.UMS.Repository.Helper;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserGroupRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UserGroupRepository : RequestHeader, IUserGroupRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        public string UserIPAddress { get; set; }
        private DBManager dbManager;
        #endregion

        #region Properties
        public string RequestId { get; set; }
        public string Code { get; set; }
        #endregion

        #region GET Methods

        /// <summary>Get Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Group Details</returns>
        public List<UserGroupModel> GetAllGroup(string ModuleCode)
        {
            Item groupType = new Item();
            UserGroupModel userGroupModel = new UserGroupModel();
            List<UserGroupModel> userGroupModels = new List<UserGroupModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["GetAllUserGroupInfo"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Status", UsersStatus.Active.ToString());
                    dbManager.AddParameters(1, "@ModuleCode", ModuleCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userGroupModel = new UserGroupModel(); groupType = new Item();
                        userGroupModel.Id = Convert.ToInt32(dr["Id"]);
                        userGroupModel.Name = dr["Name"].ToString();
                        userGroupModel.Description = dr["Description"].ToString();
                        userGroupModel.Status = Convert.ToString(dr["Status"]);
                        userGroupModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        userGroupModel.Creator = dr["CreatedBy"].ToString();
                        groupType.Id = Convert.ToInt32(dr["GroupTypeId"]);
                        groupType.Value = dr["GroupType"].ToString();
                        //userGroupModel.Module = dr["Code"].ToString();
                        userGroupModel.Module = Convert.ToString(0);
                        userGroupModel.GroupType = groupType;
                        userGroupModels.Add(userGroupModel);
                    }
                    return userGroupModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                groupType = null;
                userGroupModel = null;
            }
        }

        /// <summary>Get Group Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Group Type</returns>
        public UserGroupModel GetGroupById(string code, int groupId)
        {
            Item groupType = new Item(); List<int> users = new List<int>(); List<int> roles = new List<int>(); List<int> folders = new List<int>();
            UserGroupModel userGroupModel = new UserGroupModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["GetAllUserGroupInfoById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@GroupId", groupId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        userGroupModel = new UserGroupModel(); groupType = new Item();
                        userGroupModel.Id = Convert.ToInt32(dr["Id"]);
                        userGroupModel.Name = dr["Name"].ToString();
                        userGroupModel.Description = dr["Description"].ToString();
                        userGroupModel.Status = Convert.ToString(dr["Status"]);
                        userGroupModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        userGroupModel.Creator = (dr["CreatedBy"].ToString());
                        groupType.Id = Convert.ToInt32(dr["GroupId"]);
                        groupType.Value = dr["GroupType"].ToString();
                        userGroupModel.GroupType = groupType;
                        dr.Close();
                        string userQuery = QueryConfig.UserGroupQuerySettings["getUserByGroup"].ToString();
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@GroupId", groupId);
                        IDataReader dr1 = dbManager.ExecuteReader(CommandType.Text, userQuery);
                        while (dr1.Read())
                        {
                            users.Add(Convert.ToInt32(dr1["User_Id"]));
                        }
                        dr1.Close();
                        string roleQuery = QueryConfig.UserGroupQuerySettings["getRoleByGroup"].ToString(); ;
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@GroupId", groupId);
                        IDataReader dr2 = dbManager.ExecuteReader(CommandType.Text, roleQuery);
                        while (dr2.Read())
                        {
                            roles.Add(Convert.ToInt32(dr2["Role_Id"]));
                        }
                        dr2.Close();
                        string folderQuery = QueryConfig.UserGroupQuerySettings["GetGroupFolderByGroup"].ToString(); ;
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@GroupId", groupId);
                        IDataReader dr3 = dbManager.ExecuteReader(CommandType.Text, folderQuery);
                        while (dr3.Read())
                        {
                            folders.Add(Convert.ToInt32(dr3["FolderId"]));
                        }
                        dr3.Close();

                        userGroupModel.Users = users.Distinct().ToList();
                        userGroupModel.Folders = folders.Distinct().ToList();
                        userGroupModel.Roles = roles.Distinct().ToList();
                    }
                    return userGroupModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                groupType = null;
                users = null;
                roles = null;
            }
        }

        /// <summary>Get Group Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Single Entity of Group Details</returns>
        public List<Item> GetAllGroupType(string code)
        {
            Item groupTypeModel = new Item();
            List<Item> groupTypeModels = new List<Item>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["GetAllGroupType"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        groupTypeModel = new Item();
                        groupTypeModel.Id = Convert.ToInt32(dr["Id"]);
                        groupTypeModel.Value = dr["GroupType"].ToString();

                        groupTypeModels.Add(groupTypeModel);
                    }
                    return groupTypeModels.OrderBy(x => x.Value).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GROUPTYPEFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GROUPTYPEFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                groupTypeModel = null;
            }
        }

        public int GetRolelinkedtoGroup(string code, int roleId, int groupId)
        {
            int Id = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@roleId", roleId);
                    dbManager.AddParameters(1, "@groupId", groupId);
                    string query = QueryConfig.UserGroupQuerySettings["GetRoleLinkedtoGroup"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        Id = Convert.ToInt32(dr["Id"]);
                    }

                }
                return Id;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GROUPTYPEFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GROUPTYPEFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
        }

        public int GetUserLinkedtoGroup(string code, int userId, int groupId)
        {
            int Id = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@userId", userId);
                    dbManager.AddParameters(1, "@groupId", groupId);
                    string query = QueryConfig.UserGroupQuerySettings["GetUserLinkedtoGroup"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        Id = Convert.ToInt32(dr["Id"]);
                    }

                }
                return Id;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GROUPTYPEFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GROUPTYPEFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Get All Group Details By Type and Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Status">Status Type</param>
        /// <param name="groupTypeId">Group Type Id</param>
        /// <returns>List of Group Details</returns>
        public List<UserGroupModel> GetAllGroupByTypeStatus(string code, string Status, string Type)
        {
            Item groupType = new Item();
            UserGroupModel userGroupModel = new UserGroupModel();
            List<UserGroupModel> userGroupModels = new List<UserGroupModel>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["GetAllGroupByTypeStatus"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Status", Status);
                    dbManager.AddParameters(1, "@Type", Convert.ToInt32(Type));
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userGroupModel = new UserGroupModel(); groupType = new Item();
                        userGroupModel.Id = Convert.ToInt32(dr["Id"]);
                        userGroupModel.Name = dr["Name"].ToString();
                        userGroupModel.Description = dr["Description"].ToString();
                        userGroupModel.Status = Convert.ToString(dr["Status"]);
                        userGroupModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        groupType.Id = Convert.ToInt32(dr["GroupId"]);
                        groupType.Value = dr["GroupType"].ToString();
                        userGroupModel.GroupType = groupType;
                        userGroupModel.Module = dr["Modulename"].ToString();
                        userGroupModel.Creator = (dr["CreatedBy"].ToString());
                        userGroupModels.Add(userGroupModel);
                    }
                }
                return userGroupModels.OrderBy(x => x.Name).ToList();
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                groupType = null;
                userGroupModel = null;
            }
        }

        public List<Item> GetAllLinkedAttribute(int groupId)
        {
            Item attribute = new Item();
            List<Item> attributes = new List<Item>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["GetAllLinkedAttributeByGroupId"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@GroupId", groupId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        attribute = new Item();
                        attribute.Id = Convert.ToInt32(dr["AttributeId"]);
                        attribute.Value = dr["Label"].ToString();

                        attributes.Add(attribute);
                    }
                    return attributes.OrderBy(x => x.Value).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETATTRIBUTEFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETATTRIBUTEFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                attribute = null;
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Insert Status</returns>
        public bool AddGroup(string code, UserGroupModel userGroup)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    string query = QueryConfig.UserGroupQuerySettings["AddUserGroupDetail"].ToString();
                    dbManager.CreateParameters(7);
                    dbManager.AddParameters(0, "@Name", userGroup.Name);
                    dbManager.AddParameters(1, "@Description", userGroup.Description);
                    dbManager.AddParameters(2, "@Status", UsersStatus.Active.ToString());
                    dbManager.AddParameters(3, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                    dbManager.AddParameters(4, "@CreatedBy", Convert.ToInt32(userGroup.CreatedBy));
                    dbManager.AddParameters(5, "@GroupId", userGroup.GroupType.Id);
                    dbManager.AddParameters(6, "@Module", userGroup.Module);
                    object groupId = dbManager.ExecuteScalar(CommandType.Text, query);
                    if (userGroup.Users != null && userGroup.Users.Count > 0)
                    {
                        StringBuilder generateQuery = new StringBuilder();
                        string queryUser = QueryConfig.UserGroupQuerySettings["AddUserToGroup"].ToString();
                        for (int i = 0; i < userGroup.Users.Count; i++)
                        {
                            dbManager.CreateParameters(4);
                            dbManager.AddParameters(0, "@UserId", userGroup.Users[i]);
                            dbManager.AddParameters(1, "@GroupId", Convert.ToInt32(groupId));
                            dbManager.AddParameters(2, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                            dbManager.AddParameters(3, "@CreatedBy", userGroup.CreatedBy);
                            dbManager.ExecuteNonQuery(CommandType.Text, queryUser);
                        }
                        //string queryUser = QueryBuilder.AddUserToUserGroup(userGroup, Convert.ToInt32(groupId));
                        //dbManager.ExecuteNonQuery(CommandType.Text, queryUser);
                    }
                    if (userGroup.Roles != null && userGroup.Roles.Count > 0)
                    {
                        StringBuilder generateQuery = new StringBuilder();
                        string queryRole = QueryConfig.UserGroupQuerySettings["AddRoleToGroup"].ToString();
                        for (int i = 0; i < userGroup.Roles.Count; i++)
                        {
                            dbManager.CreateParameters(4);
                            dbManager.AddParameters(0, "@GroupId", Convert.ToInt32(groupId));
                            dbManager.AddParameters(1, "@RoleId", userGroup.Roles[i]);
                            dbManager.AddParameters(2, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                            dbManager.AddParameters(3, "@CreatedBy", userGroup.CreatedBy);
                            dbManager.ExecuteNonQuery(CommandType.Text, queryRole);
                        }
                        //string queryRole = QueryBuilder.AddRoleToUserGroup(userGroup, Convert.ToInt32(groupId));
                        //dbManager.ExecuteNonQuery(CommandType.Text, queryRole);
                    }
                    if (userGroup.Folders != null && userGroup.Folders.Count > 0)
                    {
                        StringBuilder generateQuery = new StringBuilder();
                        string queryFolder = QueryConfig.UserGroupQuerySettings["AddFolderToGroup"].ToString();
                        for (int i = 0; i < userGroup.Folders.Count; i++)
                        {
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@GroupId", Convert.ToInt32(groupId));
                            dbManager.AddParameters(1, "@FolderId", userGroup.Folders[i]);
                            dbManager.ExecuteNonQuery(CommandType.Text, queryFolder);
                        }
                        //string queryFolder = QueryBuilder.AddFolderToGroup(userGroup, Convert.ToInt32(groupId));
                        //dbManager.ExecuteNonQuery(CommandType.Text, queryFolder);
                    }
                    //DBAdminLogManager Start Here
                    //DBAdminLogManager logManager = new DBAdminLogManager();
                    //AdminLogsManager log = new AdminLogsManager();                    
                    //log.UserId = Convert.ToInt32(UserId);
                    //log.Module = userGroup.Module;
                    //log.Activity = (int)ActivityLog.ADD_GROUP;
                    //log.Message = "Group name: " + userGroup.Name + "added successfully.";
                    //log.IPAddress = UserIPAddress;
                    //logManager.AdminLogManager(code, log);
                    //DbAdminLogManager End Here
                    dbManager.Transaction.Commit();
                    return true;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERGROUPFAILED", MessageConfig.MessageSettings["ADDUSERGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERGROUPFAILED", MessageConfig.MessageSettings["ADDUSERGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Add User To Group</summary>
        /// <param name="code">Code in Entity</param>  
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        public bool AddUserToGroup(string code, CommonModel user)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //List<int> allUsers = user.Items.Select(x => x.Id).ToList();
                    string query = QueryConfig.UserGroupQuerySettings["AddUserToGroup"].ToString();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@UserId", user.Items.Id);
                    dbManager.AddParameters(1, "@GroupId", user.RelationalId);
                    dbManager.AddParameters(2, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                    dbManager.AddParameters(3, "@CreatedBy", user.CreatedBy);
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
                    //DBAdminLogManager Start Here
                    //DBAdminLogManager logManager = new DBAdminLogManager();
                    //AdminLogsManager log = new AdminLogsManager();
                    //string name = string.Empty, employeeId = string.Empty, groupName=string.Empty;
                    //string queryProfile = QueryConfig.UserGroupQuerySettings["GetEmployeeIdNameGroupName"].ToString();
                    //dbManager.CreateParameters(2);
                    //dbManager.AddParameters(0, "@UserId", user.Items.Id);
                    //dbManager.AddParameters(1, "@GroupId", user.RelationalId);
                    //dbManager.CloseReader();
                    //IDataReader drLog = dbManager.ExecuteReader(CommandType.Text, queryProfile);
                    //if (drLog.Read())
                    //{
                    //    name = drLog["Name"].ToString();
                    //    employeeId = drLog["EmployeeId"].ToString();
                    //    groupName = drLog["GroupName"].ToString();
                    //} drLog.Close();
                    //log.UserId = Convert.ToInt32(UserId);
                    //log.Module = "Mod_001";
                    //log.Activity = (int)ActivityLog.UPDATE_USER;
                    //log.Message = "Add Employee Name: " + name + "[" + employeeId + "]  to Group: " + groupName + "[" + user.RelationalId + "] done successfully.";
                    //log.IPAddress = UserIPAddress;
                    //logManager.AdminLogManager(code, log);
                    //DbAdminLogManager End Here
                    dbManager.Transaction.Commit();
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", MessageConfig.MessageSettings["ADDUSERTOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", MessageConfig.MessageSettings["ADDUSERTOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Add Role To Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        public bool AddRoleToGroup(string code, CommonModel role)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //List<int> allRoles = role.Items.Select(x => x.Id).ToList();
                    string query = QueryConfig.UserGroupQuerySettings["AddRoleToGroup"].ToString();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@GroupId", role.RelationalId);
                    dbManager.AddParameters(1, "@RoleId", role.Items.Id);
                    dbManager.AddParameters(2, "@CreatedBy", role.CreatedBy);
                    dbManager.AddParameters(3, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
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
                    //DBAdminLogManager Start Here
                    //DBAdminLogManager logManager = new DBAdminLogManager();
                    //AdminLogsManager log = new AdminLogsManager();
                    //string groupName = string.Empty, roleName = string.Empty;
                    //string queryProfile = QueryConfig.UserGroupQuerySettings["GetGroupNameRoleName"].ToString();
                    //dbManager.CreateParameters(2);
                    //dbManager.AddParameters(0, "@GroupId", role.RelationalId);
                    //dbManager.AddParameters(1, "@RoleId", role.Items.Id);
                    //dbManager.CloseReader();
                    //IDataReader drLog = dbManager.ExecuteReader(CommandType.Text, queryProfile);
                    //if (drLog.Read())
                    //{
                    //    roleName = drLog["EmployeeId"].ToString();
                    //    groupName = drLog["GroupName"].ToString();
                    //} drLog.Close();
                    //log.UserId = Convert.ToInt32(UserId);
                    //log.Module = "Mod_001";
                    //log.Activity = (int)ActivityLog.UPDATE_USER;
                    //log.Message = "Add Role: " + roleName + "[" + role.Items.Id + "]  to Group: " + groupName + "[" + role.RelationalId + "] done successfully.";
                    //log.IPAddress = UserIPAddress;
                    //logManager.AdminLogManager(code, log);
                    //DbAdminLogManager End Here
                    dbManager.Transaction.Commit();
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDROLETOGROUPFAILED", MessageConfig.MessageSettings["ADDROLETOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDROLETOGROUPFAILED", MessageConfig.MessageSettings["ADDROLETOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Remove User From Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Remove Status</returns>    
        public bool RemoveUserFromGroup(string code, CommonModel user)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    //dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //List<int> allUsers = user.Items.Select(x => x.Id).ToList();
                    string query = QueryConfig.UserGroupQuerySettings["RemoveUserFromGroup"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", user.Items.Id);
                    dbManager.AddParameters(1, "@GroupId", user.RelationalId);
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
                throw new RepositoryException("DELETEUSERTOGROUPFAILED", MessageConfig.MessageSettings["DELETEUSERTOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEUSERTOGROUPFAILED", MessageConfig.MessageSettings["DELETEUSERTOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Remove Role From Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Common Entity</param>
        /// <returns>Remove Status</returns>
        public bool RemoveRoleFromGroup(string code, CommonModel role)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    //dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //List<int> allRoles = role.Items.Select(x => x.Id).ToList();
                    string query = QueryConfig.UserGroupQuerySettings["RemoveRoleFromGroup"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@GroupId", role.RelationalId);
                    dbManager.AddParameters(1, "@RoleId", role.Items.Id);
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
                throw new RepositoryException("DELETEROLETOGROUPFAILED", MessageConfig.MessageSettings["DELETEROLETOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEROLETOGROUPFAILED", MessageConfig.MessageSettings["DELETEROLETOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Update Status</returns>
        public bool UpdateGroup(string code, UserGroupModel userGroup)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["UpdateUserGroupDetail"].ToString();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@Name", userGroup.Name);
                    dbManager.AddParameters(1, "@Description", userGroup.Description);
                    dbManager.AddParameters(2, "@GroupType", userGroup.GroupType.Id);
                    dbManager.AddParameters(3, "@Module", userGroup.Module);
                    dbManager.AddParameters(4, "@GroupId", userGroup.Id);
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
                throw new RepositoryException("UPDATEUSERGROUPFAILED", MessageConfig.MessageSettings["UPDATEUSERGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEUSERGROUPFAILED", MessageConfig.MessageSettings["UPDATEUSERGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>Deactivate Group details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Deactivate Status</returns>
        public bool RemoveGroup(string code, int groupId)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["RemoveUserGroupInfo"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@GroupId", groupId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Inactive.ToString());
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
                throw new RepositoryException("DELETEUSERFAILED", MessageConfig.MessageSettings["DELETEUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEUSERFAILED", MessageConfig.MessageSettings["DELETEUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion


        #region LinkAttributeToGroup
        public bool LinkGroupAttribute(int groupId, int attributeId)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (groupId > 0 || attributeId > 0)
                    {
                        using (dbManager = new DBManager())
                        {
                            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                            dbManager.Open();
                            string duplicateChkquery = QueryConfig.UserGroupQuerySettings["DuplicateGroupAttributeMap"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@GroupId", groupId);
                            dbManager.AddParameters(1, "@AttributeId", attributeId);
                            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateChkquery);
                            if (dr.Read())
                            {
                                dr.Close();
                                throw new DuplicateException();
                            }
                            else
                            {
                                dr.Close();
                                string userQuery = QueryConfig.UserGroupQuerySettings["GroupAttributeMap"].ToString();
                                dbManager.CreateParameters(2);
                                dbManager.AddParameters(0, "@GroupId", groupId);
                                dbManager.AddParameters(1, "@AttributeId", attributeId);
                                int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, userQuery);
                                if (rowAffacted > 0)
                                    return true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GROUPATTRIBUTEMAPPINGFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", "Mapping " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GROUPATTRIBUTEMAPPINGFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region DelinkAttributeToGroup
        public bool DelinkGroupAttribute(int groupId, int attributeId)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (groupId > 0 || attributeId > 0)
                    {
                        using (dbManager = new DBManager())
                        {
                            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                            dbManager.Open();
                            string userQuery = QueryConfig.UserGroupQuerySettings["RemoveGroupAttributeMap"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@GroupId", groupId);
                            dbManager.AddParameters(1, "@AttributeId", attributeId);
                            int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, userQuery);
                            if (rowAffacted > 0)
                                return true;
                            return false;

                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GROUPATTRIBUTEMAPPINGFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GROUPATTRIBUTEMAPPINGFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion


        #region LinkGroupToUser
        public bool LinkGroupToUser(int groupId, int userId)  
        {
            try
            {
                UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString()));
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (groupId > 0 || userId > 0)
                    {
                        using (dbManager = new DBManager())
                        {
                            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                            dbManager.Open();
                            string duplicateChkquery = QueryConfig.UserGroupQuerySettings["DuplicateGroupUserMap"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@GroupId", groupId);
                            dbManager.AddParameters(1, "@UserId", userId);
                            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateChkquery);
                            if (dr.Read())
                            {
                                dr.Close();
                                throw new DuplicateException();
                            }
                            else
                            {
                                dr.Close();
                                string query = QueryConfig.UserGroupQuerySettings["AddUserToGroup"].ToString();
                                dbManager.CreateParameters(4);
                                dbManager.AddParameters(0, "@GroupId", groupId);
                                dbManager.AddParameters(1, "@UserId", userId); 
                                dbManager.AddParameters(2, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz")); 
                                dbManager.AddParameters(3, "@CreatedBy", userContext.UserId);
                                int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, query);
                                if (rowAffacted > 0)
                                    return true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", "Mapping " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region DelinkGroupToUser
        public bool DelinkGroupToUser(int groupId, int userId)  
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (groupId > 0 || userId > 0)
                    {
                        using (dbManager = new DBManager())
                        {
                            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                            dbManager.Open();
                            string userQuery = QueryConfig.UserGroupQuerySettings["RemoveUserFromGroup"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@GroupId", groupId);
                            dbManager.AddParameters(1, "@UserId", userId);
                            int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, userQuery);
                            if (rowAffacted > 0)
                                return true;
                            return false;

                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEUSERTOGROUPFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEUSERTOGROUPFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        /// <summary>
        /// Register void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public object GetUserGroups(string userId)
        {
            object data = null;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.GenerateConnectionString(ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserGroupQuerySettings["GetLinkedUserGroups"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@userId", userId);
                    dbManager.AddParameters(1, "@App", Context.ApplicationType?.Code);
                    DataTable dt = new DataTable();
                    dt.Load(dbManager.ExecuteReader(CommandType.Text, query));
                    if (dt.Rows.Count > 0)
                    {
                        data = dt.AsEnumerable().Select(n => new {
                            Id = ConvertData.ToInt(n["Id"]),
                            Name = ConvertData.ToString(n["Name"]),
                            Description = ConvertData.ToString(n["Description"]),
                            Code = ConvertData.ToString(n["Code"]),
                            Users = ConvertData.ToString(n["UserCount"]),
                            Guid = ConvertData.ToString(n["Guid"]),
                            Linked = !string.IsNullOrEmpty(ConvertData.ToString(n["LinkedGroup"])) ? ConvertData.ToInt(n["LinkedGroup"]) > 0 ? true : false : false
                        }).ToList();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERGROUPFOUNDFAILED", MessageConfig.MessageSettings["USERGROUPFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            return data;
        }

        public bool LinkUserGroup(string gId, string uId)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.GenerateConnectionString(ConnectionId);
                    dbManager.Open();
                    string query = string.Empty;
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@UserId", uId);
                    dbManager.AddParameters(1, "@GroupId", gId);
                    dbManager.AddParameters(2, "@CreatedBy", Context.UserId);
                    query = QueryConfig.UserGroupQuerySettings["AddUserToGroupCode"].ToString();
                    dbManager.ExecuteNonQuery(CommandType.Text, query);
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", MessageConfig.MessageSettings["ADDUSERTOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", MessageConfig.MessageSettings["ADDUSERTOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        public bool DelinkUserGroup(string gId, string uId)
        {
            bool status = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.GenerateConnectionString(ConnectionId);
                    dbManager.Open();
                    string query = string.Empty;
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", uId);
                    dbManager.AddParameters(1, "@GroupId", gId);
                    query = QueryConfig.UserGroupQuerySettings["RemoveUserFromGroupCode"].ToString();
                    if (dbManager.ExecuteNonQuery(CommandType.Text, query) > 0)
                    {
                        status = true;
                    }
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", MessageConfig.MessageSettings["ADDUSERTOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERTOGROUPFAILED", MessageConfig.MessageSettings["ADDUSERTOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }
    }
}