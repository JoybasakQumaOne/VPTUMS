// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IPhoneBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>13-12-2017</createdOn>
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
    ///   Class:        <IOrganizationBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IPhoneBusiness : ICommon
    {
        #region GET Method

        List<PhoneModel> GetAllPhoneList(string ControlType, int ControlId);

        #endregion

        #region POST Methods

        bool AddPhoneDetails(List<PhoneModel> phoneDetails, int controlId);

        #endregion

        #region PUT Methods

        bool UpdatePhoneDetails(PhoneModel phoneDetails);

        bool UpdatePhoneDetailsList(List<PhoneModel> phoneDetails);

        #endregion

        #region DELETE Methods

        bool RemovePhoneDetails(int Id);

        #endregion
    }
}