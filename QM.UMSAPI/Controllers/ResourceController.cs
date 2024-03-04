// -----------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

namespace QM.UMSAPI.Controllers
{
    #region Namespace       
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ExceptionHandling;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    #endregion

    public class ResourceController : ApiController
    {
        #region Variable Declaration
        /// <summary>
        /// This is Private Variable
        /// </summary>
        private readonly IResourceBusiness _IResourceBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="IResourceBusinessLogic">IResourceBusinessLogic</param>
        public ResourceController(IResourceBusiness IResourceBusiness)
        {
            this._IResourceBusiness = IResourceBusiness;
        }
        #endregion

        #region GET Methods

        /// <summary>/// Get Resources By Id /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="Id">Resource Id</param>
        /// <returns>returns single entity of resource</returns>
        [HttpGet, ActionName("GetResourceById")]
        public IHttpActionResult Get(string code, int resourceId)
        {
            try
            {
                ResourceModel resourceDetails = this._IResourceBusiness.GetResourceById(code, resourceId);
                if (resourceDetails != null)
                {
                    return this.Content(HttpStatusCode.OK, resourceDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "RESOURCEFOUNDFAILED"));
            }
        }

        /// <summary> /// Get resources by roleid and modulecode /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="roleId">roleId</param>
        /// <param name="moduleCode">moduleCode</param>
        /// <returns>returns list of resources</returns>
        [HttpGet, ActionName("GetResourcesByRole")]
        public IHttpActionResult GetResourcesByRole(string code, int roleId, string moduleCode)
        {
            try
            {
                List<ResourceModel> resourceDetails = this._IResourceBusiness.GetResourcesByRole(code, roleId, moduleCode);
                if (resourceDetails != null && resourceDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, resourceDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "RESOURCEFOUNDFAILED"));
            }
        }
        
        /// <summary>Get role details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of roles</returns>
        [HttpGet, ActionName("GetResourceByModule")]
        public IHttpActionResult GetResourceByModule(string code, string Module)
        {
            try
            {
                List<ResourceModel> resourceDetails = this._IResourceBusiness.GetAllResourceByModule(code, Module);
                if (resourceDetails != null && resourceDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, resourceDetails);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "ROLEFOUNDFAILED"));
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
        
        /// <summary>Get RESOURCE details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of RESOURCE</returns>
        [HttpGet, ActionName("GetResourceByModuleUser")]
        public IHttpActionResult GetResourceByModuleUser(string uid)
        {
            try
            {
                List<object> resourceDetails = this._IResourceBusiness.GetAllResourceByModuleUser(uid);
                if (resourceDetails != null && resourceDetails.Count > 0)
                    return this.Content(HttpStatusCode.OK, resourceDetails);
                else
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "RESOURCESNOTASSIGNED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "RESOURCEBYUSERFOUNDFAILED"));
            }
        }

        #endregion
        
        #region POST Methods

        /// <summary>Add Resource</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of ResourceModel</param>
        /// <returns>returns bool value</returns>
        [HttpPost, ActionName("AddResource")]
        public IHttpActionResult AddResource(string code, ResourceModel resource)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IResourceBusiness.AddResource(code, resource);
                    if (isInserted)
                    {
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "ADDROLEFAILED"));
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

        /// <summary>/// Add Resources to Role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of CommonModel</param>
        /// <returns>returns true or false</returns>
        [HttpPost, ActionName("AddResourceToRole")]
        public IHttpActionResult AddResourceToRole(string code, CommonModel Resources)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IResourceBusiness.AddResourceToRole(code, Resources);
                    if (isInserted)
                    {
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDRESOURCESTOROLEFAILED"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update resource</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">role is a object of ResourceModel</param>
        /// <returns>returns bool value</returns>
        [HttpPut, ActionName("UpdateResource")]
        public IHttpActionResult UpdateResource(string code, ResourceModel resource)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = this._IResourceBusiness.UpdateResource(code, resource);
                    if (isUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATERESOURCEFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATERESOURCEFAILED"));
            }
        }
        
        #endregion

        #region DELETE Methods

        /// <summary> /// Remove resources from role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="resourceId">Resource Id</param>
        /// <returns>returns true or false</returns>
        [HttpDelete, ActionName("RemoveResourceFromRole")]
        public IHttpActionResult Delete(string code, int roleId, int resourceId)
        {
            try
            {
                bool isDeleted = this._IResourceBusiness.RemoveResourceFromRole(code, roleId, resourceId);
                if (isDeleted)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "REMOVERESOUCEFROMROLE"));
            }
        }

        #endregion
    }
}
