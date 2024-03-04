// -----------------------------------------------------------------------------------------------------------------
// <copyright file="IRoleBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using QM.UMS.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IRoleBusinessLogic>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IRoleBusiness
    {
        /// <summary>/// This method is used to get role details by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of roles</returns>
        List<RoleModel> GetRoles(string code, string ModuleCode);

        /// <summary>/// This method is used to get role details by code and Id/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Id is Parameter to fetch data</param>
        /// <returns>returns single entity of role</returns>
        RoleModel GetRoleById(string code, int Id);

        /// <summary>/// This method is used to create role by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">role is a object of RoleModel</param>
        /// <returns>returns bool value</returns>
        bool AddRole(string code, RoleModel role);

        /// <summary>/// This method is used to update role by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="role">role is a object of RoleModel</param>
        /// <returns>returns bool value</returns>
        bool UpdateRole(string code, RoleModel role);

        /// <summary>/// This method is used to delete role by Id/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Id is Parameter to delete data</param>
        /// <returns>returns bool value</returns>
        bool RemoveRoleInfo(string code, int Id);

        /// <summary>/// This method is used to create permission of role/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="id">Role Createdby Id</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="permissions">permission is object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        bool AccessPermissionToRole(string code, CommonModel permissions);

        /// <summary>/// This method is used to delete permission of role/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="id">Role createdby id</param>
        /// <param name="roleId">Role id</param>
        /// <param name="permissions">permission is object of Commonmodel</param>
        /// <returns>returns bool value</returns>
        bool DeletePermissionToRole(string code, CommonModel permissions);

        /// <summary>/// This method is used to get list of roles assigned to group/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>returns list of roles</returns>
        List<RoleModel> GetRolesAssignedtoRolesGroup(string code, int groupId);

        List<RoleModel> GetRolesByStatus(string code, string status);
    }
}