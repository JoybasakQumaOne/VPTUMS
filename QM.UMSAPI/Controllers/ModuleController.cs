// ----------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>06-09-2017</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

using CommonApplicationFramework.Common;
using CommonApplicationFramework.ExceptionHandling;
using QM.UMS.Business.IBusiness;
using QM.UMS.Models;
using QM.UMSAPI.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QM.UMSAPI.Controllers
{
    public class ModuleController : ApiController
    {
       #region Variable Declaration
        /// <summary>
        /// This is private variable
        /// </summary>
        private readonly IModuleBusiness _IModuleBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="IActionBusiness">IActionBusinessLogic</param>
        public ModuleController(IModuleBusiness _iModuleBusiness)
        {
            this._IModuleBusiness = _iModuleBusiness;
        }
        #endregion

        //[UserAuthorize(Actions = "CAN_VIEW_ALL_ACTION")]
        [HttpGet, ActionName("GetAllModule")]
        public IHttpActionResult Get(string code)
        {
            try
            {
                List<ItemCode> moduleDetails = this._IModuleBusiness.GetAllModule(code);
                if (moduleDetails != null && moduleDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, moduleDetails);
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

        //[UserAuthorize(Actions = "CAN_VIEW_ALL_ACTION")]
        [HttpGet, ActionName("GetModulesByEmail")]
        public IHttpActionResult GetModulesByEmail(string code, string emailId)
        {
            try
            {
                List<ItemCode> moduleDetails = this._IModuleBusiness.GetModulesByEmail(code, emailId);
                if (moduleDetails != null && moduleDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, moduleDetails);
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

        [HttpGet, ActionName("GetModulesLinkedtoUser")]
        public IHttpActionResult GetModulesLinkedtoUser(string code)
        {
            try
            {
                List<DisplayUserModule> moduleDetails = this._IModuleBusiness.GetAllModuleLinktoUsers(code);
                if (moduleDetails != null && moduleDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, moduleDetails);
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

        [HttpPost, ActionName("AddModuleToUser")]
        public IHttpActionResult Post(string code, CommonModel userModule)
        {
            try
            {
                bool moduleDetails = this._IModuleBusiness.AddModuleToUser(code, userModule);
                if (moduleDetails)
                {
                    return this.Content(HttpStatusCode.OK, moduleDetails);
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

        [HttpPost, ActionName("RemoveModuleFromUser")]
        public IHttpActionResult RemoveModuleFromUser(string code, CommonModel userModule)
        {
            try
            {
                bool moduleDetails = this._IModuleBusiness.RemoveModuleFromUser(code, userModule);
                if (moduleDetails)
                {
                    return this.Content(HttpStatusCode.OK, moduleDetails);
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





    }
}
