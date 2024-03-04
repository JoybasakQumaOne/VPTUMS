// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserGroupRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;   
    using System.Globalization;
    using System.Configuration;
    using CommonApplicationFramework.Common;
    using QM.UMS.Models; 
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IUserGroupRepository>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IUserGroupRepository : ICommon
    {
        string UserId { get; set; }

        string AgentCode { get; set; }

        string UserIPAddress { get; set; }

        #region GET Methods
        List<UserGroupModel> GetAllGroup(string ModuleCode);

        UserGroupModel GetGroupById(string code, int Id);

        List<Item> GetAllGroupType(string code);

        List<UserGroupModel> GetAllGroupByTypeStatus(string code, string Status, string Type);

        int GetRolelinkedtoGroup(string code, int roleId, int groupId);

        int GetUserLinkedtoGroup(string code, int userId, int groupId);

        List<Item> GetAllLinkedAttribute(int groupId);
        #endregion

        #region POST Methods

        bool AddGroup(string code, UserGroupModel userGroup);

        bool AddUserToGroup(string code, CommonModel users);

        bool RemoveUserFromGroup(string code, CommonModel users);

        bool AddRoleToGroup(string code, CommonModel roles);

        bool RemoveRoleFromGroup(string code, CommonModel roles);

        #endregion

        #region PUT Methods

        bool UpdateGroup(string code, UserGroupModel userGroup);

        #endregion

        #region DELETE Methods

        bool RemoveGroup(string code, int Id);

        #endregion      

        #region Link Attribute
        bool LinkGroupAttribute(int groupId, int attributeId);
        #endregion

        #region Delink Attribute
        bool DelinkGroupAttribute(int groupId, int attributeId);
        #endregion

        #region Link GroupToUser
        bool LinkGroupToUser(int groupId, int userId);
        #endregion

        #region Delink GroupToUser  
        bool DelinkGroupToUser(int groupId, int userId); 
        #endregion

    }
}
