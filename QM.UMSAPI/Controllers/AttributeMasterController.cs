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
    using System.Net.Http;
    using System.Web.Http;
    #endregion

    public class AttributeMasterController : ApiController
    {
        #region variable Declaration
        private readonly IAttributeMasterBusiness _AttributeMasterBusiness;
        #endregion

        #region Constructor
        public AttributeMasterController(IAttributeMasterBusiness attributeMasterBusiness)
        {
            this._AttributeMasterBusiness = attributeMasterBusiness;
        }
        #endregion

        #region GET
        public IHttpActionResult Get()
        {
            try
            {
                List<AttributeMaster> attributes = this._AttributeMasterBusiness.Get();
                if (attributes.Count > 0 && attributes != null)
                {
                    return this.Content(HttpStatusCode.OK, attributes);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETATTRIBUTEMASTERFAILED"));
            }
        }
        #endregion

        #region GET BY ID
        public IHttpActionResult Get(int Id)
        {
            try
            {
                AttributeMaster attribute = this._AttributeMasterBusiness.Get(Id);
                if (attribute != null)
                {
                    return this.Content(HttpStatusCode.OK, attribute);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETATTRIBUTEMASTERFAILED"));
            }
        }
        #endregion

        #region Post
        [HttpPost]
        //[UserAuthorize(Actions = "CAN_ADD_USERATTRIBUTE_MASTER")]
        public IHttpActionResult Post(AttributeMaster attributeMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isAdded = this._AttributeMasterBusiness.AddAttribute(attributeMaster);
                    if (isAdded)
                    {
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "NOTSUCCESS"));
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                }
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

        #region Update
        [HttpPut]
        //[UserAuthorize(Actions = "CAN_UPDATE_USERATTRIBUTE_MASTER")]
        public IHttpActionResult Put(AttributeMaster attributeMaster)
        {
            try
            {
                bool isUpdate = this._AttributeMasterBusiness.UpdateAttribute(attributeMaster);
                if (isUpdate)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEPATTRIBUTEMASTERFAILED"));
            }
        }
        #endregion

        #region Delete
        [HttpDelete]
        //[UserAuthorize(Actions = "CAN_UPDATE_USERATTRIBUTE_MASTER")]
        public IHttpActionResult Delete(int Id)
        {
            try
            {
                bool isDeleted = this._AttributeMasterBusiness.DeleteAttribute(Id);
                if (isDeleted)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "NOTSUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETEATTRIBUTEMASTERFAILED"));
            }
        }
        #endregion

        
    }
}
