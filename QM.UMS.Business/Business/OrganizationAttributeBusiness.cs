// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationAttributeBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2016</createdOn>
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
    ///   Class:        <OrganizationAttributeBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationAttributeBusiness : IOrganizationAttributeBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IOrganizationAttributeRepository _IOrganizationAttributeRepository;
        #endregion

        #region Constructor
        public OrganizationAttributeBusiness(IOrganizationAttributeRepository _iOrganizationAttributeRepository)
        {
            this._IOrganizationAttributeRepository = _iOrganizationAttributeRepository;
        }
        #endregion

        #region GET Method

        /// <summary>Get Organization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organizations Details</returns>
        public List<OrganizationAttribute> GetAllOrganizationAttribute()
        {
            return this._IOrganizationAttributeRepository.GetAllOrganizationAttribute();
        }

        #endregion

        #region POST Method

        /// <summary>Add New Organization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Add Organization Attribute Details</returns>
        public bool AddOrganizationAttributes(OrganizationAttribute organizationAttribute)
        {
            return this._IOrganizationAttributeRepository.AddOrganizationAttributes(organizationAttribute);
        }

        #endregion

        #region PUT Method

        /// <summary>Update Organisation Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organisation Model</param>
        /// <returns>Update Status</returns>
        public bool UpdateOrganizationAttribute(OrganizationAttribute organizationAttribute)
        {
            return this._IOrganizationAttributeRepository.UpdateOrganizationAttribute(organizationAttribute);
        }

        #endregion

        #region DELETE Method

        public bool RemoveOrganizationAttributes(int id)
        {
            return this._IOrganizationAttributeRepository.RemoveOrganizationAttributes(id);
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