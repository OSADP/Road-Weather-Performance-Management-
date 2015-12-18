using InfloCommon.Repositories;
using RWPMPortal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RWPMPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalFilters.Filters.Add(new SharedLayoutActionFilter(), 0);
        }

        public class SharedLayoutActionFilter : ActionFilterAttribute
        {

            public override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                bool InACurrentWeatherEvent = false;
                try
                {
                    string cs = System.Configuration.ConfigurationManager.ConnectionStrings["InfloDatabaseConnectionString"].ConnectionString;

                    IUnitOfWork uow = new UnitOfWork(cs);
                    var w = uow.WeatherEvents.OrderByDescending(d => d.StartTime).FirstOrDefault();
                    if (w != null && w.EndTime == null)
                    {
                        InACurrentWeatherEvent = true;
                    }
                    filterContext.Controller.ViewBag.InACurrentWeatherEvent = InACurrentWeatherEvent;

                }
                catch { }
               
            }

        }
    }
}
