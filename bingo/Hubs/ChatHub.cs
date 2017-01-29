using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using bingo.Models;
using bingo.Common;
using Microsoft.AspNet.SignalR.Transports;

namespace bingo.Hubs
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void getBingoInfo(string name)
        {
            List<BingoModel> bingo = getBingoInfoByUser(name);
            Clients.Caller.SetInitData(bingo);
            AzureStorage.getConnection(name, Context.ConnectionId, Func.GetLocalDate().ToString(Const.FORMAT_DATE_TIME));
        }
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string name, string message)
        {
            // create table if not
            AzureStorage.CreateTable(Const.TABLE_CHAT);
            // key
            string guid = Guid.NewGuid().ToString("N");
            // datetime

            string time = Func.GetLocalDate().ToString(Const.FORMAT_TIME);
            // make chat model
            ChatModel chatModel = new ChatModel(guid, name, message, time);
            // insert chat model
            bool result = AzureStorage.InsertData(Const.TABLE_CHAT,chatModel);
            // return client
            Clients.All.broadcastMessage(name, message, time);
        }

        /// <summary>
        /// make another bingo number
        /// </summary>
        /// <param name="name"></param>
        public void GetBingoshuffle(string name)
        {
            List<BingoModel> bingo = setInitBingo(name, 25);
            Clients.Group(Const.GROUP_BINGO).SetSlackID("");

            Clients.Caller.SetInitData(bingo);
        }

        public void setSelectNumberToAll(int number)
        {
            // db set
            //AzureStorage.
            // to all member
            Clients.All.SetSelectNumberToAll(number);
            AzureStorage.SetNumberToDB(number);
        }

        public void SetBingoGameStart()
        {
            // to member
            Clients.Others.SetBingoGameStart();
            // to host
            Clients.Caller.SetBingoGameStart();
        }

        /// <summary>
        /// 
        /// </summary>
        public void getGroupList()
        {
            Clients.Group(Const.GROUP_BINGO).SetSlackID("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task JoinGroup(string name)
        {
            AzureStorage.CreateTable(Const.TABLE_BINGO);
            //await Groups.Remove(Context.ConnectionId, Const.GROUP_BINGO);
            ConnectionModel connInfo = putConnection(name);

            List<BingoModel> bingo = AzureStorage.getBingoByID(name);
            if(bingo.Count() == 0)
            {
                bingo = setInitBingo(name, 25);
            }

            // todo: some problems with reconnection into the group
            //-> https://www.asp.net/signalr/overview/guide-to-the-api/working-with-groups
            //Groups.Add(connInfo.Connectionid, Const.GROUP_BINGO);
            await Clients.Caller.SetSlackID(connInfo.SlackID);
            Clients.Caller.SetInitData(bingo);

            
        }

        private bool checkConnectionByID(string connectionid)
        {
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();

            var connectionAlive = heartBeat.GetConnections().FirstOrDefault(c => c.ConnectionId == connectionid);

            return connectionAlive.IsAlive;
        }

        

        private List<BingoModel> setInitBingo(string name,int maxSize)
        {
            List<int> ranNum = new List<int>();
            for(int i = 1; i <= maxSize; i++)
            {
                ranNum.Add(i);
            }
            Func.Shuffle(ranNum);
            for(int i = 1; i <= maxSize; i++)
            {
                BingoModel bingo = new BingoModel(name,i, ranNum[i - 1], false);
                AzureStorage.UpdateData(Const.TABLE_BINGO, bingo);
            }

            List<BingoModel> bingoList = AzureStorage.getBingoByID(name);
            return bingoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<BingoModel> getBingoInfoByUser(string name)
        {
            List<BingoModel> bingoList = AzureStorage.getBingoByID(name);
            return bingoList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ConnectionModel putConnection(string name)
        {
            // create table if not
            string time = Func.GetLocalDate().ToString(Const.FORMAT_DATE_TIME);
            AzureStorage.CreateTable(Const.TABLE_CONNECTION);
            //ConnectionModel connModel = new ConnectionModel(name, Context.ConnectionId, time);
            ConnectionModel beforeModel = AzureStorage.getConnection(name, Context.ConnectionId, time);
            
            return beforeModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task LeaveGroup()
        {
            return Groups.Remove(Context.ConnectionId, Const.GROUP_BINGO);
        }
    }
}