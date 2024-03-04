// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>16-11-2016</createdOn>
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
    using CommonApplicationFramework.DataHandling;
    using Newtonsoft.Json;
    using System.Net;
    using System.IO;
    using CommonApplicationFramework.Logging;
    using DMSMicroService.Module;
    using DMSMicroService.DMSFileCreator;
    using QM.UMS.Repository.Helper;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <OrganizationBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationBusiness : RequestHeader, IOrganizationBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is priavte variable
        /// </summary>
        private readonly IOrganizationRepository _IOrganizationRepository;
        private readonly IUsersBusiness _IUsersBusiness;
        private readonly IUsersRepository _IUsersRepository;
        private readonly ICommonHelperRepository _ICommonHelperRepository;

        #endregion

        #region Properties
        public string RequestId { get; set; }
        #endregion

        #region Constructor
        public OrganizationBusiness(IOrganizationRepository _iOrganizationRepository, IUsersBusiness IUsersBusiness, IUsersRepository _IUsersRepository, ICommonHelperRepository _ICommonHelperRepository)
        {
            this._IOrganizationRepository = _iOrganizationRepository;
            this._IUsersBusiness= IUsersBusiness;
            this._IUsersRepository = _IUsersRepository;
            this._ICommonHelperRepository = _ICommonHelperRepository;
        }
        #endregion

        #region GET Method

        /// <summary>Get Organizations Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organizations Details</returns>
        public List<OrganizationModel> GetAllOrganizations()
        {
		 
            List<OrganizationModel> organization = new List<OrganizationModel>();
            return organization = this._IOrganizationRepository.GetAllOrganizations();
        }

        /// <summary>Get Organization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Details</returns>
        public OrganizationModel GetOrganizationDetails(Guid Id)
        {
            OrganizationModel organization = new OrganizationModel();
            return organization = this._IOrganizationRepository.GetOrganizationDetails(Id);
        }

        public OrganizationModel GetOrganizationInfo(string Id)
        {
            OrganizationModel organization = new OrganizationModel();
            return organization = this._IOrganizationRepository.GetOrganizationInfo(Id);
        }

        #endregion

        #region POST Method

        /// <summary>Add New Organizations Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Add Organizations Details</returns>
        public bool AddOrganization(OrganizationModel organization)
        {
            organization.OrgGUID = Guid.NewGuid();
            return this._IOrganizationRepository.AddOrganization(organization);
            //if (isCreated)
            //{
            //    this._IOrganizationRepository.CreatFolderDetail(organization);
            //}
            //return isCreated;
        }

        public bool Register(RegisterOrganizationModel org)
        {

            Guid orgGuid =Guid.Parse( this.ConnectionId);
            if (this._IOrganizationRepository.RegisterOrganization(new OrganizationModel()
            {
                Name = org.Name,
                Website = org.Website,
                OrganizationCode = org.Name.Length >= 5 ? org.Name.Replace(" ", string.Empty).Substring(0, 5) : Guid.NewGuid().ToString().Substring(0, 5),
                Email = org.customerProfile.Email,
                OrgGUID = orgGuid
            }))
            {
                // The below code is needed to send the mail,mail template condition
               int id= _IUsersRepository.IfUserExistsInApplicationDb(org.customerProfile.Email);
               if (_IUsersBusiness.AddAdminUser(new UserProfileModel()
                {
                    FirstName = org.customerProfile.FirstName,
                    LastName = org.customerProfile.LastName,
                    Email = org.customerProfile.Email,
                    Organisations = new List<Guid>() { orgGuid },
                    Name = org.customerProfile.Email,
                    UserType = new ItemCode() { Code = "Admin" },
                    IsSuperUser = false,
                    IsOTPVerified = true,
                    Password=org.customerProfile.Password,
                    Phone=org.customerProfile.Phone,
                }) > 0)
                {
                    if(id>0)
                    {
                        _ICommonHelperRepository.OrganizationRegistrationWithExistingUserMail(new CommonApplicationFramework.Notification.EmailSenderModel() {
                        FirstName = org.customerProfile.FirstName,
                        EmailId=org.customerProfile.Email,
                        });
                    }
                    else
                    {
                        _ICommonHelperRepository.OrganizationRegistrationMail(new CommonApplicationFramework.Notification.EmailSenderModel() { 
                        FirstName = org.customerProfile.FirstName,
                        EmailId = org.customerProfile.Email

                        });
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>Add New Organizations Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>Add Organizations Details</returns>
        public bool AddOrganizationUsers(OrganizationUsersModel organizationUsers)
        {
            return this._IOrganizationRepository.AddOrganizationUsers(organizationUsers);
        }

        public bool AddOrgTypeMapping(List<OrganizationType> orgTypeDetails, int  organizationId)
        {
            return this._IOrganizationRepository.AddOrgTypeMapping(orgTypeDetails, organizationId);
        }

        #endregion

        #region PUT Method

        /// <summary>Update Organisation Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organisation Model</param>
        /// <returns>Update Status</returns>
        public bool Updateorganization(OrganizationModel organization)
        {
            return this._IOrganizationRepository.Updateorganization(organization);
        }

        #endregion

        #region DELETE Method

        public bool RemoveOrganizationDetails(Guid Id) {
            return this._IOrganizationRepository.RemoveOrganizationDetails(Id);
        }

        public bool RemoveOrgTypeMapping(int Id)
        {
            return this._IOrganizationRepository.RemoveOrgTypeMapping(Id);
        }

        #endregion

        #region Init Method
        public void Init(string requestId)
        {
            this.RequestId = requestId;
            this._IOrganizationRepository.RequestId = requestId;
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
