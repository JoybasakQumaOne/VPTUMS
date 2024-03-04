// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IMasterRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>04-12-2017</createdOn>
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
    ///   Class:        <IMasterRepository>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IMasterRepository
    {

        #region GET Methods

        List<CountryModel> GetAllCountries();

        List<StateProvinceModel> GetAllStates(int CountryId);

        List<DBViewModel> GetInstanceDetails();

        #endregion
    }
}