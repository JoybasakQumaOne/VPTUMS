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

namespace QM.UMSAPI.Controllers
{
    public class DesignationController : ApiController
    {
       
        #region Variable Declaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IDesignationBusiness _IDesignationBusiness;
        #endregion

        #region Constructor
        /// <summary>
        /// Register Parameterized Constructor
        /// </summary>  
        /// <param name="IDepartmentBusiness"></param>
        public DesignationController(IDesignationBusiness _IDesignationBusiness)
        {
            this._IDesignationBusiness = _IDesignationBusiness;
        }
        #endregion

        #region GET Methods

        /// <summary>Get list of Department Details</summary>
        /// <param name="code">code in Entity</param>
        /// <returns>Returns List of Department Details</returns>
        [ActionName("GetAllDesignation")]
        //[UserAuthorize(Actions = "CAN_VIEW_ALL_DEPARTMENT")]
        public IHttpActionResult Get()
        {
            try
            {
                List<DesignationModel> departmentDetails = this._IDesignationBusiness.GetAllDesignation();
                if (departmentDetails != null && departmentDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, departmentDetails.OrderBy(x => x.DesignationName).ToList());
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NoContent.ToString(), "DEPARTMENTNOTFOUND"));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETDEPARTMENTFAILED"));
            }
        }

        /// <summary>Get Department Details by Id</summary>
        /// <param name="code">code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>Returns single entity of Department</returns>
        [HttpGet, ActionName("GetDesignationById")]
        //[UserAuthorize(Actions = "CAN_VIEW_DEPARTMENT")]
        public IHttpActionResult Get(string code, int designationId)
        {
            try
            {
                DesignationModel departmentDetail = this._IDesignationBusiness.GetDesignation(code, designationId);
                if (departmentDetail != null && departmentDetail.DesignationId > 0)
                {
                    return this.Content(HttpStatusCode.OK, departmentDetail);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NoContent.ToString(), "DEPARTMENTNOTFOUND"));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "GETDEPARTMENTFAILED"));
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="dept">dept is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>
        [ActionName("AddDesignation")]
      //  [UserAuthorize(Actions = "CAN_ADD_DEPARTMENT")]
        public IHttpActionResult Post(string code, DesignationModel designation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int IsCreated = this._IDesignationBusiness.AddDesignation(code, designation);
                    if (IsCreated > 0)
                    {
                        return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "ADDDEPARTMENTSUCCESS", IsCreated.ToString()));
                    }
                    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "ADDDEPARTMENTFAILED"));
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, !string.IsNullOrEmpty(errorMessage) ? APIResponse.CreateAPIResponse(errorMessage) : APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), "ADDBRANDFAILED"));
                }
            }
            catch (DuplicateException dex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Duplicate.ToString(), "ADDDEPARTMENTNOTSUCCESS"));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "ADDDEPARTMENTFAILED"));
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="department">department is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>
        [ActionName("UpdateDesignation")]
        [UserAuthorize(Actions = "CAN_UPDATE_DEPARTMENT")]
        public IHttpActionResult Put(string code, DesignationModel designation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool IsUpdated = this._IDesignationBusiness.UpdateDesignation(code, designation);
                    if (IsUpdated)
                    {
                        return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "UPDATEDEPARTMENTSUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NotModified, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "UPDATEDEPARTMENTFAILED"));
                }
                else
                {
                    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return this.Content(HttpStatusCode.BadRequest, !string.IsNullOrEmpty(errorMessage) ? APIResponse.CreateAPIResponse(errorMessage) : APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), "UPDATEBRANDFAILED"));
                }
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "UPDATEDEPARTMENTFAILED"));
            }
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>returns bool value</returns>
        [ActionName("DeleteDesignation")]
        [UserAuthorize(Actions = "CAN_DELETE_DEPARTMENT")]
        public IHttpActionResult Delete(string code, int designationId)
        {
            try
            {
                bool IsDeleted = this._IDesignationBusiness.DeleteDesignation(code, designationId);
                if ((IsDeleted))
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "DELETEDEPARTMENTSUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "DELETEDEPARTMENTFAILED"));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(bEx.ResponseType.ToString(), bEx.ErrorCode));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.Failure.ToString(), "DELETEDEPARTMENTFAILED"));
            }
        }

        #endregion
    }
}
