// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeSectionController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>06-12-2017</createdOn>
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
    ///   Class:        <AttributeSection>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------
    /// 

    public class AttributeSectionController : ApiController
    {

        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IAttributeSectionBusiness _IAttributeSectionBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public AttributeSectionController(IAttributeSectionBusiness _iAttributeSectionBusiness)
        {
            this._IAttributeSectionBusiness = _iAttributeSectionBusiness;
        }

        #endregion

        #region GET Methods

        /// <summary>Get Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Attribute Section Details</returns>
        [HttpGet, ActionName("GetAllAttributeSection")]
        public IHttpActionResult Get()
        {
            try
            {
                List<AttributeSection> attributeSectionDetails = this._IAttributeSectionBusiness.GetAllAttributeSections();
                if (attributeSectionDetails != null && attributeSectionDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, attributeSectionDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ATTRIBUTESECTIONFOUNDFAILED"));
            }
        }

        /// <summary>Get Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Attribute Section Details</returns>
        [HttpGet, ActionName("GetAttributeSectionsById")]
        public IHttpActionResult GetAttributeSectionsById(int Id)
        {
            try
            {
                AttributeSection attributeSectionDetails = this._IAttributeSectionBusiness.GetAttributeSectionsById(Id);
                if (attributeSectionDetails != null && attributeSectionDetails.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, attributeSectionDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ATTRIBUTESECTIONFOUNDFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Attribute Section Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddAttributeSection")]
        public IHttpActionResult Post(AttributeSection attributeSection)
        {
            try
            {
                bool isInserted = this._IAttributeSectionBusiness.AddAttributeSection(attributeSection);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDATTRIBUTESECTIONFAILD"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Attribute Section Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("UpdateAttributeSection")]
        public IHttpActionResult Put(AttributeSection attributeSection)
        {
            try
            {
                bool isUpdated = this._IAttributeSectionBusiness.UpdateAttributeSection(attributeSection);
                if (isUpdated)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEATTRIBUTESECTIONFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEATTRIBUTESECTIONFAILED"));
            }
        }

        #endregion

        #region DELETE Methods

        [HttpDelete, ActionName("RemoveAttributeSection")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                bool attributeSectionDetails = this._IAttributeSectionBusiness.RemoveAttributeSection(id);

                if (attributeSectionDetails)
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
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex) {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEATTRIBUTESECTIONFAIL"));
            }
        }

        #endregion

        #region GetAttributeSectionByGroupId
        [HttpGet, ActionName("GetUserAttributeSection")]
        public IHttpActionResult Get(int UserId)          
        {
            try
            {
                List<AttributeSection> attributeSection = this._IAttributeSectionBusiness.GetUserAttibuteByUserId(UserId);
                if (attributeSection.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, attributeSection);
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "NOTSUCCESS"));

            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(RepEx.ErrorCode, RepEx.ErrorMessage));
            }
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDPATTRIBUTEMASTERFAILED"));
            }
        }
        #endregion
    }
}