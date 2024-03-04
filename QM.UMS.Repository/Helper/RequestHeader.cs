using CommonApplicationFramework.Caching;
using CommonApplicationFramework.Common;
using CommonApplicationFramework.DataHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QM.UMS.Repository.Helper
{
    public class RequestHeader
    {
        public string Code { get; set; }
        public string RequestId { get; set; }
        public string BookGuid { get; set; }
        public string UserAgent { get; set; }
        public UserContext Context { get; set; }
        public string ConnectionId { get; set; }
        public RequestHeader()
        {
            try
            {
                Code = Convert.ToString(HttpContext.Current.Request.Headers["ModuleCode"]);
                RequestId = Convert.ToString(HttpContext.Current.Request.Headers["RequestId"]);
                ConnectionId = Convert.ToString(HttpContext.Current.Request.Headers["ConnectionId"]);


                if (ConfigurationManager.AppSettings["HostName"].ToString().ToUpper() == "DEV")
                {
                    if (GlobalCacheManager.Instance.Exists("usercontext_" + ConvertData.ToString(System.Web.HttpContext.Current.Request.Headers["RequestId"])))
                    {
                        if (GlobalCacheManager.Instance.Get("usercontext_" + System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString()) != null)
                        {
                            Context = JsonConvert.DeserializeObject<UserContext>(GlobalCacheManager.Instance.Get("usercontext_" + System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString()));
                            Context = Context.InstanceList.Where(n => n.Code == this.ConnectionId).FirstOrDefault().TanentContext;
                        }
                    }
                    else
                    {
                        Context = new UserContext();
                        Context.UserId = 161;
                        Context.UserGuid = "2DC3C3DE-5018-486E-A330-FEF83FA07DFC";
                        Context.Email = "anish.de@qumaone.com";
                    }
                }
                else
                {
                    if (GlobalCacheManager.Instance.Get("usercontext_" + System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString()) != null)
                    {
                        Context = JsonConvert.DeserializeObject<UserContext>(GlobalCacheManager.Instance.Get("usercontext_" + System.Web.HttpContext.Current.Request.Headers["RequestId"].ToString()));
                        if (Context != null)
                        {
                            Context = Context.InstanceList.Where(n => n.Code == this.ConnectionId).FirstOrDefault().TanentContext;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }
    }
}
