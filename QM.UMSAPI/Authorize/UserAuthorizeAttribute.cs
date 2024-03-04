using CommonApplicationFramework.Caching;
using CommonApplicationFramework.Common;
using CommonApplicationFramework.ConfigurationHandling;
using Newtonsoft.Json;
using QM.UMS.Business.Business;
using QM.UMS.Business.IBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QM.UMSAPI.Authorize
{
    public class UserAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        public string Actions { get; set; }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
			//IUsersBusiness _IUserBusiness = new UsersBusiness();
			//if (_IUserBusiness.ValidateToken(actionContext.Request.Headers.Authorization.Parameter, System.Web.HttpContext.Current.Request.Headers["UserIP"]))
			//{
			//    if (Authorize(actionContext))
			//    {
			//        return;
			//    }
			//}
			return;
            if (this.ValidateToken(System.Web.HttpContext.Current.Request.Headers["RequestId"], actionContext.Request.Headers.Authorization.Parameter))
            {
                if (Authorize(actionContext, System.Web.HttpContext.Current.Request.Headers["RequestId"]))
                {
                    return;
                }
            }
            HandleUnauthorizedRequest(actionContext);
        }

        private bool ValidateToken(string cacheKey, string RequestToken)
        {

            if (CacheManager.Instance.Exists("usercontext_" + cacheKey))
            {
                UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + cacheKey));
                if (userContext.Token.Token.Equals(RequestToken) && DateTime.Now <= userContext.Token.ExpireOn)
                {
                    userContext.Token.ExpireOn = userContext.Token.ExpireOn.AddMinutes(userContext.Token.Duration);
                    return true;
                }
            }
            return true;
        }

        private bool Authorize(System.Web.Http.Controllers.HttpActionContext actionContext, string cacheKey)
        {
            try
            {
                //IUsersBusiness _IUserBusiness = new UsersBusiness();
                //bool isAuthorize = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("UserAuthorize")).Value.ToString());
                //if (isAuthorize)
                //{
                //    return _IUserBusiness.ValidateAction(actionContext.Request.Headers.Authorization.Parameter, Actions);
                //    //Logic to validate wheather user role has access permission to execute this action.
                //    //HandleUnauthorizedRequest(actionContext);
                //    //return false;
                //}
                bool isAuthorize = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("UserAuthorize")).Value.ToString());
                if (isAuthorize)
                {
                    if (CacheManager.Instance.Exists("usercontext_" + cacheKey))
                    {
                        UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + cacheKey));
                        if (userContext.Actions.Find(x => x.Value.Equals(Actions)) != null)
                        {
                            return true;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            }
        }


    }
}