using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PokerPrototype
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "logout",
                url: "logout",
                defaults: new { controller = "Home", action = "Logout" }
            );
            routes.MapRoute(
                name: "room",
                url: "room/{roomid}",
                defaults: new { controller = "Room", action = "Index" }
            );
            routes.MapRoute(
                name: "profile",
                url: "profile/{username}",
                defaults: new { controller = "Home", action = "ViewProfile" }
            );
            routes.MapRoute(
                name: "editprofile",
                url: "editprofile",
                defaults: new { controller = "Home", action = "EditProfile" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }

            );
        }
    }
}
