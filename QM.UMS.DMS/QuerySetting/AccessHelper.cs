using QM.eBook.DMS.DBConnection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;


namespace QM.eBook.DMS.QuerySetting
{
    public class AccessHelper
    {

        public string PhysicalPath { get; set; }
        public string BrowseURL { get; set; }
        public string LogFilePath { get; set; }
        //public string VirtualPath = @"E:\DMSContentServerWithFolder";

        //public string BrowseFilePath = "http://122.166.219.125:5057/DMSContentServer";

        public string GetQueryDetail(string strQueryInput)
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            try
            {
                if (!queryString.ContainsKey(strQueryInput))
                {
                    QueryConfiguration queryConfig = new QueryConfiguration();
                    queryString = queryConfig.GetAllQuery();
                }
                return queryString[strQueryInput];
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public int GetFolderId(DBhandler dbManager, string code)// this is used in upload API
        {
            int folderId = 0;
            string query = GetQueryDetail("GetFolderIdByCode");
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@FolderCode", code);
            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
            if (dr.Read())
            {
                folderId = Convert.ToInt32(dr["Id"]);
            }
            dr.Close();
            return folderId;
        }
        public void CreateFolder(string fileName)
        {
            string createNewPath = this.PhysicalPath + "/" + fileName;
            if (!Directory.Exists(createNewPath))
            {
                Directory.CreateDirectory(createNewPath);
            }
        }
    }
}
