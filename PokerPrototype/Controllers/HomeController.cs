using PokerPrototype.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PokerPrototype.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["id"] = 1; //comment out to see landing page
            int id = Convert.ToInt32(Session["id"]);
            if (id > 0)
            {
                UserModel model = new UserModel(id);
                return View("Lobby", model);
            }
            return View("Landing");
        }

        public ActionResult Logout()
        {
            Session["id"] = 0;
            ViewBag.MessageType = "info";
            ViewBag.Message = "you have logged out";
            return View("Landing");
        }
        public ActionResult NotLoggedIn()
        {
            ViewBag.MessageType = "info";
            ViewBag.Message = "You must be logged in to view that page";
            return View("Landing");
        }
        public ActionResult PageNotFound()
        {
            ViewBag.MessageType = "warning";
            ViewBag.Message = "Page not found";
            return View("Landing");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult ViewProfile(string username)
        {
            int id = Convert.ToInt32(Session["id"]);
            if (id > 0)
            {
                ViewProfileModel model = new ViewProfileModel(id, username);
                return View(model);
            }
            return NotLoggedIn();
        }
        public ActionResult EditProfile()
        {
            int id = Convert.ToInt32(Session["id"]);
            if (id > 0)
            {
                EditProfileModel model = new EditProfileModel(id);
                return View(model);
            }
            return NotLoggedIn();
        }
    }
}