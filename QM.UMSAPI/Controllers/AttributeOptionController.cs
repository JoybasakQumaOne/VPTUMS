namespace QM.UMSAPI.Controllers
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ExceptionHandling;
    using QM.UMS.Business.IBusiness;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    #endregion

    public class AttributeOptionController : ApiController
    {
        #region variable Declaration
        private readonly IAttributeOptionBusiness _AttributeOptionBusiness;
        #endregion

        #region Constructor
        public AttributeOptionController(IAttributeOptionBusiness attributeOptionsBusiness)
        {
            this._AttributeOptionBusiness = attributeOptionsBusiness;
        }
        #endregion

        #region Get
        [HttpGet]
        public IHttpActionResult Get(int Id)
        {
            try
            {
                List<Item> productAttributeOptions = this._AttributeOptionBusiness.GetAttributeOptions(Id);
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

        #region Post
        [HttpPost]
        public IHttpActionResult Post(Item attributeOptions)
        {
            try
            {
                bool isAdded = _AttributeOptionBusiness.AddAttributeOptions(attributeOptions);
                if (isAdded)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDATTRIBUTEOPTIONFAILED"));
            }
        }
        #endregion

        #region Put
        [HttpPut]
        public IHttpActionResult Put(Item attributeOptions)  
        {
            try
            {
                bool isAdded = _AttributeOptionBusiness.UpdateAttributeOptions(attributeOptions);
                if (isAdded)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "NOCONTENT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEATTRIBUTEOPTIONFAILED"));
            }
        }
        #endregion

        #region Delete
        [HttpDelete]
        public IHttpActionResult Delete(int Id)  
        {
            try
            {
                bool isDelete = _AttributeOptionBusiness.DeleteAttributeOptions(Id);
                if (isDelete)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UPDATEATTRIBUTEOPTIONFAILED"));
            }
        }
        #endregion
    }
}
