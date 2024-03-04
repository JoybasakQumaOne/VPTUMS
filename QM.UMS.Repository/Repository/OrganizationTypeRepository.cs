// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationTypeRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>12-12-2017</createdOn>
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
    ///   Class:        <OrganizationTypeRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationTypeRepository : IOrganizationTypeRepository, IDisposable
    {
        #region Variable Declaration

        public string AgentCode { get; set; }

        private DBManager dbManager;
        #endregion
        
        #region GET Methods

        /// <summary>Get Organization Type Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Type Details</returns>
        public List<OrganizationType> GetAllOrganizationTypes()
        {
            try
            {
                OrganizationType orgType = new OrganizationType();
                List<OrganizationType> OrgTypeList = new List<OrganizationType>();
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string OrgTypeGetQuery = QueryConfig.OrganizationQuerySettings["GetAllOrgTypes"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, OrgTypeGetQuery);

                    while (dr.Read())
                    {
                        OrgTypeList.Add(new OrganizationType()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Code = Convert.ToString(dr["Code"]),
                            Name = Convert.ToString(dr["Name"]),
                        });
                    }
                    return OrgTypeList;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONMAPFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONMAPFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONMAPFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONMAPFOUNDFAIL"].ToString(), ex.StackTrace);
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