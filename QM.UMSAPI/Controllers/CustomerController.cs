// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerController.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Atanu</author>
// <createdOn>26-12-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------
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

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <Customer>
    ///   Description:  <Contains AddCustomer>
    ///   Author:       <Atanu>                    
    /// -----------------------------------------------------------------



    public class CustomerController : ApiController
    {

        #region VariableDeclaration
        /// <summary>
        /// Variable Declaration
        /// </summary>  
        private readonly ICustomerBusiness _ICustomerBusiness;
        #endregion

        #region Constructor

        /// <summary>Parameterized Constructor</summary>  
        /// <param name="IOrganizationBusiness">IOrganizationBusiness</param>
        public CustomerController(ICustomerBusiness _iCustomerBusiness)
        {
            this._ICustomerBusiness = _iCustomerBusiness;
            this._ICustomerBusiness.Init(System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString());

        }

        #endregion

        #region GET Methods

        /// <summary>Get Customer Address Details</summary>
        /// <param name="address Id">Address id in Entity</param>
        /// <returns>Address Details for Customer</returns>
        [HttpGet, ActionName("GetCustomerAddress")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                CustomerAddresses customerAddressDetail = this._ICustomerBusiness.GetCustomerAddress(id);
                if (customerAddressDetail != null && customerAddressDetail.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, customerAddressDetail);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CUSTOMERADDRESSFOUNDFAIL"));
            }
        }

        /// <summary>Get Customer Address Details</summary>
        /// <param name="address Id">Address id in Entity</param>
        /// <returns>Address Details for Customer</returns>
        [HttpGet, ActionName("GetCustomerProfileDetails")]
        public IHttpActionResult GetCustomerProfile(string customerid)
        {
            try
            {
                CustomerProfile customerProfileDetail = this._ICustomerBusiness.GetCustomerProfileDetails(customerid);
                if (customerProfileDetail != null && customerProfileDetail.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, customerProfileDetail);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CUSTOMERADDRESSFOUNDFAIL"));
            }
        }

        /// <summary>Get Customer Address Details</summary>
        /// <param name="address Id">Address id in Entity</param>
        /// <returns>Address Details for Customer</returns>
        [HttpGet, ActionName("ListCustomerAddress")]
        public IHttpActionResult GetAddress(string id)
        {
            try
            {
                List<CustomerAddresses> customerAddressDetail = this._ICustomerBusiness.GetCustomerAddressById(id);
                if (customerAddressDetail != null && customerAddressDetail.Count > 0)
                {
                    return this.Content(HttpStatusCode.OK, customerAddressDetail);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CUSTOMERADDRESSFOUNDFAIL"));
            }
        }

        [HttpGet, ActionName("VerifyCustomerOTP")]
        public IHttpActionResult VerifyCustomerOTP(string email, string otp)  
        {
            try
            {
                bool isValid = this._ICustomerBusiness.VerifyCustomerOTP(email, otp);
                if (isValid)
                {
                    return this.Content(HttpStatusCode.Accepted, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "OTPMATCHED"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "OTPUMMATCHED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "INVALIDINPUT"));
            }
        }

        [HttpGet, ActionName("GetCustomerAddresses")]
        public IHttpActionResult GetCustomerAddressDetail(string customerId)  
        {
            try
            {
                CustomerAddressDetail customerAddressDetail = this._ICustomerBusiness.GetCustomerAddressDetail(customerId);
                if (customerAddressDetail != null)
                {
                    return this.Content(HttpStatusCode.OK, customerAddressDetail);
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "NOCONTENTFOUND"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CUSTOMERADDRESSFOUNDFAIL"));
            }
        }
        #endregion

        #region POST Method

        /// <summary>Add Customer Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddCustomerDetails")]
        public IHttpActionResult Post(Customer customer)
        {
            try
            {
                int isInserted = this._ICustomerBusiness.AddCustomer(customer);
                if (isInserted > 0)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), isInserted + ""));
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
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMER"));
            }

        }

		[HttpPost, ActionName("AddCustomerDetails")]
		public IHttpActionResult RegisterCustomer(Customer customer, string bookGuid)
		{
			try
			{
				int isInserted = this._ICustomerBusiness.AddCustomer(customer, bookGuid);
				if (isInserted > 0)
				{
					return this.Content(HttpStatusCode.Created, APIResponse.CreateAPISuccessResponse(ResponseType.Created.ToString(), "SUCCESS", isInserted + ""));
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
				return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse(dEx.ErrorCode, dEx.ErrorMessage));
			}
			catch (Exception ex)
			{
				return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMER"));
			}
		}

        [HttpPost, ActionName("Send")]
        public IHttpActionResult ResendVerificationMail(string mail)
        {
            try
            {
                var data = this._ICustomerBusiness.ResendVerificationMail(mail);
                return this.Content(HttpStatusCode.Created, data);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMER"));
            }

        }

        [HttpPost, ActionName("AddCustomerDetailsFacebook")]
        public IHttpActionResult PostAddCustomerDetailsFacebook(string code, string state, string scope)
        {
            try
            {
                int isInserted = this._ICustomerBusiness.AddCustomerfacebook(code, state, scope);
                if (isInserted > 0)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMER"));
            }

        }
        /// <summary>Forgot Password</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>Request Status</returns>
     
        [HttpPost, ActionName("ForgotPassword")]
        //[UserAuthorize(Actions = "CAN_FORGOT_PASSWORD")]
        public IHttpActionResult Post(string emailId)
        {
            try
            {
                var clientInfo = this._ICustomerBusiness.ForgotPassword(emailId);//, storedLocation,orgLogo,orgName,CompanyCode);
                if (clientInfo != null && clientInfo.Id > 0)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                else
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "USERNOTFOUND"));
            }
            catch (RepositoryException RepEx) 
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(RepEx.ErrorMessage, RepEx.ErrorCode));
            }
            catch (BusinessException bEx)
            {
                return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateErrorResponse( bEx.ErrorCode, bEx.ErrorMessage));
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ERROROCCURE"));
            }
        }

        [HttpPost, ActionName("AddCustomerRole")]
        public IHttpActionResult PostAddCustomerRole(GroupCustomerModel groupCustomerRole)
        {
            try
            {
                int isInserted = this._ICustomerBusiness.AddCustomerRole(groupCustomerRole);
                if (isInserted > 0)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMERROLE"));
            }

        }

        /// <summary>Add Customer Address</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddCustomerAddress")]
        public IHttpActionResult PostAddress(CustomerAddresses customer)
        {
            try
            {
                int isInserted = this._ICustomerBusiness.AddCustomerAddress(customer);
                if (isInserted > 0)
                {
                    return this.Content(HttpStatusCode.Created, new { Id=isInserted});
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMER"));
            }

        }

        /// <summary>Add Customer Address</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Insert Status</returns>
        [HttpPost, ActionName("AddCustomerProfile")]
        public IHttpActionResult PostProfile(CustomerProfile customer)
        {
            try
            {
                int isInserted = this._ICustomerBusiness.AddCustomerProfile(customer);
                if (isInserted > 0)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Created.ToString(), "SUCCESS"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMERPROFILE"));
            }

        }
        #endregion
        
        #region PUT Methods

        /// <summary>Change Password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Change Status</returns>
        [HttpPut, ActionName("ChangePassword")]
        //[UserAuthorize(Actions = "CAN_CHANGE_PASSWORD")]
        public IHttpActionResult Put(ChangePassword changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int isUpdated = this._ICustomerBusiness.ChangePassword(changePassword);
                    switch (isUpdated)
                    {
                        case 1: return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                        case 2: return this.Content(HttpStatusCode.NotAcceptable, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "PASSWORDMATCH"));
                        case 3: return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDPASSWORD"));
                        default: return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "CUSTOMERNOTFOUND"));
                    }
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
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        /// <summary>Reset password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Reset value</returns>
        [HttpPut, ActionName("ResetPassword")]
        //[UserAuthorize(Actions = "CAN_RESET_PASSWORD")]
        public IHttpActionResult ResetPassword(ChangePassword changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int resetPasswordStatus = this._ICustomerBusiness.ResetPassword(changePassword);
                    switch (resetPasswordStatus)
                    {
                        case 1: return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                        case 2: return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDPASSWORD"));
                        default: return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "CUSTOMERNOTFOUND"));
                    }
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
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        [HttpPost, ActionName("OTP")]
        //[UserAuthorize(Actions = "CAN_RESET_PASSWORD")]
        public IHttpActionResult ValidateOTP(ChangePassword changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExists = this._ICustomerBusiness.ValidateOTP(changePassword);
                    if(isExists)
                    {
                       return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                    }
                    return this.Content(HttpStatusCode.NoContent, APIResponse.CreateAPIResponse(ResponseType.NoContent.ToString(), "SUCCESS"));
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
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        /// <summary>Reset password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Reset value</returns>
        [HttpPut, ActionName("UpdateCustomerAddress")]
        public IHttpActionResult PutCustomerAddress(CustomerAddresses customerAddress)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    int resetPasswordStatus = this._ICustomerBusiness.UpdateCustomerAddress(customerAddress);
                    switch (resetPasswordStatus)
                    {
                        case 1: return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Modified.ToString(), "SUCCESS"));
                        case 2: return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "INVALIDPASSWORD"));
                        default: return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotModified.ToString(), "CUSTOMERNOTFOUND"));
                    }
                //}
                //else
                //{
                //    var errorMessage = string.Join("<br/>", ModelState.Values.Where(v => v.Errors.Count > 0).SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                //    return this.Content(HttpStatusCode.BadRequest, APIResponse.CreateAPIResponse(ResponseType.InvalidRequest.ToString(), errorMessage));
                //}
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "CHANGEPASSWORDFAILED"));
            }
        }

        [HttpPut, ActionName("UpdateCustomerProfile")]
        //[UserAuthorize(Actions = "CAN_UPDATE_CUSTOMERPROFILE")]
        public IHttpActionResult Put(CustomerProfile customerProfile)  
        {
            try
            {
                int cusProfile = this._ICustomerBusiness.UpdateCustomerProfile(customerProfile);
                if (cusProfile > 0)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "SUCCESS"));
                }
                else
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotCreated.ToString(), "USERNOTFOUND"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ERROROCCURE"));
            }
        }

        [HttpPut, ActionName("ResendOTP")]
        //[UserAuthorize(Actions = "CAN_UPDATE_CUSTOMERPROFILE")]
        public IHttpActionResult Put(string email)
        {
            try
            {
                bool isSent = this._ICustomerBusiness.ResendOTP(email);
                if (isSent)
                {
                    return this.Content(HttpStatusCode.OK, APIResponse.CreateAPIResponse(ResponseType.Success.ToString(), "RESENDOTPSUCCESS"));
                }
                else
                    return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotFound.ToString(), "RESENDOTPFAILED"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "RESENDOTPFAILED"));
            }
        }

        #endregion

        #region DELETE Method

        [HttpDelete, ActionName("DeleteCustomerRole")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                int isDeleted = this._ICustomerBusiness.deleteCustomerRole(id);
                if (isDeleted > 0)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETECUSTOMERROLE"));
            }

        }

        [HttpDelete, ActionName("DeleteCustomerAddress")]
        public IHttpActionResult DeleteAddress(int id)
        {
            try
            {
                int isDeleted = this._ICustomerBusiness.deleteCustomerAddress(id);
                if (isDeleted > 0)
                {
                    return this.Content(HttpStatusCode.Created, APIResponse.CreateAPIResponse(ResponseType.Deleted.ToString(), "SUCCESS"));
                }
                return this.Content(HttpStatusCode.NotFound, APIResponse.CreateAPIResponse(ResponseType.NotDeleted.ToString(), "INVALIDINPUT"));
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "DELETECUSTOMERROLE"));
            }

        }

        #endregion

        #region Social Login Details
        [HttpPost, ActionName("SocialMediaLogin")]
        public IHttpActionResult SocialMediaLogin(string socialMediaUserEmail)
        {
            try
            {
                int intSocialLogin = this._ICustomerBusiness.SocialMediaUser(socialMediaUserEmail);
                if (intSocialLogin > 0)
                {
                    return this.Content(HttpStatusCode.OK, intSocialLogin);
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
                return this.Content(HttpStatusCode.BadRequest, ExceptionManager.HandleException(ex, ResponseType.Failure.ToString(), "ADDCUSTOMER"));
            }
        }
        #endregion
    }
}