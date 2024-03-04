// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>04-12-2017</createdOn>
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
    ///   Class:        <Master>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class MasterController : ApiController
    {

        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IMasterBusiness _IMasterBusiness;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public MasterController(IMasterBusiness IMasterBusiness)
        {
            this._IMasterBusiness = IMasterBusiness;
        }

        #endregion

        #region GET Methods

        /// <summary>Get Country Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Country Details</returns>
        [HttpGet, ActionName("GetAllCountries")]
        //[UserAuthorize(Actions = "CAN_VIEW_COUNTRY")]
        public IHttpActionResult Get()
        {
            try
            {
                List<CountryModel> countryDetails = this._IMasterBusiness.GetAllCountries();
                if (countryDetails != null && countryDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, countryDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "COUNTRYFOUNDFAILED"));
            }
        }

        /// <summary>Get Country Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Country Details</returns>
        [HttpGet, ActionName("GetAllStates")]
        [UserAuthorize(Actions = "CAN_VIEW_COUNTRY")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                List<StateProvinceModel> StateDetails = this._IMasterBusiness.GetAllStates(id);
                if (StateDetails != null && StateDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, StateDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "STATESFOUNDFAILED"));
            }
        }

        /// <summary>Get Country Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Country Details</returns>
        [HttpGet, ActionName("GetInstanceList")]
        [UserAuthorize(Actions = "CAN_VIEW_INSTANCE_LIST")]
        public IHttpActionResult GetList()
        {
            try
            {
                List<DBViewModel> StateDetails = this._IMasterBusiness.GetInstanceDetails();
                if (StateDetails != null && StateDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, StateDetails);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "STATESFOUNDFAILED"));
            }
        }
        #endregion
    }
}
