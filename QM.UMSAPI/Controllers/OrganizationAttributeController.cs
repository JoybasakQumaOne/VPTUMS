// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationAttributeController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
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
    ///   Class:        <Organization Attribute>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationAttributeController : ApiController
    {

        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IOrganizationAttributeBusiness _IOrganizationAttributeBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public OrganizationAttributeController(IOrganizationAttributeBusiness _iOrganizationAttributeBusiness)
        {
            this._IOrganizationAttributeBusiness =  _iOrganizationAttributeBusiness;
        }

        #endregion

        #region GET Methods

        /// <summary>Get Origanization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Origanization Attribute Details</returns>
        [HttpGet, ActionName("GetAllOriganizationAttribute")]
        public IHttpActionResult Get()
        {
            try
            {
                if(ModelState.IsValid){
                    List<OrganizationAttribute> organizationAttribute = this._IOrganizationAttributeBusiness.GetAllOrganizationAttribute();
                    if (organizationAttribute != null && organizationAttribute.Count > 0)
                    {
                        return this.Content(HttpStatusCode.OK, organizationAttribute);
                    }
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
                } else {
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ORGANIZATIONATTRIBUTEFOUNDFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Organization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Attribute Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddOrganizationAttribute")]
        public IHttpActionResult Post(OrganizationAttribute organizationAttributes)
        {
            try
            {
                bool isInserted = this._IOrganizationAttributeBusiness.AddOrganizationAttributes(organizationAttributes);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONATTRIBUTEFAILD"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Organization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Attribute Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("UpdateOrganizationAttribute")]
        public IHttpActionResult Put(OrganizationAttribute organizationAttribute)
        {
            try
            {
                bool isUpdated = this._IOrganizationAttributeBusiness.UpdateOrganizationAttribute(organizationAttribute);
                if (isUpdated)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEGROUPFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEORGANIZATIONATTRIBUTEFAILED"));
            }
        }

        #endregion

        #region DELETE Methods

        [HttpDelete, ActionName("RemoveOrganizationDetails")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                bool organizationDetails = this._IOrganizationAttributeBusiness.RemoveOrganizationAttributes(id);

                if (organizationDetails)
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
            catch (Exception ex) {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEORGANIZATIONFAIL"));
            }
        }

        #endregion
    }
}