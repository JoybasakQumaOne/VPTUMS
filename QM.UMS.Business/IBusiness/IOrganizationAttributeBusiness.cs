// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizationAttributeBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IOrganizationAttributeBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IOrganizationAttributeBusiness
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