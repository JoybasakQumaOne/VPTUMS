// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationTypeController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>12-12-2017</createdOn>
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
    ///   Class:        <Organization Type>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationTypeController : ApiController
    {
        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IOrganizationTypeBusiness _IOrganizationTypeBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public OrganizationTypeController(IOrganizationTypeBusiness _iOrganizationTypeBusiness)
        {
            this._IOrganizationTypeBusiness = _iOrganizationTypeBusiness;
            this._IOrganizationTypeBusiness.Init(System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString());

        }

        #endregion

        #region GET Methods

        /// <summary>Get Origanization Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Origanization Type Details</returns>
        [HttpGet, ActionName("GetAllOrganizationTypes")]
        [UserAuthorize(Actions = "CAN_GET_ORGANIZATION_TYPE")]
        public IHttpActionResult Get()
        {
            try
            {
                List<OrganizationType> organizationDetails = this._IOrganizationTypeBusiness.GetAllOrganizationTypes(); 
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
        #endregion
    }
}