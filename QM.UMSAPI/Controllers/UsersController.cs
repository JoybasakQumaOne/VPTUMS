// -----------------------------------------------------------------------------------------------------------------
// <copyright file="UsersController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

namespace QM.UMSAPI.Controllers
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;    
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.Notification;    
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMSAPI.Authorize;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <Users>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------


    public class UsersController : ApiController
    {
        #region Variable Declaration
        /// <summary>
        /// This is private variable
        /// </summary>
        private readonly IUsersBusiness _IUsersBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="IUsersBusinessLogic">IUsersBusinessLogic</param>
        public UsersController(IUsersBusiness IUsersBusiness)
        {
            this._IUsersBusiness = IUsersBusiness;
            this._IUsersBusiness.Init(System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString());
        }
        #endregion

        #region GET Methods

        /// <summary>Get User Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>Single Entity of User Details</returns>
       // [UserAuthorize(Actions = "CAN_VIEW_USER")]
        [HttpGet, ActionName("GetUserById")]
        public IHttpActionResult Get(string userId)
        {
            try
            {
                UserProfileModel userDetails = this._IUsersBusiness.GetUserById(userId);
                if (userDetails != null && userDetails.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }


        [HttpGet, ActionName("getuserdetails")]
        public IHttpActionResult GetUserDetails(string userId)
        {
            try
            {
                UserProfileModel userDetails = this._IUsersBusiness.GetUserDetails(userId);
                if (userDetails != null )
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }


        /// <summary>Get User Details By Module and Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="moduleCode">Module Code</param>
        /// <param name="status">User status</param>
        /// <returns>List of User Details</returns>
        //[UserAuthorize(Actions = "CAN_VIEW_USERBYMODULE")]
        //[HttpGet, ActionName("GetUserByStatus")]
        public IHttpActionResult Get(string code, string moduleCode, string status)
        {
            try
            {
                List<UserProfileModel> userDetails = this._IUsersBusiness.GetUserByStatus(code, moduleCode, status);
                if (userDetails != null && userDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }

        [HttpGet, ActionName("GetUserDetails")]
        public IHttpActionResult Get(bool IsSuperUser, string Status)
        {
            try
            {
                List<UserProfileModel> userDetails = this._IUsersBusiness.GetUserDetails(IsSuperUser, Status);
                if (userDetails != null && userDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }

        /// <summary>
        /// Get Organization Users
        /// </summary>
        /// <param name="OrgGuid"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpGet, ActionName("get")]
        public IHttpActionResult Get(Guid oid)
        {
            try
            {
                var data = this._IUsersBusiness.GetOrgAdminUsers(oid);
                if (data != null && data.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, data);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }
        /// <summary>Get Users From User Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>List of User Details</returns>
        [HttpGet, ActionName("GetUsersFromUsersGroup")]
        [UserAuthorize(Actions = "CAN_SEARCH_USER")]
        public IHttpActionResult GetUsersFromUsersGroup(string code, int groupId)
        {
            try
            {
                List<UserProfileModel> userDetails = this._IUsersBusiness.GetUsersFromUsersGroup(code, groupId);
                if (userDetails != null && userDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }

        /// <summary>Get Users From Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="roleId">Role Id</param>
        /// <returns>List of User Details</returns>
        [HttpGet, ActionName("GetUsersFromRole")]
        [UserAuthorize(Actions = "CAN_SEARCH_USER")]
        public IHttpActionResult GetUsersFromRole(string code, int roleId)
        {
            try
            {
                List<UserProfileModel> userDetails = this._IUsersBusiness.GetUsersFromRole(code, roleId);
                if (userDetails != null && userDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }

        /// <summary>Get User Details From Active Directory</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>User Details</returns>
        [HttpGet, ActionName("GetADUserInfo")]
        [UserAuthorize(Actions = "CAN_VIEW_ADUSER")]
        public IHttpActionResult GetADUserInfo(string code, string emailId)
        {
            try
            {
                UserProfileModel userDetails = this._IUsersBusiness.GetADUserInfo(code, emailId);
                if (userDetails != null)
                {
                    return this.Content(HttpStatusCode.OK, userDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERFOUNDFAILED"));
            }
        }


        [HttpGet, ActionName("Verify")]
        public IHttpActionResult Verify(string email, string otp)
        {
            try
            {
                bool isValid = this._IUsersBusiness.Verify(email, otp);
                if (isValid)
                {
                    return this.Content(HttpStatusCode.Accepted, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "OTPMATCHED"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "OTPUMMATCHED"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "INVALIDINPUT"));
            }
        }
        #endregion

        #region POST Methods

        /// <summary>Add User Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of User Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("Register")]
      //  [UserAuthorize(Actions = "CAN_ADD_USER")]
        public IHttpActionResult Post(UserProfileModel user)
       {
            try
            {
                if (ModelState.IsValid)
                {
                    int isInserted = this._IUsersBusiness.AddClientUser(user);
                    if (isInserted > 0)
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "ADDUSERSUCCESS", isInserted.ToString()));                     
                    else
                    {
                        if (isInserted <= 0)
                        {
                            return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "VERIFYUSERFAILED"));
                        }
                        return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "ADDUSERFAILED"));
                    }
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                }
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERFAILED"));
            }
        }
        
        /// <summary>Forgot Password</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>Request Status</returns>
        [HttpPost, ActionName("ForgotPassword")]
        //[UserAuthorize(Actions = "CAN_FORGOT_PASSWORD")]
        public IHttpActionResult Post(string emailId,string type="STUDENT")
        {
            try
            {
                var clientInfo = this._IUsersBusiness.ForgotPassword(emailId, type);
                if (clientInfo != null && clientInfo.Id > 0)
                {                    
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                else
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "USERNOTFOUND"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ERROROCCURE"));
            }
        }


        [HttpGet, ActionName("GetCompanyDetail")]
        public IHttpActionResult GetCompanyDetail(string username)
        {
            try
            {
                List<Company> agencyDetails = this._IUsersBusiness.GetCompanyDetail(username);
                if (agencyDetails != null && agencyDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, agencyDetails);
                }
                return this.Content(HttpStatusCode.NoContent, APIResponse.CreateAPIResponse(ResponseType.NoContent.ToString(), "NoContent"));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETAGENCYFAILED"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update User</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of User Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("UpdateUser")]
        //[UserAuthorize(Actions = "CAN_EDIT_USER")]
        public IHttpActionResult Put(UserProfileModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = this._IUsersBusiness.UpdateUser(user);
                    if (isUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEUSERFAILED"));
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                }
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEUSERFAILED"));
            }
        }

        /// <summary>Update User Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <param name="status">User Status</param>
        /// <returns>Update Status</returns>
        [UserAuthorize(Actions = "CAN_UPDATE_USERSTATUS")]
        //[HttpPut, ActionName("UpdateUserStatus")]
        public IHttpActionResult Put(string code, int userId, string status)
        {
            try
            {
                bool isUpdated = this._IUsersBusiness.UpdateUserStatus(code, userId, status);
                if (isUpdated)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEUSERFAILED"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEUSERFAILED"));
            }
        }

        /// <summary>Change Password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Change Status</returns>
        [HttpPut, ActionName("ChangePassword")]
        //[UserAuthorize(Actions = "CAN_CHANGE_PASSWORD")]
        public IHttpActionResult Put(ChangePassword changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int isUpdated = this._IUsersBusiness.ChangePassword(changePassword);
                    switch (isUpdated)
                    {
                        case 1: return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                        case 2: return this.Content(HttpStatusCode.NotAcceptable, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "PASSWORDMATCH"));
                        case 3: return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDPASSWORD"));
                        default: return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "USERNOTFOUND"));
                    }
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                }
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        /// <summary>
        /// Reset password for clinet facing user
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPut, ActionName("ResetPassword")]
        //[UserAuthorize(Actions = "CAN_RESET_PASSWORD")]
        public IHttpActionResult ResetPassword(ChangePassword changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int resetPasswordStatus = this._IUsersBusiness.ResetPassword(changePassword);  
                    switch (resetPasswordStatus)
                    {
                        case 1: return this.Content(HttpStatusCode.OK, APIResponse.CreateAPISuccessResponse(ResponseType.Modified.ToString(), "SUCCESS","UPDATED"));
                        case 2: return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDPASSWORD"));
                        default: return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "USERNOTFOUND"));
                    }
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                }
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        /// <summary>
        /// Reset password for admin facing user
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPut, ActionName("adminreset")]
        //[UserAuthorize(Actions = "CAN_RESET_PASSWORD")]
        public IHttpActionResult ResetAUPassword(ChangePassword changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool resetPasswordStatus = this._IUsersBusiness.AdminReset(changePassword);
                    if(resetPasswordStatus)
                    {
                       return this.Content(HttpStatusCode.OK, APIResponse.CreateAPISuccessResponse(ResponseType.Modified.ToString(), "SUCCESS", "UPDATED"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), "InvalidRequest"));
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                }
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        /// <summary>Moving User To Another Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <param name="existGroupId">Old GroupId</param>
        /// <param name="newGroupId">New GroupId</param>
        /// <returns>Update Status</returns>
        [UserAuthorize(Actions = "CAN_CHANGE_USERGROUP")]
        //[HttpPut, ActionName("AssignUserToGroup")]
        public IHttpActionResult Put(string code, int userId, int existGroupId, int newGroupId)
        {
            try
            {
                bool isMoving = this._IUsersBusiness.MovingUserToAnotherGroup(code, userId, existGroupId, newGroupId);
                if (isMoving)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "MOVEUSERTOGROUPFAILED"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "MOVEUSERTOGROUPFAILED"));
            }
        }


        
        [UserAuthorize(Actions = "CAN_DEACTIVATE_USER")]
        [HttpPut, ActionName("deactivate")]
        public IHttpActionResult Deactivate(string id,string oid)
        {
            try
            {
                bool isMoving = this._IUsersBusiness.Deactivate(id,oid);
                if (isMoving)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "LOGGEDINUSERNOTDEACTIVATED"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "MOVEUSERTOGROUPFAILED"));
            }
        }

        [UserAuthorize(Actions = "CAN_DEACTIVATE_USER")]
        [HttpPut, ActionName("reactivate")]
        public IHttpActionResult Reactivate(string id,string oid)
        {
            try
            {
                bool isMoving = this._IUsersBusiness.Reactivate(id,oid);
                if (isMoving)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "MOVEUSERTOGROUPFAILED"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "MOVEUSERTOGROUPFAILED"));
            }
        }
        #endregion

        #region Link Delink user group
        [HttpPost, ActionName("LinkDelinkUserGroup")]
        //[UserAuthorize(Actions = "CAN_DELETE_ORDER")]
        public IHttpActionResult LinkDelinkUserGroup(int userId, int groupId, string action)  
        {
            try
            {
                if (this._IUsersBusiness.LinkDelinkUserGroup(userId, groupId, action))
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "NOTSUCCESS"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(RepEx.ErrorCode, RepEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "LINKDELINKUSERGROUPFAILED"));
            }
        }
        #endregion

        #region Link Delink user group
        [HttpPost, ActionName("LinkDelinkUserOrg")]
        //[UserAuthorize(Actions = "CAN_DELETE_ORDER")]
        public IHttpActionResult LinkDelinkUserOrg(int userId, int organizationId, string action)   
        {
            try
            {
                if (this._IUsersBusiness.LinkDelinkUserOrganization(userId, organizationId, action))  
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "NOTSUCCESS"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(RepEx.ErrorCode, RepEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "LINKDELINKUSERORGFAILED"));
            }
        }
        #endregion

        #region DELETE Methods

        /// <summary>Delete User Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>Delete Status</returns>
        //[UserAuthorize(Actions = "CAN_DELETE_USER")]
        //[ActionName("MovingUserToAnotherGroup")]
        [HttpDelete, ActionName("DeleteUser")]
        public IHttpActionResult Delete(int userId)
        {
            try
            {
                bool userDetails = this._IUsersBusiness.RemoveUserInfo(userId);
                if (userDetails != null)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "USERFOUNDFAILED"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEUSERFAILED"));
            }
        }
        
        #endregion        
    }
}
 