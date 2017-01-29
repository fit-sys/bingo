using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using bingo.Common;
using Microsoft.AspNet.SignalR;
using bingo.Hubs;

namespace bingo.Models
{
    public static class AzureStorage
    {
        static string storageConn = ConfigurationManager.AppSettings.Get(Const.STORAGE_CONN);
        public static void CreateTable(string tableName)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        public static void DeleteAllBingoTableData()
        {
            List<BingoModel> bingoList = GetAllBingoData(Const.TABLE_BINGO);
            foreach(var item in bingoList)
            {
                DeleteData(Const.TABLE_BINGO, item);
            }    
        }

        public static void DeleteAllConnectionTableData()
        {
            List<ConnectionModel> connList = GetAllConnData(Const.TABLE_CONNECTION);
            foreach (var item in connList)
            {
                DeleteData(Const.TABLE_CONNECTION, item);
            }
        }

        internal static void DeleteAllChatData()
        {
            List<ChatModel> chatList = GetAllChatData(Const.TABLE_CHAT);
            foreach (var item in chatList)
            {
                DeleteData(Const.TABLE_CHAT, item);
            }
        }

        public static bool InsertData(string tableName, TableEntity model)
        {
            
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                TableOperation insert = TableOperation.Insert(model);
                CloudTable table = tableClient.GetTableReference(tableName);
                TableResult result = table.Execute(insert);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SetNumberToDB(int number)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(Const.TABLE_BINGO);
                var entities = table.ExecuteQuery(new TableQuery<BingoModel>()).ToList();

                foreach(BingoModel item in entities)
                {
                    if(item.BingoNo == number)
                    {
                        item.selected = true;
                        AzureStorage.UpdateData(Const.TABLE_BINGO, item);
                    }
                }
            }
            catch
            {
                
            }
        }

        public static bool DeleteData(string tableName, TableEntity model)
        {

            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                TableOperation delete = TableOperation.Delete(model);
                CloudTable table = tableClient.GetTableReference(tableName);
                TableResult result = table.Execute(delete);
                return true;
            }
            catch
            {
                return false;
            }
        }

        

        public static bool UpdateData(string tableName, TableEntity model)
        {

            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                TableOperation update = TableOperation.InsertOrReplace(model);
                CloudTable table = tableClient.GetTableReference(tableName);
                TableResult result = table.Execute(update);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ConnectionModel getConnection(string name, string connectionid, string time)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(Const.TABLE_CONNECTION);
                TableOperation retrieve = TableOperation.Retrieve<ConnectionModel>(Const.TABLE_CONNECTION_P_MAIM, name);
                TableResult result = table.Execute(retrieve);
                var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                ConnectionModel newConnModel = new ConnectionModel(name, connectionid, time);
                if (result.Result == null)
                {
                    context.Groups.Add(connectionid, Const.GROUP_BINGO);
                    // insert 
                    
                    bool ret = AzureStorage.InsertData(Const.TABLE_CONNECTION, newConnModel);
                    if (ret)
                    {
                        return newConnModel;
                    }
                    else
                    {       
                        throw new Exception();
                    }
                }
                else
                {
                    ConnectionModel dbConnModel = (ConnectionModel)result.Result;
                    if (connectionid == dbConnModel.Connectionid)
                    {
                        
                        // get from db
                        return dbConnModel;
                    }
                    else
                    {
                        context.Groups.Remove(dbConnModel.Connectionid, Const.GROUP_BINGO);
                        context.Groups.Add(newConnModel.Connectionid, Const.GROUP_BINGO);

                        //uptate
                        bool ret = AzureStorage.UpdateData(Const.TABLE_CONNECTION, newConnModel);
                        if (ret)
                        {
                            return newConnModel;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        public static ConnectionModel getConnectionByID(string name)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(Const.TABLE_CONNECTION);
                TableOperation retrieve = TableOperation.Retrieve<ConnectionModel>(Const.TABLE_CONNECTION_P_MAIM, name);
                TableResult result = table.Execute(retrieve);
                
                ConnectionModel connModel = (ConnectionModel)result.Result;
                return connModel;
            }
            catch
            {
                throw new Exception();
            }
        }

        public static List<BingoModel> getBingoByID(string name)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(Const.TABLE_BINGO);
                //TableOperation retrieve = TableOperation.Retrieve<BingoModel>(Const.TABLE_BINGO_P_MAIN, name);
                //TableResult result = table.Execute(retrieve);

                //List<BingoModel> bingoModel = (List<BingoModel>)result.Result;
                //return bingoModel;

                // Construct the query operation for all customer entities where PartitionKey="Smith".
                //TableQuery<BingoModel> query = new TableQuery<BingoModel>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, name));

                // Print the fields for each customer.
                //foreach (CustomerEntity entity in table.ExecuteQuery(query))
                //{
                //    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                //        entity.Email, entity.PhoneNumber);
                //}

                //return query.ToList();


                TableQuery<BingoModel> exQuery = new TableQuery<BingoModel>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                                                             name));

                var results = table.ExecuteQuery(exQuery).Select(ent => (BingoModel)ent).ToList();
                //var results = table.ExecuteQuery(exQuery).ToList();
                return results;

                //var entities = table.ExecuteQuery(new TableQuery<BingoModel>()).Where(w => w.PartitionKey == name).ToList();
                //var entities = table.ExecuteQuery(new TableQuery<BingoModel>()).ToList();
                //return entities.Where(w => w.PartitionKey == name).ToList();
            }
            catch
            {
                throw new Exception();
            }
        }
        
        public static List<ChatModel> GetAllMessage()
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(Const.TABLE_CHAT);
                var entities = table.ExecuteQuery(new TableQuery<ChatModel>()).ToList();
                return entities;
            }
            catch
            {
                return new List<ChatModel>();
            }
        }

        public static List<BingoModel> GetAllBingoData(string tableName)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(tableName);
                var entities = table.ExecuteQuery(new TableQuery<BingoModel>()).ToList();
                return entities;
            }
            catch
            {
                return new List<BingoModel>();
            }
        }

        public static List<ConnectionModel> GetAllConnData(string tableName)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(tableName);
                var entities = table.ExecuteQuery(new TableQuery<ConnectionModel>()).ToList();
                return entities;
            }
            catch
            {
                return new List<ConnectionModel>();
            }
        }

        public static List<ChatModel> GetAllChatData(string tableName)
        {
            CloudStorageAccount storageAccout = CloudStorageAccount.Parse(storageConn);
            CloudTableClient tableClient = storageAccout.CreateCloudTableClient();
            try
            {
                CloudTable table = tableClient.GetTableReference(tableName);
                var entities = table.ExecuteQuery(new TableQuery<ChatModel>()).ToList();
                return entities;
            }
            catch
            {
                return new List<ChatModel>();
            }
        }
    }
}