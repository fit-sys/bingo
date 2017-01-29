using bingo.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bingo.Common;

namespace bingo.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult login(string id)
        {
            Session[Const.LOGIN_USER] = id;
            if (id == Const.USER_ME)
            {
                Session[Const.USER_ADMIN] = Const.USER_ADMIN;
                return Json(new { user = Const.USER_ADMIN }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { user = Const.USER_GUEST }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}