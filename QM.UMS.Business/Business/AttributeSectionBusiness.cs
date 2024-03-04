// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeSectionBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>06-12-2016</createdOn>
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
    ///   Class:        <AttributeSectionBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class AttributeSectionBusiness : IAttributeSectionBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IAttributeSectionRepository _IAttributeSectionRepository;
        #endregion

        #region Constructor
        public AttributeSectionBusiness(IAttributeSectionRepository _iAttributeSectionRepository)
        {
            this._IAttributeSectionRepository = _iAttributeSectionRepository;
        }
        #endregion

        #region GET Method

        /// <summary>Get Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Attribute Section Details</returns>
        public List<AttributeSection> GetAllAttributeSections()
        {
            return this._IAttributeSectionRepository.GetAllAttributeSections();
        }

        /// <summary>Get Attribute Section Details By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Attribute Section Details By Id</returns>
        public AttributeSection GetAttributeSectionsById(int Id)
        {
            return this._IAttributeSectionRepository.GetAttributeSectionsById(Id);
        }

        #endregion

        #region POST Method

        /// <summary>Add New Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Add Attribute Section Details</returns>
        public bool AddAttributeSection(AttributeSection attributeSection)
        {
            return this._IAttributeSectionRepository.AddAttributeSection(attributeSection);
        }

        #endregion

        #region PUT Method

        /// <summary>Update Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Attribute Section Model</param>
        /// <returns>Update Status</returns>
        public bool UpdateAttributeSection(AttributeSection attributeSection)
        {
            return this._IAttributeSectionRepository.UpdateAttributeSection(attributeSection);
        }

        #endregion

        #region DELETE Method

        public bool RemoveAttributeSection(int Id) {
            return this._IAttributeSectionRepository.RemoveAttributeSection(Id);
        }

        #endregion

        #region GetAttributeSectionByGroupId
        public List<AttributeSection> GetUserAttibuteByUserId(int userId)  
        {
            return this._IAttributeSectionRepository.GetUserAttibuteByUserId(userId);
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