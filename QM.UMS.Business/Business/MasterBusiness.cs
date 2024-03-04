// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>04-12-2014</createdOn>
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
    ///   Class:        <MasterBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class MasterBusiness : IMasterBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IMasterRepository _IMasterRepository;
        #endregion

        #region Constructor
        public MasterBusiness(IMasterRepository _iMasterRepository)
        {
            this._IMasterRepository = _iMasterRepository;
        }
        #endregion

        #region GET Method

        /// <summary>Get Country Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Country Details</returns>
        public List<CountryModel> GetAllCountries()
        {
            List<CountryModel> country = new List<CountryModel>();
            return country = this._IMasterRepository.GetAllCountries();
        }

        /// <summary>Get Country Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Country Details</returns>
        public List<StateProvinceModel> GetAllStates(int CountryCode)
        {
            List<StateProvinceModel> state = new List<StateProvinceModel>();
            return state = this._IMasterRepository.GetAllStates(CountryCode);
        }

        public List<DBViewModel> GetInstanceDetails()
        {
            return this._IMasterRepository.GetInstanceDetails();
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
