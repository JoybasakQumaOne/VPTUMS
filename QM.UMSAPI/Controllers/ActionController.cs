// -----------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------------

namespace QM.UMSAPI.Controllers
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ExceptionHandling;
    using QM.UMS.Business.Business;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMSAPI.Authorize;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Http;    
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <Action>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    //[RoutePrefix("api/Action")]
    public class ActionController : ApiController
    {
        #region Variable Declaration
        /// <summary>
        /// This is private variable
        /// </summary>
        private readonly IActionBusiness _IActionBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="IActionBusiness">IActionBusinessLogic</param>
        public ActionController(IActionBusiness _iActionBusiness)
        {
            this._IActionBusiness = _iActionBusiness;
        }
        #endregion

        #region GET Methods

        /// <summary>Get Action Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of Action</returns>
        [UserAuthorize(Actions = "CAN_VIEW_ALL_ACTION")]       
        //[HttpGet, ActionName("GetActions")]
        public IHttpActionResult Get(string code)
        {
            try
            {
                List<ActionModel> actionDetails = this._IActionBusiness.GetActions(code);
                if (actionDetails != null && actionDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, actionDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ACTIONFOUNDFAILED"));
            }
        }

        /// <summary>Get Action Detail by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="actionId">Action Id</param>
        /// <returns>returns single entity of Action</returns>       
        [UserAuthorize(Actions = "CAN_VIEW_ACTION")]
        //[HttpGet, ActionName("GetActionById")]
        public IHttpActionResult Get(string code, int actionId)
        {
            try
            {
                ActionModel actionDetail = this._IActionBusiness.GetActionById(code, actionId);
                if (actionDetail != null && actionDetail.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, actionDetail);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ACTIONFOUNDFAILED"));
            }
        }

        /// <summary>Get Action Details by UserId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Action</returns>
        [HttpGet, ActionName("GetActionsByuserId")]
        [UserAuthorize(Actions = "CAN_SEARCH_ACTION")]
        public IHttpActionResult GetActionsByuserId(string code, int userId)
        {
            try
            {
                List<ActionModel> actionDetails = this._IActionBusiness.GetActionsByUserId(code, userId);
                if (actionDetails != null && actionDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, actionDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETACTIONBYUSERFAILED"));
            }
        }

        /// <summary>Get Action Details by RoleId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Action</returns>
        [HttpGet, ActionName("GetActionsByRoleId")]
        public IHttpActionResult GetActionsByRoleId(string code, int RoleId)
        {
            try
            {
                List<ActionModel> actionDetails = this._IActionBusiness.GetActionsByRoleId(code, RoleId);
                if (actionDetails != null && actionDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, actionDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETACTIONBYROLEFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return add status</returns>
        [UserAuthorize(Actions = "CAN_ADD_ACTION")]
        //[HttpPost, ActionName("AddAction")]
        public IHttpActionResult Post(string code, ActionModel action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IActionBusiness.AddAction(code, action);
                    if (isInserted)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDACTIONFAILED"));
            }
        }


        /// <summary>Add Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return add status</returns>
        [HttpPost, ActionName("GetStatusByActionCode")]
        public IHttpActionResult Post(string code, int UserID, List<string> Actioncode)
        {
            try
            {

                List<AcessPermissionModel> ActionList = this._IActionBusiness.GetActionPermission(code, UserID, Actioncode);
                if (ActionList != null && ActionList.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, ActionList);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "NOCONTENT"));


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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDACTIONFAILED"));
            }
        }

        /// <summary>Assign Action To Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Common Model</param>
        /// <returns>return assign status</returns>
        [UserAuthorize(Actions = "CAN_ADD_ACTIONTOROLE")]
        [ActionName("AssignActionToRole")]
        public IHttpActionResult Post(string code, CommonModel action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IActionBusiness.AssignActionToRole(code, action);
                    if (isInserted)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ASSIGNACTIONTOROLEFAILED"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return update status</returns>
        [UserAuthorize(Actions = "CAN_UPDATE_ACTION")]
        //[ActionName("UpdateAction")]
        public IHttpActionResult Put(string code, ActionModel action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = this._IActionBusiness.UpdateAction(code, action);
                    if (isUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEACTIONFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEACTIONFAILED"));
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Action Detail By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="id">Action Id</param>
        /// <returns>return delete status</returns>
        [UserAuthorize(Actions = "CAN_DELETE_ACTION")]
        //[ActionName("RemoveAction")]
        public IHttpActionResult Delete(string code, int id)
        {
            try
            {
                bool isDeleted = this._IActionBusiness.RemoveAction(code, id);
                if (isDeleted)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEACTIONFAILED"));
            }
        }

        /// <summary>Remove Action From Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Common Model</param>
        /// <returns>return remove status</returns>
        [UserAuthorize(Actions = "CAN_DELETE_ACTIONFROMROLE")]
        [HttpPost, ActionName("RemoveActionFromRole")]
        public IHttpActionResult RemoveActionFromRole(string code, CommonModel action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isDeleted = this._IActionBusiness.RemoveActionFromRole(code, action);
                    if (isDeleted)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "REMOVEACTIONFROMROLEFAILED"));
            }
        }

        #endregion
    }
}
