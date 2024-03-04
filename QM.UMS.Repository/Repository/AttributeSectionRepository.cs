// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeSectionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>06-12-2017</createdOn>
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
    ///   Class:        <AttributeSectionRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class AttributeSectionRepository : IAttributeSectionRepository, IDisposable
    {
        #region Variable Declaration

        public string AgentCode { get; set; }

        private DBManager dbManager;
        #endregion

        #region GET Methods

        /// <summary>Get Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Details</returns>
        public List<AttributeSection> GetAllAttributeSections()
        {
            AttributeSection attributeSection = new AttributeSection();
            List<AttributeSection> attributeSectionList = new List<AttributeSection>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string attributeGetQuery = QueryConfig.ControlMasterQuerySettings["GetAllAttributes"].ToString();

                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, attributeGetQuery);
                    
                    while (dr.Read())
                    {
                        attributeSection = new AttributeSection();
                        attributeSection.Id = Convert.ToInt32(dr["Id"]);
                        attributeSection.Name = dr["AttributeSection"].ToString();
                        attributeSection.Label = dr["DisplayText"].ToString();
                        attributeSection.DisplayOrder = (dr["DisplayOrder"] == DBNull.Value)?(int?)null : Convert.ToInt32(dr["DisplayOrder"]);
                        attributeSection.IsRepeated = Convert.ToBoolean(dr["IsRepeated"]);
                        attributeSectionList.Add(attributeSection);
                    }
                    return attributeSectionList;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ATTRIBUTESECTIONFOUNDFAIL", MessageConfig.MessageSettings["ATTRIBUTESECTIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ATTRIBUTESECTIONFOUNDFAIL", MessageConfig.MessageSettings["ATTRIBUTESECTIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                attributeSection = null;
            }
        }

        /// <summary>Get Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Details</returns>
        public AttributeSection GetAttributeSectionsById(int Id)
        {
            AttributeSection attributeSection = new AttributeSection();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string attributeGetIdQuery = QueryConfig.ControlMasterQuerySettings["GetAttributesById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);

                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, attributeGetIdQuery);

                    if (dr.Read())
                    {
                        attributeSection = new AttributeSection();
                        attributeSection.Id = Convert.ToInt32(dr["Id"]);
                        attributeSection.Name = dr["AttributeSection"].ToString();
                        attributeSection.Label = dr["DisplayText"].ToString();
                        attributeSection.DisplayOrder = (dr["DisplayOrder"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["DisplayOrder"]);
                        attributeSection.IsRepeated = Convert.ToBoolean(dr["IsRepeated"]);
                    }
                    return attributeSection;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ATTRIBUTESECTIONFOUNDFAIL", MessageConfig.MessageSettings["ATTRIBUTESECTIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ATTRIBUTESECTIONFOUNDFAIL", MessageConfig.MessageSettings["ATTRIBUTESECTIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Attribute Section</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Organization">Object of Attribute Section Model</param>
        /// <returns>Insert Status</returns>
        public bool AddAttributeSection(AttributeSection attributeSection)
        {
            bool retval = false;
            int attributeSectionReturn = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string attributeSectionQuery = QueryConfig.ControlMasterQuerySettings["AddAttributeSectionDetails"].ToString();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@AttributeSection", attributeSection.Name);
                    dbManager.AddParameters(1, "@DisplayText", attributeSection.Label);
                    dbManager.AddParameters(2, "@DisplayOrder", attributeSection.DisplayOrder);
                    dbManager.AddParameters(3, "@IsRepeated", attributeSection.IsRepeated);
                    attributeSectionReturn = dbManager.ExecuteNonQuery(CommandType.Text, attributeSectionQuery);
                    dbManager.Close();
                }
                if (Convert.ToInt32(attributeSectionReturn) > 0)
                {
                    retval = true;
                }
                return retval;

            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDATTRIBUTESECTIONFAILED", MessageConfig.MessageSettings["ADDATTRIBUTESECTIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {                
                LogManager.Log(ex);
                throw new RepositoryException("ADDATTRIBUTESECTIONFAILED", MessageConfig.MessageSettings["ADDATTRIBUTESECTIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Attribute Section Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Attribute Section Model</param>
        /// <returns>Update Attribute Section Details</returns>
        public bool UpdateAttributeSection(AttributeSection attributeSection)
        {
            try
            {
                bool status = false;
                int returnId = 0;

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string updateAttributeSectionQuery = QueryConfig.ControlMasterQuerySettings["UpdateAttributeSectionDetails"].ToString();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@AttributeSection", attributeSection.Name);
                    dbManager.AddParameters(1, "@DisplayText", attributeSection.Label);
                    dbManager.AddParameters(2, "@DisplayOrder", attributeSection.DisplayOrder);
                    dbManager.AddParameters(3, "@Id", attributeSection.Id);
                    dbManager.AddParameters(4, "@IsRepeated", attributeSection.IsRepeated);
                    returnId = dbManager.ExecuteNonQuery(CommandType.Text, updateAttributeSectionQuery);
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
                throw new RepositoryException("UPDATEATTRIBUTESECTIONFAILED", MessageConfig.MessageSettings["UPDATEATTRIBUTESECTIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEATTRIBUTESECTIONFAILED", MessageConfig.MessageSettings["UPDATEATTRIBUTESECTIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        ///<summary>Remove Organization from Ecom and Control</summary>
        ///<param name="Code">Code in Entity</param>
        ///<return>Status</return>
        public bool RemoveAttributeSection(int Id)
        {
            try
            {
                int returnId = 0;
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string getAttributeSectionMap = QueryConfig.ControlMasterQuerySettings["GetAttributeMapping"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, getAttributeSectionMap);

                    if(dr.Read()){
                        dr.Close();
                        throw new DuplicateException();
                    } else{
                        dr.Close();
                        string removeAttributeSectionQuery = QueryConfig.ControlMasterQuerySettings["RemoveAttributeSection"].ToString();
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@Id", Id);
                        returnId = dbManager.ExecuteNonQuery(CommandType.Text, removeAttributeSectionQuery);
                    }

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
                throw new RepositoryException("DELETEATTRIBUTESECTIONFAILED", MessageConfig.MessageSettings["DELETEATTRIBUTESECTIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("MAPPING", " " + MessageConfig.MessageSettings["MAPPED"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEATTRIBUTESECTIONFAILED", MessageConfig.MessageSettings["DELETEATTRIBUTESECTIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region GetAttibuteSectionByGroupId
        public List<AttributeSection> GetUserAttibuteByUserId(int userId)    
        {
            try
            {
                List<int> sec = new List<int>();

                AttributeSection attributeSection = new AttributeSection();
                List<AttributeSection> attributeSections = new List<AttributeSection>();
                UserAttribute userAttribute = new UserAttribute();
                List<UserAttribute> userAttributes = new List<UserAttribute>();
                List<AttributeMaster> attributeMasters = new List<AttributeMaster>();
                List<RecordSet> recordSets = new List<RecordSet>();
                RecordSet recordSet = new RecordSet();
                List<UserAttributeValue> userAttributeValues = new List<UserAttributeValue>();
                UserAttributeValue userAttributeValue = new UserAttributeValue();
                //string groupId = string.Join(",", groupIds);
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.EcommAttributeMasterLogQuerySettings["GetUserAttibuteByUserId"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@User_Id", userId);  
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    attributeSection.Atrributes = new List<UserAttribute>();
                    while (dr.Read())
                    {
                        userAttribute = new UserAttribute();
                        userAttribute.Attribute = new AttributeMaster();
                        userAttribute.Attribute.AttributeControl = new Item();
                        attributeSection = new AttributeSection();
                        if (!attributeSections.Exists(x => x.Id == Convert.ToInt32(dr["AttributeSectionId"])))
                        {
                            attributeSection.Id = Convert.ToInt32(dr["AttributeSectionId"]);
                            attributeSection.Name = Convert.ToString(dr["AttributeSection"]);
                            attributeSection.Label = Convert.ToString(dr["DisplayText"]);
                            attributeSection.IsRepeated = Convert.ToBoolean(dr["IsRepeated"]);
                            attributeSections.Add(attributeSection);
                            attributeSection.Atrributes = new List<UserAttribute>();
                            userAttribute.Attribute = new AttributeMaster();
                            userAttribute.Attribute.AttributeControl = new Item();
                        }
                        userAttribute.Attribute.AttributeControl.Id = Convert.ToInt32(dr["ControlTypeId"]);
                        userAttribute.Attribute.AttributeControl.Value = Convert.ToString(dr["AttributeControlType"]);
                        userAttribute.Attribute.Id = Convert.ToInt32(dr["AttributeId"]);
                        userAttribute.Attribute.Name = Convert.ToString(dr["Name"]);
                        userAttribute.Attribute.Label = Convert.ToString(dr["Label"]);
                        userAttribute.Attribute.Description = Convert.ToString(dr["Description"]);
                        userAttribute.Attribute.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        userAttribute.Attribute.IsRequired = Convert.ToBoolean(dr["IsRequired"]);
                        userAttribute.Attribute.IsRepeated = Convert.ToBoolean(dr["IsRepeated"]);
                        userAttribute.Attribute.DispalyOrder = Convert.ToInt32(dr["DisplayOrder"]);
                        userAttribute.Attribute.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        userAttribute.Attribute.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        userAttribute.Attribute.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        userAttribute.Attribute.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                        if (attributeSections.Exists(x => x.Id == Convert.ToInt32(dr["AttributeSectionId"])))
                        {
                            attributeSection = attributeSections.Where(x => x.Id == Convert.ToInt32(dr["AttributeSectionId"])).Select(x => x).FirstOrDefault();
                            attributeSection.Atrributes.Add(userAttribute);
                        }
                        else
                        {
                            attributeSection.Atrributes.Add(userAttribute);
                            attributeSections.Add(attributeSection);
                        }
                    }
                    dr.Close();
                    
                    
                    foreach (AttributeSection attributeSec in attributeSections)
                    {
                        string queryAttVal = QueryConfig.EcommAttributeMasterLogQuerySettings["GetUserAttibuteValue"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@User_Id", userId);
                        dbManager.AddParameters(1, "@SectionId", attributeSec.Id);
                        IDataReader drAtt = dbManager.ExecuteReader(CommandType.Text, queryAttVal);
                        recordSet = new RecordSet();
                        while (drAtt.Read())
                        {
                            userAttributeValue = new UserAttributeValue();
                            
                            if (!recordSets.Any(x => x.RowCollectionId == Convert.ToInt32(drAtt["Collection"]) && x.SectionId == attributeSec.Id))
                            {
                                recordSet = new RecordSet();
                                recordSet.UserAttributeValues = new List<UserAttributeValue>();
                                recordSet.RowCollectionId = Convert.ToInt32(drAtt["Collection"]);
                                recordSet.SectionId = attributeSec.Id;
                                recordSets.Add(recordSet);
                            }
                            userAttributeValue.AttributeValueId = Convert.ToInt32(drAtt["Id"]);
                            userAttributeValue.AttributeId = Convert.ToInt32(drAtt["UserAttributeId"]);
                            userAttributeValue.AttributeOptionId = Convert.ToInt32(drAtt["AttributeOptionId"]);
                            userAttributeValue.AttributeValue = Convert.ToString(drAtt["Value"]);
                            userAttributeValue.IsSelected = true;
                            userAttributeValues.Add(userAttributeValue);
                            if (recordSets.Exists(x => x.RowCollectionId == Convert.ToInt32(drAtt["Collection"])))
                            {
                                recordSet = recordSets.Where(x => x.RowCollectionId == Convert.ToInt32(drAtt["Collection"]) && x.SectionId == attributeSec.Id).Select(x => x).FirstOrDefault();
                                recordSet.UserAttributeValues.Add(userAttributeValue);
                                userAttributeValues = new List<UserAttributeValue>();
                            }
                            else
                            {
                                recordSet.UserAttributeValues = userAttributeValues;
                                userAttributeValues = new List<UserAttributeValue>();
                                recordSets.Add(recordSet);
                            }
                        }
                        
                        drAtt.Close();
                    }
                }
                //List<RecordSet> RecSet = new List<RecordSet>();
                //RecSet = recordSets;
                foreach (AttributeSection attributeSec in attributeSections)
                {
                    attributeSec.AttributeValue = recordSets.Where(x => x.SectionId == attributeSec.Id).Select(x => x).ToList();
                    if (attributeSec.IsRepeated == false)
                    {
                        attributeSec.AttributeValue.Where(x => x.SectionId == attributeSec.Id).FirstOrDefault().RowCollectionId = 0;
                    }
                }
                //attributeSection.AttributeValue=recordSets;
                return attributeSections;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETATTRIBUTEMASTERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETATTRIBUTEMASTERFAILED", ex.Message, ex.StackTrace);
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