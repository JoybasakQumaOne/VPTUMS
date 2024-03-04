// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationAttributeOptionRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <OrganizationAttributeOptionRepository>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------
    
    public class OrganizationAttributeOptionRepository : IOrganizationAttributeOptionRepository
    {
        #region Variable declaration
        private DBManager dbManager;
        #endregion

        #region Get By Id
        public List<Item> GetOrganizationAttributeOptions(int Id)
        {
            try
            {
                List<Item> options = new List<Item>();
                Item option = new Item();
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string queryOptions = QueryConfig.OrganizationQuerySettings["GetOrgAttributeOptionByAttId"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@AtttributeId", Id);
                    IDataReader drOption = dbManager.ExecuteReader(CommandType.Text, queryOptions);
                    while (drOption.Read())
                    {
                        option = new Item();
                        option.Id = Convert.ToInt32(drOption["Id"]);
                        option.Value = drOption["AttributeListValue"].ToString();
                        options.Add(option);
                    } drOption.Close();
                    return options;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETATTRIBUTEOPTIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETATTRIBUTEOPTIONFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Post
        public bool AddOrganizationAttributeOptions(Item attributeOptions)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (attributeOptions != null)
                    {
                        string attOptionQuery = QueryConfig.OrganizationQuerySettings["AddOrgAttributeOption"].ToString();
                        dbManager.CreateParameters(3);
                        dbManager.AddParameters(0, "@AttributeId", Convert.ToInt32(attributeOptions.Id));
                        dbManager.AddParameters(1, "@AttributeListValue", attributeOptions.Value);
                        dbManager.AddParameters(2, "@AttributeListText", attributeOptions.Value);
                        int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, attOptionQuery);
                        if (rowAffacted > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDATTRIBUTEOPTIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDATTRIBUTEOPTIONFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Put
        public bool UpdateOrganizationAttributeOptions(Item attributeOptions)  
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (attributeOptions != null)
                    {
                        string attOptionQuery = QueryConfig.OrganizationQuerySettings["UpdateOrgAttributeOption"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@Id", Convert.ToInt32(attributeOptions.Id));
                        dbManager.AddParameters(1, "@AttributeListValue", attributeOptions.Value);
                        int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, attOptionQuery);
                        if (rowAffacted > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEATTRIBUTEOPTIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEATTRIBUTEOPTIONFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Delete
        public bool DeleteOrganizationAttributeOptions(int id)  
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (id>0)
                    {
                        string attOptionQuery = QueryConfig.OrganizationQuerySettings["DeleteOrgAttributeOption"].ToString();
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@Id", id);
                        int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, attOptionQuery);
                        if (rowAffacted > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEATTRIBUTEOPTIONFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEATTRIBUTEOPTIONFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion
    }
}