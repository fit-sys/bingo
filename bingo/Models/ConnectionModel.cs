using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bingo.Common;

namespace bingo.Models
{

    public class ConnectionModel : TableEntity
    {
        public ConnectionModel(string slackid, string connectionid,string lastLoginTime)
        {
            this.SlackID = slackid;
            this.Connectionid = connectionid;
            this.LastLoginTime = lastLoginTime;
            this.PartitionKey = Const.TABLE_CONNECTION_P_MAIM;
            this.RowKey = slackid.ToString();
        }
        public ConnectionModel() { }
        public string Connectionid { get; set; }
        public string SlackID { get; set; }
        public string Message { get; set; }
        public string LastLoginTime { get; set; }
    }
}