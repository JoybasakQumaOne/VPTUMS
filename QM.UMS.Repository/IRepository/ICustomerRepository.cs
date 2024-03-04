// -----------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomerRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Atanu</author>
// <createdOn>26-12-2017</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.IRepository
{
    #region Namespace
	using CommonApplicationFramework.DataHandling;
	using System.Collections.Generic;
	using QM.UMS.Models;
    using System;
    using CommonApplicationFramework.Common;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ICustomerRepository>
    ///   Description:  <Contains AddCustomer>
    ///   Author:       <Atanu>                    
    /// -----------------------------------------------------------------
    public interface ICustomerRepository : ICommon
    {

        #region GET Methods
        CustomerAddresses GetCustomerAddress(int addressId);
        CustomerProfile GetCustomerProfileDetails(string customerid);
        Customer GetCustomerDetailsByEmail(string emailId);
        bool IsVarifiedCustomer(string emailId);
        CustomerModel GetCustomerDetailsByPasswordResetCode(string PasswordResetCode);
        List<CustomerAddresses> GetCustomerAddressById(string customerId);
        CustomerAddressDetail GetCustomerAddressDetail(string customerId);
        bool VerifyCustomerOTP(string email, string otp);  
        #endregion

        #region POST Methods
        
        int AddCustomer(Customer customer, string bookGuid="");
		int AddAdminAsCustomer(string email);
        int AddCustomerfacebook(string code, string state, string scope);
        bool ResetPassword(Customer customer);
        int AddCustomerRole(GroupCustomerModel groupCustomer);
        int AddCustomerAddress(CustomerAddresses customerAddress);
        int AddCustomerProfile(CustomerProfile customerProfile);

        #endregion

        #region PUT Method
        
        int UpdateCustomerAddress(CustomerAddresses customerAddress);
        int UpdateCustomerProfile(CustomerProfile customerProfile);
        bool ResendOTP(string email, string OTP);
		bool UpdatePasswordResetCode(string emailId, string passwordResetCode);
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
