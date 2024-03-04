// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IMasterBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>04-12-2017</createdOn>
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
    ///   Class:        <IMasterBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IMasterBusiness
    {

        #region GET Method

        List<CountryModel> GetAllCountries();

        List<StateProvinceModel> GetAllStates(int CountryId);

        List<DBViewModel> GetInstanceDetails();

        #endregion
    }
}
