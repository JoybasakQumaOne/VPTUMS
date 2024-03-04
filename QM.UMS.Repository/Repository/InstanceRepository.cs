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

namespace QM.UMS.Repository.Repository
{
    public class InstanceRepository : IInstanceRepository,IDisposable  
    {
        #region Variable Declaration
        private DBManager dbManager;
        #endregion

        #region Get
        /// <summary>
        /// Get Instance details
        /// </summary>
        /// <returns>List of Instance</returns>
        public List<Item> GetInstance()  
        {
            Item instance = new Item();
            List<Item> instances = new List<Item>();  
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string countryListQuery = QueryConfig.ControlMasterQuerySettings["GetInstanceDetails"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, countryListQuery);
                    while (dr.Read())
                    {
                        instance = new Item();
                        instance.Id = Convert.ToInt32(dr["Id"]);
                        instance.Value = dr["Name"].ToString();
                        instances.Add(instance);  
                    }
                    return instances;    
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("INSTANCEFOUNDFAILED", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("INSTANCEFOUNDFAILED", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                instances = null;
            }
        }
        #endregion

        #region Link-Group
        #region MapProdcutAttribute
        public bool MapUserInstance(int userId, int instanceId)  
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (userId > 0 || instanceId > 0)
                    {
                        
                            string duplicateChkquery = QueryConfig.ControlMasterQuerySettings["DuplicateUserInstanceMap"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@UserId", userId);
                            dbManager.AddParameters(1, "@InstanceId", instanceId);
                            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateChkquery);
                            if (dr.Read())
                            {
                                dr.Close();
                                throw new DuplicateException();
                            }
                            else
                            {
                                dr.Close();
                                string instanceMapQuery = QueryConfig.ControlMasterQuerySettings["UserInstanceMapping"].ToString();
                                dbManager.CreateParameters(2);
                                dbManager.AddParameters(0, "@UserId", userId);
                                dbManager.AddParameters(1, "@InstanceId", instanceId);
                                int userInstanceMapingId = dbManager.ExecuteNonQuery(CommandType.Text, instanceMapQuery);
                                if (userInstanceMapingId > 0)
                                    return true;
                                return false;
                            }
                        
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
                throw new RepositoryException("MAPPINGPUSERINSTANCEFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", "Mapping " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("MAPPINGPUSERINSTANCEFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion
        #endregion

        #region Delink-Group
        public bool UnMapUserInstance(int userId, int instanceId)  
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (userId > 0 || instanceId > 0)
                    {

                        string productMapQuery = QueryConfig.ControlMasterQuerySettings["UserInstanceUnMapping"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@UserId", userId);
                        dbManager.AddParameters(1, "@InstanceId", instanceId);
                        int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, productMapQuery);
                        if (rowAffacted > 0)
                            return true;
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
                throw new RepositoryException("UNMAPPINGPUSERINSTANCEFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UNMAPPINGPUSERINSTANCEFAILED", ex.Message, ex.StackTrace);
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
