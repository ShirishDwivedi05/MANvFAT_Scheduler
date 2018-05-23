using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MANvFAT_Football
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "LeagueTagRoute",
            //    url: "{id}",
            //    defaults: new { controller = "Leagues", action = "FixtureByTag" }
            //);

            routes.MapRoute(
              name: "MemberRoute_NoPremiumMembership",
              url: "Member/NoPremiumMembership",
              defaults: new { controller = "Member", action = "NoPremiumMembership", id = UrlParameter.Optional }
          );

            routes.MapRoute(
          name: "MemberRoute_Login",
          url: "Member/Login",
          defaults: new { controller = "Member", action = "Login", id = UrlParameter.Optional }
      );

            routes.MapRoute(
               name: "MemberRoute",
               url: "Member/{id}",
               defaults: new { controller = "Member", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
             name: "ReferRoute",
             url: "Refer/{id}",
             defaults: new { controller = "Refer", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
           name: "ReferCodeRoute",
           url: "ReferCode/{id}",
           defaults: new { controller = "ReferCode", action = "Index", id = UrlParameter.Optional }
       );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
