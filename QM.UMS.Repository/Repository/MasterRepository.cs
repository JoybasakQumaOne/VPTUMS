// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>04-12-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Globalization;
    using System.Data;
    using System.Configuration;
    using System.Data.SqlClient;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using CommonApplicationFramework.ConfigurationHandling;
    using System.Text;
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <MasterRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class MasterRepository : IMasterRepository, IDisposable
    {
        #region Variable Declaration
        public string AgentCode { get; set; }
        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get Country Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Coutry Details</returns>
        public List<CountryModel> GetAllCountries()
        {
            CountryModel countryDetails = new CountryModel();
            List<CountryModel> countryList = new List<CountryModel>();

            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string countryListQuery = QueryConfig.ControlMasterQuerySettings["GetAllCountries"].ToString();

                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, countryListQuery);

                    while (dr.Read())
                    {
                        countryDetails = new CountryModel();
                        countryDetails.Id = Convert.ToInt32(dr["Id"]);
                        countryDetails.Name = dr["Name"].ToString();
                        countryDetails.ThreeLetterIsoCode = dr["ThreeLetterIsoCode"].ToString();
                        countryDetails.TwoLetterIsoCode = dr["TwoLetterIsoCode"].ToString();
                        countryDetails.NumericIsoCode = Convert.ToInt32(dr["NumericIsoCode"]);
                        countryList.Add(countryDetails);
                    }
                    return countryList;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                countryDetails = null;
            }
        }

        /// <summary>Get State Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of State Details</returns>
        public List<StateProvinceModel> GetAllStates(int CountryNumCode)
        {
            StateProvinceModel stateDetails = new StateProvinceModel();
            List<StateProvinceModel> stateList = new List<StateProvinceModel>();

            try {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string StateListQuery = QueryConfig.ControlMasterQuerySettings["GetAllStateDetails"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@CountryCode", CountryNumCode);

                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, StateListQuery);
                    while (dr.Read())
                    {
                        stateDetails = new StateProvinceModel();
                        stateDetails.Id = Convert.ToInt32(dr["Id"]);
                        stateDetails.Name = dr["Name"].ToString();
                        stateDetails.CountryId = Convert.ToInt32(dr["CountryId"]);
                        stateDetails.Abbreviation = dr["Abbreviation"].ToString();
                        stateDetails.Published = Convert.ToInt32(dr["Published"]);
                        stateList.Add(stateDetails);
                    }
                    return stateList;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                stateDetails = null;
            }
        }
        
        /// <summary>
        /// Get the Instance Details
        /// </summary>
        /// <returns>List of instance and Details of it</returns>
        public List<DBViewModel> GetInstanceDetails()
        {
            try
            {
                List<DBViewModel> dBViewModel = new List<DBViewModel>();
                using(dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string queryGetInstanceList = QueryConfig.ControlMasterQuerySettings["GetInstanceList"].ToString();
                    IDataReader idr = dbManager.ExecuteReader(CommandType.Text, queryGetInstanceList);

                    while (idr.Read())
                    {
                        dBViewModel.Add(new DBViewModel() {
                            Id = Convert.ToInt32(idr["Id"]),
                            CompanyName = Convert.ToString(idr["Name"]),
                            Code = Convert.ToString(idr["Code"])
                        });
                    }
                    idr.Close();
                }
                return dBViewModel;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("INSTANCELISTFOUNDFAIL", MessageConfig.MessageSettings["INSTANCELISTFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("INSTANCELISTFOUNDFAIL", MessageConfig.MessageSettings["INSTANCELISTFOUNDFAIL"].ToString(), ex.StackTrace);
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