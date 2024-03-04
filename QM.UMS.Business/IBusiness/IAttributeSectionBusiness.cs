// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IAttributeSectionBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>06-12-2017</createdOn>
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
    ///   Class:        <IAttributeSectionBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IAttributeSectionBusiness
    {

        #region GET Methods

        List<AttributeSection> GetAllAttributeSections();

        AttributeSection GetAttributeSectionsById(int Id);

        #endregion

        #region POST Methods

        bool AddAttributeSection(AttributeSection attributeSection);
        
        #endregion

        #region PUT Methods

        bool UpdateAttributeSection(AttributeSection attributeSection);

        #endregion

        #region DELETE Methods

        bool RemoveAttributeSection(int Id);

        #endregion

        #region GET AttibuteSectionByGroupId
        List<AttributeSection> GetUserAttibuteByUserId(int userId);  
        #endregion
        

    }
}