// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>13-12-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.Business
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
	using CommonApplicationFramework.Logging;
	using CommonApplicationFramework.Notification;
    using CommonApplicationFramework.Security;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    #endregion


    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <CustomerBusiness>
    ///   Description:  <Contains AddCustomer>
    ///   Author:       <Atanu>                    
    /// -----------------------------------------------------------------

    public class CustomerBusiness : ICustomerBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly ICustomerRepository _ICustomerRepository;
        private readonly IDocumentProcessRepository _IDocumentProcessRepository;
        private readonly ICommonHelperRepository _ICommonHelperRepository;
        private DBManager dbManager;
        #endregion

        #region Properties
        public string RequestId { get; set; }
        public string Code { get; set; }
        #endregion

        #region Constructor
        public CustomerBusiness(ICustomerRepository _iCustomerRepository, IDocumentProcessRepository _IDocumentProcessRepository, ICommonHelperRepository _ICommonHelperRepository)
        {
            this._ICustomerRepository = _iCustomerRepository;
            this._IDocumentProcessRepository = _IDocumentProcessRepository;
            this._ICommonHelperRepository = _ICommonHelperRepository;
        }
        #endregion

        public void Init(string requestId)
        {
            this.RequestId = requestId;
            this._ICustomerRepository.RequestId = requestId;
        }

        #region GET Method

        /// <summary>Get Customer Address Details</summary>
        /// <param name="address id">Address Id in Entity</param>
        /// <returns>Address Details for Customer</returns>
        public CustomerAddresses GetCustomerAddress(int addressId)
        {
            return this._ICustomerRepository.GetCustomerAddress(addressId);
        }

        /// <summary>Get Customer Profile Details</summary>
        /// <param name="address id">Customer Id in Entity</param>
        /// <returns>Profile Details for Customer</returns>
        public CustomerProfile GetCustomerProfileDetails(string customerid)
        {
            return this._ICustomerRepository.GetCustomerProfileDetails(customerid);
        }

        /// <summary>Forgot Password</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>Request Status</returns>
        public Customer ForgotPassword(string emailId)//, string storedLocation, string OrgLogo, string orgName, string CompanyCode)
        {
            EmailSenderModel email = new EmailSenderModel();
            Customer userEntity = new Customer();
            try
            {
                Regex regex = new Regex(@"^(([^<>()\[\]\\.,;:\s@""]+(\.[^<>()\[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
                Match match = regex.Match(emailId);
                if (match.Success)
                {
                    userEntity = new Customer();
                    userEntity = this._ICustomerRepository.GetCustomerDetailsByEmail(emailId);
                    if (userEntity != null && userEntity.Id>0)
                    {
                        if (this._ICustomerRepository.IsVarifiedCustomer(emailId))
                        {
                            if (userEntity.Id > 0)
                            {
                                email = new EmailSenderModel();
                                email.Id = userEntity.Id;
                                email.Name = userEntity.customerProfile.FirstName;
                                email.Salt = Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
                                email.EmailId = userEntity.Email;
                                email.FirstName = userEntity.customerProfile.FirstName;
                                this._ICustomerRepository.UpdatePasswordResetCode(emailId, email.Salt);

                                email.CompanyCode = "BOOK";// userContext.CurCompanyCode;
                                email.CompanyName = "The Online Book Company";// orgName;
                                email.IsFirstLogin = true;
                                //email.CompanyLogo = filePath;
                                // companyInfo = null;// this._IUsersRepository.GetCompanyInfo();
                                //  if (companyInfo != null)
                                //  email.CompanyName = companyInfo.CompanyName;
                                EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERPASSWORDREMAINDER"], "");
                                _ICommonHelperRepository.UpdateActivity(ActivityType.PASSWORDRESET.ToString(), ActivityType.PASSWORDRESET.ToString());
                                return userEntity;
                            }
                            return userEntity;
                        }
                        else
                        {
                            throw new BusinessException("NOTVARIFIEDEMAIL", MessageConfig.MessageSettings["NOTVARIFIEDEMAIL"].ToString(), string.Empty);
                        }
                    }
                    else
                    {
                        throw new BusinessException("USEREMAILNOTFOUND", MessageConfig.MessageSettings["USEREMAILNOTFOUND"].ToString(), string.Empty);
                    }
                }
                else
                {
                    throw new BusinessException("INVALIDEMAIL", MessageConfig.MessageSettings["INVALIDEMAIL"].ToString(), string.Empty);
                }
            }
            catch (BusinessException repEx)
            {
                throw new BusinessException(repEx.ErrorCode, repEx.ErrorMessage, string.Empty);
            }
            catch (Exception ex)
            {
                throw new BusinessException("ERROROCCURE", ex.Message, ex.StackTrace);
            }
            finally
            {
                email = null;
                userEntity = null;
            }
        }

        public List<CustomerAddresses> GetCustomerAddressById(string id)
        {
            return this._ICustomerRepository.GetCustomerAddressById(id);
        }

        public bool VerifyCustomerOTP(string email, string otp)
        {
            bool status = this._ICustomerRepository.VerifyCustomerOTP(email, otp);
            
            return status;
        }

        public CustomerAddressDetail GetCustomerAddressDetail(string customerId)
        {
            return this._ICustomerRepository.GetCustomerAddressDetail(customerId);
        }

        #endregion

        #region POST Methods

        /// <summary>Add New ustomer Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Add ustomer Details</returns>
        public int AddCustomer(Customer customer, string bookGuid = "")
        {
			UriBuilder uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
			string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
			path = path.Substring(0, path.Length - 3);
			string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResourceFileServer")).Value.ToString(); // path + "Resources" + @"\Book.png";
			AttachmentFile image = new AttachmentFile();
            EmailSenderModel email = null;
            customer.OTP = Guid.NewGuid().ToString();
            customer.IsOTPVerified = false;
            SecureModel sc = new SecureModel(customer.Password);
            customer.Password = sc.Password;
            customer.Salt = sc.Salt;
            int flag = 0;

            flag = this._ICustomerRepository.AddCustomer(customer, bookGuid);

            if (flag > 0)
            {
                email = new EmailSenderModel();
                email.Id = flag;
                email.Name = customer.customerProfile.FirstName;
                email.Password = customer.OTP;
                email.FirstName = customer.customerProfile.FirstName;
                email.EmailId = customer.Email;
                email.CompanyCode = customer.CompanyCode;
                email.IsFirstLogin = true;
                email.CompanyName = customer.OrgnizationName;
                email.FilePath = filePath;
                EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERNEWREGISTRATION"], string.Empty);
                return flag;
            }
            return flag;
        }

        public object ResendVerificationMail(string mail)
        {
            Customer customer= _ICustomerRepository.GetCustomerDetailsByEmail(mail);
            if (customer !=null)
            {
                if(!customer.IsOTPVerified)
                {
                    EmailSenderModel email = new EmailSenderModel();
                    email.Name = customer.customerProfile.FirstName;
                    email.Password = customer.OTP;
                    email.FirstName = customer.customerProfile.FirstName;
                    email.EmailId = customer.Email;
                    email.CompanyCode = customer.CompanyCode;
                    email.IsFirstLogin = true;
                    email.CompanyName = customer.OrgnizationName;
                    email.FilePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResourceFileServer")).Value.ToString();
                    EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERNEWREGISTRATION"], string.Empty);
                    //Task.Run(() => EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERNEWREGISTRATION"], string.Empty));
                    return new { code = "SUCCESS" };

                }
                else
                {
                    return new { code = "VERIFIED" };
                }
            }
            else
            {
                return new { code = "NOTFOUND" };
            }

        }
        public int AddCustomerfacebook(string code, string state, string scope)
        {
            return this._ICustomerRepository.AddCustomerfacebook(code, state, scope);
        }

        public int AddCustomerRole(GroupCustomerModel groupCustomer)
        {
            return this._ICustomerRepository.AddCustomerRole(groupCustomer);
        }

        public int AddCustomerAddress(CustomerAddresses customerAddress)
        {
            return this._ICustomerRepository.AddCustomerAddress(customerAddress);
        }

        public int AddCustomerProfile(CustomerProfile customerProfile)
        {
            return this._ICustomerRepository.AddCustomerProfile(customerProfile);
        }

        #endregion

        #region PUT Methods

        /// <summary>Change Password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Change Status</returns>
        public int ChangePassword(ChangePassword changePassword)
        {
            bool Status = false;
            PasswordHasher passwordHasher = new PasswordHasher();
            Customer customerModel = new Customer();
            Customer customerDetail = new Customer();
            EmailSenderModel email = new EmailSenderModel();
            string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResourceFileServer")).Value.ToString() + "Resources" + "/" + @"/Book.png";
            customerDetail.customerProfile = new CustomerProfile();
            try
            {
                //string testData = CacheManager.Instance.Get("usercontext_1");// + this.RequestId.ToString());
                //UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString()));
                int flag = 1;
                if (changePassword.NewPassword.Equals(changePassword.OldPassword))
                {
                    return flag += 1;
                }
                customerDetail = this._ICustomerRepository.GetCustomerDetailsByEmail(changePassword.UserName);
                if (customerDetail != null)
                {
                    var validPass = passwordHasher.ValidatePassword(changePassword.OldPassword, customerDetail.Salt, customerDetail.Password) ? customerDetail : null;
                    if (validPass != null)
                    {
                        customerModel.Password = changePassword.NewPassword;
                        SecureModel sc = new SecureModel(customerModel.Password);
                        customerModel.Id = customerDetail.Id;
                        customerModel.Password = sc.Password;
                        customerModel.Salt = sc.Salt;
                        int passwordExpiry = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value);
                        Status = this._ICustomerRepository.ResetPassword(customerModel);
                        email = new EmailSenderModel();
                        email.Id = customerDetail.Id;
                        email.Name = customerDetail.Username;
                        email.FirstName = customerDetail.customerProfile.FirstName;
                        email.Password = customerDetail.Salt;
                        email.EmailId = customerDetail.Email;
                        email.CompanyCode = changePassword.CompanyCode;
                        email.FilePath = filePath;
                        email.CompanyLogo = filePath;
                        email.IsFirstLogin = true;
                        EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CHANGEPASSWORD"], string.Empty);//companyInfo.CompanyName
                        _ICommonHelperRepository.UpdateActivity(ActivityType.PASSWORDCHANGED.ToString(), ActivityType.PASSWORDCHANGED.ToString());
                        return flag;
                    }
                }
                return flag += 2;
            }
            catch (BusinessException repEx)
            {
                throw new BusinessException("CHANGEPASSWORDFAILED", repEx.Message, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new BusinessException("CHANGEPASSWORDFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                passwordHasher = null;
                customerModel = null;
                email = null;
            }
        }

        /// <summary>Reset password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Reset value</returns>
        public int ResetPassword(ChangePassword changePassword)
        {
            Customer userModel = new Customer();
            CustomerModel usersDetail = new CustomerModel();
            EmailSenderModel email = new EmailSenderModel();
            CustomerModel user = new CustomerModel();
            try
            {
                // UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId.ToString()));
                int flag = 1;
                bool Status = false;
                if (changePassword.NewPassword.Length < 8) return flag += 1;
                //string plainTextSaltENC = changePassword.PasswordResetCode.Replace("*", "=");
                //var base64EncodedBytes = System.Convert.FromBase64String(plainTextSaltENC);
                //var plainTextSaltDEC = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                var base64EncodedBytes = System.Convert.FromBase64String(changePassword.PasswordResetCode.Replace("*", "="));
                changePassword.PasswordResetCode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                usersDetail = this._ICustomerRepository.GetCustomerDetailsByPasswordResetCode(changePassword.PasswordResetCode);
                string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResourceFileServer")).Value.ToString();
                
                if (usersDetail != null)
                {
                    if (usersDetail.Id > 0)
                    {
                        user.UnhashedPassword = changePassword.NewPassword;
                        SecureClientModel sc = new SecureClientModel(user.Id, user.UnhashedPassword);
                        userModel = new Customer();
                        userModel.Id = usersDetail.Id;
                        userModel.Password = sc.Password;
                        userModel.Salt = sc.Salt;
                        // int passwordExpiry = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value);
                        // userModel.ExpiresOn = usersDetail.ExpiresOn.AddDays(passwordExpiry);

                        Status = this._ICustomerRepository.ResetPassword(userModel);

                        email = new EmailSenderModel();
                        email.Id = usersDetail.Id;
                        email.Name = usersDetail.Name;
                        email.FirstName = usersDetail.FirstName;
                        email.Password = userModel.Salt = sc.Salt;
                        email.EmailId = usersDetail.Email;
                        email.CompanyCode = "";//userContext.CurCompanyCode;
                        email.IsFirstLogin = true;
                        email.CompanyLogo = filePath;
                        //companyInfo = null; //this._IUsersRepository.GetCompanyInfo();
                        //if (companyInfo != null)
                        //    email.CompanyName = companyInfo.CompanyName;
                        EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CHANGEPASSWORD"], "");
                        _ICommonHelperRepository.UpdateActivity(ActivityType.PASSWORDCHANGED.ToString(), ActivityType.PASSWORDCHANGED.ToString());
                        return flag;
                    } return flag += 3;
                }
                return flag += 2;
            }
            catch (RepositoryException repEx)
            {
                throw new BusinessException("CHANGEPASSWORDFAILED", repEx.Message, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new BusinessException("CHANGEPASSWORDFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                userModel = null;
                email = null;
                user = null;
            }
        }

        
        public bool ValidateOTP(ChangePassword changePassword)
        {
            bool Status = false;
            try
            {
                if (string.IsNullOrEmpty( changePassword.PasswordResetCode)) return Status;
                var base64EncodedBytes = System.Convert.FromBase64String(changePassword.PasswordResetCode.Replace("*", "="));
                changePassword.PasswordResetCode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                CustomerModel usersDetail = this._ICustomerRepository.GetCustomerDetailsByPasswordResetCode(changePassword.PasswordResetCode);
                if (usersDetail != null)
                {
                    Status = true;
                }
            }
            catch (RepositoryException repEx)
            {
                LogManager.Log(repEx);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
            }
            return Status;
        }

        public int UpdateCustomerAddress(CustomerAddresses customerAddress)
        {
            return this._ICustomerRepository.UpdateCustomerAddress(customerAddress);
        }

        public int UpdateCustomerProfile(CustomerProfile customerProfile)
        {
            if (customerProfile.Image != null && customerProfile.Image.FileName != null  && customerProfile.Image.File != null)
            {
                customerProfile.Image.Id= _IDocumentProcessRepository.ImageProcess(customerProfile.Image);
            }
            return this._ICustomerRepository.UpdateCustomerProfile(customerProfile);
        }

        public bool ResendOTP(string email)  
        {
            EmailSenderModel mail = new EmailSenderModel();
            string OTP = Guid.NewGuid().ToString().Replace("-", "").Substring(1, 6).ToUpper();
         
            bool flag = false;
            flag = this._ICustomerRepository.ResendOTP(email, OTP);
            if (flag)
            {
                mail = new EmailSenderModel();
                mail.Password = OTP;
                mail.EmailId = email;
                mail.IsFirstLogin = true;
                EmailSender.SendPasswordReminderEmail(mail, MessageConfig.MessageSettings["NEWREGISTRATION"], string.Empty);
            }
            return flag;
        }

        #endregion

        #region DELETE Method

        public int deleteCustomerRole(int Id)
        {
            return this._ICustomerRepository.deleteCustomerRole(Id);
        }

        public int deleteCustomerAddress(int Id)
        {
            return this._ICustomerRepository.deleteCustomerAddress(Id);
        }

        public int deleteCustomerProfile(int Id)
        {
            return this._ICustomerRepository.deleteCustomerProfile(Id);
        }

        #endregion

        #region Social Media Login
        public int SocialMediaUser(string socialMediaUserEmail)
        {
            return this._ICustomerRepository.SocialMediaUser(socialMediaUserEmail);
        }
        #endregion

        /// <summary>
        /// Register Void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
