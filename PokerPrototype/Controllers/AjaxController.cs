﻿using PokerPrototype.Models;
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
            {
                Response.Cookies["MYCOOKIE"].Value = "THIS IS A COOKIE!!!";
                Session["id"] = model.id;
            }

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

        public ActionResult deleteAccount(string password)
        {
            DeleteModel model = new DeleteModel(Convert.ToInt32(Session["id"]), password);
            if (model.success == true)
                Session["id"] = 0;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewProfile(string username)
        {
            ViewProfileModel model = new ViewProfileModel(Convert.ToInt32(Session["id"]), username);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddFriend(string newuser)
        {
            AddFriendModel model = new AddFriendModel(Convert.ToInt32(Session["id"]), newuser);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveFriend(string newuser)
        {
            RemoveFriendModel model = new RemoveFriendModel(Convert.ToInt32(Session["id"]), newuser);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BanUser(string user)
        {
            BanUser model = new BanUser(user);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UnbanUser(string user)
        {
            unbanUser model = new unbanUser(user);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PromoteUser(string user)
        {
            promoteUser model = new promoteUser(user);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DemoteUser(string user)
        {
            demoteUser model = new demoteUser(user);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult blockUser(string user)
        {
            blockUserModel model = new blockUserModel(Convert.ToInt32(Session["id"]), user);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult unblockUser(string user)
        {
            unblockUserModel model = new unblockUserModel(Convert.ToInt32(Session["id"]), user);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult inbox()
        {
            InboxList list = new InboxList(Convert.ToInt32(Session["id"]));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SentMessages()
        {
            SentList list = new SentList(Convert.ToInt32(Session["id"]));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendMessage(string to, string message)
        {
            SendMessageModel model = new SendMessageModel(Convert.ToInt32(Session["id"]), to, message);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Lobby()
        {
            
            return Json(new RoomList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportList()
        {
            return Json(new ReportList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportUser(string title, string toUser, string issue)
        {
            ReportUserModel model = new ReportUserModel(Convert.ToInt32(Session["id"]), title, toUser, issue);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportBug(string title, string issue)
        {
            ReportBugModel model = new ReportBugModel(Convert.ToInt32(Session["id"]), title, issue);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResolveReport(int reportID)
        {
            ResolveReport model = new ResolveReport(reportID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}