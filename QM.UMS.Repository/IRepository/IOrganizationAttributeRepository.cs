// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizationAttributeRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;   
    using System.Globalization;
    using System.Configuration;
    using CommonApplicationFramework.Common;
    using QM.UMS.Models; 
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IOrganizationAttributeRepository>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IOrganizationAttributeRepository
    {

        #region GET Methods

        List<OrganizationAttribute> GetAllOrganizationAttribute();

        #endregion

        #region POST Methods

        bool AddOrganizationAttributes(OrganizationAttribute organizationAttribute);
        
        #endregion

        #region PUT Methods

        bool UpdateOrganizationAttribute(OrganizationAttribute organizationAttributes);

        #endregion

        #region DELETE Methods

        bool RemoveOrganizationAttributes(int id);

        #endregion      
    }
}