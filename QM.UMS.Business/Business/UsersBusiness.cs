// ----------------------------------------------------------------------------------------------------------------
// <copyright file="UsersBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.Business
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;    
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.Security;
    using CommonApplicationFramework.Notification;
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Models;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Web.Hosting;
    using QM.UMS.Repository.Repository;
    using CommonApplicationFramework.Logging;
    using CommonApplicationFramework.Caching;
    using Newtonsoft.Json;
    using QM.UMS.Repository.Helper;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UsersBusinessLogic>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UsersBusiness : RequestHeader, IUsersBusiness, IDisposable
    {
        #region Variable Declaration
        private readonly IUsersRepository _IUsersRepository;
        private readonly ICustomerRepository _ICustomerRepository;
        private readonly ICustomerBusiness _ICustomerBusiness;
        private readonly IDocumentProcessRepository _IDocumentProcessRepository;
        private readonly ICommonHelperRepository _ICommonHelperRepository;
        DBModel companyInfo = new DBModel();
        #endregion

        #region Properties
        public string RequestId { get; set; }
        public string Code { get; set; }
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="_prmIHttpClientBusiness">_prmIHttpClientBusiness</param>
        public UsersBusiness(IUsersRepository _iUsersRepository, ICustomerRepository _iCustomerRepository, CustomerBusiness _iCustomerBusiness, 
                            IDocumentProcessRepository _IDocumentProcessRepository, ICommonHelperRepository iCommonHelperRepository)
        {
            this._IUsersRepository = _iUsersRepository;
            this._ICustomerRepository = _iCustomerRepository;
            this._IDocumentProcessRepository = _IDocumentProcessRepository;
            _ICommonHelperRepository = iCommonHelperRepository; 
        }
        public UsersBusiness()
        {
            _IUsersRepository = new UsersRepository();
        }
        #endregion

        public void Init(string requestId)
        {
            this.RequestId = requestId;
            this._IUsersRepository.RequestId = requestId;
        }

        #region GET Methods

        /// <summary>Get User Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>Single Entity of User Details</returns>
        public UserProfileModel GetUserById(string userId)
        {
            return this._IUsersRepository.GetControlUser(userId);           
        }

        public UserProfileModel GetUserDetails(string userId)
        {
            return this._IUsersRepository.GetContentUserDetails(userId);
        }


        /// <summary>Get User Details By Module and Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="moduleCode">Module Code</param>
        /// <param name="status">User status</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUserByStatus(string code, string moduleCode, string status)
        {
            List<UserProfileModel> user = new List<UserProfileModel>();
            return user = this._IUsersRepository.GetUserByStatus(code, moduleCode, status);
        }

        /// <summary>Get Users From User Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUsersFromUsersGroup(string code, int groupId)
        {
            List<UserProfileModel> user = new List<UserProfileModel>();
            return user = this._IUsersRepository.GetUsersFromGroup(code, groupId);            
        }

        /// <summary>Get Users From Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="roleId">Role Id</param>
        /// <returns>List of User Details</returns>
        public List<UserProfileModel> GetUsersFromRole(string code, int roleId)
        {
            List<UserProfileModel> user = new List<UserProfileModel>();
            return user = this._IUsersRepository.GetUsersFromRole(code, roleId);           
        }        
        
        /// <summary>Get User Details From Active Directory</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>User Details</returns>
        public UserProfileModel GetADUserInfo(string code, string emailId)
        {
            using (HostingEnvironment.Impersonate())
            {
                // This code runs as the application pool user
                DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://pentechs.com/DC=PENTECHS,DC=com");
                ldapConnection.Path = "LDAP://pentechs.com/DC=PENTECHS,DC=com";
                ldapConnection.AuthenticationType = AuthenticationTypes.Secure;
               
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "pentechs.com"))
                {
                    using (var search = new PrincipalSearcher(new UserPrincipal(pc)))
                    {
                        var searchResults = search.FindAll();
                        var userInfo = searchResults.Where(x => x.UserPrincipalName == emailId).FirstOrDefault();
                        UserProfileModel user = new UserProfileModel();
                        user.Name = userInfo.UserPrincipalName;
                        user.FirstName = userInfo.Name;
                        user.Email = userInfo.UserPrincipalName;
                        user.Phone = ((System.DirectoryServices.AccountManagement.UserPrincipal)(userInfo)).VoiceTelephoneNumber;
                        return user;                      
                    }
                   
                }               
            }
        }
        public List<UserProfileModel> GetUserDetails(bool isSuperUser, string status)
        {
            return this._IUsersRepository.GetUserDetails(isSuperUser, status);
        }
        public dynamic GetOrgAdminUsers(Guid orgGuid)
        {
            return this._IUsersRepository.GetOrgAdminUsers(orgGuid);
        }

        public bool Verify(string email, string otp)
        {
            return  this._IUsersRepository.Verify(email, otp);
        }
        #endregion

        #region POST Method

        /// <summary>Add User Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of User Model</param>
        /// <returns>Insert Status</returns>
        public int AddClientUser(UserProfileModel user)
        {
            try
            {
                user.UserType = new ItemCode() { Code = UserTypes.Client.ToString() };
                /*Get the company info to which user want to creates the user*/
                CompanyModuleModel companyInfo = null;
                //CompanyModuleModel companyInfo = this._IUsersRepository.GetCompanyModuleInfo();
                if (_IUsersRepository.IfUserExistsInApplicationDb(user.Email) > 0)
                {
                    /*If user exists in Application DB then Break the operation and throw duplicate exception*/
                    throw new DuplicateException("DUPLICATE ITEM", "DUPLICATE", "DUPLICATE");
                }
                else
                {
                    //Check if user exists in the control level,if yes then get the Id
                    string controlUserCode = _IUsersRepository.IfUserExistsInControlDb(user.Email);
                    if (!string.IsNullOrEmpty(controlUserCode))
                    {
                        int isUserCreatedInAppDB = 0;
                        if (companyInfo!=null &&  companyInfo.AuthMode.Trim().ToUpper().Equals(AuthMode.AD.ToString()))
                        {
                            //user = GetActiveDirectoryUserDetails(user);
                            //isUserCreatedInAppDB = _IUsersRepository.CreateApplicationUser(user, controlUserCode, companyInfo.Id);
                        }
                        else
                        {
                            isUserCreatedInAppDB = _IUsersRepository.CreateApplicationUser(user, controlUserCode, 0);
                        }

                        if (isUserCreatedInAppDB > 0)
                        {
                            //SendRegistrationEmail(user, companyInfo, "CustomerNewRegistration");
                            EmailSenderModel email = new EmailSenderModel();
                            email.Id = isUserCreatedInAppDB;
                            email.Name = user.FirstName;
                            email.Password = user.OTP;
                            email.FirstName = user.FirstName;
                            email.EmailId = user.Email;
                            //email.CompanyCode = code;
                            email.IsFirstLogin = true;
                            //email.CompanyName = companyInfo.CompanyName;

                            _ICommonHelperRepository.StudentRegistrationMail(email);
                            //EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERNEWREGISTRATION"], string.Empty);
                            return isUserCreatedInAppDB;
                        }
                        else
                        {
                            return isUserCreatedInAppDB;
                        }
                    }
                    else
                    {
                        int userId= CreateControlUser(companyInfo, user);
                        if(userId>0)
                        {
                            EmailSenderModel email = new EmailSenderModel();
                            email.Id = userId;
                            email.Name = user.FirstName;
                            email.Password = user.OTP;
                            email.FirstName = user.FirstName;
                            email.EmailId = user.Email;
                            //email.CompanyCode = code;
                            email.IsFirstLogin = true;
                            //email.CompanyName = companyInfo.CompanyName;
                            //EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERNEWREGISTRATION"], string.Empty);
                            _ICommonHelperRepository.StudentRegistrationMail(email);

                        }
                        return userId;
                    }
                }
            }
            catch (BusinessException bex)
            {
                LogManager.Log(bex);
                throw new BusinessException("ERROROCCURE", bex.Message, bex.StackTrace);
            }
            catch (DuplicateException dex)
            {
                LogManager.Log(dex);
                throw new DuplicateException("DUPLICATE", dex.ErrorCode, dex.ErrorDescription);

            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new BusinessException("ERROROCCURE", ex.Message, ex.StackTrace);

            }
        }


        public List<Company> GetCompanyDetail(string username)
        {
            try
            {
                List<CompanyDetails> companyInfo = _IUsersRepository.GetAgencyDetail(username);  //R:: just return as it is
                if (companyInfo != null && companyInfo.Count > 0)
                {
                    var parentData = JsonConvert.SerializeObject(companyInfo);
                    return JsonConvert.DeserializeObject<List<Company>>(parentData);
                }
                else
                {
                    return new List<Company>();
                }

            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETUSERFAILED", ex.Message, ex.StackTrace);
            }

        }


        public int AddClientUserOld(UserProfileModel user)
        {
            AttachmentFile image = new AttachmentFile();
            EmailSenderModel email = new EmailSenderModel();
            try
            {
                string code = "BOOK";
                int verifyUserId = 0;
                int flag = 0;
                string authMode = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AuthMode")).Value.ToString();

                verifyUserId = this._IUsersRepository.CheckIfExists(user.Email, "Customer");
                if (verifyUserId <= 0)
                {
                    SecureModel sc = new SecureModel(user.Password);
                    user.Password = sc.Password;
                    user.Salt = sc.Salt;
                    user.Status = UsersStatus.Active.ToString();
                    user.IsFirstLogin = true;
                    if (user.userImage != null && user.userImage.FileName != null && user.userImage.File != null)
                    {
                        byte[] imageBytes = Convert.FromBase64String(user.userImage.File);
                        string fileLocPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString();
                        string userPath = Constant.USERIMAGE;

                        FileInfo fileInfo = new FileInfo(user.userImage.FileName);
                        string fileName = Guid.NewGuid() + fileInfo.Extension;
                        string virtualPath = fileLocPath + ConfigurationManager.AppSettings["ProjectName"].ToString() + @"\" + userPath;
                        var path = Path.Combine(virtualPath, Path.GetFileName(fileName));
                        if (!Directory.Exists(virtualPath))
                        {
                            Directory.CreateDirectory(virtualPath);
                        }
                        System.IO.File.WriteAllBytes(path, imageBytes);
                        user.userImage.FileName = fileName;
                    }
                    else
                    {
                    }
                    user.FirstName = user.FirstName.Replace("'", "''");
                    user.UserType = new ItemCode() { Code = "Customer" };
                    user.OTP = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpper();
                    user.IsOTPVerified = true;
                    flag = this._IUsersRepository.AddUser(user);
                    if (flag > 0)
                    {
                        if (user.UserType.Code == "Customer")
                        {
                            email = new EmailSenderModel();
                            email.Id = flag;
                            email.Name = user.FirstName;
                            email.Password = user.OTP;
                            email.FirstName = user.FirstName;
                            email.EmailId = user.Email;
                            email.CompanyCode = code;
                            email.IsFirstLogin = true;
                            email.CompanyName = companyInfo.CompanyName;
                            EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CUSTOMERNEWREGISTRATION"], string.Empty);
                        }
                        else
                        {
                            email = new EmailSenderModel();
                            email.Id = flag;
                            email.Name = user.Name;
                            email.Password = user.Password;
                            email.FirstName = user.FirstName;
                            email.EmailId = user.Email;
                            email.CompanyCode = code;
                            email.IsFirstLogin = true;
                            email.CompanyName = companyInfo.CompanyName;
                            EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["NEWREGISTRATION"], companyInfo.CompanyName);
                        }


                        return flag;
                    }
                    return flag;
                }
                return flag;

            }
            catch (RepositoryException repEx)
            {
                throw new BusinessException("ADDUSERFAILED", repEx.Message, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new BusinessException("ADDUSERFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                image = null;
                email = null;
            }
        }
        //public int AddAdminUser(UserProfileModel user)
        //{
        //    AttachmentFile image = new AttachmentFile();
        //    EmailSenderModel email = new EmailSenderModel();
        //    try
        //    {
        //        string code = "BOOK";
        //        int verifyUserId = 0;
        //        int flag = 0;
        //        string authMode = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AuthMode")).Value.ToString();

        //        verifyUserId = this._IUsersRepository.CheckIfExists(user.Email, "Admin");
        //        if (verifyUserId <= 0)
        //        {
        //            SecureModel sc = new SecureModel(user.Password);
        //            user.Password = sc.Password;
        //            user.Salt = sc.Salt;
        //            user.Status = UsersStatus.Active.ToString();
        //            user.FirstName = user.FirstName.Replace("'", "''");
        //            user.UserType = new ItemCode() { Code = "Admin" };
        //            user.IsOTPVerified = true;
        //            flag = this._IUsersRepository.AddUser(user);
        //            if (flag > 0)
        //            {

        //                    email = new EmailSenderModel();
        //                    email.Id = flag;
        //                    email.Name = user.FirstName;
        //                    email.Password = user.Password;
        //                    email.FirstName = user.FirstName;
        //                    email.EmailId = user.Email;
        //                    email.CompanyCode = code;
        //                    email.IsFirstLogin = true;
        //                    email.CompanyLogo = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ImageURL")).Value.ToString() + "/Resources/Logo.png";
        //                    EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["OrganizationAdminRegistration"], companyInfo.CompanyName);
        //                return flag;
        //            }
        //            return flag;
        //        }
        //        return flag;

        //    }
        //    catch (RepositoryException repEx)
        //    {
        //        throw new BusinessException("ADDUSERFAILED", repEx.Message, repEx.StackTrace);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException("ADDUSERFAILED", ex.Message, ex.StackTrace);
        //    }
        //    finally
        //    {
        //        image = null;
        //        email = null;
        //    }
        //}


        public int AddAdminUser(UserProfileModel user)
        {
            int userId = 0;
            try
            {
                user.IsAdmin = true;
                /*Get the company info to which user want to creates the user*/
                CompanyModuleModel companyInfo = this._IUsersRepository.GetCompanyModuleInfo();
                if (_IUsersRepository.IfUserExistsInApplicationDb(user.Email) > 0)
                {
                    _IUsersRepository.LinkUserToCompany(user.Email, user.Organisations.FirstOrDefault().ToString());
                    string controlUserCode = _IUsersRepository.IfUserExistsInControlDb(user.Email);
                    int isUserCreatedInAppDB= _IUsersRepository.CreateApplicationUser(user, controlUserCode, companyInfo.Id);
                    if (isUserCreatedInAppDB > 0)
                    {
                        //SendRegistrationEmail(user, companyInfo, "CustomerNewRegistration");
                        userId= isUserCreatedInAppDB;
                    }
                    else
                    {
                        userId= isUserCreatedInAppDB;
                    }
                    /*If user exists in Application DB then Break the operation and throw duplicate exception*/
                    //throw new DuplicateException("DUPLICATE ITEM", "DUPLICATE", "DUPLICATE");
                }
                else
                {
                    //Check if user exists in the control level,if yes then get the Id
                    string controlUserCode = _IUsersRepository.IfUserExistsInControlDb(user.Email);
                    if (!string.IsNullOrEmpty(controlUserCode))
                    {
                        int isUserCreatedInAppDB = 0;
                        if (companyInfo.AuthMode.Trim().ToUpper().Equals(AuthMode.AD.ToString()))
                        {
                            //user = GetActiveDirectoryUserDetails(user);
                            //isUserCreatedInAppDB = _IUsersRepository.CreateApplicationUser(user, controlUserCode, companyInfo.Id);
                        }
                        else
                        {
                            isUserCreatedInAppDB = _IUsersRepository.CreateApplicationUser(user, controlUserCode, companyInfo.Id);
                        }

                        if (isUserCreatedInAppDB > 0)
                        {
                            //SendRegistrationEmail(user, companyInfo, "CustomerNewRegistration");
                            userId= isUserCreatedInAppDB;
                        }
                        else
                        {
                            userId= isUserCreatedInAppDB;
                        }
                    }
                    else
                    {
                        userId= CreateControlUser(companyInfo, user);
                    }
                }

                if(userId>0)
                {
                    //_ICommonHelperRepository.AdminAddUserMail(new EmailSenderModel()
                    //{
                    //    Name= user.FirstName,
                    //    CompanyName= companyInfo.CompanyName,
                    //    EmailId=user.Email
                    //});
                }
                return userId;
            }
            catch (BusinessException bex)
            {
                LogManager.Log(bex);
                throw new BusinessException("ERROROCCURE", bex.Message, bex.StackTrace);
            }
            catch (DuplicateException dex)
            {
                LogManager.Log(dex);
                throw new DuplicateException("DUPLICATE", dex.ErrorCode, dex.ErrorDescription);

            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new BusinessException("ERROROCCURE", ex.Message, ex.StackTrace);

            }
        }
        private int CreateControlUser(CompanyModuleModel company, UserProfileModel user)
        {
            if (company!=null && company.AuthMode.ToUpper().Equals(AuthMode.AD.ToString()))
            {
                //UserProfileModel userInfo = GetActiveDirectoryUserDetails(user);
                //GenerateUserConfiguration(userInfo, company.AuthMode);
                //int userId = CreateActiveDirectoryUser(userInfo, company.Id);
                //if (userId > 0)
                //{
                //    SendRegistrationEmail(userInfo, company, MessageConfig.MessageSettings["NEWREGISTRATION"]);
                //    return userId;
                //}
                return 0;
            }
            else
            {
                GenerateUserConfiguration(user, company?.AuthMode);
                int userId = _IUsersRepository.CreateControlUser(user,company!=null && company.Id>0? company.Id:0);
                if (userId > 0)
                {
                    //SendRegistrationEmail(user, company, MessageConfig.MessageSettings["NEWREGISTRATION"]);
                    return userId;
                }
                return 0;
            }
        }
        
        private void GenerateUserConfiguration(UserProfileModel user, string authMode)
        {
            if (authMode == "AD")
            {
                //user.ControlUserCode = Guid.NewGuid().ToString();
                //user.Status = UsersStatus.Active.ToString();
                //user.ExpiresOn = DateTimeOffset.Now.AddDays(Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value));
                //if (user.UserImage != null && user.UserImage.FileName != null && user.UserImage.FileType != null && user.UserImage.File != null)
                //{
                //    byte[] imageBytes = Convert.FromBase64String(user.UserImage.File);
                //    string fileLocPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString();
                //    string virtualPath = fileLocPath + Code + @"\" + Constant.USERIMAGE;
                //    var filePath = Path.Combine(virtualPath, Path.GetFileName(Guid.NewGuid() + new FileInfo(user.UserImage.FileName).Extension));
                //    if (!Directory.Exists(virtualPath))
                //        Directory.CreateDirectory(virtualPath);
                //    File.WriteAllBytes(filePath, imageBytes);
                //    user.UserImage.FileName = Guid.NewGuid() + new FileInfo(user.UserImage.FileName).Extension;
                //}
                //else
                //    user.UserImage = new AttachmentFile();
            }
            else
            {
                string temporaryPassword = string.Empty;
                temporaryPassword = !string.IsNullOrEmpty(user.Password) ? user.Password : Guid.NewGuid().ToString().Replace("-", "").Substring(1, 8);
                user.ControlUserCode = Guid.NewGuid().ToString();
                SecureModel sc = new SecureModel(temporaryPassword);
                user.Password = sc.Password;
                user.Salt = sc.Salt;
                user.Status = UsersStatus.Active.ToString();
                user.ExpiresOn = DateTimeOffset.Now.AddDays(Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value));
                if (user.UserImage != null && user.UserImage.FileName != null && user.UserImage.FileType != null && user.UserImage.File != null)
                {
                    byte[] imageBytes = Convert.FromBase64String(user.UserImage.File);
                    string fileLocPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString();
                    string virtualPath = fileLocPath + Code + @"\" + Constant.USERIMAGE;
                    var filePath = Path.Combine(virtualPath, Path.GetFileName(Guid.NewGuid() + new FileInfo(user.UserImage.FileName).Extension));
                    if (!Directory.Exists(virtualPath))
                        Directory.CreateDirectory(virtualPath);
                    File.WriteAllBytes(filePath, imageBytes);
                    user.UserImage.FileName = Guid.NewGuid() + new FileInfo(user.UserImage.FileName).Extension;
                }
                else
                    user.UserImage = new AttachmentFile();
            }

        }
        public int AddRegisterOrganizationAdminUser(UserProfileModel user)
        {
            AttachmentFile image = new AttachmentFile();
            EmailSenderModel email = new EmailSenderModel();
            try
            {
                int verifyUserId = 0;
                int flag = 0;
                string authMode = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AuthMode")).Value.ToString();

                verifyUserId = this._IUsersRepository.CheckIfExists(user.Email, "Admin");
                if (verifyUserId <= 0)
                {
                    SecureModel sc = new SecureModel(user.Password);
                    user.Password = sc.Password;
                    user.Salt = sc.Salt;
                    user.Status = UsersStatus.Active.ToString();
                    user.FirstName = user.FirstName.Replace("'", "''");
                    user.UserType = new ItemCode() { Code = "Admin" };
                    flag = this._IUsersRepository.AddUser(user);
                    if(flag>0)
                    {
                        email = new EmailSenderModel();
                        email.Id = flag;
                        email.Name = user.Name;
                        email.Password = user.Password;
                        email.FirstName = user.FirstName;
                        email.EmailId = user.Email;
                        //email.CompanyCode = code;
                        email.IsFirstLogin = true;
                        email.CompanyLogo = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ImageURL")).Value.ToString() + "/Resources/Logo.png";
                        email.CompanyName = companyInfo.CompanyName;
                        EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["OrganizationRegistration"], companyInfo.CompanyName);
                    }
                    return flag;
                }
                return flag;

            }
            catch (RepositoryException repEx)
            {
                throw new BusinessException("ADDUSERFAILED", repEx.Message, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new BusinessException("ADDUSERFAILED", ex.Message, ex.StackTrace);
            }
            finally
            {
                image = null;
                email = null;
            }
        }

        /// <summary>Forgot Password</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="emailId">Email Id</param>
        /// <returns>Request Status</returns>
        public UserModel ForgotPassword(string emailId, string type)
        {
            EmailSenderModel email = new EmailSenderModel();
            UserModel userEntity = new UserModel();
            try
            {
                Regex regex = new Regex(@"^(([^<>()\[\]\\.,;:\s@""]+(\.[^<>()\[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
                Match match = regex.Match(emailId);
                if (match.Success)
                {
                    userEntity = new UserModel();                   
                    userEntity = this._IUsersRepository.GetUserDetailsByEmail(emailId);
                    if (userEntity != null)
                    {
                        
                        if (userEntity.Id > 0)
                        {
                            email = new EmailSenderModel();
                            email.Id = userEntity.Id;
                            email.Name = userEntity.Name;
                            email.Salt = Guid.NewGuid().ToString().Substring(0,10).ToUpper();
                            email.EmailId = userEntity.Email;
                            email.FirstName = userEntity.FirstName;
                            this._IUsersRepository.UpdatePasswordResetCode(emailId, email.Salt);
                            email.IsFirstLogin = true;
                            email.CompanyLogo = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ImageURL")).Value.ToString() + "/Resources/Logo.png";
                            _ICommonHelperRepository.ForgotPasswordMail(email, type);
                            //EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["PASSWORDREMAINDER"], "");
                            return userEntity;
                        }
                        return userEntity;
                    }
                    
                    return userEntity;
                }
                else
                {
                    throw new BusinessException(MessageConfig.MessageSettings["INVALIDEMAIL"]);
                }
            }
            catch (RepositoryException repEx)
            {
                throw new BusinessException("ERROROCCURE", repEx.Message, repEx.StackTrace);
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

        #endregion
         
        #region PUT Method

        /// <summary>Update User</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of User Model</param>
        /// <returns>Update Status</returns>
        public bool UpdateUser(UserProfileModel user)
        {
            try
            {
                return this._IUsersRepository.UpdateUser(user);
            }
            catch (RepositoryException repEx)
            {
                throw new BusinessException("UPDATEUSERFAILED", repEx.Message, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new BusinessException("UPDATEUSERFAILED", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>Update User Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <param name="status">User Status</param>
        /// <returns>Update Status</returns>
        public bool UpdateUserStatus(string code, int userId, string status)
        {
            return this._IUsersRepository.UpdateUserStatus(code, userId, status);
        }

        /// <summary>Change Password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Change Status</returns>
        public int ChangePassword(ChangePassword changePassword)
        {
            bool Status = false;
            PasswordHasher passwordHasher = new PasswordHasher();
            UserProfileModel userModel = new UserProfileModel();
            UserProfileModel usersDetail = new UserProfileModel();
            EmailSenderModel email = new EmailSenderModel();
            try
            {
                UserModel userEntity = this._IUsersRepository.GetUserDetailsByEmail(Context.Email);

                int flag = 1;
                if (changePassword.NewPassword.Equals(changePassword.OldPassword))
                {
                    return flag += 1;
                }
                usersDetail = this._IUsersRepository.GetControlUser(userEntity.UserGuid);
                if (usersDetail != null)
                {
                    var validPass = passwordHasher.ValidatePassword(changePassword.OldPassword, usersDetail.Salt, usersDetail.Password) ? usersDetail : null;
                    if (validPass != null)
                    {                         
                        userModel.Password = changePassword.NewPassword;
                        SecureModel sc = new SecureModel(userModel.Password);
                        userModel.Id = usersDetail.Id;
                        userModel.Password = sc.Password;
                        userModel.Salt = sc.Salt;
                        userModel.Email = Context.Email;
                        int passwordExpiry = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value);
                        userModel.ExpiresOn = usersDetail.ExpiresOn.AddDays(passwordExpiry);
                        if( this._IUsersRepository.ResetPassword(userModel))
                        {
                            email = new EmailSenderModel();
                            email.Id = usersDetail.Id;
                            email.Name = usersDetail.Name;
                            email.FirstName = usersDetail.FirstName;
                            email.Password = usersDetail.Salt;
                            email.EmailId = usersDetail.Email;
                            email.CompanyCode = userEntity.CompanyCode;
                            email.IsFirstLogin = true;
                            email.CompanyName = "eBook";
                            this._ICommonHelperRepository.ChangePasswordMail(email);
                            //EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CHANGEPASSWORD"], companyInfo.CompanyName);                      
                            return flag;
                        }
                        else
                        {
                            return 0;
                        }
						
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
                userModel = null;
                email = null;
            }
        }

        /// <summary>Reset password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Reset value</returns>
        public int ResetPassword(ChangePassword changePassword)
        {
            UserProfileModel userModel = new UserProfileModel();
            UserModel usersDetail = new UserModel();
            EmailSenderModel email = new EmailSenderModel();            
            UserModel user = new UserModel();
            try
            {
                int flag = 1;
                bool Status = false;
                if (changePassword.NewPassword.Length < 8) return flag += 1;
				changePassword.PasswordResetCode = changePassword.PasswordResetCode.Replace("*", "=");

                var base64EncodedBytes = System.Convert.FromBase64String(changePassword.PasswordResetCode);
                changePassword.PasswordResetCode = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                usersDetail = this._IUsersRepository.GetUserDetailsByPasswordResetCode(changePassword.PasswordResetCode);
                //UserModel userEntity = this._IUsersRepository.GetUserDetailsByEmail(usersDetail.Email);
                if (usersDetail != null)
                {
                    if (usersDetail.Id > 0)
                    {
                        user.UnhashedPassword = changePassword.NewPassword;
                        SecureClientModel sc = new SecureClientModel(user.Id, user.UnhashedPassword);
                        userModel = new UserProfileModel();
                        userModel.Email = usersDetail.Email;
                        userModel.Id = usersDetail.Id;
                        userModel.Password = sc.Password;
                        userModel.Salt = sc.Salt;
                        int passwordExpiry = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value);
                        userModel.ExpiresOn = usersDetail.ExpiresOn.AddDays(passwordExpiry);
                        Status = this._IUsersRepository.ResetPassword(userModel);   
						
                        if(Status)
                        {
                            email = new EmailSenderModel();
                            email.Id = usersDetail.Id;
                            email.Name = usersDetail.Name;
                            email.FirstName = usersDetail.FirstName;
                            email.Password = userModel.Salt = sc.Salt;
                            email.EmailId = usersDetail.Email;
                            email.CompanyCode = usersDetail.CompanyCode;
                            email.IsFirstLogin = true;
                            //companyInfo = null; //this._IUsersRepository.GetCompanyInfo();
                            //if (companyInfo != null)                            
                            //email.CompanyName = "The Online Book Company";
                            _ICommonHelperRepository.ChangePasswordMail(email);
                            //EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CHANGEPASSWORD"], "");
                            return flag;
                        }
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

        public bool AdminReset(ChangePassword changePassword)
        {
            UserProfileModel userModel = null;
            bool flag = false;
            try
            {
                userModel = this._IUsersRepository.GetControlUser(changePassword.UserName);
                if (userModel != null)
                {
                    SecureClientModel sc = new SecureClientModel(userModel.Id, changePassword.NewPassword);
                    userModel.Password = sc.Password;
                    userModel.Salt = sc.Salt;
                    int passwordExpiry = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("PasswordExpiry")).Value);
                    userModel.ExpiresOn = userModel.ExpiresOn.AddDays(passwordExpiry);
                    if (this._IUsersRepository.ResetPassword(userModel))
                    {

                        //EmailSenderModel email = new EmailSenderModel();
                        //email.Id = userModel.Id;
                        //email.Name = userModel.Name;
                        //email.FirstName = userModel.FirstName;
                        //email.Password = changePassword.NewPassword;
                        //email.EmailId = userModel.Email;
                        //email.IsFirstLogin = true;
                        //this._ICommonHelperRepository.AdminPasswordResetMail(email);
                        //EmailSender.SendPasswordReminderEmail(email, MessageConfig.MessageSettings["CHANGEPASSWORD"], "");
                        flag = true;
                    }
                }
            }
            catch (RepositoryException repEx)
            {
                throw new BusinessException("CHANGEPASSWORDFAILED", repEx.Message, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new BusinessException("CHANGEPASSWORDFAILED", ex.Message, ex.StackTrace);
            }
            return flag;
        }

        /// <summary>Moving User To Another Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <param name="existGroupId">Old GroupId</param>
        /// <param name="newGroupId">New GroupId</param>
        /// <returns>Update Status</returns>
        public bool MovingUserToAnotherGroup(string code, int userId, int existGroupId, int newGroupId)
        {
            return this._IUsersRepository.MovingUserToAnotherGroup(code, userId, existGroupId, newGroupId);
        }        

        public bool Deactivate(string id, string oid)
        {
            if(id!=Context.UserGuid)
            {
                return this._IUsersRepository.Deactivate(id,oid);
            }
            return false;
        }
        public bool Reactivate(string id, string oid)
        {
            return this._IUsersRepository.Reactivate(id,oid);
        }
        #endregion

        #region DELETE Method

        /// <summary>Delete User Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>Delete Status</returns>
        public bool RemoveUserInfo(int userId)
        {
            return this._IUsersRepository.RemoveUserInfo(userId);
        }
        
        #endregion        

        #region Link Delink User group
        public bool LinkDelinkUserGroup(int UserId, int GroupId, string Action)
        {
            return this._IUsersRepository.LinkDelinkUserGroup(UserId, GroupId, Action);
        }
        #endregion

        #region Link Delink User organization
        public bool LinkDelinkUserOrganization(int UserId, int OrganizationId, string Action) 
        {
            return this._IUsersRepository.LinkDelinkUserOrganization(UserId, OrganizationId, Action);
        }
        #endregion

        /// <summary>
        /// Register Void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #region Validation Methods

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>Boolean value</returns>
        public bool ValidateToken(string token, string IPAddress)
        {
            try
            {
                Token userToken = new Token();
                userToken = this._IUsersRepository.GetUserToken(token, IPAddress);
                if (userToken.Value == token)
                {
                    if (DateTime.Now <= userToken.ExpireOn)
                    {
                        this._IUsersRepository.UpdateTokenExpireTime(token, IPAddress);
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }            
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("TOKENVALIDATIONFAILED", ex.Message, ex.StackTrace);
            }
        }


        /// <summary>
        /// Validate Action
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="action">action</param>
        /// <returns>Boolean value</returns>
        public bool ValidateAction(string token, string action)
        {
            try
            {
                List<string> actions = this._IUsersRepository.UserActions(token);
                if (actions.Contains(action))
                    return true;
                else
                    return false;
            }            
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ACTIONVALIDATIONFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion
    }
}