// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>05-04-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

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
    ///   Class:        <IResourceRepository>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IResourceRepository
    {
        string UserId { get; set; }

        string AgentCode { get; set; }

        string UserIPAddress { get; set; }

        /// <summary>/// Get Resources By Id /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="Id">Resource Id</param>
        /// <returns>returns single entity of resource</returns>
        ResourceModel GetResourceById(string Code, int Id);

        /// <summary> /// Get resources by roleid and modulecode /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="roleId">roleId</param>
        /// <param name="moduleCode">moduleCode</param>
        /// <returns>returns list of resources</returns>
        List<ResourceModel> GetResourcesByRole(string Code, int roleId, string moduleCode);

        /// <summary> /// Remove resources from role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="resourceId">Resource Id</param>
        /// <returns>returns true or false</returns>
        bool RemoveResourceFromRole(string Code, int roleId, int resourceId);

        /// <summary>/// Add Resources to Role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of CommonModel</param>
        /// <returns>returns true or false</returns>
        bool AddResourceToRole(string Code, CommonModel Resources);

        /// <summary>Get Resource Details by UserId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Resource</returns>
        List<ResourceModel> GetResourcesByUserId(string code, string Module, int userId);

        /// <summary>Get Resource Details by Module</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">Module</param>
        /// <returns>returns list of Resource</returns>
        List<ResourceModel> GetAllResourcesByModuleId(string code, string Module);


        bool AddResource(string code, ResourceModel resource);
        bool UpdateResource(string code, ResourceModel resource);

        List<string> GetResourcesByUser(string uid);

    }
}