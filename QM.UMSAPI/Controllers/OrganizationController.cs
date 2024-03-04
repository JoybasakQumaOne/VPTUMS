// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>18-10-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMSAPI.Controllers
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ExceptionHandling;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMSAPI.Authorize;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <Organization>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationController : ApiController
    {

        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IOrganizationBusiness _IOrganizationBusiness;
        private readonly IUsersBusiness _IUsersBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public OrganizationController(IOrganizationBusiness IOrganizationBusiness, IUsersBusiness _IUsersBusiness)
        {
            this._IOrganizationBusiness = IOrganizationBusiness;
            this._IUsersBusiness = _IUsersBusiness;
            this._IOrganizationBusiness.Init(Convert.ToString( System.Web.HttpContext.Current.Request.Headers["RequestId"]));
        }

        #endregion
        
        #region GET Methods

        /// <summary>Get Origanization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Origanization Details</returns>
        [HttpGet, ActionName("GetAllOrganizations")]
        [UserAuthorize(Actions = "CAN_VIEW_ALL_ORGANIZATION")]
        public IHttpActionResult Get()
        {
            try
            {
		
                List<OrganizationModel> organizationDetails = this._IOrganizationBusiness.GetAllOrganizations();
                if (organizationDetails != null && organizationDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, organizationDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ORGANIZATIONFOUNDFAILED"));
            }
        }

        /// <summary>Get Group Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Single Entity of Group Details</returns>
        [HttpGet, ActionName("GetOrganizationDetails")]
        //[UserAuthorize(Actions = "CAN_VIEW_ORGANIZATION")]
        public IHttpActionResult GetOrganizationDetails(Guid id)
        {
            try
            {
                OrganizationModel organizationDetails = this._IOrganizationBusiness.GetOrganizationDetails(id);
                if (organizationDetails != null && !string.IsNullOrEmpty(organizationDetails.Id))
                {
                    return this.Content(HttpStatusCode.OK, organizationDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ORGANIZATIONFOUNDFAILED"));
            }
        }

        /// <summary>Get Group Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Single Entity of Group Details</returns>
        [HttpGet, ActionName("GetOrganizationInfo")]
        //[UserAuthorize(Actions = "CAN_VIEW_ORGANIZATION")]
        public IHttpActionResult GetOrganizationInfo(string code)
        {
            try
            {
                OrganizationModel organizationDetails = this._IOrganizationBusiness.GetOrganizationInfo(code);
                if (organizationDetails != null && !string.IsNullOrEmpty(organizationDetails.Id))
                {
                    return this.Content(HttpStatusCode.OK, organizationDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ORGANIZATIONFOUNDFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Organization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddOrganization")]
        //[UserAuthorize(Actions = "CAN_ADD_Organization")]
        public IHttpActionResult Post(OrganizationModel Organization)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IOrganizationBusiness.AddOrganization(Organization);
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
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONFAILD"));
            }
        }


        /// <summary>Add Organization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("Register")]
        //[UserAuthorize(Actions = "CAN_ADD_Organization")]
        public IHttpActionResult Post(RegisterOrganizationModel Organization)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isInserted = this._IOrganizationBusiness.Register(Organization);
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
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONFAILD"));
            }
        }


        [HttpPost, ActionName("adduser")]
        //[UserAuthorize(Actions = "CAN_ADD_Organization")]
        public IHttpActionResult AddUser(UserProfileModel profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int id = this._IUsersBusiness.AddAdminUser(profile);
                    if (id>0)
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
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONFAILD"));
            }
        }


        /// <summary>Map Organization User Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization User Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddOrganizationUsers")]
        //[UserAuthorize(Actions = "CAN_ADD_Organization")]
        public IHttpActionResult Post(OrganizationUsersModel OrganizationUser)
        {
            try
            {
                bool isInserted = this._IOrganizationBusiness.AddOrganizationUsers(OrganizationUser);
                if (isInserted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Created.ToString(), "SUCCESS", "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONUSERFAILD"));
            }
        }

        /// <summary>Map Organization Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Type Model</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddOrgTypeMapping")]
        [UserAuthorize(Actions = "CAN_ADD_Organization")]
        public IHttpActionResult Post(List<OrganizationType> organizationTypeDetail, int OrgGuid)
        {
            try
            {
                bool isInserted = this._IOrganizationBusiness.AddOrgTypeMapping(organizationTypeDetail, OrgGuid);
                if (isInserted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Created.ToString(), "SUCCESS", "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(dEx.ErrorMessage, dEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDORGANIZATIONTYPEMAPFAILD"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Update Status</returns>
        [HttpPut, ActionName("Updateorganization")]
        [UserAuthorize(Actions = "CAN_UPDATE_ORGANIZATION")]
        public IHttpActionResult Put(OrganizationModel Organization)
        {
            try
            {
                bool isUpdated = this._IOrganizationBusiness.Updateorganization(Organization);
                if (isUpdated)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Modified.ToString(), "SUCCESS", "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEORGANIZATIONFAILED"));
            }
        }

        #endregion

        #region DELETE Methods

        [HttpDelete, ActionName("RemoveOrganizationDetails")]
        [UserAuthorize(Actions ="CAN_DELETE_ORGANIZATION")]
        public IHttpActionResult Delete(Guid id)
        {
            try
            {
                bool organizationDetails = this._IOrganizationBusiness.RemoveOrganizationDetails(id);

                if (organizationDetails)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Deleted.ToString(), "SUCCESS", "SUCCESS"));
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

        [HttpDelete, ActionName("RemoveOrgTypeMapping")]
        [UserAuthorize(Actions = "CAN_DELETE_ORGANIZATIONTYPE_MAPPING")]
        public IHttpActionResult DeleteOrgTypeMapping(int id)
        {
            try
            {
                bool organizationDetails = this._IOrganizationBusiness.RemoveOrgTypeMapping(id);

                if (organizationDetails)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Deleted.ToString(), "SUCCESS", "SUCCESS"));
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