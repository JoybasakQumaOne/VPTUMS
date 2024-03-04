// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationTypeBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>12-12-2017</createdOn>
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
    ///   Class:        <OrganizationTypeBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationTypeBusiness : IOrganizationTypeBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IOrganizationTypeRepository _IOrganizationTypeRepository;
        #endregion

        #region Properties
            public string RequestId { get; set; }
            public string Code { get; set; }
        #endregion

        #region Constructor
            public OrganizationTypeBusiness(IOrganizationTypeRepository _iOrganizationTypeRepository)
        {
            this._IOrganizationTypeRepository = _iOrganizationTypeRepository;
        }
        #endregion

        #region GET Method

        /// <summary>Get Organization Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Type Details</returns>
        public List<OrganizationType> GetAllOrganizationTypes()
        {
            return this._IOrganizationTypeRepository.GetAllOrganizationTypes();
        }

        #endregion

        public void Init(string requestId)
        {
            this.RequestId = requestId;
        }

        /// <summary>
        /// Register void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}