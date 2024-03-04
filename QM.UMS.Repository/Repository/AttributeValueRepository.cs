namespace QM.UMS.Repository.Repository
{
    #region Namespace
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

    public class AttributeValueRepository : IAttributeValueRepository, IDisposable
    {
        #region Variable Declaration
        private DBManager dbManager;
        #endregion

        #region Post
        public bool AddAttributeValue(AttributeValue attributeValue)         
        {
            try
            {
                long count = 0;
                int rowAffacted = 0;
                int secRowAffacted = 0;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();

                    if (attributeValue.RecordStatus != Actions.DELETE.ToString())
                    {
                        //TODO : Get section count+1
                        string getCollectionNum = QueryConfig.UserAttributeValueLogQuerySettings["GetAttributeSectionCount"].ToString();
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@Id", attributeValue.SectionId);
                        IDataReader dr = dbManager.ExecuteReader(CommandType.Text, getCollectionNum);
                        if (dr.Read())
                        {
                            count = Convert.ToInt64(dr["Count"]);
                            dr.Close();
                        }
                        dr.Close();
                        //TODO : Add attribute value to EAV
                        foreach (AttributeRecordSet attributeRecord in attributeValue.AttributeRecords)
                        {
                            if (attributeRecord.Action == Actions.ADD.ToString())
                            {
                                string addAttValueQuery = QueryConfig.UserAttributeValueLogQuerySettings["AddUserAttributeValue"].ToString();
                                dbManager.CreateParameters(7);
                                dbManager.AddParameters(0, "@UserAttributeId", attributeRecord.UserAttributeId);
                                dbManager.AddParameters(1, "@UserId", attributeValue.UserId);
                                dbManager.AddParameters(2, "@AttributeOptionId", attributeRecord.UserAttributeOptionId);
                                dbManager.AddParameters(3, "@Value", attributeRecord.AttributeVal);
                                dbManager.AddParameters(4, "@IsPreSelected", attributeRecord.IsSelected);
                                dbManager.AddParameters(5, "@DisplayOrder", attributeRecord.DisplayOrder);
                                if (attributeValue.RowCollectionId == 0)
                                {
                                    dbManager.AddParameters(6, "@Collection", count+1);
                                }
                                else
                                {
                                    dbManager.AddParameters(6, "@Collection", attributeValue.RowCollectionId);
                                }
                                rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttValueQuery);
                            }
                            if (attributeRecord.Action == Actions.EDIT.ToString())
                            {
                                if (!string.IsNullOrEmpty(attributeRecord.AttributeVal))
                                {
                                    string addAttValueQuery = QueryConfig.UserAttributeValueLogQuerySettings["EditUserAttributeValue"].ToString();
                                    dbManager.CreateParameters(5);
                                    dbManager.AddParameters(0, "@UserAttributeId", attributeRecord.UserAttributeId);
                                    dbManager.AddParameters(1, "@UserId", attributeValue.UserId);
                                    dbManager.AddParameters(2, "@AttributeOptionId", attributeRecord.UserAttributeOptionId);
                                    dbManager.AddParameters(3, "@Value", attributeRecord.AttributeVal);
                                    if (attributeValue.RowCollectionId == 0)
                                    {
                                        dbManager.AddParameters(4, "@Collection", 1);
                                    }
                                    else
                                    {
                                        dbManager.AddParameters(4, "@Collection", attributeValue.RowCollectionId);
                                    }
                                    rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttValueQuery);
                                }
                                else
                                {
                                    string addAttValueQuery = QueryConfig.UserAttributeValueLogQuerySettings["DeleteUserAttributeValue"].ToString();
                                    dbManager.CreateParameters(3);
                                    dbManager.AddParameters(0, "@UserAttributeId", attributeRecord.UserAttributeId);
                                    dbManager.AddParameters(1, "@UserId", attributeValue.UserId);
                                    if (attributeValue.RowCollectionId == 0)
                                    {
                                        dbManager.AddParameters(2, "@Collection", 1);
                                    }
                                    else
                                    {
                                        dbManager.AddParameters(2, "@Collection", attributeValue.RowCollectionId);
                                    }
                                    rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttValueQuery);
                                }
                            }
                            if (attributeRecord.Action == Actions.DELETE.ToString())
                            {
                                string addAttValueQuery = QueryConfig.UserAttributeValueLogQuerySettings["DeleteUserAttributeValue"].ToString();
                                dbManager.CreateParameters(3);
                                dbManager.AddParameters(0, "@UserAttributeId", attributeRecord.UserAttributeId);
                                dbManager.AddParameters(1, "@UserId", attributeValue.UserId);
                                dbManager.AddParameters(2, "@Collection", attributeValue.RowCollectionId);
                                rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttValueQuery);
                            }

                        }
                        //TODO : Update Attribute Section count
                        //if (count > 0)
                        //{
                            string updateSectionQuery = QueryConfig.UserAttributeValueLogQuerySettings["UpdateAttributeSection"].ToString();
                            dbManager.CreateParameters(2);
                            if (attributeValue.RowCollectionId == 0)
                            {
                                dbManager.AddParameters(0, "@Count", (count+1));
                            }
                            else
                            {
                                dbManager.AddParameters(0, "@Count", attributeValue.RowCollectionId);
                            }
                            dbManager.AddParameters(1, "@Id", attributeValue.SectionId);
                            secRowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, updateSectionQuery);
                        //}
                        dbManager.Transaction.Commit();
                        return true;
                    }
                    else if(attributeValue.RecordStatus == Actions.DELETE.ToString())
                    {
                        string addAttValueQuery = QueryConfig.UserAttributeValueLogQuerySettings["DeleteUserAttributeValueSection"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@UserId", attributeValue.UserId);
                        dbManager.AddParameters(1, "@Collection", attributeValue.RowCollectionId);
                        rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, addAttValueQuery);
                        dbManager.Transaction.Commit();
                        if(rowAffacted>0)
                            return true;
                    }
                    return true;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERATTRIBUTEVALUEFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("USERATTRIBUTEVALUEFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion


        public void EditAttributeValue(AttributeValue attributeValue)
        {
            try
            {

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Register Void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
