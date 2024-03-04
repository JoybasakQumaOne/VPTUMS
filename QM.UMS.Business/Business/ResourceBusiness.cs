using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonApplicationFramework.Common;
using QM.UMS.Business.IBusiness;
using QM.UMS.Repository.IRepository;
using QM.UMS.Models;
using QM.UMS.Repository.Repository;

namespace QM.UMS.Business.Business
{
    public class ResourceBusiness:IResourceBusiness
    {
        #region Variable Declaration
        /// <summary>
        /// This is Private Variable
        /// </summary>
        private readonly IResourceRepository _IResourceRepository;
        #endregion

        #region Constructor
        /// <summary>Parameterized Constructor</summary>
        /// <param name="_prmIHttpClientBusiness">_prmIHttpClientBusiness</param>
        public ResourceBusiness(IResourceRepository _iResourceRepository)
        {
            this._IResourceRepository = _iResourceRepository;
        }
        #endregion

        #region Get
        /// <summary>Get Resource details</summary>
        /// <param name="code">Code in Entity</param>
        ///  /// <param2 name="code">Module in Entity</param>
        /// <returns>returns list of Resource</returns>
        public List<ResourceModel> GetAllResourceByModule(string code, string Module)
        {
            List<ResourceModel> resource = new List<ResourceModel>();
            return resource = _IResourceRepository.GetAllResourcesByModuleId(code, Module);            
        }

        /// <summary>Get Resource details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param2 name="code">Module in Entity</param>
        /// <param2 name="code">UserId in Entity</param>
        /// <returns>returns list of Resource</returns>
        public List<object> GetAllResourceByModuleUser(string uid)
        {
            List<string> resource = new List<string>();
            resource = this._IResourceRepository.GetResourcesByUser(uid);
            if (resource != null)
            {
                var sample = resource.ToArray();
                List<object> objParams = resource.ToList<object>();
                return objParams;
            }
            return null;
        }


        /// <summary> /// Get resources by roleid and modulecode /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="roleId">roleId</param>
        /// <param name="moduleCode">moduleCode</param>
        /// <returns>returns list of resources</returns>
        public List<ResourceModel> GetResourcesByRole(string code, int roleId, string moduleCode)
        {
            List<ResourceModel> resource = new List<ResourceModel>();
            return resource = this._IResourceRepository.GetResourcesByRole(code, roleId, moduleCode);            
        }

        /// <summary>/// Get Resources By Id /// </summary>
        /// <param name="Code">Code in Entity</param>
        /// <param name="Id">Resource Id</param>
        /// <returns>returns single entity of resource</returns>
        public ResourceModel GetResourceById(string code, int resourceId)
        {
            ResourceModel resource = new ResourceModel();
            return resource = this._IResourceRepository.GetResourceById(code, resourceId);            
        }
        #endregion

        #region Post
        /// <summary>Add Resource</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permission">Object of Resource Model</param>
        /// <returns>returns bool value</returns>
        public bool AddResource(string code, ResourceModel resource)
        {
            return this._IResourceRepository.AddResource(code, resource);
        }


        /// <summary>/// Add Resources to Role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="Resources">Resources is object of CommonModel</param>
        /// <returns>returns true or false</returns>
        public bool AddResourceToRole(string code, CommonModel Resources)
        {
            return this._IResourceRepository.AddResourceToRole(code, Resources);
        }
        #endregion

        #region Put
        /// <summary>Update Resource</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="permission">Object of Resource Model</param>
        /// <returns>returns bool value</returns>
        public bool UpdateResource(string code, ResourceModel resource)
        {
            return this._IResourceRepository.UpdateResource(code, resource);
        }

        #endregion

        #region Delete

        /// <summary> /// Remove resources from role /// </summary>
        /// <param name="Code">Code in entity</param>
        /// <param name="roleId">Role Id</param>
        /// <param name="resourceId">Resource Id</param>
        /// <returns>returns true or false</returns>
        public bool RemoveResourceFromRole(string code, int roleId, int resourceId)
        {
            return this._IResourceRepository.RemoveResourceFromRole(code, roleId, resourceId);
        }
        #endregion

    }
}