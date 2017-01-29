using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bingo.Common; 

namespace bingo.Models
{
    public class BingoModel : TableEntity
    {
        public BingoModel(string slackid, int bingoID, int bingoNo, bool selected)
        {
            this.SlackID = slackid;
            this.BingoID = bingoID;
            this.BingoNo = bingoNo;
            this.selected = selected;
            this.PartitionKey = slackid;
            this.RowKey = slackid + Const.UNDER_BAR + bingoID.ToString();
        }
        public BingoModel() { }
        public string SlackID { get; set; }
        public int BingoID { get; set; }
        public int BingoNo { get; set; }
        public bool selected { get; set; }
    }
}
