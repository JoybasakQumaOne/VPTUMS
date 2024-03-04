// -----------------------------------------------------------------------------------------------------------------
// <copyright file="IUserRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web; 
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IUserRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IUsersRepository : ICommon
    {
        string UserId { get; set; }

        string AgentCode { get; set; }

        string UserIPAddress { get; set; }

        #region GET Methods
        
        UserProfileModel GetControlUser(string userId);
        UserProfileModel GetContentUserDetails(string userId);

        dynamic GetOrgAdminUsers(Guid orgGuid);
        List<CompanyDetails> GetAgencyDetail(string username);

        List<UserProfileModel> GetUserByStatus(string code, string moduleCode, string status);

        List<UserProfileModel> GetUsersOfExpiredAccount(string code, DateTimeOffset expiryDate, string moduleId);

        List<UserProfileModel> GetUsersFromGroup(string code, int groupId);

        List<UserProfileModel> GetUsersFromRole(string code, int roleId);

        List<UserProfileModel> GetUserDetails(bool isSuperUser, string status);    

        // Forgot Password : Get User Detail By Email
        UserModel GetUserDetailsByEmail(string emailId);

        // Change Password : Get User By Email  
        UserProfileModel GetUserByEmail(string emailId);

        // Reset Password : Get User By Salt
        UserModel GetUserDetailsByPasswordResetCode(string passwordResetCode);

		bool UpdatePasswordResetCode(string emailId, string passwordResetCode);

		// Add User : Verify User Module

        DBModel GetCompanyInfo(string code);

        DBModel GetCompanyInfo();

        CompanyModuleModel GetCompanyModuleInfo();
        int IfUserExistsInApplicationDb(string email);
        bool LinkUserToCompany(string email, string CompanyId);
        string IfUserExistsInControlDb(string email);
        int CreateApplicationUser(UserProfileModel applicationUser, string controlUserCode, int companyId);
        int CreateControlUser(UserProfileModel userProfileModel, int companyId);
        #endregion

        #region POST Methods

        int AddUser(UserProfileModel user);

        // Add User : Verify User
        int CheckIfExists(string userName, string usertypeCode);
        bool Verify(string email, string otp);

        #endregion

        #region PUT Methods

        bool UpdateUser(UserProfileModel user);
        
        bool UpdateUserStatus(string code, int userId, string status);

        //Reset Password : Change Password
        bool ResetPassword(UserProfileModel user);

        bool MovingUserToAnotherGroup(string code, int userId, int existGroupId, int newGroupId);

        bool Deactivate(string id, string oid);
        bool Reactivate(string id, string oid);
        #endregion

        #region DELETE Methods

        bool RemoveUserInfo(int userId);

        #endregion       

        #region Link Delink User group
        bool LinkDelinkUserGroup(int UserId, int GroupId, string Action);    
        #endregion

        #region Link Delink User organization
        bool LinkDelinkUserOrganization(int UserId, int OrganizationId, string Action);  
        #endregion

        #region Token
        Token GetUserToken(string userToken, string IPAddress);
        bool UpdateTokenExpireTime(string userToken, string IPAddress);
        List<string> UserActions(string token);
        #endregion
        
    }
}