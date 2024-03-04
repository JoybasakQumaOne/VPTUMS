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
    public class DepartmentController : ApiController
    {
        // GET api/department

        #region Variable Declaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly IDepartmentBusiness _IDepartmentBusiness;
        #endregion

        #region Constructor
        /// <summary>
        /// Register Parameterized Constructor
        /// </summary>  
        /// <param name="IDepartmentBusiness"></param>
        public DepartmentController(IDepartmentBusiness IDepartmentBusiness)
        {
            this._IDepartmentBusiness = IDepartmentBusiness;
        }
        #endregion

        #region GET Methods

        /// <summary>Get list of Department Details</summary>
        /// <param name="code">code in Entity</param>
        /// <returns>Returns List of Department Details</returns>
        [ActionName("GetAllDepartment")]
        //[UserAuthorize(Actions = "CAN_VIEW_ALL_DEPARTMENT")]
        public IHttpActionResult Get()
        {
            try
            {
                List<DepartmentModel> departmentDetails = this._IDepartmentBusiness.GetAllDepartment();
                if (departmentDetails != null && departmentDetails.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, departmentDetails.OrderBy(x => x.DepartmentName).ToList());
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
        [HttpGet, ActionName("GetDepartmentById")]
        [UserAuthorize(Actions = "CAN_VIEW_DEPARTMENT")]
        public IHttpActionResult Get(string code, int departmentId)
        {
            try
            {
                DepartmentModel departmentDetail = this._IDepartmentBusiness.GetDepartment(code, departmentId);
                if (departmentDetail != null && departmentDetail.DepartmentId > 0)
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
        [ActionName("AddDepartment")]
        [UserAuthorize(Actions = "CAN_ADD_DEPARTMENT")]
        public IHttpActionResult Post(string code, DepartmentModel department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int IsCreated = this._IDepartmentBusiness.AddDepartment(code, department);
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
        [ActionName("UpdateDepartment")]
        [UserAuthorize(Actions = "CAN_UPDATE_DEPARTMENT")]
        public IHttpActionResult Put(string code, DepartmentModel department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool IsUpdated = this._IDepartmentBusiness.UpdateDepartment(code, department);
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
        [ActionName("DeleteDepartment")]
        [UserAuthorize(Actions = "CAN_DELETE_DEPARTMENT")]
        public IHttpActionResult Delete(string code, int departmentId)
        {
            try
            {
                bool IsDeleted = this._IDepartmentBusiness.DeleteDepartment(code, departmentId);
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
