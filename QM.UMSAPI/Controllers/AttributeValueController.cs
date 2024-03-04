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

namespace QM.UMSAPI.Controllers
{
    public class AttributeValueController : ApiController
    {
        #region variable Declaration
        private readonly IAttributeValueBusiness _AttributeValueBusiness;
        #endregion

        #region Constructor
        public AttributeValueController(IAttributeValueBusiness attributeValueBusiness)
        {
            this._AttributeValueBusiness = attributeValueBusiness;
        }
        #endregion

        #region Post
        [HttpPost, ActionName("AddAttributeValue")]
        //[UserAuthorize(Actions = "CAN_ADD_USERATTRIBUTE_MASTER")]
        public IHttpActionResult Post(AttributeValue attributeValue)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isAdded = this._AttributeValueBusiness.AddAttributeValue(attributeValue);
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
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDUSERATTRIBUTEVALUEFAILED"));
            }
        }
        #endregion
    }
}
