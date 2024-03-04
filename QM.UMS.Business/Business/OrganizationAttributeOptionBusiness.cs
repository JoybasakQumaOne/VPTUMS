// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationAttributeOptionBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.Business
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <OrganizationAttributeOptionBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationAttributeOptionBusiness : IOrganizationAttributeOptionBusiness
    {
        #region variable Declaration
        private readonly IOrganizationAttributeOptionRepository _IOrganizationAttributeOptionRepository;    
        #endregion

        #region Constructor
        public OrganizationAttributeOptionBusiness(IOrganizationAttributeOptionRepository _iorganizationAttributeOptionsRepository)    
        {
            this._IOrganizationAttributeOptionRepository = _iorganizationAttributeOptionsRepository;  
        }
        #endregion

        #region GetAllById
        public List<Item> GetOrganizationAttributeOptions(int id)
        {
            return this._IOrganizationAttributeOptionRepository.GetOrganizationAttributeOptions(id);
        }
        #endregion

        #region Post
        public bool AddOrganizationAttributeOptions(Item attributeOptions)
        {
            return this._IOrganizationAttributeOptionRepository.AddOrganizationAttributeOptions(attributeOptions);
        }
        #endregion

        #region Put
        public bool UpdateOrganizationAttributeOptions(Item attributeOptions)
        {
            return this._IOrganizationAttributeOptionRepository.UpdateOrganizationAttributeOptions(attributeOptions);
        }
        #endregion

        #region Delete
        public bool DeleteOrganizationAttributeOptions(int id)
        {
            return this._IOrganizationAttributeOptionRepository.DeleteOrganizationAttributeOptions(id);
        }
        #endregion
    }
}