// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
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
    ///   Class:        <OrganizationAttributeRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationAttributeRepository : IOrganizationAttributeRepository, IDisposable
    {
        #region Variable Declaration

        public string AgentCode { get; set; }

        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get Organization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Attribute Details</returns>
        public List<OrganizationAttribute> GetAllOrganizationAttribute()
        {
            OrganizationAttribute organizationAttribute = new OrganizationAttribute();
            List<OrganizationAttribute> organizationAttributeList = new List<OrganizationAttribute>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string organizationAttributeQuery = QueryConfig.OrganizationQuerySettings["GetAllOrganizationAttribute"].ToString();

                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, organizationAttributeQuery);
                    while (dr.Read())
                    {
                        organizationAttribute = new OrganizationAttribute();
                        organizationAttribute.AttributeControlTypeId = new Item();
                        organizationAttribute.Id = Convert.ToInt32(dr["Id"]);
                        organizationAttribute.Name = Convert.ToString(dr["Name"]);
                        organizationAttribute.Description = Convert.ToString(dr["Description"]);
                        organizationAttribute.Label = Convert.ToString(dr["Label"]);
                        organizationAttribute.IsActive = Convert.ToInt32(dr["IsActive"]);
                        organizationAttribute.IsRepeated = Convert.ToInt32(dr["IsRepeated"]);
                        organizationAttribute.IsRequired = Convert.ToInt32(dr["IsRequired"]);
                        organizationAttribute.AttributeControlTypeId.Id = Convert.ToInt32(dr["AttributeControlTypeId"]);
                        organizationAttribute.AttributeControlTypeId.Value = Convert.ToString(dr["AttributeControlType"]);
                        organizationAttribute.ObjectType = Convert.ToString(dr["ObjectType"]);
                        organizationAttribute.DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]);
                        organizationAttribute.AttributeCode = Convert.ToString(dr["AttributeCode"]);
                        organizationAttribute.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        organizationAttribute.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        organizationAttribute.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        organizationAttribute.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                        organizationAttributeList.Add(organizationAttribute);
                    }
                    return organizationAttributeList.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONATTRIBUTEFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONATTRIBUTEFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONATTRIBUTEFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONATTRIBUTEFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                organizationAttribute = null;
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Organization Attribute</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Organization">Object of Organization Attribute Model</param>
        /// <returns>Insert Status</returns>
        public bool AddOrganizationAttributes(OrganizationAttribute organizationAttribute)
        {
            bool retval = false;
            int OrgAttributeStatus = 0;
            try
            {
                //var guidValue = Guid.NewGuid();
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string addOrgAttributesQuery = QueryConfig.OrganizationQuerySettings["AddOrganizationAttributes"].ToString();
                    dbManager.CreateParameters(12);
                    dbManager.AddParameters(0, "@Name", organizationAttribute.Name);
                    dbManager.AddParameters(1, "@Description", organizationAttribute.Description);
                    dbManager.AddParameters(2, "@Label", organizationAttribute.Label);
                    dbManager.AddParameters(3, "@IsActive", Convert.ToInt32(organizationAttribute.IsActive));
                    dbManager.AddParameters(4, "@IsRepeated", Convert.ToInt32(organizationAttribute.IsRepeated));
                    dbManager.AddParameters(5, "@IsRequired", Convert.ToInt32(organizationAttribute.IsRequired));
                    dbManager.AddParameters(6, "@ObjectType", Convert.ToString(organizationAttribute.ObjectType));
                    dbManager.AddParameters(7, "@DisplayOrder", Convert.ToInt32(organizationAttribute.DisplayOrder));
                    dbManager.AddParameters(8, "@AttributeControlTypeId", Convert.ToInt32(organizationAttribute.AttributeControlTypeId.Id));
                    dbManager.AddParameters(9, "@AttributeCode", organizationAttribute.AttributeCode);
                    dbManager.AddParameters(10, "@CreatedBy", Convert.ToInt32(organizationAttribute.CreatedBy));
                    dbManager.AddParameters(11, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                    OrgAttributeStatus = dbManager.ExecuteNonQuery(CommandType.Text, addOrgAttributesQuery);
                    dbManager.Close();
                }
                if (Convert.ToInt32(OrgAttributeStatus) > 0)
                {
                    retval = true;
                }
                return retval;

            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDORGANIZATIONATTRIBUTEFAILED", MessageConfig.MessageSettings["ADDORGANIZATIONATTRIBUTEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {                
                LogManager.Log(ex);
                throw new RepositoryException("ADDORGANIZATIONATTRIBUTEFAILED", MessageConfig.MessageSettings["ADDORGANIZATIONATTRIBUTEFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Organization Attribute Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Attribute Model</param>
        /// <returns>Update Organization Attribute Details</returns>
        public bool UpdateOrganizationAttribute(OrganizationAttribute organizationAttribute)
        {
            try
            {
                bool status = false;
                int returnId = 0;

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string updateOrgAttributesQuery = QueryConfig.OrganizationQuerySettings["UpdateOrganizationAttributes"].ToString();
                    dbManager.CreateParameters(13);
                    dbManager.AddParameters(0, "@Name", organizationAttribute.Name);
                    dbManager.AddParameters(1, "@Description", organizationAttribute.Description);
                    dbManager.AddParameters(2, "@Label", organizationAttribute.Label);
                    dbManager.AddParameters(3, "@IsActive", Convert.ToInt32(organizationAttribute.IsActive));
                    dbManager.AddParameters(4, "@IsRepeated", Convert.ToInt32(organizationAttribute.IsRepeated));
                    dbManager.AddParameters(5, "@IsRequired", Convert.ToInt32(organizationAttribute.IsRequired));
                    dbManager.AddParameters(6, "@ObjectType", Convert.ToString(organizationAttribute.ObjectType));
                    dbManager.AddParameters(7, "@DisplayOrder", Convert.ToInt32(organizationAttribute.DisplayOrder));
                    dbManager.AddParameters(8, "@AttributeControlTypeId", Convert.ToInt32(organizationAttribute.AttributeControlTypeId.Id));
                    dbManager.AddParameters(9, "@AttributeCode", organizationAttribute.AttributeCode);
                    dbManager.AddParameters(10, "@ModifiedBy", Convert.ToInt32(organizationAttribute.ModifiedBy));
                    dbManager.AddParameters(11, "@ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                    dbManager.AddParameters(12, "@Id", organizationAttribute.Id);
                    returnId = dbManager.ExecuteNonQuery(CommandType.Text, updateOrgAttributesQuery);
                    dbManager.Close();
                }

                if (returnId > 0)
                {
                    status = true;
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEORGANIZATIONATTRIBUTEFAILED", MessageConfig.MessageSettings["UPDATEORGANIZATIONATTRIBUTEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEORGANIZATIONATTRIBUTEFAILED", MessageConfig.MessageSettings["UPDATEORGANIZATIONATTRIBUTEFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        ///<summary>Remove Organization from Ecom and Control</summary>
        ///<param name="Code">Code in Entity</param>
        ///<return>Status</return>
        public bool RemoveOrganizationAttributes(int id)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    //TODO : Check for mapping presence and stop deleting.
                    string removeOrgAttributeQuery = QueryConfig.OrganizationQuerySettings["RemoveOrganizationAttribute"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", id);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, removeOrgAttributeQuery);

                    if (returnId > 0)
                    {
                        status = true;
                    }
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEORGANIZATIONATTRIBUTEFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONATTRIBUTEFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEORGANIZATIONATTRIBUTEFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONATTRIBUTEFAILED"].ToString(), ex.StackTrace);
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