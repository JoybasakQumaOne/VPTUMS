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

    public class UserTypeController : ApiController
    {
        #region variable Declaration
        private readonly IUserTypeBusiness _UserTypeBusiness;
        #endregion

        #region Constructor
        public UserTypeController(IUserTypeBusiness userTypeBusiness)
        {
            this._UserTypeBusiness = userTypeBusiness;
        }
        #endregion

        #region Get
        [HttpGet, ActionName("GetAllUserTypes")]
        public IHttpActionResult Get()
        {
            try
            {
                List<UserType> productAttributeOptions = this._UserTypeBusiness.Get();
                if (productAttributeOptions.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, productAttributeOptions);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETATTRIBUTEOPTIONFAILED"));
            }
        }
        #endregion

        #region Get
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                UserType userType = this._UserTypeBusiness.Get(id);
                if (userType!=null)
                {
                    return this.Content(HttpStatusCode.OK, userType);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETATTRIBUTEOPTIONFAILED"));
            }
        }
        #endregion

        #region Post
        [HttpPost]
        public IHttpActionResult Post(UserType userType)
        {
            try
            {
                bool isAdded = this._UserTypeBusiness.AddUserType(userType);
                if (isAdded)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERTYPEFAILED"));
            }
        }
        #endregion

        #region Put
        [HttpPut]
        public IHttpActionResult Put(UserType userType)  
        {
            try
            {
                bool isUpdate = this._UserTypeBusiness.UpdateUserType(userType);
                if (isUpdate)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "NOTSUCCESS"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(RepEx.ErrorCode, RepEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEUSERTYPEFAILED"));
            }
        }
        #endregion

        #region LinkAttribute
        [HttpPost]
        public IHttpActionResult Post(int UserTypeId, int AttributeId)    
        {
            try
            {
                bool isAdded = this._UserTypeBusiness.LinkAttribute(UserTypeId, AttributeId);
                if (isAdded)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "USERATTRIBUTEMAPPINGFAILED"));
            }
        }
        #endregion
    }

}
