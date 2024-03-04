
using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QM.UMS.Business.IBusiness
{
    public interface IResourceBusiness
    {
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

        /// <summary>/// Get Resources By Id /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="Id">Resource Id</param>
        /// <returns>returns single entity of resource</returns>
        List<ResourceModel> GetAllResourceByModule(string code, string Module);

        /// <summary>/// Get Resources By Id /// </summary>
        /// <param name="Id">Module</param>
        /// /// <param name="Id">User Id</param>
        /// <returns>returns list of string containing action and resource code</returns>
        List<object> GetAllResourceByModuleUser(string uid);

        /// <summary>/// Add Resources to Role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of CommonModel</param>
        /// <returns>returns true or false</returns>
        bool AddResource(string code, ResourceModel resource);

        /// <summary>/// Add Resources to Role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of CommonModel</param>
        /// <returns>returns true or false</returns>
        bool UpdateResource(string code, ResourceModel resource);
    }
}