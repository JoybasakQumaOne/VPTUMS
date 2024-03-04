// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>13-12-2016</createdOn>
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
    ///   Class:        <PhoneBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class PhoneBusiness : IPhoneBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IPhoneRepository _IPhoneRepository;
        #endregion

        #region Properties
            public string RequestId { get; set; }
            public string Code { get; set; }
        #endregion

        #region Constructor
        public PhoneBusiness(IPhoneRepository _iPhoneRepository)
        {
            this._IPhoneRepository = _iPhoneRepository;
        }
        #endregion

        #region GET Method

        /// <summary>Get Phone Details</summary>
        /// <param name="ContactType">ContactType in Entity</param>
        /// <param name="ContactId">ContactId in Entity</param>
        /// <returns>List of Phone Details</returns>
        public List<PhoneModel> GetAllPhoneList(string ControlType, int ControlId)
        {
            return this._IPhoneRepository.GetAllPhoneList(ControlType, ControlId);
        }
        
        #endregion

        #region POST Method

        /// <summary>Add New Phone Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Add Phone Details</returns>
        public bool AddPhoneDetails(List<PhoneModel> phoneDetails, int controlId)
        {
            try
            {
                bool status = false;
                foreach (PhoneModel phoneModel in phoneDetails)
                {
                    status = this._IPhoneRepository.AddPhoneDetails(phoneModel, controlId);
                }
                return status;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion

        #region PUT Method

        /// <summary>Update Phone Details</summary>
        /// <param name="Phone">Object of Phone Model</param>
        /// <returns>Update Status</returns>
        public bool UpdatePhoneDetails(PhoneModel phoneDetails)
        {
            return this._IPhoneRepository.UpdatePhoneDetails(phoneDetails);
        }

        /// <summary>Update Phone Details</summary>
        /// <param name="Phone">Object of Phone Model</param>
        /// <returns>Update Status</returns>
        public bool UpdatePhoneDetailsList(List<PhoneModel> phoneDetails)
        {
            return this._IPhoneRepository.UpdatePhoneDetailsList(phoneDetails);
        }
        
        #endregion

        #region DELETE Method

        /// <summary>Remove Phone Details</summary>
        /// <param name="id">id in Entity</param>
        /// <returns>deleted Status</returns>
        public bool RemovePhoneDetails(int Id)
        {
            return this._IPhoneRepository.RemovePhoneDetails(Id);
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
