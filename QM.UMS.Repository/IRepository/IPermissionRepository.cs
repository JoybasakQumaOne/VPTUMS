// -----------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.IRepository
{
    #region Namespace    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using QM.UMS.Models;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IPermissionRepository>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IPermissionRepository
    {
        string UserId { get; set; }

        string AgentCode { get; set; }

        string UserIPAddress { get; set; }

        /// <summary>/// This method is used to get Permission details by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of Permission</returns>
        List<PermissionModel> GetPermission(string code);

        /// <summary>/// This method is used to get Permission details by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Id is Parameter to fetch data</param>
        /// <returns>returns single entity of permission</returns>
        PermissionModel GetPermissionById(string code, int permissionId);

        /// <summary>/// This method is used to create permission by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permission">permission is a object of PermissionModel</param>
        /// <returns>returns bool value</returns>
        bool AddPermission(string code, PermissionModel permission);

        /// <summary>/// This method is used to update permission by code/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permission">permission is a object of PermissionModel</param>
        /// <returns>returns bool value</returns>
        bool UpdatePermission(string code, PermissionModel permission);

        /// <summary>/// This method is used to delete permission by Id/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Id">Id is Parameter to delete data</param>
        /// <returns>returns bool value</returns>
        bool RemovePermissionInfo(string code, int permissionId);

        /// <summary>/// This method is used to get permission by role/// </summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="permissionStatus">Bool Value</param>
        /// <returns>returns list of permission</returns>
        List<PermissionModel> GetPermissionByRole(string code, int roleId);
    }
}