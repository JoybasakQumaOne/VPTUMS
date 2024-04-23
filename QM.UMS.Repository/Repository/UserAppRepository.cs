using CommonApplicationFramework.ConfigurationHandling;
using CommonApplicationFramework.DataHandling;
using CommonApplicationFramework.Logging;
using QM.UMS.Models;
using QM.UMS.Repository.Helper;
using QM.UMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.Repository
{
    public class UserAppRepository : RequestHeader, IUserAppRepository
    {
        private DBManager dbManager;


        public bool MapControlUserApp(string email)
        {
            throw new NotImplementedException();
        }

        public AppModel GetAppInfo()
        {
            AppModel app = null;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@ConnectionId", this.ConnectionId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, QueryConfig.UserQuerySettings["GetAppInfo"].ToString());
                    if (dr.Read())
                    {
                        app = new AppModel()
                        {
                            Code = ConvertData.ToString(dr["Code"]),
                            Name = ConvertData.ToString(dr["Name"]),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                return null;
            }
            return app;
        }

        public bool MapUserApp(string email)
        {
            AppModel app = GetAppInfo();
            bool result = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@Email", email);
                    dbManager.AddParameters(1, "@ConnectionId", this.ConnectionId);
                    dbManager.AddParameters(2, "@App", Context.ApplicationType?.Code);
                    if (dbManager.ExecuteNonQuery(CommandType.Text, QueryConfig.UserQuerySettings["LinkUserToProductInControl"].ToString()) > 0)
                    {
                        result = true;
                    }
                }
                if (result)
                {
                    using (dbManager = new DBManager())
                    {
                        dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId);
                        dbManager.Open();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@email", email);
                        dbManager.AddParameters(1, "@App", Context.ApplicationType?.Code);
                        if (dbManager.ExecuteNonQuery(CommandType.Text, QueryConfig.UserQuerySettings["LinkUserToProductInContent"].ToString()) > 0)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                return false;
            }
            return result;
        }
    }
}
