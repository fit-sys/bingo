using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bingo.Common;

namespace bingo.Models
{
    public class ChatModel : TableEntity
    {
        public ChatModel(string id, string slackid, string message,string time)
        {
            this.UniqueID = id;
            this.SlackID = slackid;
            this.Message = message;
            this.SendTime = time;
            this.PartitionKey = Const.TABLE_CHAT_P_CHAT;
            this.RowKey = id.ToString();
        }
        public ChatModel() { }
        public string UniqueID { get; set; }
        public string SlackID { get; set; }
        public string Message { get; set; }
        public string SendTime { get; set; }
    }
}