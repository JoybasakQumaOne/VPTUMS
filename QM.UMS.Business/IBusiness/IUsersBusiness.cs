// -----------------------------------------------------------------------------------------------------------------
// <copyright file="IUsersBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
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
    ///   Class:        <IUsersBusinessLogic>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------
    
    public interface IUsersBusiness : ICommon
    {
        void Init(string requestId);

        #region GET Methods

        UserProfileModel GetUserById(string Id);
        UserProfileModel GetUserDetails(string userId);

        dynamic GetOrgAdminUsers(Guid orgGuid);

        List<UserProfileModel> GetUserByStatus(string code, string moduleCode, string status);

        List<UserProfileModel> GetUsersFromUsersGroup(string code, int groupId);

        List<UserProfileModel> GetUsersFromRole(string code, int roleId);

        UserProfileModel GetADUserInfo(string code, string emailId);

        List<UserProfileModel> GetUserDetails(bool isSuperUser, string status);
        bool Verify(string email, string otp);


        #endregion

        #region POST Methods

        int AddAdminUser(UserProfileModel user);
        int AddClientUser(UserProfileModel user);
        List<Company> GetCompanyDetail(string username);
        int AddRegisterOrganizationAdminUser(UserProfileModel user);

        UserModel ForgotPassword(string emailId,string type);
        
        #endregion

        #region PUT Methods

        bool UpdateUser(UserProfileModel user);
        
        bool UpdateUserStatus(string code, int Id, string status);
        
        int ChangePassword(ChangePassword changePassword);

        int ResetPassword(ChangePassword changePassword);
        bool AdminReset(ChangePassword changePassword);

        bool MovingUserToAnotherGroup(string code, int Id, int existGroupId, int newGroupId);

        bool Deactivate(string id, string oid);
        bool Reactivate(string id, string oid);

        #endregion

        #region DELETE Methods

        bool RemoveUserInfo(int Id);

        #endregion   

        #region Link Delink User group
        bool LinkDelinkUserGroup(int UserId, int GroupId, string Action);
        #endregion

        #region Link Delink User organization
        bool LinkDelinkUserOrganization(int UserId, int OrganizationId, string Action);
        #endregion

        bool ValidateToken(string token, string IPAddress);
        bool ValidateAction(string token, string action);
    }
}