// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserGroupBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IUserGroupBusinessLogic>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IUserGroupBusiness
    {

        void Init(string requestId);

        #region GET Methods

       

        UserGroupModel GetGroupById(string code, int groupId);

        List<Item> GetAllGroupType(string code);

        List<UserGroupModel> GetAllGroupByTypeStatus(string code, string Status, string Type);

        List<UserGroupModel> GetAllGroup(string ModuleCode);

        List<Item> GetAllLinkedAttribute(int groupId);

        dynamic GetUserGroups(string userId);
        bool LinkUserGroup(string gId, string uId);
        bool DeLinkUserGroup(string gId, string uId);

        #endregion

        #region POST Methods

        bool AddGroup(string code, UserGroupModel userGroup);

        bool AddUserToGroup(string code, CommonModel users);

        bool RemoveUserFromGroup(string code, CommonModel users);

        bool AddRoleToGroup(string code, CommonModel roles);

        bool DeleteRoleFromGroup(string code, CommonModel roles);

        #endregion

        #region PUT Methods

        bool UpdateGroup(string code, UserGroupModel userGroup);

        #endregion

        #region DELETE Methods

        bool RemoveGroup(string code, int groupId);

        #endregion      

        #region Link Attribute
        bool LinkGroupAttribute(int groupId, int attributeId);
        #endregion

        #region Delink Attribute
        bool DelinkGroupAttribute(int groupId, int attributeId);
        #endregion

        #region Link UserToGroup
        bool LinkGroupToUser(int groupId, int userId);
        #endregion

        #region Delink UserToGroup
        bool DelinkGroupToUser(int groupId, int userId);    
        #endregion
    }
}
