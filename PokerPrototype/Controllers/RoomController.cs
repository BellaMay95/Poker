using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PokerPrototype.Models;

namespace PokerPrototype.Controllers
{
    public class RoomController : Controller
    {
        // GET: Room
        public ActionResult Index(string roomid)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(roomid, @"^\d+$"))
            {
                ViewBag.RoomID = roomid;
                int id = Convert.ToInt32(Session["id"]);
                if (id > 0)
                {
                    UserModel model = new UserModel(id);
                    return View(model);
                }
            }
            ViewBag.MessageType = "warning";
            ViewBag.Message = "Page not found";
            return View("/Views/Home/Landing.cshtml");
        }
    }
}