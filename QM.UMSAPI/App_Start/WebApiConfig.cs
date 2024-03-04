using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace QM.UMSAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);
            config.MapHttpAttributeRoutes();

         //   config.Routes.MapHttpRoute(
         //    name: "DefaultApi2",
         //    routeTemplate: "api/{Admin}/{controller}/{code}/{id}/{action}",
         //    defaults: new { id = RouteParameter.Optional }
         //);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{Admin}/{controller}/{code}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi3",
            //    routeTemplate: "api/{Admin}/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{Admin}/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            

            QM.UMS.Business.UnitySettings.RegisterComponents(config);
        }
    }
}
