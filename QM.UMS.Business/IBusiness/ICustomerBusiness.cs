// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomerBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Atanu</author>
// <createdOn>26-12-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    using CommonApplicationFramework.Common;
    #region Namespace
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ICustomerBusiness>
    ///   Description:  <Contains AddCustomer>
    ///   Author:       <Atanu>                    
    /// -----------------------------------------------------------------

    public interface ICustomerBusiness : ICommon
    {

        void Init(string requestId);

        #region GET Method

        CustomerAddresses GetCustomerAddress(int addressId);

        CustomerProfile GetCustomerProfileDetails(string customerid);

        Customer ForgotPassword(string emailId);//, string storedLocation, string orgLogo ,string orgName, string CompanyCode);  

        List<CustomerAddresses> GetCustomerAddressById(string id);

        CustomerAddressDetail GetCustomerAddressDetail(string customerId);

        bool VerifyCustomerOTP(string email, string otp);
        
        #endregion

        #region POST Methods

        int AddCustomer(Customer customer, string bookGuid="");
        object ResendVerificationMail(string mail);

        int AddCustomerfacebook(string code, string state, string scope);

        int AddCustomerRole(GroupCustomerModel groupCustomer);

        int AddCustomerAddress(CustomerAddresses customerAddress);

        int AddCustomerProfile(CustomerProfile customerProfile);

        #endregion

        #region PUT Methods

        int ChangePassword(ChangePassword changePassword);

        int ResetPassword(ChangePassword changePassword);
        bool ValidateOTP(ChangePassword changePassword);

        int UpdateCustomerAddress(CustomerAddresses customerAddress);
        
        int UpdateCustomerProfile(CustomerProfile customerProfile);

        bool ResendOTP(string email);

        #endregion

        #region DELETE Method

        int deleteCustomerRole(int Id);

        int deleteCustomerAddress(int Id);
        
        int deleteCustomerProfile(int Id);

        #endregion

        #region Social Media Login
        int SocialMediaUser(string socialMediaUserEmail);
        #endregion

    }
}
