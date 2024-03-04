// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Atanu</author>
// <createdOn>26-12-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using QM.UMS.DMS.Module;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web.Script.Serialization;
    #endregion
    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <CustomerRepository>
    ///   Description:  <Contains AddCustomer>
    ///   Author:       <Atanu>                    
    /// -----------------------------------------------------------------

    public class CustomerRepository : ICustomerRepository, IDisposable
    {
        #region Variable Declaration

        private DBManager dbManager;
        private readonly IDocumentProcessRepository _IDocumentProcessRepository;
        #endregion
        public CustomerRepository(IDocumentProcessRepository _IDocumentProcessRepository)
        {
            this._IDocumentProcessRepository = _IDocumentProcessRepository;
        }

        #region Properties
        public string RequestId { get; set; }
        public string Code { get; set; }
        #endregion

        #region GET Methods

        /// <summary>Get Customer Address Details</summary>
        /// <param name="address Id">Address Id of customer</param>
        /// <returns>Address Details for Customer</returns>
        public CustomerAddresses GetCustomerAddress(int addressId)
        {
            CustomerAddresses addressDetails = new CustomerAddresses();
			
			try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string addressGetQuery = QueryConfig.CustomerQuerySettings["GetUserAddress"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@addressId", addressId);
                    IDataReader orderAddress = dbManager.ExecuteReader(CommandType.Text, addressGetQuery);
                    if (orderAddress.Read())
                    {
                        addressDetails = new CustomerAddresses();
                        //addressDetails.StateProvinceModel = new Item();
                        addressDetails.countryModel = new Item();

                        addressDetails.Id = Convert.ToInt32(orderAddress["Id"]);
                        addressDetails.CustomerId = Convert.ToString(orderAddress["CustomerId"]);
                        addressDetails.CustomerName = Convert.ToString(orderAddress["CustomerName"]);
                        addressDetails.Address1 = Convert.ToString(orderAddress["Address1"]);
                        addressDetails.Address2 = Convert.ToString(orderAddress["Address2"]);
                        addressDetails.ZipPostalCode = Convert.ToString(orderAddress["ZipPostalCode"]);
                        addressDetails.StateProvinceModel = Convert.ToString(orderAddress["StateId"]);
                        //addressDetails.StateProvinceModel.Value = Convert.ToString(orderAddress["stateName"]);
                        addressDetails.countryModel.Id = Convert.ToInt32(orderAddress["CountryId"]);
                        addressDetails.countryModel.Value = Convert.ToString(orderAddress["CountryName"]);
                        addressDetails.Longitude = (orderAddress["Longitude"] == DBNull.Value) ? 0 : Convert.ToDecimal(orderAddress["Longitude"]);
                        addressDetails.Latitude = (orderAddress["Latitude"] == DBNull.Value) ? 0 : Convert.ToDecimal(orderAddress["Latitude"]);
                        addressDetails.City = Convert.ToString(orderAddress["City"]);
                        addressDetails.AddressNote = Convert.ToString(orderAddress["AddressNote"]);
                        addressDetails.isDelivery = Convert.ToBoolean(orderAddress["isDelivery"]);
                        addressDetails.isInvoice = Convert.ToBoolean(orderAddress["isInvoice"]);
                        addressDetails.PhoneNumber = Convert.ToString(orderAddress["PhoneNumber"]);
                    }
                    orderAddress.Close();
                }
                return addressDetails;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERADDRESSFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERADDRESSFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERADDRESSFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERADDRESSFOUNDFAIL"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Get Customer Profile Details</summary>
        /// <param name="customer Id">Customer Id to get details</param>
        /// <returns>Profile Details of Customer</returns>
        public CustomerProfile GetCustomerProfileDetails(string customerid)
        {
            CustomerProfile customerProfileDetail = new CustomerProfile();
            string filePath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + "/" +
				ConfigurationManager.AppSettings["ProjectName"].ToString() + "/" + Constant.USERIMAGE.ToString() + "/";
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string getCustomerDetails = QueryConfig.CustomerQuerySettings["GetCustomerDetails"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@CustomerId", customerid);
                    IDataReader customerDetails = dbManager.ExecuteReader(CommandType.Text, getCustomerDetails);
                    if (customerDetails.Read())
                    {
                        customerProfileDetail = new CustomerProfile();
                        customerProfileDetail.Image = new Model.ImageDocumentUpload();
                        customerProfileDetail.Id = Convert.ToInt32(customerDetails["Id"]);
                        customerProfileDetail.CustomerId = Convert.ToInt32(customerDetails["CustomerId"]);
                        customerProfileDetail.FirstName = Convert.ToString(customerDetails["FirstName"]);
                        customerProfileDetail.LastName = Convert.ToString(customerDetails["LastName"]);
                        customerProfileDetail.MiddleName = Convert.ToString(customerDetails["MiddleName"]);
                        customerProfileDetail.Email = Convert.ToString(customerDetails["Email"]);
                        customerProfileDetail.Phone1 = Convert.ToString(customerDetails["Phone1"]);
                        customerProfileDetail.Phone2 = Convert.ToString(customerDetails["Phone2"]);
                        customerProfileDetail.Image.FileName = string.IsNullOrEmpty(customerDetails["Image"].ToString()) ? null : filePath + customerDetails["Image"].ToString();
                        customerProfileDetail.DOB = (customerDetails["DOB"] == DBNull.Value) ? (DateTime?)null : DateTime.Parse(customerDetails["DOB"].ToString());
                        customerProfileDetail.Country = ConvertData.ToInt(customerDetails["CountryId"]) > 0 ? new ItemCode { Id = ConvertData.ToInt(customerDetails["CountryId"]), Code = ConvertData.ToString(customerDetails["CountryName"]) } : null;
                    }
                    return customerProfileDetail;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERPROFILEFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERPROFILEFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERPROFILEFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERPROFILEFOUNDFAIL"].ToString(), ex.StackTrace);
            }
        }


        /// <summary>Forgot Password : Get Customer Detail By Email</summary>
        /// <param name="emailId">Email Id</param>
        /// <returns>Customer Detail</returns>
        public Customer GetCustomerDetailsByEmail(string emailId)
        {
            Customer customer = new Customer();
            customer.customerProfile = new CustomerProfile();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.CustomerQuerySettings["GetCustomerDetailsByEmail"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", emailId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        customer.Id = Convert.ToInt32(dr["Id"]);
                        customer.Username = dr["Username"].ToString();
                        customer.customerProfile.FirstName = dr["FirstName"].ToString();
                        customer.Email = dr["Email"].ToString();
                        customer.Salt = dr["Salt"].ToString();
                        customer.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                        customer.IsFirstLogin = Convert.ToBoolean(dr["IsFirstLogin"].ToString());
                        customer.IsOTPVerified = Convert.ToBoolean(dr["IsOTPVerified"].ToString());
                        customer.Password = dr["Password"].ToString();
                        customer.OTP = dr["OTP"].ToString();
                    }
                    return customer;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERDETAILSNOTFOUND", MessageConfig.MessageSettings["CUSTOMERDETAILSNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERDETAILSNOTFOUND", MessageConfig.MessageSettings["CUSTOMERDETAILSNOTFOUND"].ToString(), ex.StackTrace);
            }
        }


        public bool IsVarifiedCustomer(string emailId)
        {
            bool isvarified = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.CustomerQuerySettings["IsVarifiedCustomer"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", emailId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        isvarified = true;
                    }
                }
                return isvarified;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERDETAILSNOTFOUND", MessageConfig.MessageSettings["CUSTOMERDETAILSNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERDETAILSNOTFOUND", MessageConfig.MessageSettings["CUSTOMERDETAILSNOTFOUND"].ToString(), ex.StackTrace);
            }
        }
        /// <summary>Reset Password : Get User By Salt</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="PasswordResetCode">User Salt</param>
        /// <returns>User Details</returns>
        public CustomerModel GetCustomerDetailsByPasswordResetCode(string PasswordResetCode)
        {
            CustomerModel user = null;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    //PasswordResetCode = PasswordResetCode.Replace(" ", "+");
                    string query = QueryConfig.CustomerQuerySettings["GetCustomerDetailsByPasswordResetCode"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@PasswordResetCode", PasswordResetCode);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        user = new CustomerModel();
                        user.Id = Convert.ToInt32(dr["Id"].ToString());
                        user.Name = dr["Username"].ToString();
                        user.Email = dr["Email"].ToString();
                        user.FirstName = dr["FirstName"].ToString();
                       // user.ExpiresOn = DateTimeOffset.Parse(dr["ExpiresOn"].ToString());
                    }
                    return user;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERNOTFOUND", MessageConfig.MessageSettings["CUSTOMERNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERNOTFOUND", MessageConfig.MessageSettings["CUSTOMERNOTFOUND"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Get Customer Address Details</summary>
        /// <param name="Customer Id">Customer Id of customer</param>
        /// <returns>Address Details for Customer</returns>
        public List<CustomerAddresses> GetCustomerAddressById(string customerId)
        {
            CustomerAddresses addressDetails = new CustomerAddresses();
            List<CustomerAddresses> listAllAddress = new List<CustomerAddresses>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string addressGetQuery = QueryConfig.CustomerQuerySettings["GetCustomerAllAddress"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@customerId", customerId);
                    IDataReader orderAddress = dbManager.ExecuteReader(CommandType.Text, addressGetQuery);
                    while (orderAddress.Read())
                    {
                        addressDetails = new CustomerAddresses();
                        //addressDetails.StateProvinceModel = new Item();
                        addressDetails.countryModel = new Item();

                        addressDetails.Id = Convert.ToInt32(orderAddress["Id"]);
                        //addressDetails.CustomerId = Convert.ToInt32(orderAddress["CustomerId"]);
                        addressDetails.CustomerName = Convert.ToString(orderAddress["CustomerName"]);
                        addressDetails.Address1 = Convert.ToString(orderAddress["Address1"]);
                        addressDetails.Address2 = Convert.ToString(orderAddress["Address2"]);
                        addressDetails.ZipPostalCode = Convert.ToString(orderAddress["ZipPostalCode"]);
                        addressDetails.StateProvinceModel = Convert.ToString(orderAddress["StateId"]);
                        //addressDetails.StateProvinceModel.Value = Convert.ToString(orderAddress["stateName"]);
                        addressDetails.countryModel.Id = Convert.ToInt32(orderAddress["CountryId"]);
                        addressDetails.countryModel.Value = Convert.ToString(orderAddress["CountryName"]);
                        addressDetails.Longitude = (orderAddress["Longitude"] == DBNull.Value) ? 0 : Convert.ToDecimal(orderAddress["Longitude"]);
                        addressDetails.Latitude = (orderAddress["Latitude"] == DBNull.Value) ? 0 : Convert.ToDecimal(orderAddress["Latitude"]);
                        addressDetails.City = Convert.ToString(orderAddress["City"]);
                        addressDetails.AddressNote = Convert.ToString(orderAddress["AddressNote"]);
                        addressDetails.isDelivery = Convert.ToBoolean(orderAddress["isDelivery"]);
                        addressDetails.isInvoice = Convert.ToBoolean(orderAddress["isInvoice"]);
                        addressDetails.PhoneNumber = Convert.ToString(orderAddress["PhoneNumber"]);
                        listAllAddress.Add(addressDetails);
                    }

                    return listAllAddress;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERADDRESSFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERADDRESSFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERADDRESSFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERADDRESSFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                addressDetails = null;
            }
        }

        /// <summary>Get the Customer Details with Address</summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>Customer Address Detail</returns>
        public CustomerAddressDetail GetCustomerAddressDetail(string customerId)
        {
            CustomerAddressDetail customerAddressDetail = new CustomerAddressDetail();
			 
			try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string customerAddressDetails = QueryConfig.CustomerQuerySettings["CustomerAddressDetails"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@CustomerId", customerId);
                    IDataReader idr = dbManager.ExecuteReader(CommandType.Text, customerAddressDetails);

                    if (idr.Read())
                    {
                        customerAddressDetail = new CustomerAddressDetail();
                        customerAddressDetail.customerAddress = new CustomerAddresses();
                        customerAddressDetail.customerAddress.countryModel = new Item();

                        //customerAddressDetail.Id = Convert.ToInt32(idr["CustomerId"]);
                        customerAddressDetail.Name = Convert.ToString(idr["Username"]);
                        customerAddressDetail.Email = Convert.ToString(idr["Email"]);
                        customerAddressDetail.customerAddress.Id = Convert.ToInt32(idr["Id"]);
                        customerAddressDetail.customerAddress.CustomerId = Convert.ToString(idr["CustomerId"]);
                        customerAddressDetail.customerAddress.CustomerName = Convert.ToString(idr["CustomerName"]);
                        customerAddressDetail.customerAddress.Address1 = Convert.ToString(idr["Address1"]);
                        customerAddressDetail.customerAddress.Address2 = Convert.ToString(idr["Address2"]);
                        customerAddressDetail.customerAddress.ZipPostalCode = Convert.ToString(idr["ZipPostalCode"]);
                        customerAddressDetail.customerAddress.StateProvinceModel = Convert.ToString(idr["StateId"]);
                        customerAddressDetail.customerAddress.countryModel.Id = Convert.ToInt32(idr["CountryId"]);
                        customerAddressDetail.customerAddress.countryModel.Value = Convert.ToString(idr["CountryName"]);
                        customerAddressDetail.customerAddress.Longitude = (idr["Longitude"] == DBNull.Value) ? 0 : Convert.ToDecimal(idr["Longitude"]);
                        customerAddressDetail.customerAddress.Latitude = (idr["Latitude"] == DBNull.Value) ? 0 : Convert.ToDecimal(idr["Latitude"]);
                        customerAddressDetail.customerAddress.City = Convert.ToString(idr["City"]);
                        customerAddressDetail.customerAddress.AddressNote = Convert.ToString(idr["AddressNote"]);
                        customerAddressDetail.customerAddress.isDelivery = Convert.ToBoolean(idr["isDelivery"]);
                        customerAddressDetail.customerAddress.isInvoice = Convert.ToBoolean(idr["isInvoice"]);
                        customerAddressDetail.customerAddress.PhoneNumber = Convert.ToString(idr["PhoneNumber"]);
                    }
                    idr.Close();
                }
                return customerAddressDetail;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("CUSTOMERADDRESSFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERADDRESSFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("CUSTOMERADDRESSFOUNDFAIL", MessageConfig.MessageSettings["CUSTOMERADDRESSFOUNDFAIL"].ToString(), ex.StackTrace);
            }
        }

        public bool VerifyCustomerOTP(string email, string otp)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.CustomerQuerySettings["VerifyCustomerOTP"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@Email", email);
                    dbManager.AddParameters(1, "@OTP", otp);
                    int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (rowAffacted > 0)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("INVALIDINPUT", MessageConfig.MessageSettings["INVALIDINPUT"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("INVALIDINPUT", MessageConfig.MessageSettings["INVALIDINPUT"].ToString(), ex.StackTrace);
            }
        }

		private int AddUserPreference(string email)
		{
			int rowAffacted = 0;
			using (dbManager = new DBManager())
			{
				dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
				dbManager.Open();
				string query = QueryConfig.CustomerQuerySettings["AddUserPreference"].ToString();
				dbManager.CreateParameters(1);
				dbManager.AddParameters(0, "@Email", email);
				rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, query);
			}
			return rowAffacted;
		}

		#endregion

		#region POST Method

		/// <summary>Add Customer Details</summary>
		/// <param name="Customer"Customer in Entity</param>
		/// <returns>Insert Status</returns>

		public int AddCustomer(Customer customer, string bookGuid = "")
        {
            Customer customerModel = new Customer();
            customer.CustomerGuid = Guid.NewGuid();
            int retValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    string duplicateCheckCustomer = QueryConfig.CustomerQuerySettings["DuplicateCheckCustomer"].ToString();  
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", customer.Email);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateCheckCustomer);
                    if (dr.Read())
                    {
                        dr.Close();
                        throw new DuplicateException();
                    }
                    else
                    {
                        dr.Close();
                        customer.IsOTPVerified = false;
                        retValue = CustomerDetails(customer);
                        if (retValue > 0)
                        {
                            customer.customerProfile.CustomerId = retValue;
                            customer.customerProfile.Email = customer.Email;
							retValue = CustomerProfile(customer.customerProfile);
							if (retValue > 0)
							{
								if (!string.IsNullOrEmpty(bookGuid))
								{
									LinkCustomerToBook(bookGuid, customer.customerProfile.CustomerId, dbManager);
								}
                                LinkkCustomerWithGroup(customer.customerProfile.CustomerId, GroupNames.Registered.ToString());

                                dbManager.Transaction.Commit();

                                FolderModel folderModel = new FolderModel()
                                {
                                    FolderName = customer.CustomerGuid.ToString(),
                                    FolderCode = customer.CustomerGuid.ToString() + "_BOOKIMAGES",
                                    ParentFolderCode = ConfigurationManager.AppSettings["ProjectName"].ToString(),
                                    PhysicallFolderName = "Customer/" + customer.CustomerGuid.ToString() + "/BOOKIMAGES",// + '_' + bookDetail.Item2,
                                    CreatedOn = DateTimeOffset.Now
                                };

                                _IDocumentProcessRepository.CreateFolder(folderModel, dbManager.GetControlDBConnectionString(), ConfigurationManager.AppSettings["ProjectName"].ToString());

                                string folderUrl = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();

                                FolderModel folderModel_= new FolderModel()
                                {
                                    FolderName = customer.CustomerGuid.ToString(),
                                    FolderCode = customer.CustomerGuid.ToString() + "_CUSTOMERDOCUMENT",
                                    ParentFolderCode = ConfigurationManager.AppSettings["ProjectName"].ToString(),
                                    PhysicallFolderName = "Customer/" + customer.CustomerGuid.ToString() + "/CUSTOMERDOCUMENT",// + '_' + bookDetail.Item2,
                                    CreatedOn = DateTimeOffset.Now
                                };
                                _IDocumentProcessRepository.CreateFolder(folderModel_, dbManager.GetControlDBConnectionString(), ConfigurationManager.AppSettings["ProjectName"].ToString());

                            }
                            else
                                dbManager.Transaction.Rollback();
                        }
                    }
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDCUSTOMER", MessageConfig.MessageSettings["ADDCUSTOMER"].ToString(), sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", customer.Email + " " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDCUSTOMER", MessageConfig.MessageSettings["ADDCUSTOMER"].ToString(), ex.StackTrace);
            }
            
        }

        public int LinkkCustomerWithGroup(int customerId,string groupName)
        {
            dbManager.CreateParameters(2);
            dbManager.AddParameters(0, "@CustomerId", customerId);
            dbManager.AddParameters(1, "@GroupName", groupName);
            string query = QueryConfig.CustomerQuerySettings["LinkCustomerWithGroup"].ToString();
            int data = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, query));
            return data;
        }
		public int AddAdminAsCustomer(string email)
		{
			int result = 0;

			try
			{
				using (dbManager = new DBManager())
				{
					dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
					dbManager.Open();

					string query = QueryConfig.CustomerQuerySettings["AddAdminAsCustomer"].ToString();
					dbManager.CreateParameters(1);
					dbManager.AddParameters(0, "@email", email);
					result = ConvertData.ToInt(dbManager.ExecuteScalar(CommandType.Text, query));

					if (result > 0)
					{
					}
				}


					return result;
			}
			catch (SqlException sqlEx)
			{
				LogManager.Log(sqlEx);
				throw new RepositoryException("ADDCUSTOMER", MessageConfig.MessageSettings["ADDCUSTOMER"].ToString(), sqlEx.StackTrace);
			}
			catch (Exception ex)
			{
				LogManager.Log(ex);
				throw new RepositoryException("ADDCUSTOMER", MessageConfig.MessageSettings["ADDCUSTOMER"].ToString(), ex.StackTrace);
			}
		}
		private int LinkCustomerToBook(string bookGuid, int customerId, DBManager dBManager)
		{
			int retValue = 0;
			 
					 
				string query = QueryConfig.CustomerQuerySettings["LinkCustomerToBook"].ToString();
				dbManager.CreateParameters(3);
				dbManager.AddParameters(0, "@CustomerId", customerId);
				dbManager.AddParameters(1, "@BookGUID", bookGuid);
				dbManager.AddParameters(2, "@UserType", "customer");
				retValue = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, query));
							 
				return retValue;
			 
		}

		public int AddCustomerAddress(CustomerAddresses customerAddress)
        {
            int retValue = 0;
            try 
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    if (customerAddress.isDelivery && customerAddress.Id>0)
                    {
                        string customerIsDeliveryCheck = QueryConfig.CustomerQuerySettings["UpdateDeliveryAddress"].ToString();
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@CustomerId", customerAddress.CustomerId);
                        dbManager.ExecuteNonQuery(CommandType.Text, customerIsDeliveryCheck);
                    }

                    if (customerAddress.isInvoice && customerAddress.Id > 0)
                    {
                        string customerIsInvoiceCheck = QueryConfig.CustomerQuerySettings["UpdateInvoiceAddress"].ToString();
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@CustomerId", customerAddress.CustomerId);
                        dbManager.ExecuteNonQuery(CommandType.Text, customerIsInvoiceCheck);
                    }

                    retValue = CustomerAddresses(customerAddress);
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDCUSTOMERADDRESS", MessageConfig.MessageSettings["ADDCUSTOMERADDRESS"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDCUSTOMERADDRESS", MessageConfig.MessageSettings["ADDCUSTOMERADDRESS"].ToString(), ex.StackTrace);
            }
        }

        public int AddCustomerProfile(CustomerProfile customerProfile)
        {
            int retValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    retValue = CustomerProfile(customerProfile);
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDCUSTOMERPROFILE", MessageConfig.MessageSettings["ADDCUSTOMERPROFILE"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDCUSTOMERPROFILE", MessageConfig.MessageSettings["ADDCUSTOMERPROFILE"].ToString(), ex.StackTrace);
            }
        }

        public int AddCustomerfacebook(string code, string state, string scope)
        {
            int retValue = 0;
            string FirstName = string.Empty;
            string LastName = string.Empty;
            string EmailId = string.Empty;
            string SocialMediaTypeId = string.Empty;
            string SocailMediaTypeName = string.Empty;

            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();

                    FacebookUserData socialdata = GetFacebookUserData(code, state);
                    bool isexists = ISExists(socialdata.id);
                    if (isexists)
                    {
                        retValue = GetSocialMediaId(socialdata.id);
                    }
                    else
                    {
                        Customer customer = new Customer();
                        customer.Email = socialdata.email;
                        customer.IsActive = true;
                        customer.Deleted = false;

                        Int32 retvalcustomer = CustomerDetails(customer);
                        if (retvalcustomer > 0)
                        {
                            //Int32 retvalcustomerProfile = CustomerProfile(customer, retvalcustomer);
                            //if (retvalcustomerProfile > 0)
                            //{
                            //    Int32 retvalcustomerSocial = CustomerSocial(customer, retvalcustomer);
                            //    if (retvalcustomerSocial > 0)
                            //    {
                                    dbManager.Transaction.Commit();
                            //        retValue = retvalcustomerSocial;
                            //    }
                            //    else
                            //    {
                            //        dbManager.Transaction.Rollback();
                            //    }
                            //}
                            //else
                            //{
                            //    dbManager.Transaction.Rollback();
                            //}
                        }
                        else
                        {
                            dbManager.Transaction.Rollback();
                        }
                    }
                }

                return retValue;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDCUSTOMER", MessageConfig.MessageSettings["ADDCUSTOMER"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();

                LogManager.Log(ex);
                throw new RepositoryException("ADDCUSTOMER", MessageConfig.MessageSettings["ADDCUSTOMER"].ToString(), ex.StackTrace);
            }
        }

        public int AddCustomerRole(GroupCustomerModel groupCustomer)
        {
            int retValue = 0;
            try 
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    int retvalCustomerRole = CustomerRole(groupCustomer);

                    if (retvalCustomerRole > 0)
                    {
                        retValue = 1;
                    }
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDCUSTOMERROLEFAIL", MessageConfig.MessageSettings["ADDCUSTOMERROLEFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDCUSTOMERROLEFAIL", MessageConfig.MessageSettings["ADDCUSTOMERROLEFAIL"].ToString(), ex.StackTrace);
            }
        }

        private int CustomerSocial(Customer customer, Int32 retvalcustomer)
        {
            string customerSocialInsertTransaction = QueryConfig.CustomerQuerySettings["AddCustomerSocial"];
            dbManager.CreateParameters(9);
            dbManager.AddParameters(0, "@CustomerId", retvalcustomer);
            dbManager.AddParameters(1, "@SocialMediaTypeId", customer.customerSocial.SocialMediaTypeId);
            dbManager.AddParameters(2, "@SocialMediaTypeName", customer.customerSocial.SocialMediaTypeName);
            object returncustomerSocialId = dbManager.ExecuteScalar(CommandType.Text, customerSocialInsertTransaction);
            Int32 retvalcustomerSocial = Int32.Parse(returncustomerSocialId.ToString());
            return retvalcustomerSocial;
        }

        private int CustomerProfile(CustomerProfile customer)
        {
			object objNull = null;
            string customerProfileInsertTransaction = QueryConfig.CustomerQuerySettings["AddCustomerProfile"];
            dbManager.CreateParameters(10);
            dbManager.AddParameters(0, "@CustomerId", customer.CustomerId);
            dbManager.AddParameters(1, "@FirstName", customer.FirstName);
            dbManager.AddParameters(2, "@MiddleName", customer.MiddleName);
            dbManager.AddParameters(3, "@LastName", customer.LastName);
            dbManager.AddParameters(4, "@Email", customer.Email);
            dbManager.AddParameters(5, "@Phone1", customer.Phone1);
            dbManager.AddParameters(6, "@Phone2", customer.Phone2);
            dbManager.AddParameters(7, "@Image", customer.Image);
            dbManager.AddParameters(8, "@DOB", DBNull.Value);
			dbManager.AddParameters(9, "@Country",  customer.Country != null? customer.Country.Id: objNull);
			object returncustomerProfileId = dbManager.ExecuteScalar(CommandType.Text, customerProfileInsertTransaction);
            Int32 retvalcustomerProfile = Int32.Parse(returncustomerProfileId.ToString());
            return retvalcustomerProfile;
        }

        private int CustomerAddresses(CustomerAddresses customer)
        {
            string customerAddressesInsertTransaction = QueryConfig.CustomerQuerySettings["AddCustomerAddresses"];
            dbManager.CreateParameters(14);
            dbManager.AddParameters(0, "@CustomerId", customer.CustomerId);
            dbManager.AddParameters(1, "@Address1", customer.Address1);
            dbManager.AddParameters(2, "@Address2", customer.Address2);
            dbManager.AddParameters(3, "@ZipPostalCode", customer.ZipPostalCode);
            dbManager.AddParameters(4, "@City", customer.City);
            dbManager.AddParameters(5, "@StateId", customer.StateProvinceModel);
            dbManager.AddParameters(6, "@CountryId", customer.countryModel.Id);
            dbManager.AddParameters(7, "@Longitude", (customer.Longitude==null)?0:customer.Longitude);
            dbManager.AddParameters(8, "@Latitude", (customer.Longitude==null)?0:customer.Latitude);
            dbManager.AddParameters(9, "@isDelivery", customer.isDelivery);
            dbManager.AddParameters(10, "@isInvoice", customer.isInvoice);
            dbManager.AddParameters(11, "@CustomerName", customer.CustomerName);
            dbManager.AddParameters(12, "@AddressNote", customer.AddressNote);
            dbManager.AddParameters(13, "@PhoneNumber", customer.PhoneNumber);
            object returncustomerAddressesId = dbManager.ExecuteScalar(CommandType.Text, customerAddressesInsertTransaction);
            Int32 retvalcustomerAddresses = Int32.Parse(returncustomerAddressesId.ToString());
            return retvalcustomerAddresses;
        }

        private int CustomerDetails(Customer customer)
        {
            string customerInsertTransaction = QueryConfig.CustomerQuerySettings["AddCustomer"];
            dbManager.CreateParameters(14);
            dbManager.AddParameters(0, "@CustomerGuid",customer.CustomerGuid.ToString());
            dbManager.AddParameters(1, "@Email", customer.Email);
            dbManager.AddParameters(2, "@Password", customer.Password);
            dbManager.AddParameters(3, "@Salt", customer.Salt);
            dbManager.AddParameters(4, "@IsActive", true);
            dbManager.AddParameters(5, "@Deleted", false);
            dbManager.AddParameters(6, "@LastIpAddress", customer.LastIpAddress);
            dbManager.AddParameters(7, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            dbManager.AddParameters(8, "@IsFirstLogin", true);
            dbManager.AddParameters(9, "@IsGuest", false);
            dbManager.AddParameters(10, "@OTP", customer.OTP);
            dbManager.AddParameters(11, "@IsOTPVerified", customer.IsOTPVerified);
            dbManager.AddParameters(12, "@IsAdmin", false);
            dbManager.AddParameters(13, "@PhoneNumber", customer.PhoneNumber);
            object returncustomerId = dbManager.ExecuteScalar(CommandType.Text, customerInsertTransaction);
            Int32 retvalcustomer = Int32.Parse(returncustomerId.ToString());
            return retvalcustomer;
        }

        private int CustomerRole(GroupCustomerModel groupCustomer)
        {
            string customerRole = QueryConfig.CustomerQuerySettings["AddCustomerRole"].ToString();
            dbManager.CreateParameters(4);
            dbManager.AddParameters(0, "@customerId", Convert.ToInt32(groupCustomer.CustomerId));
            dbManager.AddParameters(1, "@groupId", Convert.ToInt32(groupCustomer.GroupId));
            dbManager.AddParameters(2, "@createdBy", Convert.ToInt32(groupCustomer.CreatedBy));
            dbManager.AddParameters(3, "@createdOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            object returnCustomerRoleId = dbManager.ExecuteNonQuery(CommandType.Text, customerRole);
            int retvalCustomerRole = Convert.ToInt32(returnCustomerRoleId);
            return retvalCustomerRole;
        }
        
        #endregion
        
        #region Facebook Login Integration ############################################################################################
        private FacebookUserData GetFacebookUserData(string code, string state)
        {
            // Exchange the code for an access token
            // Uri targetUri = new Uri("https://graph.facebook.com/v2.8/oauth/access_token?client_id=" + ConfigurationManager.AppSettings["FacebookAppId"] + "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] + "&redirect_uri=" + ConfigurationManager.AppSettings["FbredirectUrl"] + "/SocialLogin/SocialRegistration&code=" + code);
            Uri targetUri = new Uri("https://graph.facebook.com/v2.8/oauth/access_token?client_id=" + ConfigurationManager.AppSettings["FacebookAppId"] + "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] + "code=" + code);
            HttpWebRequest at = (HttpWebRequest)HttpWebRequest.Create(targetUri);

            System.IO.StreamReader str = new System.IO.StreamReader(at.GetResponse().GetResponseStream());
            string token = str.ReadToEnd().ToString().Replace("access_token=", "");

            JavaScriptSerializer sr1 = new JavaScriptSerializer();
            FbToken combined = sr1.Deserialize<FbToken>(token);

            //string[] combined = token.Split(',');
            string accessToken = combined.access_token;
            //Session["AccessToken"] = accessToken;

            // Request the Facebook user information
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=first_name,last_name,gender,email&access_token=" + accessToken);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);

            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();

            // Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;
            FacebookUserData converted = sr.Deserialize<FacebookUserData>(jsondata);

            // Write the user data to a List
            List<FacebookUserData> currentUser = new List<FacebookUserData>();
            currentUser.Add(converted);

            // Return the current Facebook user
            return currentUser.FirstOrDefault();
        }

        public bool ISExists(string Id)
        {
            bool isPresent = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.LovQuerySettings["ISExistsSocialMedia"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        isPresent = true;
                    }
                    this.dbManager.CloseReader();
                    return isPresent;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETSOCIALFACEBOOKFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETSOCIALFACEBOOKFAILED", ex.Message, ex.StackTrace);
            }

        }

        public int GetSocialMediaId(string Id)
        {
            int returnId = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.LovQuerySettings["GetSocialMedia"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        returnId = Convert.ToInt32(dr["Id"]);
                    }
                    this.dbManager.CloseReader();
                }
                return returnId;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETSOCIALFACEBOOKFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETSOCIALFACEBOOKFAILED", ex.Message, ex.StackTrace);
            }

        }
        #endregion ##############################################################################################################

        #region PUT Methods
        /// <summary>Reset password : Change Password</summary>
        /// <param name="code">Code is Entity</param>
        /// <param name="changePassword">Object of ChangePassword Model</param>
        /// <returns>Reset value</returns>
        public bool ResetPassword(Customer customer)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    string query = QueryConfig.CustomerQuerySettings["ResetPassword"].ToString();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@CustomerId", customer.Id);
                    dbManager.AddParameters(1, "@Password", customer.Password);
                    dbManager.AddParameters(2, "@Salt", customer.Salt);
                    dbManager.AddParameters(3, "@ModifiedOn",customer.ModifiedOn);
                    dbManager.AddParameters(4, "@IsFirstLogin", false);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                        status = true;
                    dbManager.Transaction.Commit();
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("INVALIDPASSWORD", MessageConfig.MessageSettings["INVALIDPASSWORD"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("INVALIDPASSWORD", MessageConfig.MessageSettings["INVALIDPASSWORD"].ToString(), ex.StackTrace);
            }
        }

        public int UpdateCustomerAddress(CustomerAddresses customerAddress)
        {
            int retValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    if(customerAddress.isDelivery)
                    {
                        string customerIsDeliveryCheck = QueryConfig.CustomerQuerySettings["UpdateDeliveryAddress"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@CustomerId", customerAddress.CustomerId);
                        dbManager.AddParameters(1, "@Id", Convert.ToInt32(customerAddress.Id));
                        dbManager.ExecuteNonQuery(CommandType.Text, customerIsDeliveryCheck);
                    }

                    if(customerAddress.isInvoice)
                    {
                        string customerIsInvoiceCheck = QueryConfig.CustomerQuerySettings["UpdateInvoiceAddress"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@CustomerId", customerAddress.CustomerId);
                        dbManager.AddParameters(1, "@Id", Convert.ToInt32(customerAddress.Id));
                        dbManager.ExecuteNonQuery(CommandType.Text, customerIsInvoiceCheck);
                    }

                    string customerAddressesUpdateTransaction = QueryConfig.CustomerQuerySettings["UpdateCustomerAddresses"];
                    dbManager.CreateParameters(14);
                    //dbManager.AddParameters(0, "@CustomerId", customerAddress.CustomerId);
                    dbManager.AddParameters(0, "@Address1", customerAddress.Address1);
                    dbManager.AddParameters(1, "@Address2", customerAddress.Address2);
                    dbManager.AddParameters(2, "@ZipPostalCode", customerAddress.ZipPostalCode);
                    dbManager.AddParameters(3, "@City", customerAddress.City);
                    dbManager.AddParameters(4, "@StateId", Convert.ToString(customerAddress.StateProvinceModel));
                    dbManager.AddParameters(5, "@CountryId", Convert.ToInt32(customerAddress.countryModel.Id));
                    dbManager.AddParameters(6, "@Longitude", customerAddress.Longitude == null ? 0 : Convert.ToDecimal(customerAddress.Longitude));
                    dbManager.AddParameters(7, "@Latitude", customerAddress.Latitude == null ? 0 : Convert.ToDecimal(customerAddress.Latitude));
                    dbManager.AddParameters(8, "@isDelivery", customerAddress.isDelivery);
                    dbManager.AddParameters(9, "@isInvoice", customerAddress.isInvoice);
                    dbManager.AddParameters(10, "@Id", Convert.ToInt32(customerAddress.Id));
                    dbManager.AddParameters(11, "@CustomerName", customerAddress.CustomerName);
                    dbManager.AddParameters(12, "@AddressNote", customerAddress.AddressNote);
                    dbManager.AddParameters(13, "@PhoneNumber", customerAddress.PhoneNumber);
                    object returncustomerAddressesId = dbManager.ExecuteNonQuery(CommandType.Text, customerAddressesUpdateTransaction);
                    retValue = Int32.Parse(returncustomerAddressesId.ToString());
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATECUSTOMERADDRESS", MessageConfig.MessageSettings["UPDATECUSTOMERADDRESS"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATECUSTOMERADDRESS", MessageConfig.MessageSettings["UPDATECUSTOMERADDRESS"].ToString(), ex.StackTrace);
            }
        }

        public int UpdateCustomerProfile(CustomerProfile customerProfile)
        {
            int retValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string customerProfileInsertTransaction = QueryConfig.CustomerQuerySettings["UpdateCustomerProfile"];
                    dbManager.CreateParameters(10);
                    dbManager.AddParameters(0, "@CustomerId", customerProfile.CustomerId);
                    dbManager.AddParameters(1, "@FirstName", customerProfile.FirstName);
                    dbManager.AddParameters(2, "@MiddleName", customerProfile.MiddleName);
                    dbManager.AddParameters(3, "@LastName", customerProfile.LastName);
                    dbManager.AddParameters(4, "@Email", customerProfile.Email);
                    dbManager.AddParameters(5, "@Phone1", customerProfile.Phone1);
                    dbManager.AddParameters(6, "@Phone2", customerProfile.Phone2);
                    dbManager.AddParameters(7, "@Image", customerProfile?.Image?.Id>0? customerProfile.Image.Id:(int?)null);
                    dbManager.AddParameters(8, "@DOB", customerProfile.DOB);
                    dbManager.AddParameters(9, "@Country", customerProfile?.Country?.Id);
                    retValue = dbManager.ExecuteNonQuery(CommandType.Text, customerProfileInsertTransaction);
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATECUSTOMERPROFILE", MessageConfig.MessageSettings["UPDATECUSTOMERPROFILE"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATECUSTOMERPROFILE", MessageConfig.MessageSettings["UPDATECUSTOMERPROFILE"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>
        /// Resend OTP
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="OTP">OTP</param>
        /// <returns></returns>
        public bool ResendOTP(string email, string OTP)
        {
            try
            {
                int rowAffacted;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.CustomerQuerySettings["ResendOTP"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@OTP", OTP);
                    dbManager.AddParameters(1, "@Email", email);
                    rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, query);
                }
                if (rowAffacted > 0)
                    return true;
                else
                    return false;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("RESENDOTPFAILED", MessageConfig.MessageSettings["RESENDOTPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("RESENDOTPFAILED", MessageConfig.MessageSettings["RESENDOTPFAILED"].ToString(), ex.StackTrace);
            }
        }
		public bool UpdatePasswordResetCode(string email, string passwordResetCode)
		{
			bool result = false;

			try
			{
				using (dbManager = new DBManager())
				{

					dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
					dbManager.Open();
					string query = QueryConfig.CustomerQuerySettings["UpdatePasswordResetCode"].ToString();
					dbManager.CreateParameters(2);
					dbManager.AddParameters(0, "@passwordResetCode", passwordResetCode);
					dbManager.AddParameters(1, "email", email);

					dbManager.ExecuteNonQuery(CommandType.Text, query);
					result = true;
				}
				return result;
			}
			catch (SqlException sqlEx)
			{
				LogManager.Log(sqlEx);
				throw new RepositoryException("", sqlEx.Message, sqlEx.StackTrace);
			}
			catch (Exception ex)
			{
				LogManager.Log(ex);
				throw new RepositoryException("", ex.Message, ex.StackTrace);
			}
		}


		#endregion

		#region DELETE Method

		public int deleteCustomerRole(int Id) 
        {
            int retValue = 0;
            try 
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string removeCustomerRole = QueryConfig.CustomerQuerySettings["RemoveCustomerRole"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);
                    int deleteReturnValue = dbManager.ExecuteNonQuery(CommandType.Text, removeCustomerRole);

                    if (deleteReturnValue > 0)
                    {
                        retValue = 1;
                    }
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETECUSTOMERROLEFAILED", MessageConfig.MessageSettings["DELETECUSTOMERROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETECUSTOMERROLEFAILED", MessageConfig.MessageSettings["DELETECUSTOMERROLEFAILED"].ToString(), ex.StackTrace);
            }
        }

        public int deleteCustomerAddress(int Id)
        {
            int retValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string removeCustomerAddress = QueryConfig.CustomerQuerySettings["RemoveCustomerAddress"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);
                    int deleteReturnValue = dbManager.ExecuteNonQuery(CommandType.Text, removeCustomerAddress);

                    if (deleteReturnValue > 0)
                    {
                        retValue = 1;
                    }
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETECUSTOMERROLEFAILED", MessageConfig.MessageSettings["DELETECUSTOMERROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETECUSTOMERROLEFAILED", MessageConfig.MessageSettings["DELETECUSTOMERROLEFAILED"].ToString(), ex.StackTrace);
            }
        }

        public int deleteCustomerProfile(int Id)
        {
            int retValue = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string removeCustomerProfile = QueryConfig.CustomerQuerySettings["RemoveCustomerProfile"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);
                    int deleteReturnValue = dbManager.ExecuteNonQuery(CommandType.Text, removeCustomerProfile);

                    if (deleteReturnValue > 0)
                    {
                        retValue = 1;
                    }
                }
                return retValue;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETECUSTOMERROLEFAILED", MessageConfig.MessageSettings["DELETECUSTOMERROLEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETECUSTOMERROLEFAILED", MessageConfig.MessageSettings["DELETECUSTOMERROLEFAILED"].ToString(), ex.StackTrace);
            }

        }

        #endregion

        #region Social Media Login
        public int SocialMediaUser(string socialMediaUserEmail)
        {
            int returnValue = 0;
            int returnCustomerId = 0;
            Customer customerDetails = new Customer();
            try
            {
                
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = dbManager.Connection.BeginTransaction();

                    //string duplicateCheckCustomer = QueryConfig.CustomerQuerySettings["DuplicateCheckCustomer"].ToString();
                    //dbManager.CreateParameters(1);
                    //dbManager.AddParameters(0, "@Email", socialMediaUserEmail);
                    //IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateCheckCustomer);
                    //if (dr.Read())
                    //{
                    //    returnCustomerId = Convert.ToInt32(dr["Id"]);
                    //    dr.Close();
                    //}
                    //else
                    //{
                    //    dr.Close();
                        customerDetails.Email = socialMediaUserEmail;
                        customerDetails.Password = string.Empty;
                        customerDetails.Salt = string.Empty;
                        customerDetails.LastIpAddress = string.Empty;
                        customerDetails.OTP = "123456";
                        customerDetails.IsOTPVerified = true;
                        //customerDetails.IsFirstLogin = false;
                        returnCustomerId = CustomerDetails(customerDetails);
                    //}
                }
                if (returnCustomerId > 0)
                {
                    dbManager.Transaction.Commit();
                }
                else
                {
                    dbManager.Transaction.Rollback();
                }
                return returnCustomerId;
            }
            catch (SqlException sqlEx)
            {
                if(dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("INSERTINGSOCIALLOGINFAIL", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("INSERTINGSOCIALLOGINFAIL", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        /// <summary>
        /// Register void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
