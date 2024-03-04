// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationAttributeOptionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IOrganizationAttributeBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IOrganizationAttributeOptionBusiness
    {
        bool AddOrganizationAttributeOptions(Item attributeOptions);

        List<Item> GetOrganizationAttributeOptions(int id);

        bool UpdateOrganizationAttributeOptions(Item attributeOptions);

        bool DeleteOrganizationAttributeOptions(int id);
    }
}