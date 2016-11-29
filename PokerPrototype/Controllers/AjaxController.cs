using PokerPrototype.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PokerPrototype.Controllers
{
    public class AjaxController : Controller
    {
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            LoginModel model = new LoginModel(username, password);
            if (model.id > 0)
                Session["id"] = model.id;

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Register(string email, string username, string password, string confirm)
        {
            RegisterModel register = new RegisterModel(email, username, password, confirm);
            if (register.id > 0)
            {
                Session["id"] = register.id;
            }
            return Json(register, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateRoom(string title, string maxPlayers, string blind, string time, string buyIn, string isPrivate)
        {
            createRoomModel create = new createRoomModel(Convert.ToInt32(Session["id"]), title, maxPlayers, blind, time, buyIn, isPrivate);
            if (create.success)
            {

            }
            return Json(create, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Payment(string amount, string name, string cardNumber, string cvc, string expires, string password)
        {
            PaymentModel payment = new PaymentModel(Convert.ToInt32(Session["id"]), amount, name, cardNumber, cvc, expires, password);
            if (payment.success)
            {
                
            }
            return Json(payment, JsonRequestBehavior.AllowGet);
        }
        public ActionResult changeAvatar(string src)
        {
            AvatarModel avatar = new AvatarModel(Convert.ToInt32(Session["id"]), src);

            return Json(avatar, JsonRequestBehavior.AllowGet);
        }

        public ActionResult changePassword(string oldPassword, string newPassword, string Confirm)
        {
            PasswordModel password = new PasswordModel(Convert.ToInt32(Session["id"]), oldPassword, newPassword, Confirm);

            return Json(password, JsonRequestBehavior.AllowGet);
        }

        public ActionResult changeEmail(string email, string password)
        {
            EmailModel model = new EmailModel(Convert.ToInt32(Session["id"]), email, password);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        // GET: Ajax
        public ActionResult Lobby()
        {
            
            return Json(new RoomList(), JsonRequestBehavior.AllowGet);
        }
    }
}