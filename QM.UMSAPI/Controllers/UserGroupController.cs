// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="UserGroupController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

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
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMSAPI.Authorize;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserGroup>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UserGroupController : ApiController
    {
        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IUserGroupBusiness _IUserGroupBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IUserGroupBusinessLogic">IUserGroupBusinessLogic</param>
        public UserGroupController(IUserGroupBusiness IUserGroupBusiness)
        {
            this._IUserGroupBusiness = IUserGroupBusiness;
            this._IUserGroupBusiness.Init(System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString());
        }
        #endregion

        #region GET Methods

        /// <summary>Get Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Group Details</returns>
        [HttpGet, ActionName("GetAllGroup")]
        //[UserAuthorize(Actions = "CAN_VIEW_ALL_GROUP")]
        public IHttpActionResult Get(string ModuleCode)
        {
            try
            {
                List<UserGroupModel> groupDetails = this._IUserGroupBusiness.GetAllGroup(ModuleCode);
                if (groupDetails != null && groupDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, groupDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GROUPFOUNDFAILED"));
            }
        }

        /// <summary>Get Group Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Single Entity of Group Details</returns>
        [HttpGet, ActionName("GetGroupById")]
        [UserAuthorize(Actions = "CAN_VIEW_GROUP")]
        public IHttpActionResult Get(string code, int groupId)
        {
            try
            {
                UserGroupModel groupDetails = this._IUserGroupBusiness.GetGroupById(code, groupId);
                if (groupDetails != null && groupDetails.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, groupDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GROUPFOUNDFAILED"));
            }
        }
        
        /// <summary>Get Group Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Group Type</returns>
        [HttpGet, ActionName("GetAllGroupType")]
        [UserAuthorize(Actions = "CAN_VIEW_ALL_GROUP_TYPE")]
        public IHttpActionResult GetAllGroupType(string code)
        {
            try
            {
                List<Item> groupTypeDetails = this._IUserGroupBusiness.GetAllGroupType(code);
                if (groupTypeDetails != null && groupTypeDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, groupTypeDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GROUPTYPEFOUNDFAILED"));
            }
        }

        /// <summary>Get All Group Details By Type and Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Status">Status Type</param>
        /// <param name="groupTypeId">Group Type Id</param>
        /// <returns>List of Group Details</returns>
        [HttpGet, ActionName("GetAllGroupByTypeStatus")]
        [UserAuthorize(Actions = "CAN_SEARCH_GROUP")]
        public IHttpActionResult GetAllGroupByTypeStatus(string code, string Status, string groupTypeId)
        {
            try
            {
                List<UserGroupModel> groupDetails = this._IUserGroupBusiness.GetAllGroupByTypeStatus(code, Status, groupTypeId);
                if (groupDetails != null && groupDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, groupDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GROUPFOUNDFAILED"));
            }
        }

        [HttpGet, ActionName("GetAllLinkedAttribute")]
        //[UserAuthorize(Actions = "CAN_VIEW_ALL_GROUP_TYPE")]
        public IHttpActionResult GetAllLinkedAttribute(int groupId)  
        {
            try
            {
                List<Item> attributeDetails = this._IUserGroupBusiness.GetAllLinkedAttribute(groupId);
                if (attributeDetails != null && attributeDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, attributeDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETATTRIBUTEFAILED"));
            }
        }

        [HttpGet, ActionName("GetUserGroups")]
        public IHttpActionResult GetUserGroups(string userId)
        {
            try
            {
                var data = this._IUserGroupBusiness.GetUserGroups(userId);
                if (data != null)
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GROUPFOUNDFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddGroup")]
        //[UserAuthorize(Actions = "CAN_ADD_GROUP")]
        public IHttpActionResult Post(string code, UserGroupModel Group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IUserGroupBusiness.AddGroup(code, Group);
                    if (isInserted)
                    {
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDGROUPFAILED"));
            }
        }

        /// <summary>Add User To Group</summary>
        /// <param name="code">Code in Entity</param>  
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        [HttpPost, ActionName("AddUserToGroup")]
        [UserAuthorize(Actions = "CAN_ADD_USERTOGROUP")]
        public IHttpActionResult Post(string code, CommonModel user)
        {
            try
            {
                bool isInserted = this._IUserGroupBusiness.AddUserToGroup(code, user);
                if (isInserted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (DuplicateException dex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Duplicate.ToString(), "DUPLICATE"));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.Message));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERTOGROUPFAILED"));
            }
        }

        /// <summary>Add Role To Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        [HttpPost, ActionName("AddRoleToGroup")]
        [UserAuthorize(Actions = "CAN_ADD_ROLETOGROUP")]
        public IHttpActionResult AddRoleToGroup(string code, CommonModel role)
        {
            try
            {
                bool isInserted = this._IUserGroupBusiness.AddRoleToGroup(code, role);
                if (isInserted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDROLETOGROUPFAILED"));
            }
        }

        /// <summary>Remove User From Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Remove Status</returns>    
        [HttpPost, ActionName("RemoveUserFromGroup")]
        [UserAuthorize(Actions = "CAN_REMOVE_USERFROMGROUP")]
        public IHttpActionResult RemoveUserToGroup(string code, CommonModel user)
        {
            try
            {
                bool isRemoved = this._IUserGroupBusiness.RemoveUserFromGroup(code, user);
                if (isRemoved)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "INVALIDINPUT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage , RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage, bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, "DELETEUSERFROMGROUPFAILED",code));
            }
        }

        /// <summary>Remove Role From Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Common Entity</param>
        /// <returns>Remove Status</returns>
        [HttpPost, ActionName("RemoveRoleFromGroup")]
        [UserAuthorize(Actions = "CAN_REMOVE_ROLEFROMGROUP")]
        public IHttpActionResult DeleteRoleToGroup(string code, CommonModel role)
        {
            try
            {
                bool isRemoved = this._IUserGroupBusiness.DeleteRoleFromGroup(code, role);
                if (isRemoved)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, "DELETEROLEFROMGROUPFAILED",code));
            }
        }

        [HttpPost, ActionName("LinkAttribute")]
        //[UserAuthorize(Actions = "CAN_REMOVE_ROLEFROMGROUP")]
        public IHttpActionResult LinkAttributeToGroup(int groupId, int attributeId)  
        {
            try
            {
                bool isLinked = this._IUserGroupBusiness.LinkGroupAttribute(groupId, attributeId);
                if (isLinked)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "LINKATTRIBUTETOGROUPFAILED"));
            }
        }

        [HttpPost, ActionName("LinkGroupToUser")]
        //[UserAuthorize(Actions = "CAN_REMOVE_ROLEFROMGROUP")]
        public IHttpActionResult LinkGroupToUser(int groupId, int userId)  
        {
            try
            {
                bool isLinked = this._IUserGroupBusiness.LinkGroupToUser(groupId, userId);
                if (isLinked)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERTOGROUPFAILED"));
            }
        }


        [HttpPut, ActionName("LinkGroup")]
        public IHttpActionResult LinkGroup(string gId, string uId)
        {
            try
            {
                bool isInserted = this._IUserGroupBusiness.LinkUserGroup(gId, uId);
                if (isInserted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (DuplicateException dex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Duplicate.ToString(), "DUPLICATE"));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.Message));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERTOGROUPFAILED"));
            }
        }

        [HttpPut, ActionName("DeLinkGroup")]
        public IHttpActionResult DeLinkGroup(string gId, string uId)
        {
            try
            {
                bool isInserted = this._IUserGroupBusiness.DeLinkUserGroup(gId, uId);
                if (isInserted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "INVALIDINPUT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (DuplicateException dex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Duplicate.ToString(), "DUPLICATE"));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.Message));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERTOGROUPFAILED"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("UpdateGroup")]
        [UserAuthorize(Actions = "CAN_UPDATE_GROUP")]
        public IHttpActionResult Put(string code, UserGroupModel Group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = this._IUserGroupBusiness.UpdateGroup(code, Group);
                    if (isUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEGROUPFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEGROUPFAILED"));
            }
        }
        [HttpPost, ActionName("DelinkAttribute")]
        //[UserAuthorize(Actions = "CAN_REMOVE_ROLEFROMGROUP")]
        public IHttpActionResult DelinkAttributeToGroup(int groupId, int attributeId)
        {
            try
            {
                bool isLinked = this._IUserGroupBusiness.DelinkGroupAttribute(groupId, attributeId);
                if (isLinked)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELINKATTRIBUTETOGROUPFAILED"));
            }
        }

        [HttpPost, ActionName("DelinkGroupToUser")]
        //[UserAuthorize(Actions = "CAN_REMOVE_ROLEFROMGROUP")]
        public IHttpActionResult DelinkGroupToUser(int groupId, int userId)
        {
            try
            {
                bool isLinked = this._IUserGroupBusiness.DelinkGroupToUser(groupId, userId);
                if (isLinked)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEUSERTOGROUPFAILED"));
            }
        }
        #endregion

        #region DELETE Methods

        /// <summary>Deactivate Group details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Deactivate Status</returns>
        [HttpDelete, ActionName("DeActivateGroup")]
        [UserAuthorize(Actions = "CAN_DELETE_GROUP")]
        public IHttpActionResult Delete(string code, int groupId)
        {
            try
            {
                bool GroupDetails = this._IUserGroupBusiness.RemoveGroup(code, groupId);
                if (GroupDetails != null)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "NOCONTENT"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEGROUPFAILED"));
            }
        }
        
        #endregion      
    }
}
