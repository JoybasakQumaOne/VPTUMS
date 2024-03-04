using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CommonApplicationFramework.ConfigurationHandling;
using CommonApplicationFramework.DataHandling;
using CommonApplicationFramework.Common;
using CommonApplicationFramework.Caching;
using System.Data.SqlClient;
using CommonApplicationFramework.Logging;
using CommonApplicationFramework.ExceptionHandling;
using System.Web;

namespace QM.UMS.Repository.Repository
{
    public static class Connection
    {
        private static DBManager dbManager = null;

        public static string GetConnectionString(string bookGuid)
        {
            string connectionString = string.Empty;
            
            try
            {
				if (GlobalCacheManager.Instance.Exists("book_content_connectionstring"))
				{
					connectionString = GlobalCacheManager.Instance.Get("book_content_connectionstring").ToString();
				}
				else
				{

					using (dbManager = new DBManager())
                    {
                        //string code = dbManager.GetCodeFromURL(this.URL); 
                        dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                        dbManager.Open();

                        string query = QueryConfig.ControlMasterQuerySettings["GetInstanceDetails"];
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@BookGuid", Guid.Parse(bookGuid));
                        IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);


                        while (dr.Read())
                        {
                            connectionString = "Server=" + dr["DBServer"].ToString() + "; Database=" + dr["DBName"].ToString() + "; User Id=" + dr["DBUserName"].ToString() + "; password=" + dr["DBPassword"].ToString();
                            GlobalCacheManager.Instance.Set("book_content_connectionstring", connectionString);
                        }
                        dr.Close();
                    }
				}
				//connectionString = "Data Source =.; Initial Catalog = BookContent; Integrated Security = True";
				return connectionString;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETCONNECTIONFAILED", MessageConfig.MessageSettings["GETCONNECTIONFAILED"].ToString(), string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETCONNECTIONFAILED", MessageConfig.MessageSettings["GETCONNECTIONFAILED"].ToString(), string.Empty);
            }
            finally
            {
                 
            }
            
        }

        public static Item GetConnectionStringOrg(Guid OrganizationGuid)
        {
            string connectionString = string.Empty;
            Item item = new Item();

            try
            {
                //if (GlobalCacheManager.Instance.Exists("book_content_connectionstring"))
                //{
                //    item.Value = GlobalCacheManager.Instance.Get("book_content_connectionstring").ToString();
                //    //connectionString = GlobalCacheManager.Instance.Get("book_content_connectionstring").ToString();
                //}
                //else
                //{
                    using (dbManager = new DBManager())
                    {
                        dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                        dbManager.Open();

                        string query = QueryConfig.ControlMasterQuerySettings["GetOrganizationInstanceDetails"];
                        dbManager.CreateParameters(1);
                        dbManager.AddParameters(0, "@OrganizationGuid", OrganizationGuid);
                        IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);


                        while (dr.Read())
                        {
                            item.Id = Convert.ToInt32(dr["Id"]);
                            item.Value = "Server=" + dr["DBServer"].ToString() + "; Database=" + dr["DBName"].ToString() + "; User Id=" + dr["DBUserName"].ToString() + "; password=" + dr["DBPassword"].ToString();
						
						//connectionString = "Server=" + dr["DBServer"].ToString() + "; Database=" + dr["DBName"].ToString() + "; User Id=" + dr["DBUserName"].ToString() + "; password=" + dr["DBPassword"].ToString();
						//GlobalCacheManager.Instance.Set("book_content_connectionstring", item.Value);
					}
                        dr.Close();
                    }
                //}
                return item;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETCONNECTIONFAILED", MessageConfig.MessageSettings["GETCONNECTIONFAILED"].ToString(), string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETCONNECTIONFAILED", MessageConfig.MessageSettings["GETCONNECTIONFAILED"].ToString(), string.Empty);
            }
            finally
            {

            }
        }

		public static Item GetConnectionStringOrg(int  OrganizationId)
		{
			string connectionString = string.Empty;
			Item item = new Item();

			try
			{
				//if (GlobalCacheManager.Instance.Exists("book_content_connectionstring"))
				//{
				//    item.Value = GlobalCacheManager.Instance.Get("book_content_connectionstring").ToString();
				//    //connectionString = GlobalCacheManager.Instance.Get("book_content_connectionstring").ToString();
				//}
				//else
				//{
				using (dbManager = new DBManager())
				{
					dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
					dbManager.Open();

					string query = QueryConfig.ControlMasterQuerySettings["GetOrganizationInstanceDetailsByOrgId"];
					dbManager.CreateParameters(1);
					dbManager.AddParameters(0, "@OrganizationId", OrganizationId);
					IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);


					while (dr.Read())
					{
						item.Id = Convert.ToInt32(dr["Id"]);
						item.Value = "Server=" + dr["DBServer"].ToString() + "; Database=" + dr["DBName"].ToString() + "; User Id=" + dr["DBUserName"].ToString() + "; password=" + dr["DBPassword"].ToString();
						//connectionString = "Server=" + dr["DBServer"].ToString() + "; Database=" + dr["DBName"].ToString() + "; User Id=" + dr["DBUserName"].ToString() + "; password=" + dr["DBPassword"].ToString();
						//GlobalCacheManager.Instance.Set("book_content_connectionstring", item.Value);
					}
					dr.Close();
				}
				//}
				return item;
			}
			catch (SqlException sqlEx)
			{
				LogManager.Log(sqlEx);
				throw new RepositoryException("GETCONNECTIONFAILED", MessageConfig.MessageSettings["GETCONNECTIONFAILED"].ToString(), string.Empty);
			}
			catch (Exception ex)
			{
				LogManager.Log(ex);
				throw new RepositoryException("GETCONNECTIONFAILED", MessageConfig.MessageSettings["GETCONNECTIONFAILED"].ToString(), string.Empty);
			}
			finally
			{

			}
		}
		public static string GetControlDBName()
		{
			DBManager dbManager = new DBManager();
			dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
			dbManager.Open();
			string controlDB = dbManager.Connection.Database;
			dbManager.Close();
			dbManager = null;
			return controlDB;
		}
        public static string GetContentDBName()
        {
            string contentDbName = string.Empty;
            string connectionString= GetConnectionString(Convert.ToString(HttpContext.Current.Request.Headers["BookGuid"]));
            DBManager dbManager = null ;
            using (dbManager=new DBManager())
            {
                dbManager.ConnectionString = connectionString;
                dbManager.Open();
                contentDbName = dbManager.Connection.Database;
                dbManager.Close();
                dbManager = null;
                return contentDbName; 
            }
        }
    }
}
