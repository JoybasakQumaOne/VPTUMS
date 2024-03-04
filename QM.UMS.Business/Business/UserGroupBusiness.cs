// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="UserGroupBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.Business
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;     
    using CommonApplicationFramework.Common;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Models;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.Repository;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.ConfigurationHandling;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserGroupBusinessLogic>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UserGroupBusiness : IUserGroupBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IUserGroupRepository _IUserGroupRepository;
        #endregion

        #region Constructor
        public UserGroupBusiness(IUserGroupRepository _iUserGroupRepository)
        {
            this._IUserGroupRepository = _iUserGroupRepository;
        }
        #endregion

        #region Properties
        public string RequestId { get; set; }
        public string Code { get; set; }
        #endregion

        public void Init(string requestId)
        {
            this.RequestId = requestId;
            this._IUserGroupRepository.RequestId = requestId;
        }

        #region GET Method

        /// <summary>Get Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Group Details</returns>
        public List<UserGroupModel> GetAllGroup(string ModuleCode)
        {
            List<UserGroupModel> userGroup = new List<UserGroupModel>();
            return userGroup = this._IUserGroupRepository.GetAllGroup(ModuleCode);            
        }

        /// <summary>Get Group Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Single Entity of Group Details</returns>
        public UserGroupModel GetGroupById(string code, int groupId)
        {
            UserGroupModel userGroup = new UserGroupModel();
            return userGroup = this._IUserGroupRepository.GetGroupById(code, groupId);           
        }

        /// <summary>Get Group Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Group Type</returns>
        public List<Item> GetAllGroupType(string code)
        {
            List<Item> GroupType = new List<Item>();
            return GroupType = this._IUserGroupRepository.GetAllGroupType(code);
        }

        /// <summary>Get All Group Details By Type and Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Status">Status Type</param>
        /// <param name="groupTypeId">Group Type Id</param>
        /// <returns>List of Group Details</returns>
        public List<UserGroupModel> GetAllGroupByTypeStatus(string code, string Status, string Type)
        {
            List<UserGroupModel> userGroup = new List<UserGroupModel>();
            return userGroup = this._IUserGroupRepository.GetAllGroupByTypeStatus(code, Status, Type);
        }

        public List<Item> GetAllLinkedAttribute(int groupId)
        {
            return this._IUserGroupRepository.GetAllLinkedAttribute(groupId);
        }
        
        #endregion

        #region POST Method

        /// <summary>Add Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Insert Status</returns>
        public bool AddGroup(string code, UserGroupModel user)
        {
            user.Name = user.Name.Replace("'", "''");
            if (!string.IsNullOrEmpty(user.Description)) user.Description = user.Description.Replace("'", "''");
            return this._IUserGroupRepository.AddGroup(code, user);
        }

        /// <summary>Add User To Group</summary>
        /// <param name="code">Code in Entity</param>  
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        public bool AddUserToGroup(string code, CommonModel user)
        {
            int GroupUserId = 0;
            GroupUserId = this._IUserGroupRepository.GetUserLinkedtoGroup(code, user.Items.Id, user.RelationalId);
            if (GroupUserId > 0)
            {
                 throw new DuplicateException();
            }
            else
            {
                return this._IUserGroupRepository.AddUserToGroup(code, user);
            }
        }

        /// <summary>Add Role To Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        public bool AddRoleToGroup(string code, CommonModel roles)
        {
            int GroupRoleId = 0;
            GroupRoleId = this._IUserGroupRepository.GetRolelinkedtoGroup(code, roles.Items.Id, roles.RelationalId);
            if (GroupRoleId > 0)
            {
                throw new BusinessException(MessageConfig.MessageSettings["DUPLICATE"]);
            }
            else
            {
                return this._IUserGroupRepository.AddRoleToGroup(code, roles);
            }
        }

        /// <summary>Remove User From Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Remove Status</returns>    
        public bool RemoveUserFromGroup(string code, CommonModel user)
        {
           return this._IUserGroupRepository.RemoveUserFromGroup(code, user);
        }

        /// <summary>Remove Role From Group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Common Entity</param>
        /// <returns>Remove Status</returns>
        public bool DeleteRoleFromGroup(string code, CommonModel roles)
        {
            return this._IUserGroupRepository.RemoveRoleFromGroup(code, roles);
        }

        #endregion

        #region PUT Method

        /// <summary>Update Group Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of UserGroup Model</param>
        /// <returns>Update Status</returns>
        public bool UpdateGroup(string code, UserGroupModel user)
        {
            user.Name = user.Name.Replace("'", "''");
            if (!string.IsNullOrEmpty(user.Description)) user.Description = user.Description.Replace("'", "''");
            return this._IUserGroupRepository.UpdateGroup(code, user);
        }
        
        #endregion

        #region DELETE Method

        /// <summary>Deactivate Group details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Deactivate Status</returns>
        public bool RemoveGroup(string code, int groupId)
        {
            return this._IUserGroupRepository.RemoveGroup(code, groupId);
        }

        #endregion        

        #region Link Attribute
        public bool LinkGroupAttribute(int groupId, int attributeId)
        {
            return this._IUserGroupRepository.LinkGroupAttribute(groupId, attributeId);
        }
        #endregion

        #region Delink Attribute
        public bool DelinkGroupAttribute(int groupId, int attributeId)
        {
            return this._IUserGroupRepository.DelinkGroupAttribute(groupId, attributeId);
        }
        #endregion

        #region Link GroupToUser
        public bool LinkGroupToUser(int groupId, int userId)    
        {
            return this._IUserGroupRepository.LinkGroupToUser(groupId, userId);
        }
        #endregion

        #region Delink GroupToUser
        public bool DelinkGroupToUser(int groupId, int userId)  
        {
            return this._IUserGroupRepository.DelinkGroupToUser(groupId, userId);
        }
        #endregion

        /// <summary>
        /// Register void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}