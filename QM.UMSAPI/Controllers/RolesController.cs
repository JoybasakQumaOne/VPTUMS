// -----------------------------------------------------------------------------------------------------------------
// <copyright file="RolesController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
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
    using System.Web;
    using System.Web.Http;
    using System.Net;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMSAPI.Authorize;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <Roles>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    //[RoutePrefix("api/Roles")]
    public class RolesController : ApiController
    {
        #region Variable Declaration
        /// <summary>
        /// This is Private Variable
        /// </summary>
        private readonly IRoleBusiness _IRoleBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="IRoleBusinessLogic">IRoleBusinessLogic</param>
        public RolesController(IRoleBusiness IRoleBusiness)
	    {
            this._IRoleBusiness = IRoleBusiness;
	    }
        #endregion

        #region GET Methods
        
        /// <summary>Get role details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of roles</returns>
        //[UserAuthorize(Actions = "CAN_VIEW_ALL_ROLE")]
        [HttpGet, ActionName("GetRoles")]
        public IHttpActionResult Get(string code, string ModuleCode)
        {
            try
            {
                List<RoleModel> roleDetails = this._IRoleBusiness.GetRoles(code, ModuleCode);
                if (roleDetails != null && roleDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, roleDetails);
                }
                return this.Content(HttpStatusCode.NoContent, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ROLEFOUNDFAILED"));
            }
        }

        /// <summary>Get role details by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="id">Role Id</param>
        /// <returns>returns single entity of role</returns>
        [HttpGet, ActionName("GetRoleById")]
        //[Route("GetRoleById/{code}/{Id}")]
        [UserAuthorize(Actions = "CAN_VIEW_ROLE")]
        public IHttpActionResult GetRoleById(string code, int id)
        {
            try
            {
                RoleModel roleDetails = this._IRoleBusiness.GetRoleById(code, id);
                if (roleDetails != null && roleDetails.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, roleDetails);
                }
                return this.Content(HttpStatusCode.NoContent, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ROLEFOUNDFAILED"));
            }
        }

        /// <summary>Get list of roles assigned to group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>returns list of roles</returns>
        [HttpGet, ActionName("GetRolesFromRolesGroup")]
        [UserAuthorize(Actions = "CAN_SEARCH_ROLE")]
        public IHttpActionResult GetRolesFromRolesGroup(string code, int id)
        {
            try
            {
                List<RoleModel> roleDetails = this._IRoleBusiness.GetRolesAssignedtoRolesGroup(code, id);
                if (roleDetails != null && roleDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, roleDetails);
                }
                return this.Content(HttpStatusCode.NoContent, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ROLEFOUNDFAILED"));
            }
        }

        /// <summary>Get Roles By Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="status">Role Status</param>
        /// <returns>returns list of roles</returns>
        [UserAuthorize(Actions = "CAN_SEARCH_ROLE")]
        [HttpGet, ActionName("GetRolesByStatus")]
        public IHttpActionResult GetRolesByStatus(string code, string status)
        {
            try
            {
                List<RoleModel> roleDetails = this._IRoleBusiness.GetRolesByStatus(code, status);
                if (roleDetails != null && roleDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, roleDetails);
                }
                return this.Content(HttpStatusCode.NoContent, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ROLEFOUNDFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of RoleModel</param>
        /// <returns>returns bool value</returns>
        [HttpPost, ActionName("AddRole")]
        [UserAuthorize(Actions = "CAN_ADD_ROLE")]
        public IHttpActionResult Post(string code, RoleModel role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IRoleBusiness.AddRole(code, role);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDROLEFAILED"));
            }
        }

        /// <summary>Add permission To Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permissions">Object of Common Model</param>
        /// <returns>returns bool value</returns>
        [UserAuthorize(Actions = "CAN_ADD_ROLE")]
        //[HttpPost, ActionName("AccessPermissionToRole")]
        public IHttpActionResult Post(string code, CommonModel permissions)
        {
            try
            {
                bool isInserted = this._IRoleBusiness.AccessPermissionToRole(code, permissions);
                if (isInserted)
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
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ACCESSPERMISSIONTOROLEFAILED"));
            }
        }

        /// <summary>Delete permission of role</summary>
        /// <param name="code">Code in Entity</param>        
        /// <param name="permissions">Object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        [HttpPost, ActionName("RemovePermissionOfRole")]
        [UserAuthorize(Actions = "CAN_REMOVE_ROLE")]
        public IHttpActionResult RemovePermissionOfRole(string code, CommonModel permissions)
        {
            try
            {
                bool isDeleted = this._IRoleBusiness.DeletePermissionToRole(code, permissions);
                if (isDeleted)
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
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "REMOVEPERMISSIONTOROLEFAILED"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Role Model</param>
        /// <returns>returns bool value</returns>
        [UserAuthorize(Actions = "CAN_UPDATE_ROLE")]
        //[ActionName("UpdateRole")]
        public IHttpActionResult Put(string code, RoleModel role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = this._IRoleBusiness.UpdateRole(code, role);
                    if (isUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEROLEFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEROLEFAILED"));
            }
        }      

        #endregion

        #region DELETE Methods

        /// <summary>Delete role by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Role Id</param>
        /// <returns>returns bool value</returns>
        [UserAuthorize(Actions = "CAN_DELETE_ROLE")]
        //[ActionName("RemoveRoleInfo")]
        public IHttpActionResult Delete(string code, int Id)
        {
            try
            {
                bool isDeleted = this._IRoleBusiness.RemoveRoleInfo(code, Id);
                if (isDeleted != null)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEROLEFAILED"));
            }
        }

        #endregion        
    }
}
