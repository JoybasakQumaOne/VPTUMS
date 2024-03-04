// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>13-12-2017</createdOn>
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
    ///   Class:        <Phone>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class PhoneController : ApiController
    {
        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IPhoneBusiness _IPhoneBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public PhoneController(IPhoneBusiness _iPhoneBusiness)
        {
            this._IPhoneBusiness = _iPhoneBusiness;

        }

        #endregion

        #region GET Method

        /// <summary>Get Phone Details</summary>
        /// <param name="ContactType">ContactType in Entity</param>
        /// <param name="ContactId">ContactId in Entity</param>
        /// <returns>List of Phone Details</returns>
        [HttpGet, ActionName("GetAllPhoneList")]
        [UserAuthorize(Actions="CAN_VIEW_PHONE_DETAILS")]
        public IHttpActionResult Get(string ControlType, int Id)
        {
            try
            {
                List<PhoneModel> phoneDetails = this._IPhoneBusiness.GetAllPhoneList(ControlType, Id);
                if (phoneDetails != null && phoneDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, phoneDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "PHONEFOUNDFAILED"));
            }
        }
        
        #endregion

        #region POST Method

        /// <summary>Add Organization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddPhoneDetails")]
        //[UserAuthorize(Actions="CAN_ADD_PHONE_DETAILS")]
        public IHttpActionResult Post(List<PhoneModel> phoneDetails, int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IPhoneBusiness.AddPhoneDetails(phoneDetails, Id);
                    if (isInserted)
                    {
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Created.ToString(), "SUCCESS", "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONFAILD"));
            }
        }
        
        #endregion

        #region PUT Method
        
        /// <summary>Update Phone Details</summary>
        /// <param name="Phone">Object of Phone Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("UpdatePhoneDetails")]
        [UserAuthorize(Actions="CAN_UPDATE_PHONE_DETAILS")]
        public IHttpActionResult Put(PhoneModel phoneDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = this._IPhoneBusiness.UpdatePhoneDetails(phoneDetails);
                    if (isUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPISuccessResponse(ResponseType.Modified.ToString(), "SUCCESS", "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEPHONEFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEPHONEFAILED"));
            }
        }

        /// <summary>Update Phone Details</summary>
        /// <param name="Phone">Object of Phone Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("UpdatePhoneDetailsList")]
        [UserAuthorize(Actions = "CAN_UPDATE_PHONE_DETAILS")]
        public IHttpActionResult Put(List<PhoneModel> phoneDetails)
        {
            try
            {
                bool isUpdated = this._IPhoneBusiness.UpdatePhoneDetailsList(phoneDetails);
                if (isUpdated)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPISuccessResponse(ResponseType.Modified.ToString(), "SUCCESS", "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEPHONEFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEPHONEFAILED"));
            }
        }
        
        #endregion

        #region DELETE Methods

        [HttpDelete, ActionName("RemovePhoneDetails")]
        [UserAuthorize(Actions = "CAN_DELETE_PHONE_DETAILS")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                bool organizationDetails = this._IPhoneBusiness.RemovePhoneDetails(id);

                if (organizationDetails)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPISuccessResponse(ResponseType.Deleted.ToString(), "SUCCESS", "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEORGANIZATIONFAIL"));
            }
        }
        #endregion
    }
}