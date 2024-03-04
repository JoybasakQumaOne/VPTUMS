namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class AttributeMasterRepository : IAttributeMasterRepository
    {
        #region Variable declaration
        private DBManager dbManager;
        #endregion

        #region Get
        public List<AttributeMaster> Get()  
        {
            try
            {
                List<AttributeMaster> attributes = new List<AttributeMaster>();
                AttributeMaster attribute = new AttributeMaster();
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.EcommAttributeMasterLogQuerySettings[""].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        attribute = new AttributeMaster();
                        attribute.Section = new AttributeSection();
                        attribute.AttributeControl = new Item();
                        attribute.Id = Convert.ToInt32(dr["Id"]);
                        attribute.Name = Convert.ToString(dr["Name"]);
                        attribute.Label = Convert.ToString(dr["Label"]);
                        attribute.Description = Convert.ToString(dr["Description"]);
                        attribute.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        attribute.IsRequired = Convert.ToBoolean(dr["IsRequired"]);
                        attribute.IsRepeated = Convert.ToBoolean(dr["IsRepeated"]);
                        attribute.Section.Id = Convert.ToInt32(dr["AttributeSectionId"]);
                        attribute.Section.Name = Convert.ToString(dr["AttributeSection"]);
                        attribute.Section.Label = Convert.ToString(dr["DisplayText"]);
                        attribute.DispalyOrder = Convert.ToInt32(dr["DisplayOrder"]);
                        attribute.AttributeControl.Id = Convert.ToInt32(dr["AttributeControlTypeId"]);
                        attribute.AttributeControl.Value = Convert.ToString(dr["AttributeControlType"]);
                        //attribute.AttributeOptionId = (dr["AtrributeOptionMasterId"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["AtrributeOptionMasterId"]);// Convert.ToInt32(dr["AtrributeOptionMasterId"]);
                        attribute.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        attribute.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        attribute.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        attribute.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                        attributes.Add(attribute);
                    }
                }
                return attributes;
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

        #region Get By Id
        public AttributeMaster Get(int id)
        {
            try
            {
                AttributeMaster attribute = new AttributeMaster();
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.EcommAttributeMasterLogQuerySettings["GetAttributeMasterById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        attribute = new AttributeMaster();
                        attribute.Section = new AttributeSection();
                        attribute.AttributeControl = new Item();
                        attribute.Id = Convert.ToInt32(dr["Id"]);
                        attribute.Name = Convert.ToString(dr["Name"]);
                        attribute.Label = Convert.ToString(dr["Label"]);
                        attribute.Description = Convert.ToString(dr["Description"]);
                        attribute.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        attribute.IsRequired = Convert.ToBoolean(dr["IsRequired"]);
                        attribute.IsRepeated = Convert.ToBoolean(dr["IsRepeated"]);
                        attribute.Section.Id = Convert.ToInt32(dr["AttributeSectionId"]);
                        attribute.Section.Name = Convert.ToString(dr["AttributeSection"]);
                        attribute.Section.Label = Convert.ToString(dr["DisplayText"]);
                        attribute.DispalyOrder = Convert.ToInt32(dr["DisplayOrder"]);
                        attribute.AttributeControl.Id = Convert.ToInt32(dr["AttributeControlTypeId"]);
                        attribute.AttributeControl.Value = Convert.ToString(dr["AttributeControlType"]);
                        //attribute.AttributeOptionId = (dr["AtrributeOptionMasterId"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["AtrributeOptionMasterId"]);// Convert.ToInt32(dr["AtrributeOptionMasterId"]);
                        attribute.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        attribute.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        attribute.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        attribute.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                    }
                }
                return attribute;
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

        #region Post
        public bool AddAttribute(AttributeMaster attribute)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string duplicateChkquery = QueryConfig.EcommAttributeMasterLogQuerySettings["DuplicateAttributeCheck"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Name", attribute.Name);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateChkquery);
                    if (dr.Read())
                    {
                        dr.Close();
                        throw new DuplicateException();
                    }
                    else
                    {
                        dr.Close();
                        string addAttQuery = QueryConfig.EcommAttributeMasterLogQuerySettings["AddAttributeMaster"].ToString();
                        dbManager.CreateParameters(12);
                        dbManager.AddParameters(0, "@Name", attribute.Name);
                        dbManager.AddParameters(1, "@Label", attribute.Label);
                        dbManager.AddParameters(2, "@Description", attribute.Description);
                        dbManager.AddParameters(3, "@IsActive", attribute.IsActive);
                        dbManager.AddParameters(4, "@IsRequired", attribute.IsRequired);
                        dbManager.AddParameters(5, "@AttributeSectionId", attribute.Section.Id);
                        dbManager.AddParameters(6, "@AttributeControlTypeId", attribute.AttributeControl.Id);
                        dbManager.AddParameters(7, "@IsRepeated", attribute.IsRepeated);
                        dbManager.AddParameters(8, "@ObjectType", User.User.ToString());
                        dbManager.AddParameters(9, "@DisplayOrder", attribute.DispalyOrder);
                        dbManager.AddParameters(10, "@CreatedOn", attribute.CreatedOn);
                        dbManager.AddParameters(11, "@CreatedBy", attribute.CreatedBy);
                        int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttQuery);
                        if (rowAffacted > 0)
                            return true;
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDATTRIBUTEMASTERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", "Attribute " + attribute.Name + " " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDATTRIBUTEMASTERFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Put
        public bool UpdateAttribute(AttributeMaster attribute)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string addAttQuery = QueryConfig.EcommAttributeMasterLogQuerySettings["UpdateAttributeMaster"].ToString();
                    dbManager.CreateParameters(9);
                    dbManager.AddParameters(0, "@Name", attribute.Name);
                    dbManager.AddParameters(1, "@Label", attribute.Label);
                    dbManager.AddParameters(2, "@Description", attribute.Description);
                    dbManager.AddParameters(3, "@AttributeSectionId", attribute.Section.Id);
                    dbManager.AddParameters(4, "@AttributeControlTypeId", attribute.AttributeControl.Id);
                    dbManager.AddParameters(5, "@IsRepeated", attribute.IsRepeated);
                    dbManager.AddParameters(6, "@ModifiedOn", attribute.CreatedOn);
                    dbManager.AddParameters(7, "@ModifiedBy", attribute.CreatedBy);
                    dbManager.AddParameters(8, "@Id", attribute.Id);
                    int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttQuery);
                    if (rowAffacted > 0)
                        return true;
                    return false;

                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEATTRIBUTEMASTERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEATTRIBUTEMASTERFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Delete
        public bool DeleteAttribute(int id)  
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string deleteAttQuery = QueryConfig.EcommAttributeMasterLogQuerySettings["InActiveAttributeMaster"].ToString();  
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", id);
                    int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, deleteAttQuery);
                    if (rowAffacted > 0)
                        return true;
                    return false;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEATTRIBUTEMASTERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEATTRIBUTEMASTERFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

    }
}
