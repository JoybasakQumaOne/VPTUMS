using CommonApplicationFramework.Common;
using CommonApplicationFramework.ExceptionHandling;
using QM.UMS.Business.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QM.UMSAPI.Controllers
{
    public class InstanceController : ApiController
    {
        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IInstanceBusiness _InstanceBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public InstanceController(IInstanceBusiness InstanceBusiness)
        {
            this._InstanceBusiness = InstanceBusiness;
        }

        #endregion
        /// <summary>
        /// Get Instance
        /// </summary>
        /// <returns>List of Instance</returns>
        [HttpGet, ActionName("GetInstance")]
        // [UserAuthorize(Actions = "CAN_VIEW_ALL_ORGANIZATION")]
        public IHttpActionResult Get()
        {
            try
            {
                List<Item> instances = this._InstanceBusiness.GetInstance();
                if (instances != null && instances.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, instances);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "INSTANCEFOUNDFAILED"));
            }
        }

        #region Post Map Attribute
        [HttpPost, ActionName("MapInstance")]
        public IHttpActionResult Post(int UserId, int InstanceId)  
        {
            try
            {
                bool isAffacted = this._InstanceBusiness.MapUserInstance(UserId, InstanceId);  
                if (isAffacted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "NOTSUCCESS"));

            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(RepEx.ErrorCode, RepEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "MAPPINGPUSERINSTANCEFAILED"));
            }
        }
        #endregion

        #region Post Un Map Attribute
        [HttpPost, ActionName("UnMapInstance")]
        public IHttpActionResult Post(long UserId, long InstanceId)
        {
            try
            {
                bool isAffacted = this._InstanceBusiness.UnMapUserInstance(Convert.ToInt32(UserId), Convert.ToInt32(InstanceId));
                if (isAffacted)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "NOTSUCCESS"));
            }
            catch (RepositoryException RepEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(RepEx.ErrorCode, RepEx.ErrorMessage));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (DuplicateException dEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "UNMAPPINGPUSERINSTANCEFAILED"));
            }
        }
        #endregion
    }
}
