using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bingo.Common;
using bingo.Hubs;
using Microsoft.AspNet.SignalR;
using bingo.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace bingo.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            if(Session[Const.USER_ADMIN] == null)
            {
                Session[Const.USER_ADMIN] = Const.USER_ADMIN;
                Session[Const.LOGIN_USER] = Const.USER_ADMIN;
            }
            AzureStorage.CreateTable(Const.TABLE_CHAT);
            AzureStorage.CreateTable(Const.TABLE_CONNECTION);
            AzureStorage.CreateTable(Const.TABLE_BINGO);

            List<ChatModel> chatModel = AzureStorage.GetAllMessage();
            if (Session[Const.LOGIN_USER] != null)
            {
                string slackID = Session[Const.LOGIN_USER].ToString();
                ConnectionModel beforeModel = AzureStorage.getConnectionByID(slackID);
                List<BingoModel> bingoModel = AzureStorage.getBingoByID(slackID);
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                context.Clients.Client(beforeModel.Connectionid).SetInitData(bingoModel);
            }
            return View(chatModel.OrderByDescending(o => o.SendTime).ToList());
        }

        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(string id)
        {
            if (id == "choco")
            {
                Session[Const.USER_ADMIN] = Const.USER_ADMIN;
                //Session[Const.LOGIN_USER] = Const.USER_ME;
                return RedirectToAction("index", "admin");
            }
            else
            {
                return View();
            }
            
        }

        public ActionResult bingoGameReset()
        {
            //reset conn and bingo table
            AzureStorage.DeleteAllConnectionTableData();
            AzureStorage.DeleteAllBingoTableData();

            
            return Json(new { result="success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getUserListData()
        {
            List<ConnectionModel> connModel = AzureStorage.GetAllConnData(Const.TABLE_CONNECTION);


            return Json(new { result = "success", conn = connModel }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult setTurnUser(string id)
        {
            ConnectionModel connModel = AzureStorage.getConnectionByID(id);
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //context.Clients.User(connModel.Connectionid).setBingoMyTurn();
            //context.Clients.All.setBingoMyTurn();
            //context.Clients.Group(Const.GROUP_BINGO,"").setBingoNotMyTurn();
            context.Clients.Client(connModel.Connectionid).setBingoMyTurn();
            //context.Clients.AllExcept(connModel.Connectionid).setBingoNotMyTurn();
            return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult chatReset()
        {
            //reset chat table
            AzureStorage.DeleteAllChatData();

            return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult Suffle()
        //{
        //    if (Session[Const.USER_ADMIN] != null && Session[Const.USER_ADMIN].ToString() == Const.USER_ADMIN)
        //    {
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("index", "home");
        //    }
        //}
    }
}