// ----------------------------------------------------------------------------------------------------------------
// <copyright file="UserRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

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
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.Common;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Models;
    using CommonApplicationFramework.Caching;
    using Newtonsoft.Json;
    using QM.UMS.Repository.Helper;
    using System.Drawing.Text;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UsersRepository : RequestHeader, IUsersRepository, IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        public string UserIPAddress { get; set; }
        private DBManager dbManager;
        private string extendTime;
        #endregion

        #region Properties
        public string RequestId { get; set; }
        public string Code { get; set; }
        #endregion

        #region Constructor
        public UsersRepository()
        {
            extendTime = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ExtendTime")).Value.ToString();
        }
        #endregion

        #region GET Methods


        /// <summary>Get User Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>Single Entity of User Details</returns>
        public UserProfileModel GetControlUser(string userId)
        {
            UserProfileModel userModel = null;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetControlUserByEmail"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", userId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        userModel = new UserProfileModel();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.FirstName = dr["FirstName"].ToString();
                        userModel.Email = dr["Email"].ToString();
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModel.Salt = dr["Salt"].ToString();
                        userModel.Password = dr["Password"].ToString();
                        //userModel.ExpiresOn =!string.IsNullOrEmpty(ConvertData.ToString(dr["ExpiresOn"]))? Convert.ToDateTime(dr["ExpiresOn"].ToString()):new DateTime();
                        userModel.IsFirstLogin = ConvertData.ToBoolean(dr["IsFirstLogin"]);
                        userModel.UserId = userId;
                    }
                    return userModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERNOTFOUND", MessageConfig.MessageSettings["USERNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERNOTFOUND", MessageConfig.MessageSettings["USERNOTFOUND"].ToString(), ex.StackTrace);
            }
        }

        public UserProfileModel GetContentUserDetails(string userId)
        {
            UserProfileModel userModel = null;
            try
            {
                string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + ConfigurationManager.AppSettings["ProjectName"].ToString() + "/" + "UserImage" + "/";

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string  getUserQuery = QueryConfig.UserQuerySettings["GetContentUserDetails"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Code", userId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, getUserQuery);
                    if (dr.Read())
                    {
                        userModel = new UserProfileModel();
                        userModel.Name = dr["Name"].ToString();
                        userModel.FirstName = dr["FirstName"].ToString();
                        userModel.LastName = dr["LastName"].ToString();
                        userModel.Email = dr["Email"].ToString();
                        //userModel.Phone = dr["PhoneNumber"].ToString();
                        userModel.Address1 = dr["Address1"].ToString();
                        userModel.Address2 = dr["Address2"].ToString();
                        userModel.City = dr["City"].ToString();
                        userModel.State = dr["State"].ToString();
                        userModel.Country = new ItemCode()
                        {
                            Id = ConvertData.ToInt(dr["countryId"]),
                            Code = ConvertData.ToString(dr["countryCode"]),
                            Value = ConvertData.ToString(dr["countryName"]),
                        };
                        //userModel.Country = dr["Country"].ToString();
                        userModel.Zipcode = dr["Zipcode"].ToString();
                        userModel.Status = dr["Status"].ToString();
                        userModel.ExpiresOn = string.IsNullOrEmpty(dr["ExpiresOn"].ToString()) ? new DateTimeOffset() : DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModel.JobProfile = dr["JobProfile"].ToString();
                        userModel.Accomplishments = dr["Accomplishments"].ToString();
                        userModel.OrderIndex = string.IsNullOrEmpty(dr["DisplayOrderIndex"].ToString()) ? (int?)null : Convert.ToInt32(dr["DisplayOrderIndex"]);
                        userModel.UserId = UserId;
                    }
                    return userModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Get User Details By Module and Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="moduleCode">Module Code</param>
        /// <param name="status">User status</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUserByStatus(string code, string moduleCode, string status)
        {
            AttachmentFile userImage = new AttachmentFile();
            UserProfileModel userModel = new UserProfileModel();
            List<UserProfileModel> userModels = new List<UserProfileModel>();
            try
            {
                UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString()));
                string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + userContext.Org.Guid + "/" + "UserImage" + "/";
                

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUserByStatus"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Status", status);
                    dbManager.AddParameters(1, "@ModuleCode", moduleCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userModel = new UserProfileModel(); userImage = new AttachmentFile();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.FirstName = dr["FirstName"].ToString();
                        userModel.LastName = dr["LastName"].ToString();
                        userModel.Email = dr["Email"].ToString();
                        userModel.Phone = dr["Phone"].ToString();
                        userModel.Address1 = dr["Address1"].ToString();
                        userModel.Address2 = dr["Address2"].ToString();
                        userModel.City = dr["City"].ToString();
                        userModel.State = dr["State"].ToString();
                      //  userModel.Country = dr["Country"].ToString();
                        userModel.Zipcode = dr["Zipcode"].ToString();
                        userModel.Status = dr["Status"].ToString();
                        userModel.ExpiresOn = string.IsNullOrEmpty(dr["ExpiresOn"].ToString()) ? new DateTimeOffset() : DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModel.JobProfile = dr["JobProfile"].ToString();
                        userModel.Accomplishments = dr["Accomplishments"].ToString();
                        userModel.OrderIndex = string.IsNullOrEmpty(dr["DisplayOrderIndex"].ToString()) ? (int?)null : Convert.ToInt32(dr["DisplayOrderIndex"]);
                        userImage.FileName = string.IsNullOrEmpty(dr["Image"].ToString()) ? null : filePath + dr["Image"].ToString();
                        if (string.IsNullOrEmpty(userImage.FileName))
                        {
                            userImage.FileName = filePath + "usericon.jpg";
                        }
                        userModels.Add(userModel);
                    }
                    return userModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                userImage = null;
                userModel = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="moduleCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<UserProfileModel> GetUserDetails(bool isSuperUser, string status)    
        {
            AttachmentFile userImage = new AttachmentFile();
            UserProfileModel userModel = new UserProfileModel();
            List<UserProfileModel> userModels = new List<UserProfileModel>();
            string getUserQuery = string.Empty;
            try
            {
                //UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString()));
                string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + ConfigurationManager.AppSettings["ProjectName"].ToString() + "/" + "UserImage" + "/";

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    if(!string.IsNullOrEmpty(status))
                        getUserQuery = QueryConfig.UserQuerySettings["GetUserDetails"].ToString();
                    else
                        getUserQuery = QueryConfig.UserQuerySettings["GetAllUserDetails"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@IsSuperUser", isSuperUser);
                    dbManager.AddParameters(1, "@Status", status);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, getUserQuery);
                    while (dr.Read())
                    {
                        userModel = new UserProfileModel(); userImage = new AttachmentFile();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.FirstName = dr["FirstName"].ToString();
                        userModel.LastName = dr["LastName"].ToString();
                        userModel.Email = dr["Email"].ToString();
                       //userModel.Phone = dr["PhoneNumber"].ToString();
                        userModel.Address1 = dr["Address1"].ToString();
                        userModel.Address2 = dr["Address2"].ToString();
                        userModel.City = dr["City"].ToString();
                        userModel.State = dr["State"].ToString();
						userModel.Country = new ItemCode()
						{
							Id = ConvertData.ToInt(dr["countryId"]),
							Code = ConvertData.ToString(dr["countryCode"]),
							Value = ConvertData.ToString(dr["countryName"]),
						};
                        //userModel.Country = dr["Country"].ToString();
                        userModel.Zipcode = dr["Zipcode"].ToString();
                        userModel.Status = dr["Status"].ToString();
                        userModel.ExpiresOn = string.IsNullOrEmpty(dr["ExpiresOn"].ToString()) ? new DateTimeOffset() : DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModel.JobProfile = dr["JobProfile"].ToString();
                        userModel.Accomplishments = dr["Accomplishments"].ToString();
                        userModel.OrderIndex = string.IsNullOrEmpty(dr["DisplayOrderIndex"].ToString()) ? (int?)null : Convert.ToInt32(dr["DisplayOrderIndex"]);
                        userImage.FileName = string.IsNullOrEmpty(dr["Image"].ToString()) ? null : filePath + dr["Image"].ToString();
                        if (string.IsNullOrEmpty(userImage.FileName))
                        {
                            userImage.FileName = filePath + "usericon.jpg";
                        }
                        userModels.Add(userModel);
                    }
                    return userModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                userImage = null;
                userModel = null;
            }
        }

        public List<CompanyDetails> GetAgencyDetail(string username)
        {
            List<CompanyDetails> organizations = new List<CompanyDetails>(); //R:: change variable name organizations to companies
            CompanyDetails organization = new CompanyDetails();
            try
            {
                //string controlDBConnectionString = ConfigurationManager.ConnectionStrings["ControlDB"].ConnectionString;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string query = QueryConfig.UserQuerySettings["GetUsersCompany"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Username", username);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        organization = new CompanyDetails();
                        organization.Id = Convert.ToInt32(dr["Id"]);
                        organization.Name = dr["CompanyName"].ToString();
                        organization.Code = dr["Code"].ToString();
                        organization.AuthMode = dr["AuthMode"].ToString();
                        organization.CompanyId = dr["CompanyId"].ToString();
                        organization.DomainName = dr["DomainName"].ToString();
                        organization.ActiveDirectoryURL = dr["ActiveDirectoryURL"].ToString();
                        organization.ADUserId = dr["ADUserId"].ToString();
                        organization.ADPassword = dr["ADPassword"].ToString();
                        organizations.Add(organization);
                    }
                    return organizations;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETCOMPANYFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETCOMPANYFAILED", ex.Message, ex.StackTrace);
            }
        }
        public dynamic GetOrgAdminUsers(Guid orgGuid)  
        {
            AttachmentFile userImage = new AttachmentFile();
            UserProfileModel userModel = new UserProfileModel();
            dynamic userModels = new List<object>();
            try
            {
                string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + "/" + "UserImage" + "/";
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = string.Empty;
                    query = QueryConfig.UserQuerySettings["GetUserDetailsByOrg"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@OrgGUID", orgGuid);
                    dbManager.AddParameters(1, "@App", Context.ApplicationType?.Code);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userModels.Add(new {
                        UserId = Convert.ToString(dr["UserId"]),
                        cUserId = Convert.ToString(dr["ControlUserCode"]),
                        Name = dr["Name"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Email = dr["Email"].ToString(),
                        Status = dr["Status"].ToString(),
                        Phone = ConvertData.ToString(dr["PhoneNumber"]),
                        path = string.IsNullOrEmpty(dr["Image"].ToString()) ? null : filePath + ConvertData.ToString(dr["Image"])
                    });
                    }
                    return userModels;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", "Logged in cannot be deactivated", sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", "Logged in cannot be deactivated", ex.StackTrace);
            }
            finally
            {
                userImage = null;
                userModel = null;
            }
        }

        /// <summary>Get User Details of Expired Account</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="expiryDate">User Expiry Date</param>
        /// <param name="moduleId">Module Id</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUsersOfExpiredAccount(string code, DateTimeOffset expiryDate, string moduleId)
        {
            AttachmentFile userImage = new AttachmentFile();
            UserProfileModel userModel = new UserProfileModel();
            List<UserProfileModel> userModels = new List<UserProfileModel>();
            try
            {
                string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + code + "/" + "USERIMAGE" + "/";

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUsersOfExpiredAccount"].ToString();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@Today", DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(1, "@ExpiryDate", expiryDate.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(2, "@Status", UsersStatus.Expired.ToString());
                    dbManager.AddParameters(3, "@Module", moduleId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userModel = new UserProfileModel(); userImage = new AttachmentFile();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.FirstName = dr["FirstName"].ToString();
                        userModel.LastName = dr["LastName"].ToString();
                        userModel.Email = dr["Email"].ToString();
                        userModel.Phone = dr["Phone"].ToString();
                        userModel.Address1 = dr["Address1"].ToString();
                        userModel.Address2 = dr["Address2"].ToString();
                        userModel.City = dr["City"].ToString();
                        userModel.State = dr["State"].ToString();
                       // userModel.Country = dr["Country"].ToString();
                        userModel.Zipcode = dr["Zipcode"].ToString();
                        userModel.Status = dr["Status"].ToString();
                        userModel.ExpiresOn = DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModel.JobProfile = dr["JobProfile"].ToString();
                        userModel.Accomplishments = dr["Accomplishments"].ToString();
                        //userModel.OrderIndex = Convert.ToInt32(dr["DisplayOrderIndex"]);
                        userImage.FileName = string.IsNullOrEmpty(dr["Image"].ToString()) ? null : filePath + dr["Image"].ToString();
                        userModels.Add(userModel);
                    }
                    return userModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                userImage = null;
                userModel = null;
            }
        }

        /// <summary>Get Users From User Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUsersFromGroup(string code, int groupId)
        {
            UserProfileModel userModel = new UserProfileModel();
            List<UserProfileModel> userModels = new List<UserProfileModel>();
            try
            {

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUsersAssignedtoUsersGroup"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@GroupId", groupId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userModel = new UserProfileModel();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.Status = dr["Status"].ToString();
                        userModel.ExpiresOn = DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModels.Add(userModel);
                    }
                    return userModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                userModel = null;
            }
        }

        /// <summary>Get Users From Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="roleId">Role Id</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUsersFromRole(string code, int roleId)
        {
            UserProfileModel userModel = new UserProfileModel();
            List<UserProfileModel> userModels = new List<UserProfileModel>();
            try
            {

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUsersAssignedToRole"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@RoleId", roleId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Active.ToString());
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userModel = new UserProfileModel();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.Status = dr["Status"].ToString();
                        userModel.ExpiresOn = DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModels.Add(userModel);
                    }
                    return userModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERFOUNDFAILED", MessageConfig.MessageSettings["USERFOUNDFAILED"].ToString(), ex.StackTrace);
            }
            finally
            {
                userModel = null;
            }
        }

        /// <summary>Forgot Password : Get User Detail By Email</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>User Detail</returns>
        public UserModel GetUserDetailsByEmail(string emailId)
        {
            UserModel user = new UserModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUserDetailsByEmail"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", emailId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        user.Id = Convert.ToInt32(dr["Id"]);
                        user.Name = dr["Name"].ToString();
                        user.FirstName = dr["FirstName"].ToString();
                        user.Email = dr["Email"].ToString();
                        user.Salt = dr["Salt"].ToString();
                        user.Status = dr["Status"].ToString();
                        user.UserGuid = dr["ControlUserCode"].ToString();
                    }

                    return user;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Change Password : Get user details by emailId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">User emailId</param>
        /// <returns>Single Entity of User</returns>
        public UserProfileModel GetUserByEmail(string emailId)
        {
            UserProfileModel userModel = new UserProfileModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUserByEmail"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", emailId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        userModel = new UserProfileModel();
                        userModel.Id = Convert.ToInt32(dr["Id"]);
                        userModel.Name = dr["Name"].ToString();
                        userModel.FirstName = dr["FirstName"].ToString();
                        userModel.Email = dr["Email"].ToString();
                        userModel.Creator = dr["CreatedBy"].ToString();
                        userModel.Salt = dr["Salt"].ToString();
                        userModel.Password = dr["Password"].ToString();
                        userModel.ExpiresOn = Convert.ToDateTime(dr["ExpiresOn"].ToString());
                        userModel.IsFirstLogin = Convert.ToBoolean(dr["IsFirstLogin"]);
                    }
                    return userModel;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERNOTFOUND", MessageConfig.MessageSettings["USERNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERNOTFOUND", MessageConfig.MessageSettings["USERNOTFOUND"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Reset Password : Get User By Salt</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="passwordResetCode">User Salt</param>
        /// <returns>User Details</returns>
        public UserModel GetUserDetailsByPasswordResetCode(string passwordResetCode)
        {
            UserModel user = new UserModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                   // passwordresetcode = passwordresetcode.Replace(" ", "+");
                    string query = QueryConfig.UserQuerySettings["GetUserDetailsByPasswordResetCode"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@passwordresetcode", passwordResetCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        user.Id = Convert.ToInt32(dr["Id"].ToString());
                        user.Name = dr["Name"].ToString();
                        user.Email = dr["Email"].ToString();
                        user.FirstName = dr["FirstName"].ToString();
                        user.ExpiresOn =!string.IsNullOrEmpty(ConvertData.ToString(dr["ExpiresOn"]))? DateTimeOffset.Parse(dr["ExpiresOn"].ToString()): new DateTimeOffset();
						user.Salt = passwordResetCode; //dr["Salt"].ToString();
						user.Status = dr["Status"].ToString();
						user.IsFirstLogin = Convert.ToBoolean(dr["IsFirstLogin"].ToString());
						if (Convert.ToInt32(dr["IsSuperUser"]) == 1)
							user.CompanyCode = "BOOK";
						else
							user.CompanyCode = "BOOK";
					}
                    return user;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERNOTFOUND", MessageConfig.MessageSettings["USERNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERNOTFOUND", MessageConfig.MessageSettings["USERNOTFOUND"].ToString(), ex.StackTrace);
            }
        }

        public DBModel GetCompanyInfo(string code)
        {
            DBModel company = new DBModel();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);//DBSServers.DBServerList.Find(x => x.Name.Equals(ConfigurationManager.AppSettings["HostName"].ToString())).ConnectionString;                   
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetCompanyInfo"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Code", code);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        company = new DBModel();
                        company.Id = Convert.ToInt32(dr["Id"]);
                        company.CompanyName = dr["CompanyName"].ToString();
                        company.Code = dr["Code"].ToString();
                        company.DBServerName = dr["DBServerName"].ToString();
                        company.DBUserName = dr["DBUserName"].ToString();
                        company.DBName = dr["DBName"].ToString();
                        company.DBPassword = dr["DBPassword"].ToString();
                        company.AuthMode = dr["AuthMode"].ToString();
                    }
                    return company;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETCOMPANYFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETCOMPANYFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        public DBModel GetCompanyInfo()
        {
            DBModel company = new DBModel();
            try
            {
                string context = CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString());
                UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString()));
                DBInstance dbInstance = userContext.InstanceList.Find(x => x.Code.Trim().Equals(userContext.Org.Guid));
                company.CompanyName = dbInstance.Name;
                company.Code = dbInstance.Code;
                company.DBName = dbInstance.DBName;
                company.DBServerName = dbInstance.DBServer;
                company.DBUserName = dbInstance.DBUserName;
                company.DBPassword = dbInstance.DBPassword;
                return company;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETCOMPANYFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETCOMPANYFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        public CompanyModuleModel GetCompanyModuleInfo()
        {
            using (dbManager = new DBManager())
            {
                CompanyModuleModel moduleModel = null;
                dbManager.ConnectionString = DBSServers.DBServerList.Find(x => x.Name.Equals(ConfigurationManager.AppSettings["HostName"].ToString())).ConnectionString;
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@CompanyModuleId", ConnectionId);
                IDataReader dr = dbManager.ExecuteReader(CommandType.Text, QueryConfig.ModuleQuerySettings["GetCompanyModuleInfo"].ToString());
                while (dr.Read())
                {
                    moduleModel = new CompanyModuleModel()
                    {
                        ActiveDirectoryUrl = Convert.ToString(dr["ActiveDirectoryUrl"]),
                        ADConnection = Convert.ToString(dr["ADConnection"]),
                        AuthMode = Convert.ToString(dr["AuthMode"]),
                        Code = Convert.ToString(dr["Code"]),
                        CompanyName = Convert.ToString(dr["CompanyName"]),
                        DBName = Convert.ToString(dr["DBName"]),
                        DBPassword = Convert.ToString(dr["DBPassword"]),
                        DBServerName = Convert.ToString(dr["DBServerName"]),
                        DBUserName = Convert.ToString(dr["DBUserName"]),
                        DomainName = Convert.ToString(dr["DomainName"]),
                        Id = Convert.ToInt32(dr["Id"])
                    };
                }
                return moduleModel;
            }
        }


        public int IfUserExistsInApplicationDb(string email)
        {
            using (dbManager = new DBManager())
            {
                dbManager.GenerateConnectionString(ConnectionId);
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@UserName", email);
                IDataReader dr = dbManager.ExecuteReader(CommandType.Text, QueryConfig.UserQuerySettings["VerifyUser"].ToString());
                int userId = 0;
                if (dr.Read())
                {
                    userId = Convert.ToInt32(dr["Id"]);
                }
                return userId;
            }
        }

        public string IfUserExistsInControlDb(string email)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = DBSServers.DBServerList.Find(x => x.Name.Equals(ConfigurationManager.AppSettings["HostName"].ToString())).ConnectionString;
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", email);
                    string userCode = Convert.ToString(dbManager.ExecuteScalar(CommandType.Text, QueryConfig.UserQuerySettings["IfUserExistsInControlDb"].ToString()));
                    return userCode;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("VERIFYUSERFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("VERIFYUSERFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        public int CreateApplicationUser(UserProfileModel applicationUser, string controlUserCode, int companyId=0)
        {
            string userGuid = Guid.NewGuid().ToString();
            using (dbManager = new DBManager())
            {
                try
                {
                    dbManager.GenerateConnectionString(ConnectionId);
                    dbManager.Open();
                    dbManager.Transaction = dbManager.Connection.BeginTransaction();
                    dbManager.CreateParameters(15);
                    dbManager.AddParameters(0, "@Username", applicationUser.UserId);
                    dbManager.AddParameters(1, "@Status", UsersStatus.Active.ToString());
                    dbManager.AddParameters(2, "@ExpiryOn", applicationUser.ExpiresOn.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(3, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(4, "@CreatedBy", null);
                    dbManager.AddParameters(5, "@IsFirstLogin", true);
                    dbManager.AddParameters(6, "@controlUserCode", controlUserCode);
                    dbManager.AddParameters(7, "@Email", applicationUser.Email);
                    dbManager.AddParameters(8, "@UserType", applicationUser?.UserType?.Code);
                    dbManager.AddParameters(9, "@OrganizationType", null);
                    dbManager.AddParameters(10, "@UserId", controlUserCode);
                    dbManager.AddParameters(11, "@Guid", controlUserCode);
                    dbManager.AddParameters(12, "@FirstName", applicationUser.FirstName);
                    dbManager.AddParameters(13, "@LastName", applicationUser.LastName);
                    dbManager.AddParameters(14, "@Phone", applicationUser.Phone);
                    int userId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, QueryConfig.UserQuerySettings["AddUserDetail"].ToString()));
                    if (CreateApplicationUserProfile(applicationUser, userId, companyId, controlUserCode))
                    {
                        if (applicationUser.Organisations!=null && !string.IsNullOrEmpty(applicationUser.Organisations[0].ToString()))
                        {
                            string queryUserInstance = QueryConfig.UserQuerySettings["LinkUserToOrganisation"].ToString();
                            dbManager.CreateParameters(3);
                            dbManager.AddParameters(0, "@UserId", Convert.ToInt32(userId));
                            dbManager.AddParameters(1, "@OrgGUID", applicationUser.Organisations[0].ToString());
                            dbManager.AddParameters(2, "@isAdmin", applicationUser.IsAdmin);
                            if ( dbManager.ExecuteNonQuery(CommandType.Text, queryUserInstance)>0)
                            {
                                dbManager.Transaction.Commit();
                                return userId;
                            }
                            else
                            {
                                dbManager.Transaction.Rollback();
                                return 0;
                            }
                        }
                        else
                        {
                            dbManager.Transaction.Commit();
                            return userId;
                        }
                    }
                    else
                    {
                        dbManager.Transaction.Rollback();
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    dbManager.Transaction.Rollback();
                    LogManager.Log(ex);
                    throw new RepositoryException("ADDUSERFAILED", MessageConfig.MessageSettings["ADDUSERFAILED"].ToString(), ex.StackTrace);
                }
            }
        }

        private bool CreateApplicationUserProfile(UserProfileModel user, int applicationUserId, int companyId, string controlUserCode)
        {
            dbManager.CreateParameters(22);
            dbManager.AddParameters(0, "@UserId", applicationUserId);
            dbManager.AddParameters(1, "@FirstName", user.FirstName);
            dbManager.AddParameters(2, "@LastName", user.LastName);
            dbManager.AddParameters(3, "@Email", user.Email);
            dbManager.AddParameters(4, "@Phone", user.Phone);
            dbManager.AddParameters(5, "@Address1", user.Address1);
            dbManager.AddParameters(6, "@Address2", user.Address2);
            dbManager.AddParameters(7, "@City", user.City);
            dbManager.AddParameters(8, "@State", user.State);
            dbManager.AddParameters(9, "@Country", user.Country);
            dbManager.AddParameters(10, "@Zipcode", user.Zipcode);
            dbManager.AddParameters(11, "@FileName", controlUserCode + ".png");
            dbManager.AddItemParameters(12, "@ReportingTo", user.ReportingTo);
            dbManager.AddParameters(13, "@TotalYrsExp", user.TotalYrsExp);
            dbManager.AddParameters(14, "@DOB", user.DOB);
            dbManager.AddParameters(15, "@Hobbies", user.Hobbies);
            dbManager.AddParameters(16, "@JoiningDate", user.JoiningDate);
            dbManager.AddItemParameters(17, "@Department", user.Department);
            dbManager.AddItemParameters(18, "@Designation", user.Designation);
            dbManager.AddParameters(19, "@EmployeeId", user.EmployeeId);
            dbManager.AddParameters(20, "@JobProfile", user.JobProfile);
            dbManager.AddParameters(21, "@Accomplishments", user.Accomplishments);
            //dbManager.AddParameters(22, "@UserType", user.UserType!=null ? user.UserType.Id : (int?)null);
            //dbManager.AddParameters(23, "@OrganizationType", user.OrganizationType!=null ? user.OrganizationType.Id : (int?)null);
            int isInserted = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, QueryConfig.UserQuerySettings["AddUserProfileDetail"].ToString()));
            if (isInserted > 0)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        public int CreateControlUser(UserProfileModel userProfileModel, int companyId)
        {
            using (dbManager = new DBManager())
            {
                dbManager.ConnectionString = DBSServers.DBServerList.Find(x => x.Name.Equals(ConfigurationManager.AppSettings["HostName"].ToString())).ConnectionString;
                dbManager.Open();
                string query = string.Empty;
                dbManager.CreateParameters(16);
                query = QueryConfig.UserQuerySettings["CreateControlUser"].ToString();
                dbManager.AddParameters(0, "@Email", userProfileModel.Email);
                dbManager.AddParameters(1, "@Password", userProfileModel.Password);
                dbManager.AddParameters(2, "@Status", UsersStatus.Active.ToString());
                dbManager.AddParameters(3, "@ExpiresOn", userProfileModel.ExpiresOn.ToString("yyyy-MM-dd hh:mm:ss"));
                dbManager.AddParameters(4, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                dbManager.AddParameters(5, "@CreatedBy", ConvertData.ToInt(userProfileModel.CreatedBy));
                dbManager.AddParameters(6, "@Salt", userProfileModel.Salt);
                dbManager.AddParameters(7, "@IsFirstLogin", false);
                dbManager.AddParameters(8, "@FirstName", userProfileModel.FirstName);
                dbManager.AddParameters(9, "@LastName", userProfileModel.LastName);
                dbManager.AddParameters(10, "@Phone", userProfileModel.Phone);
                dbManager.AddParameters(11, "@Image", userProfileModel.ControlUserCode + ".png");
                dbManager.AddParameters(12, "@ControlUserCode", userProfileModel.ControlUserCode);
                dbManager.AddParameters(13, "@UserId", userProfileModel.UserId);
                dbManager.AddParameters(14, "@AuthenticationMode", userProfileModel.AuthenticationMode);
                dbManager.AddParameters(15, "@UserType", userProfileModel?.UserType?.Value);

                int userControlId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, query));
                if (userControlId > 0)
                {
                    //drawavatar(userProfileModel.ControlUserCode, userProfileModel.FirstName, userProfileModel.LastName);
                    if(companyId>0)
                    {
                        bool isUserLinked = LinkUserToCompany(userControlId, userProfileModel.Organisations!=null && userProfileModel.Organisations.Count>0? userProfileModel.Organisations[0].ToString():null);
                        if (isUserLinked)
                        {
                            return CreateApplicationUser(userProfileModel, userProfileModel.ControlUserCode, companyId);
                        }
                        return 0;
                    }
                    else
                    {
                        return CreateApplicationUser(userProfileModel, userProfileModel.ControlUserCode, companyId);
                    }

                }
                else
                {
                    return 0;
                }
            }
        }
        public bool LinkUserToCompany(int userId, string CompanyId)
        {
            string query = QueryConfig.UserQuerySettings["LinkUserCompany"].ToString();
            dbManager.CreateParameters(2);
            dbManager.AddParameters(0, "@userId", userId);
            dbManager.AddParameters(1, "@companyId", CompanyId);
            int returnId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, query));
            if (returnId > 0)
            {
                return true;
            }
            else
            {
                throw new RepositoryException(MessageConfig.MessageSettings["USEREXISTS"].ToString());
            }
        }

        public bool LinkUserToCompany(string email, string CompanyId)
        {
            using (dbManager = new DBManager())
            {
                dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                dbManager.Open();
                string query = QueryConfig.UserQuerySettings["LinkUserCompanyByEmail"].ToString();
                dbManager.CreateParameters(2);
                dbManager.AddParameters(0, "@Email", email);
                dbManager.AddParameters(1, "@CompanyId", CompanyId);
                int returnId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, query));
                return true;
            }
        }
        #endregion

        #region POST Methods

        /// <summary>Add User Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of User Model</param>
        /// <returns>Insert Status</returns> 
        public int AddUser(UserProfileModel user)
        {
            try
            {
                int userId = 0;
                string userguid = Guid.NewGuid().ToString();
                long? createdby = Context.UserId > 0 ? Context.UserId:(long?)null;
                using (dbManager = new DBManager())
                { 
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    int passwordExpiry = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value);
                    user.ExpiresOn = DateTimeOffset.Now.AddDays(passwordExpiry);
                    string queryuser = QueryConfig.UserQuerySettings["VerifyUser"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@UserName", user.Email);
                    IDataReader exdr = dbManager.ExecuteReader(CommandType.Text, queryuser);
                    if (exdr.Read())
                        userId = Convert.ToInt32(exdr["Id"]);
                    exdr.Close(); dbManager.CloseReader();
                    //Add user details
                    if (userId <= 0)
                    {
                        string queryUser = QueryConfig.UserQuerySettings["AddUserDetail"].ToString();
                        dbManager.CreateParameters(15);
                        dbManager.AddParameters(0, "@Username", user.FirstName + " "+ user.LastName);
                        dbManager.AddParameters(1, "@Password", user.Password);
                        dbManager.AddParameters(2, "@Status", UsersStatus.Active.ToString());
                        dbManager.AddParameters(3, "@ExpiryOn", user.ExpiresOn.ToString("yyyy-MM-dd hh:mm:ss"));
                        dbManager.AddParameters(4, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                        dbManager.AddParameters(5, "@CreatedBy", createdby);
                        dbManager.AddParameters(6, "@Salt", user.Salt);
                        dbManager.AddParameters(7, "@IsFirstLogin", user.IsFirstLogin);
                        dbManager.AddParameters(8, "@IsSuperUser", user.IsSuperUser);
                        dbManager.AddParameters(9, "@UserType", user.UserType.Code);
                        dbManager.AddParameters(10, "@Email", user.Email);
                        dbManager.AddParameters(11, "@IsOtpVerified", user.IsOTPVerified);
                        dbManager.AddParameters(12, "@OTP", user.OTP);
                        dbManager.AddParameters(13, "@IsForceChange", user.IsForceChange);
                        dbManager.AddParameters(14, "@UserId", userguid);
                        object useridentity = dbManager.ExecuteScalar(CommandType.Text, queryUser);
                        userId = Convert.ToInt32(useridentity);
                        dbManager.CloseReader();
                        drawavatar(userguid, user.FirstName, user.LastName);
                        string queryProfile = QueryConfig.UserQuerySettings["AddUserProfileDetail"].ToString();
                        dbManager.CreateParameters(23);
                        dbManager.AddParameters(0, "@UserId", Convert.ToInt32(userId));
                        dbManager.AddParameters(1, "@FirstName", user.FirstName);
                        dbManager.AddParameters(2, "@LastName", user.LastName);
                        dbManager.AddParameters(3, "@Email", user.Email);
                        dbManager.AddParameters(4, "@Phone", user.Phone);
                        dbManager.AddParameters(5, "@Address1", user.Address1);
                        dbManager.AddParameters(6, "@Address2", user.Address2);
                        dbManager.AddParameters(7, "@City", user.City);
                        dbManager.AddParameters(8, "@State", user.State);
                        dbManager.AddParameters(9, "@Country", user?.Country?.Id);
                        dbManager.AddParameters(10, "@Zipcode", user.Zipcode);
                        dbManager.AddParameters(11, "@FileName", userguid+".png");
                        dbManager.AddParameters(12, "@ReportingTo", user?.ReportingTo?.Id);
                        dbManager.AddParameters(13, "@TotalYrsExp", user.TotalYrsExp);
                        dbManager.AddParameters(14, "@DOB", user.DOB);
                        dbManager.AddParameters(15, "@Hobbies", user.Hobbies);
                        dbManager.AddParameters(16, "@JoiningDate", user.JoiningDate);
                        dbManager.AddParameters(17, "@Department", user?.Department?.Id);
                        dbManager.AddParameters(18, "@Designation", user?.Designation?.Id);
                        dbManager.AddParameters(19, "@EmployeeId", user.EmployeeId);
                        dbManager.AddParameters(20, "@JobProfile", user.JobProfile);
                        dbManager.AddParameters(21, "@Accomplishments", user.Accomplishments);
                        dbManager.AddParameters(22, "@UserType", user.UserType.Code);
                        
                        int returnValue = Convert.ToInt32(dbManager.ExecuteNonQuery(CommandType.Text, queryProfile));

                        //Link user to module
                        string queryModule = QueryConfig.UserQuerySettings["AddUserModuleDetail"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@UserId", Convert.ToInt32(userId));
                        dbManager.AddParameters(1, "@ModuleCode", user.ModuleCode);
                        dbManager.ExecuteNonQuery(CommandType.Text, queryModule);
                        int UserOrganisation = 0;
                        int UserInstance = 0;

                        //Uesr link with organisation is user is not superuser.
                        if (!user.IsSuperUser)
                        {
                            //Link user to organisation
                            if (user.Organisations!=null &&  user.Organisations.Count > 0)
                            {
                                foreach (Guid org in user.Organisations)
                                {
                                    string queryUserexistInstance = QueryConfig.UserQuerySettings["GetUserOrganisation"].ToString();
                                    dbManager.CreateParameters(2);
                                    dbManager.AddParameters(0, "@UserId", userId);
                                    dbManager.AddParameters(1, "@OrgGuid", org);
                                    IDataReader userdr = dbManager.ExecuteReader(CommandType.Text, queryUserexistInstance);
                                    if (userdr.Read())
                                        UserOrganisation = Convert.ToInt32(userdr["OrganizationId"]);
                                    userdr.Close(); dbManager.CloseReader();
                                    if (UserOrganisation <= 0)
                                    {
                                        string queryUserInstance = QueryConfig.UserQuerySettings["LinkUserToOrganisation"].ToString();
                                        dbManager.CreateParameters(2);
                                        dbManager.AddParameters(0, "@UserId", Convert.ToInt32(userId));
                                        dbManager.AddParameters(1, "@OrgGUID", org);
                                        dbManager.ExecuteNonQuery(CommandType.Text, queryUserInstance);
                                    }
                                }
                            }
                            //TODO : Link user to Groups
                            if (user.Group!=null && user.Group.Count > 0)
                            {
                                List<Item> listGroupDetails = new List<Item>();
                                listGroupDetails = GetAllGroup();
                                foreach (int groupId in user.Group)
                                {
                                    if (listGroupDetails.Where(x => x.Id.Equals(groupId) && (x.Value.Equals("OrgAdminGroup") || x.Value.Equals("SuperAdminGroup"))).Count() >= 1) //.(x=>x.Id).FirstOrDefault() == groupId) uncomment
                                    {
                                        string queryUserexistGroups = QueryConfig.UserGroupQuerySettings["GetUserGroups"].ToString();
                                        dbManager.CreateParameters(2);
                                        dbManager.AddParameters(0, "@UserId", userId);
                                        dbManager.AddParameters(1, "@GroupId", groupId);
                                        IDataReader userdr = dbManager.ExecuteReader(CommandType.Text, queryUserexistGroups);
                                        if (userdr.Read())
                                            UserInstance = Convert.ToInt32(userdr["Id"]);
                                        userdr.Close(); dbManager.CloseReader();
                                        if (UserInstance <= 0)
                                        {
                                            string queryUserGroup = QueryConfig.UserGroupQuerySettings["AddUserToGroup"].ToString();
                                            dbManager.CreateParameters(4);
                                            dbManager.AddParameters(0, "@UserId", userId);
                                            dbManager.AddParameters(1, "@GroupId", Convert.ToInt32(groupId));
                                            dbManager.AddParameters(2, "@CreatedOn", DateTimeOffset.Now.ToString());
                                            dbManager.AddParameters(3, "@CreatedBy", Context.UserId);
                                            dbManager.ExecuteNonQuery(CommandType.Text, queryUserGroup);
                                        }
                                    }
                                    //else if (listGroupDetails.Where(x => x.Id == groupId && x.Value is "CustomerGroup").Select(x => x.Id).FirstOrDefault() == groupId)
                                    //{
                                    //    customerUserGroup(userId, groupId, Actions.LINK.ToString());
                                    //}
                                }
                            }
                        }
                        else if (user.IsSuperUser) //Uesr link with instance is user is superuser.
                        {
                            //if (user.Instances.Count > 0)
                            //{
                            //    foreach (int instance in user.Instances)
                            //    {
                            //        string queryUserexistOrganisation = QueryConfig.UserQuerySettings["GetUserInstance"].ToString();
                            //        dbManager.CreateParameters(2);
                            //        dbManager.AddParameters(0, "@UserId", userId);
                            //        dbManager.AddParameters(1, "@InstanceId", instance);
                            //        IDataReader userdr = dbManager.ExecuteReader(CommandType.Text, queryUserexistOrganisation);
                            //        if (userdr.Read())
                            //            UserInstance = Convert.ToInt32(userdr["Instance_Id"]);
                            //        userdr.Close(); dbManager.CloseReader();
                            //        if (UserInstance <= 0)
                            //        {
                            //            string queryUserCompany = QueryConfig.UserQuerySettings["LinkUserToInstance"].ToString();
                            //            dbManager.CreateParameters(2);
                            //            dbManager.AddParameters(0, "@UserId", Convert.ToInt32(userId));
                            //            dbManager.AddParameters(1, "@InstanceId", instance);
                            //            dbManager.ExecuteNonQuery(CommandType.Text, queryUserCompany);
                            //        }
                            //    }
                            //}
                            //TODO : Link user with super admin.
                            string queryUserGroup = QueryConfig.UserGroupQuerySettings["AddSuperUserToGroup"].ToString();
                            dbManager.CreateParameters(3);
                            dbManager.AddParameters(0, "@UserId", userId);
                            dbManager.AddParameters(1, "@CreatedOn", DateTimeOffset.Now.ToString());
                            dbManager.AddParameters(2, "@CreatedBy", Context.UserId);
                            dbManager.ExecuteNonQuery(CommandType.Text, queryUserGroup);
                        }
                    }
                    else
                    {
                        //TODO : throug user alredy exist
                        throw new DuplicateException();
                    }

                    //if (user.PhoneModel.Count > 0)
                    //{
                    //    PhoneRepository phoneRepository = new PhoneRepository();
                    //    phoneRepository.AddPhoneDetails(user.PhoneModel, userId);
                    //}
                    
                    dbManager.Transaction.Commit();
                    return Convert.ToInt32(userId);
                    
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", user.Name + " " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERFAILED", ex.Message, ex.StackTrace);
            }
        }

        #region User Customer Grouping
        public int customerUserGroup(int userId, int groupId, string Action)
        {
            int customerId = 0;
            int returnValue = 0;
            int isExistCustomerId = 0;

            string getUserExist = QueryConfig.CustomerQuerySettings["CheckUserExist"].ToString();
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@userId", userId);
            IDataReader usreExistReader = dbManager.ExecuteReader(CommandType.Text, getUserExist);

            if (usreExistReader.Read())
            {
                isExistCustomerId = Convert.ToInt32(usreExistReader["Id"]);
            }
            usreExistReader.Close();

            if (Action.ToString() == Actions.LINK.ToString())
            {
                if (isExistCustomerId > 0)
                {
                    returnValue = UpdateCustomerActiveStatus(isExistCustomerId, Convert.ToInt32(0));
                    //string updateCustomerDetails = QueryConfig.CustomerQuerySettings["UpdateCustomerDetails"].ToString();
                    //dbManager.CreateParameters(2);
                    //dbManager.AddParameters(0, "@CustomerId", isExistCustomerId);
                    //dbManager.AddParameters(1, "@DeleteValue", Convert.ToInt32(1));
                    //returnValue = dbManager.ExecuteNonQuery(CommandType.Text, updateCustomerDetails);
                }
                else
                {
                    string customerInsertTransaction = QueryConfig.CustomerQuerySettings["AddCustomerUser"].ToString();
                    dbManager.CreateParameters(11);
                    dbManager.AddParameters(0, "@CustomerGuid", Guid.NewGuid());
                    //dbManager.AddParameters(1, "@Email", user.Email);
                    //dbManager.AddParameters(2, "@Password", user.Password);
                    //dbManager.AddParameters(3, "@Salt", user.Salt);
                    dbManager.AddParameters(1, "@IsActive", true);
                    dbManager.AddParameters(2, "@Deleted", false);
                    dbManager.AddParameters(3, "@LastIpAddress", " ");
                    dbManager.AddParameters(4, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(5, "@IsFirstLogin", false);
                    dbManager.AddParameters(6, "@IsGuest", false);
                    dbManager.AddParameters(7, "@OTP", "123456");
                    dbManager.AddParameters(8, "@IsOTPVerified", true);
                    dbManager.AddParameters(9, "@IsAdmin", true);
                    dbManager.AddParameters(10, "@UserId", userId);
                    customerId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, customerInsertTransaction));

                    if (customerId > 0)
                    {
                        string postProductTaxQuery = QueryConfig.CustomerQuerySettings["InsertCustomerGroup"].ToString();
                        dbManager.CreateParameters(4);
                        dbManager.AddParameters(0, "@User_Id", customerId);
                        dbManager.AddParameters(1, "@Group_Id", groupId);
                        dbManager.AddParameters(2, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                        dbManager.AddParameters(3, "@CreatedBy", customerId);
                        returnValue = dbManager.ExecuteNonQuery(CommandType.Text, postProductTaxQuery);
                    }
                }
            }
            else if (Actions.DELINK.ToString() == Action.ToString())
            {
                returnValue = UpdateCustomerActiveStatus(isExistCustomerId, Convert.ToInt32(1));
                //string removeCustomerDetails = QueryConfig.CustomerQuerySettings["UpdateCustomerDetails"].ToString();
                //dbManager.CreateParameters(2);
                //dbManager.AddParameters(0, "@CustomerId", isExistCustomerId);
                //dbManager.AddParameters(1, "@DeleteValue", Convert.ToInt32(0));
                //returnValue = dbManager.ExecuteNonQuery(CommandType.Text, removeCustomerDetails);
            }
            return returnValue;
        }

        public int UpdateCustomerActiveStatus(int customerId, int deleteStatus)
        {
            int returnValue = 0;
            string updateCustomerDetails = QueryConfig.CustomerQuerySettings["UpdateCustomerDetails"].ToString();
            dbManager.CreateParameters(2);
            dbManager.AddParameters(0, "@CustomerId", customerId);
            dbManager.AddParameters(1, "@DeleteValue", deleteStatus);
            returnValue = dbManager.ExecuteNonQuery(CommandType.Text, updateCustomerDetails);

            return returnValue;
        }

        public List<Item> GetAllGroup()
        {
            Item groupDetails = new Item();
            List<Item> listGroupDetails = new List<Item>();

            string queryGetAllGroups = QueryConfig.UserGroupQuerySettings["GetAllGroups"].ToString();
            IDataReader groupDataReader = dbManager.ExecuteReader(CommandType.Text, queryGetAllGroups);

            while (groupDataReader.Read())
            {
                groupDetails = new Item()
                {
                    Id = Convert.ToInt32(groupDataReader["Id"]),
                    Value = Convert.ToString(groupDataReader["Name"]),
                };
                listGroupDetails.Add(groupDetails);
            }
            groupDataReader.Close();

            return listGroupDetails;
        }

        #endregion


        /// <summary>Add User : Verify User Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userName">User Name</param>
        /// <returns>Verify User</returns>
        public int CheckIfExists(string userName, string usertypeCode)
        {
            try
            {
                int userId = 0;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["VerifyUserWithModule"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserName", userName);
                    dbManager.AddParameters(1, "@UserType", usertypeCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        userId = Convert.ToInt32(dr["Id"]);
                    }
                    return userId;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("VERIFYUSERFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("VERIFYUSERFAILED", MessageConfig.MessageSettings["VERIFYUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        public bool Verify(string email, string otp)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["VerifyOTP"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Email", email);
                    dbManager.AddParameters(1, "@OTP", otp);
                    int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (rowAffacted > 0)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("INVALIDINPUT", MessageConfig.MessageSettings["INVALIDINPUT"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("INVALIDINPUT", MessageConfig.MessageSettings["INVALIDINPUT"].ToString(), ex.StackTrace);
            }
        }
        #endregion

        #region PUT Methods

        /// <summary>Update user</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of UserModel</param>
        /// <returns>returns bool value</returns>  
        public bool UpdateUser(UserProfileModel user)
        {
            try
            {
                string Image = string.Empty;
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    //drawavatar(user.UserId, user.FirstName, user.LastName);
                    string query = QueryConfig.UserQuerySettings["UpdateUserProfileDetailWithImage"].ToString();
                    dbManager.CreateParameters(22);
                    dbManager.AddParameters(0, "@UserId", user.UserId);
                    dbManager.AddParameters(1, "@FirstName", user.FirstName);
                    dbManager.AddParameters(2, "@LastName", user.LastName);
                    dbManager.AddParameters(3, "@Email", user.Email);
                    dbManager.AddParameters(4, "@Phone", user.Phone);
                    dbManager.AddParameters(5, "@Address1", user.Address1);
                    dbManager.AddParameters(6, "@Address2", user.Address2);
                    dbManager.AddParameters(7, "@City", user.City);
                    dbManager.AddParameters(8, "@State", user.State);
                    dbManager.AddParameters(9, "@Country", user?.Country?.Id);
                    dbManager.AddParameters(10, "@Zipcode", user.Zipcode);
                    dbManager.AddParameters(11, "@FileName", user.UserId+".png");
                    if (user.ReportingTo != null && user.ReportingTo.Id > 0)
                        dbManager.AddParameters(12, "@ReportingTo", user.ReportingTo.Id);
                    else
                        dbManager.AddParameters(12, "@ReportingTo", DBNull.Value);
                    dbManager.AddParameters(13, "@TotalYrsExp", user.TotalYrsExp);
                    dbManager.AddParameters(14, "@DOB", user.DOB);
                    dbManager.AddParameters(15, "@Hobbies", user.Hobbies);
                    dbManager.AddParameters(16, "@JoiningDate", user.JoiningDate);
                    if (user.Department != null && user.Department.Id > 0)
                        dbManager.AddParameters(17, "@Department", user?.Department?.Id);
                    else
                        dbManager.AddParameters(17, "@Department", DBNull.Value);
                    if (user.Designation != null && user.Designation.Id > 0)
                        dbManager.AddParameters(18, "@Designation", user?.Designation?.Id);
                    else
                        dbManager.AddParameters(18, "@Designation", DBNull.Value);
                    dbManager.AddParameters(19, "@EmployeeId", user.EmployeeId);
                    dbManager.AddParameters(20, "@JobProfile", user.JobProfile);
                    dbManager.AddParameters(21, "@Accomplishments", user.Accomplishments);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    dbManager.Transaction.Commit();
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEUSERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEUSERFAILED", ex.Message, ex.StackTrace);
            }
        }

        public void drawavatar(string filename, string firstname, string lastname)
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            FontFamily font = null;
            foreach (FontFamily ff in fonts.Families)
            {
                if (ff.Name == "Arial")
                {
                    font = ff;
                    break;
                }
            }
            float emSize = 12;

            String path = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + "/USERIMAGE/" + filename + ".png";
            List<string> _BackgroundColours = new List<string> { "#8B8000", "#722F37", "#E3735E", "#80461B", "#770737", "#7F00FF", "#800080", "#5D3FD3", "#800020", "#FF5F1F", "#008080", "#40826D", "#C4B454", "#B4C424", "#808000", "#478778", "#71797E", "#6082B6", "#0818A8", "#000000" };
            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            //measure the string to see how big the image needs to be
            var data = (drawing.MeasureString(firstname[0].ToString(), new Font(font, emSize, FontStyle.Bold)).Width + drawing.MeasureString(lastname[0].ToString(), new Font(font, emSize, FontStyle.Bold)).Width);
            Pen colourPen = new Pen(Color.Transparent, 1);

            using (var bitmap = new Bitmap(50, 50))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    using (Brush b = new SolidBrush(ColorTranslator.FromHtml(bgColour)))
                    {
                        g.FillEllipse(b, 0, 0, 49, 49);
                    }
                    //g.DrawString(firstname[0].ToString() + lastname[0].ToString(), new Font(font, emSize, FontStyle.Bold), new SolidBrush(Color.White), new PointF() { });

                    // Create a StringFormat object with the each line of text, and the block
                    // of text centered on the page.
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Draw the text and the surrounding rectangle.
                    g.DrawString(firstname[0].ToString() + lastname[0].ToString(), new Font(font.Name, 12, FontStyle.Bold, GraphicsUnit.Point), new SolidBrush(ColorTranslator.FromHtml("#FFFFFF")), new RectangleF(0, 15, 49, 19), stringFormat);
                    //g.DrawRectangle(colourPen, 2, 15, 45, 19);
                }

                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>Update User Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <param name="status">User Status</param>
        /// <returns>Update Status</returns>
        public bool UpdateUserStatus(string code, int userId, string status)
        {
            try
            {
                bool returnStatus = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["UpdateUserStatus"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", userId);
                    dbManager.AddParameters(1, "@Status", status);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        returnStatus = true;
                    }
                    return returnStatus;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEUSERFAILED", MessageConfig.MessageSettings["UPDATEUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEUSERFAILED", MessageConfig.MessageSettings["UPDATEUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Reset password : Change Password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Reset value</returns>
        public bool ResetPassword(UserProfileModel user)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["ResetPassword"].ToString();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@Email", user.Email);
                    dbManager.AddParameters(1, "@Password", user.Password);
                    dbManager.AddParameters(2, "@Salt", user.Salt);
                    dbManager.AddParameters(3, "@ExpireOn", user.ExpiresOn.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(4, "@IsFirstLogin", false);

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
                throw new RepositoryException("INVALIDPASSWORD", MessageConfig.MessageSettings["INVALIDPASSWORD"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("INVALIDPASSWORD", MessageConfig.MessageSettings["INVALIDPASSWORD"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Moving User To Another Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">User Id</param>
        /// <param name="existGroupId">Old GroupId</param>
        /// <param name="newGroupId">New GroupId</param>
        /// <returns>returns bool value</returns>
        public bool MovingUserToAnotherGroup(string code, int userId, int existGroupId, int newGroupId)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    string query = QueryConfig.UserQuerySettings["MovingUserToAnotherGroup"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@UserId", userId);
                    dbManager.AddParameters(1, "@ExistGroupId", existGroupId);
                    dbManager.AddParameters(2, "@NewGroupId", newGroupId);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    dbManager.Transaction.Commit();
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEUSERFAILED", MessageConfig.MessageSettings["DELETEUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("DELETEUSERFAILED", MessageConfig.MessageSettings["DELETEUSERFAILED"].ToString(), ex.StackTrace);
            }
        }

        public bool Deactivate(string id, string oid)
        {
            bool status = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["DeactivateUser"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", id);
                    dbManager.AddParameters(1, "@oId", oid);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                }

                //if(status)
                //{
                //    using (dbManager = new DBManager())
                //    {
                //        dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                //        dbManager.Open();
                //        string query = QueryConfig.UserQuerySettings["DeactivateControlUser"].ToString();
                //        dbManager.CreateParameters(2);
                //        dbManager.AddParameters(0, "@UserId", id);
                //        dbManager.AddParameters(1, "@oId", oid);
                //        int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                //        if (returnId > 0)
                //            status = true;
                //    }
                //}
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
            return status;
        }
        public bool Reactivate(string id, string oid)
        {
            bool status = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["ReactivateUser"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", id);
                    dbManager.AddParameters(1, "@oId", oid);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                }
                //if(status)
                //{
                //    using (dbManager = new DBManager())
                //    {
                //        dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                //        dbManager.Open();
                //        string query = QueryConfig.UserQuerySettings["ReactivateControlUser"].ToString();
                //        dbManager.CreateParameters(2);
                //        dbManager.AddParameters(0, "@UserId", id);
                //        dbManager.AddParameters(1, "@oId", oid);
                //        int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                //        if (returnId > 0)
                //            status = true;
                //    }
                //}
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
            return status;
        }
        #endregion

        #region DELETE Methods

        /// <summary>Delete User Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>Delete Status</returns>
        public bool RemoveUserInfo(int userId)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["RemoveUsersInfo"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", userId);
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
                throw new RepositoryException("DELETEUSERFAILED", MessageConfig.MessageSettings["DELETEUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEUSERFAILED", MessageConfig.MessageSettings["DELETEUSERFAILED"].ToString(), ex.StackTrace);
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

        #region Methods
        /// <summary>
        /// Ger user token details
        /// </summary>
        /// <param name="userToken">token</param>
        /// <returns>Token details</returns>
        public Token GetUserToken(string userToken, string IPAddress)
        {
            try
            {
                Token token = new Token();
                using (dbManager = new DBManager())
                {
                    dbManager.GetCacheConnection();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetToken"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Token", userToken);
                    dbManager.AddParameters(1, "@IPAddress", IPAddress);

                    dbManager.ExecuteReader(CommandType.Text, query);
                    if (dbManager.DataReader.Read())
                    {
                        token.Email = Convert.ToString(dbManager.DataReader["Email"]);
                        token.Value = Convert.ToString(dbManager.DataReader["Token"]);
                        token.ExpireOn = Convert.ToDateTime(dbManager.DataReader["TokenExpiry"]);
                        token.Duration = Convert.ToInt32(dbManager.DataReader["TokenDuration"]);
                        token.Status = Convert.ToString(dbManager.DataReader["Status"]);
                        token.CompanyCode = Convert.ToString(dbManager.DataReader["InstanceCode"]);
                        token.ModuleCode = Convert.ToString(dbManager.DataReader["ModuleCode"]);
                        token.IPAddress = Convert.ToString(dbManager.DataReader["IPAddress"]);
                    }
                }
                return token;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETUSERTOKENFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETUSERTOKENFAILED", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Update token expiry time
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>Boolean value</returns>
        public bool UpdateTokenExpireTime(string token, string IPAddress)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.GetCacheConnection();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["UpdateToken"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@Token", token);
                    dbManager.AddParameters(1, "IPAddress", IPAddress);
                    dbManager.AddParameters(2, "@ExtendTime", Convert.ToInt32(extendTime));
                    dbManager.ExecuteNonQuery(CommandType.Text, query);
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("", ex.Message, ex.StackTrace);
            }
        }

		public bool UpdatePasswordResetCode(string email, string passwordResetCode) {

			bool result = false;

			try
			{
				using (dbManager = new DBManager())
				{

					dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
					dbManager.Open();
					string query = QueryConfig.UserQuerySettings["UpdatePasswordResetCode"].ToString();
					dbManager.CreateParameters(2);
					dbManager.AddParameters(0, "@passwordResetCode", passwordResetCode);
					dbManager.AddParameters(1, "@email", email);
					if(ConvertData.ToInt( dbManager.ExecuteNonQuery(CommandType.Text, query))>0)
                    {
                        result = true;
                    }
                }
				return result;
			}
			catch (SqlException sqlEx)
			{
				LogManager.Log(sqlEx);
				throw new RepositoryException("", sqlEx.Message, sqlEx.StackTrace);
			}
			catch (Exception ex)
			{
				LogManager.Log(ex);
				throw new RepositoryException("", ex.Message, ex.StackTrace);
			}

			
		}

		/// <summary>
		/// Get user actions
		/// </summary>
		/// <param name="token">token</param>
		/// <returns>List of action</returns>
		public List<string> UserActions(string token)
        {
            try
            {
                List<string> actions = new List<string>();
                string action = string.Empty;
                using (dbManager = new DBManager())
                {
                    dbManager.GetCacheConnection();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetActions"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Token", token);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        actions.Add(Convert.ToString(dr["ActionCode"]));
                    }
                }
                return actions;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETUSERACTIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETUSERACTIONFAILED", ex.Message, ex.StackTrace);
            }
        }

        #region Link/Delink User Group
        /// <summary>
        /// Link / Delink user group.
        /// </summary>
        /// <param name="ProductGuid"></param>
        /// <param name="TaxRateId"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public bool LinkDelinkUserGroup(int UserId, int GroupId, string Action)     
        {
            bool status = false;
            int returnValue = 0;
            try
            {
                UserContext userDetails = JsonConvert.DeserializeObject<UserContext>(GlobalCacheManager.Instance.Get("usercontext_" + this.RequestId));  
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();

                    if (Actions.LINK.ToString() == Action.ToString())
                    {
                        string taxDuplicate = QueryConfig.UserQuerySettings["CheckGroupDuplicate"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@User_Id", UserId);
                        dbManager.AddParameters(1, "@Group_Id", GroupId);
                        IDataReader dr = dbManager.ExecuteReader(CommandType.Text, taxDuplicate);

                        if (dr.Read())
                        {
                            dr.Close();
                            throw new DuplicateException();
                        }
                        else
                        {
                            dr.Close();
                            string postProductTaxQuery = QueryConfig.UserQuerySettings["InsertUserGroup"].ToString();
                            dbManager.CreateParameters(4);
                            dbManager.AddParameters(0, "@User_Id", UserId);
                            dbManager.AddParameters(1, "@Group_Id", GroupId);
                            dbManager.AddParameters(2, "@CreatedOn", DateTimeOffset.Now);
                            dbManager.AddParameters(3, "@CreatedBy", UserId);
                            returnValue = dbManager.ExecuteNonQuery(CommandType.Text, postProductTaxQuery);
                        }
                    }
                    else if (Actions.DELINK.ToString() == Action.ToString())
                    {
                        string deleteProductTaxQuery = QueryConfig.UserQuerySettings["DeleteUserGroup"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@User_Id", UserId);
                        dbManager.AddParameters(1, "@Group_Id", GroupId);
                        returnValue = dbManager.ExecuteNonQuery(CommandType.Text, deleteProductTaxQuery);
                    }

                    if (Convert.ToInt32(returnValue) > 0)
                    {
                        status = true;
                    }
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("LINKDELINKUSERGROUPFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("LINKDELINKUSERGROUPFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Link/Delink user organization
        /// <summary>
        /// Link / Delink user group.
        /// </summary>
        /// <param name="ProductGuid"></param>
        /// <param name="TaxRateId"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public bool LinkDelinkUserOrganization(int UserId, int OrganizationId, string Action)    
        {
            bool status = false;
            int returnValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                    dbManager.Open();

                    if (Actions.LINK.ToString() == Action.ToString())
                    {
                        string taxDuplicate = QueryConfig.UserQuerySettings["CheckOrgnizationDuplicate"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@UserId", UserId);
                        dbManager.AddParameters(1, "@OrganizationId", OrganizationId);
                        IDataReader dr = dbManager.ExecuteReader(CommandType.Text, taxDuplicate);

                        if (dr.Read())
                        {
                            dr.Close();
                            throw new DuplicateException();
                        }
                        else
                        {
                            dr.Close();
                            string postProductTaxQuery = QueryConfig.UserQuerySettings["InsertUserOrgnization"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@OrganizationId", OrganizationId);
                            dbManager.AddParameters(1, "@UserId", UserId);
                            returnValue = dbManager.ExecuteNonQuery(CommandType.Text, postProductTaxQuery);
                        }
                    }
                    else if (Actions.DELINK.ToString() == Action.ToString())
                    {
                        string deleteProductTaxQuery = QueryConfig.UserQuerySettings["DeleteUserOrgnization"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@UserId", UserId);
                        dbManager.AddParameters(1, "@OrganizationId", OrganizationId);
                        returnValue = dbManager.ExecuteNonQuery(CommandType.Text, deleteProductTaxQuery);
                    }

                    if (Convert.ToInt32(returnValue) > 0)
                    {
                        status = true;
                    }
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("LINKDELINKUSERORGFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("LINKDELINKUSERORGFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #endregion
    }
}