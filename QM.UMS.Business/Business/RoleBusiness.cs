// ----------------------------------------------------------------------------------------------------------------
// <copyright file="RoleBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.Business
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;   
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Models;
    using QM.UMS.Repository.Repository;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <RoleBusinessLogic>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class RoleBusiness : IRoleBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is private variable
        /// </summary>
        private readonly IRoleRepository _IRoleRepository;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
            /// <param name="_prmIHttpClientBusiness">_prmIHttpClientBusiness</param>
        public RoleBusiness(IRoleRepository iRoleRepository)
        {
            this._IRoleRepository = iRoleRepository;
        }
        #endregion

        #region GET Methods

        /// <summary>Get role details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of roles</returns>
        public List<RoleModel> GetRoles(string code, string ModuleCode)
        {
            List<RoleModel> role = new List<RoleModel>();
            return role = this._IRoleRepository.GetRoles(code, ModuleCode);              
        }

        /// <summary>Get role details Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Role Id</param>
        /// <returns>returns single entity of role</returns>
        public RoleModel GetRoleById(string code, int Id)
        {
            RoleModel role = new RoleModel();
            return role = this._IRoleRepository.GetRoleById(code, Id);           
        }

        /// <summary>Get list of roles assigned to group</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>returns list of roles</returns>
        public List<RoleModel> GetRolesAssignedtoRolesGroup(string code, int groupId)
        {
            List<RoleModel> role = new List<RoleModel>();
            return role = this._IRoleRepository.GetRolesAssignedToRolesGroup(code, groupId);           
        }

        /// <summary>Get Roles By Status</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="status">Role Status</param>
        /// <returns>returns list of roles</returns>
        public List<RoleModel> GetRolesByStatus(string code, string status)
        {
            List<RoleModel> role = new List<RoleModel>();
            return role = this._IRoleRepository.GetRolesByStatus(code, status);           
        }

        #endregion

        #region POST Methods

        /// <summary>Add Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Role Model</param>
        /// <returns>returns bool value</returns>
        public bool AddRole(string code, RoleModel role)
        {
            bool Status = false;
            Status = this._IRoleRepository.AddRole(code, role);
            return Status;
        }

        /// <summary>Access permission of role</summary>
        /// <param name="code">Code in Entity</param>        
        /// <param name="permissions">Object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        public bool AccessPermissionToRole(string code, CommonModel permissions)
        {
            return this._IRoleRepository.AccessPermissionToRole(code, permissions);           
        }

        /// <summary>Delete permission of role</summary>
        /// <param name="code">Code in Entity</param>        
        /// <param name="permissions">Object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        public bool DeletePermissionToRole(string code, CommonModel permissions)
        {
            return this._IRoleRepository.RemovePermissionFromRole(code, permissions);
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">Object of Role Model</param>
        /// <returns>returns bool value</returns>
        public bool UpdateRole(string code, RoleModel role)
        {
            return this._IRoleRepository.UpdateRole(code, role);
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Role by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Role Id</param>
        /// <returns>returns bool value</returns>
        public bool RemoveRoleInfo(string code, int Id)
        {
            return this._IRoleRepository.RemoveRoleInfo(code, Id);
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