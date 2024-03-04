// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizationAttributeOptionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
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
    using CommonApplicationFramework.Common;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IOrganizationAttributeOptionRepository>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------


    public interface IOrganizationAttributeOptionRepository
    {
        bool AddOrganizationAttributeOptions(Item attributeOptions);

        bool UpdateOrganizationAttributeOptions(Item attributeOptions);

        List<Item> GetOrganizationAttributeOptions(int Id);

        bool DeleteOrganizationAttributeOptions(int id);  
    }
}