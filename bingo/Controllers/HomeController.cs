using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bingo.Hubs;
using Microsoft.AspNet.SignalR;
using bingo.Models;
using Microsoft.WindowsAzure.Storage.Table;
using bingo.Common;


namespace bingo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            AzureStorage.CreateTable(Const.TABLE_CHAT);
            AzureStorage.CreateTable(Const.TABLE_CONNECTION);
            AzureStorage.CreateTable(Const.TABLE_BINGO);

            List<ChatModel> chatModel = AzureStorage.GetAllMessage();
            //if(Session[Const.LOGIN_USER] != null)
            //{
            //    var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //    //GlobalHost.ConnectionManager.GetConnectionContext<ChatHub>();
            //    string slackID = Session[Const.LOGIN_USER].ToString();
            //    ConnectionModel beforeModel = AzureStorage.getConnectionByID(slackID);
            //    List<BingoModel> bingoModel = AzureStorage.getBingoByID(slackID);
                
            //    context.Clients.Client(beforeModel.Connectionid).SetInitData(bingoModel);
            //}
            


            return View(chatModel.OrderByDescending(o => o.SendTime).ToList());
        }

        
    }
}
