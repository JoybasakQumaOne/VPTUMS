// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>13-12-2017</createdOn>
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
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <PhoneRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class PhoneRepository : IPhoneRepository, IDisposable
    {
        #region Variable Declaration
        public string AgentCode { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Method

        /// <summary>Get Phone Details</summary>
        /// <param name="ControlType">ControlType in Entity</param>
        /// <param name="controlId">Control Id in Entity</param>
        /// <returns>List of Phones</returns>
        public List<PhoneModel> GetAllPhoneList(string ControlType, int ControlId) 
        {
            PhoneModel phone = new PhoneModel();
            List<PhoneModel> phoneDetailList = new List<PhoneModel>();

            try 
            {
                using (dbManager = new DBManager()) 
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string phoneDetailQuery = QueryConfig.PhoneQueryConfiguration["GetAllPhoneDetails"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@ContactType", ControlType);
                    dbManager.AddParameters(1, "@ContactId", ControlId);
                    IDataReader phoneDataReader = dbManager.ExecuteReader(CommandType.Text, phoneDetailQuery);

                    while (phoneDataReader.Read()) 
                    {
                        phoneDetailList.Add(new PhoneModel()
                        {
                            Id = Convert.ToInt32(phoneDataReader["Id"]),
                            PhoneType = Convert.ToInt32(phoneDataReader["PhoneType"]),
                            PhoneNumber = phoneDataReader["PhoneNumber"].ToString(),
                            Extn = (phoneDataReader["Extn"] == DBNull.Value) ? (int?)null : Convert.ToInt32(phoneDataReader["Extn"]),
                            ContactType = phoneDataReader["ContactType"].ToString(),
                            ContactId = Convert.ToInt32(phoneDataReader["ContactId"]),
                            IsPrimary = Convert.ToInt32(phoneDataReader["IsPrimary"]),
                            Status = "",
                        });
                    }
                    return phoneDetailList;
                }    
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("PHONEFOUNDFAIL", MessageConfig.MessageSettings["PHONEFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("PHONEFOUNDFAIL", MessageConfig.MessageSettings["PHONEFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                phone = null;
            }
        }
                
        #endregion

        #region POST Method

        /// <summary>Add Phone Details</summary>
        /// <param name="Phone">List of Phone Numbers in Entity</param>
        /// <param name="ControlType">Related to from Control</param>
        /// <param name="ControlId">Related to from Control Id</param>
        /// <returns>Insert Status</returns>
        public bool AddPhoneDetails(List<PhoneModel> phoneDetails, int organizationId, DBManager dbManager) {
            bool status = false; 
            int phoneId = 0;
            try
            {

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = dbManager.Connection.BeginTransaction();

                    foreach (PhoneModel items in phoneDetails)
                    {
                        string query = QueryConfig.PhoneQueryConfiguration["addPhoneNumber"].ToString();
                        dbManager.CreateParameters(6);
                        dbManager.AddParameters(0, "@PhoneType", items.PhoneType);
                        dbManager.AddParameters(1, "@PhoneNumber", items.PhoneNumber);
                        dbManager.AddParameters(2, "@ContactType", items.ContactType);
                        dbManager.AddParameters(3, "@Extn", items.Extn);
                        dbManager.AddParameters(4, "@ContactId", organizationId);
                        dbManager.AddParameters(5, "@IsPrimary", items.IsPrimary);
                        phoneId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    }

                    if (Convert.ToInt32(phoneId) > 0)
                    {
                        dbManager.Transaction.Commit();
                        status = true;
                    }
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDPHONEFAILED", MessageConfig.MessageSettings["ADDPHONEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDPHONEFAILED", MessageConfig.MessageSettings["ADDPHONEFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Add Phone Detail</summary>
        /// <param name="Phone">Phone Detail in Entity</param>
        /// <param name="ControlId">Related to from Control Id</param>
        /// <returns>Insert Status</returns>
        public bool AddPhoneDetails(PhoneModel phoneDetails, int controlId)
        {
            bool status = false;

            try
            {
                int phoneId = 0;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    
                    string phoneInsertTransaction = QueryConfig.PhoneQueryConfiguration["addPhoneNumber"].ToString();
                    dbManager.CreateParameters(6);
                    dbManager.AddParameters(0, "@PhoneType", phoneDetails.PhoneType);
                    dbManager.AddParameters(1, "@PhoneNumber", phoneDetails.PhoneNumber);
                    dbManager.AddParameters(2, "@ContactType", phoneDetails.ContactType);
                    dbManager.AddParameters(3, "@Extn", phoneDetails.Extn);
                    dbManager.AddParameters(4, "@ContactId", controlId);
                    dbManager.AddParameters(5, "@IsPrimary", phoneDetails.IsPrimary);
                    phoneId = dbManager.ExecuteNonQuery(CommandType.Text, phoneInsertTransaction);                    
                }
                if (Convert.ToInt32(phoneId) > 0)
                {
                    status = true;
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDPHONEFAILED", MessageConfig.MessageSettings["ADDPHONEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();

                LogManager.Log(ex);
                throw new RepositoryException("ADDPHONEFAILED", MessageConfig.MessageSettings["ADDPHONEFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region PUT Method

        /// <summary>Update Phone Details</summary>
        /// <param name="Phone">List of Phone Numbers in Entity</param>
        /// <returns>Update Status</returns>
        public bool UpdatePhoneDetails(PhoneModel phoneDetails)
        {
            bool status = false;
            try
            {
                int phoneId = 0;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string phoneInsertTransaction = QueryConfig.PhoneQueryConfiguration["UpdatePhoneDetails"].ToString();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@PhoneType", phoneDetails.PhoneType);
                    dbManager.AddParameters(1, "@PhoneNumber", phoneDetails.PhoneNumber);
                    dbManager.AddParameters(2, "@phoneId", phoneDetails.Id);
                    dbManager.AddParameters(3, "@Extn", phoneDetails.Extn);
                    dbManager.AddParameters(4, "@IsPrimary", phoneDetails.IsPrimary);
                    phoneId = dbManager.ExecuteNonQuery(CommandType.Text, phoneInsertTransaction);
                    
                }
                if (Convert.ToInt32(phoneId) > 0)
                {
                    status = true;
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEPHONEFAILED", MessageConfig.MessageSettings["UPDATEPHONEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEPHONEFAILED", MessageConfig.MessageSettings["UPDATEPHONEFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>
        /// Update the Phone Detail List
        /// </summary>
        /// <param name="phoneDetails"></param>
        /// <returns></returns>
        public bool UpdatePhoneDetailsList(List<PhoneModel> phoneDetails)
        {
            bool status = false;

            try
            {
                int phoneId = 0;
                bool phoneStatus = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = dbManager.Connection.BeginTransaction();

                    foreach (PhoneModel items in phoneDetails)
                    {
                        if (items.Status == "New")
                        {
                            string phoneInsertTransaction = QueryConfig.PhoneQueryConfiguration["addPhoneNumber"].ToString();
                            dbManager.CreateParameters(6);
                            dbManager.AddParameters(0, "@PhoneType", items.PhoneType);
                            dbManager.AddParameters(1, "@PhoneNumber", items.PhoneNumber);
                            dbManager.AddParameters(2, "@ContactType", items.ContactType);
                            dbManager.AddParameters(3, "@Extn", items.Extn);
                            dbManager.AddParameters(4, "@ContactId", items.ContactId);
                            dbManager.AddParameters(5, "@IsPrimary", items.IsPrimary);
                            phoneId = dbManager.ExecuteNonQuery(CommandType.Text, phoneInsertTransaction);
                            if (Convert.ToInt32(phoneId) > 0)
                            {
                                phoneStatus = true;
                            }
                        }
                        else if (items.Status == "Edit")
                        {
                            string phoneInsertTransaction = QueryConfig.PhoneQueryConfiguration["UpdatePhoneDetails"].ToString();
                            dbManager.CreateParameters(5);
                            dbManager.AddParameters(0, "@PhoneType", items.PhoneType);
                            dbManager.AddParameters(1, "@PhoneNumber", items.PhoneNumber);
                            dbManager.AddParameters(2, "@phoneId", items.Id);
                            dbManager.AddParameters(3, "@Extn", items.Extn);
                            dbManager.AddParameters(4, "@IsPrimary", items.IsPrimary);
                            phoneId = dbManager.ExecuteNonQuery(CommandType.Text, phoneInsertTransaction);
                            if (Convert.ToInt32(phoneId) > 0)
                            {
                                phoneStatus = true;
                            }
                        }
                        else if (items.Status == "Delete")
                        {
                            phoneStatus = RemovePhoneDetails(items.Id);
                        }
                    }
                    if (phoneStatus)
                    {
                        dbManager.Transaction.Commit();
                        status = true;
                    }
                    else
                        dbManager.Transaction.Rollback();
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEPHONEFAILED", MessageConfig.MessageSettings["UPDATEPHONEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEPHONEFAILED", MessageConfig.MessageSettings["UPDATEPHONEFAILED"].ToString(), ex.StackTrace);
            }
        }
        
        #endregion

        #region DELETE Method

        ///<summary>Remove Phone Details</summary>
        ///<param name="id">Id in Entity</param>
        ///<return>Status</return>
        public bool RemovePhoneDetails(int Id) 
        {
            try
            {
                int returnId = 0;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string removePhoneDetails = QueryConfig.PhoneQueryConfiguration["DeletePhoneDetails"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);

                    returnId = dbManager.ExecuteNonQuery(CommandType.Text, removePhoneDetails);
                }
                if (returnId > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }            
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEPHONEFAILED", MessageConfig.MessageSettings["DELETEPHONEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEPHONEFAILED", MessageConfig.MessageSettings["DELETEPHONEFAILED"].ToString(), ex.StackTrace);
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