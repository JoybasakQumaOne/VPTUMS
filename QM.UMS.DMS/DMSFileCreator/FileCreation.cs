using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSMicroService.Module;
using DMSMicroService.DBConnection;
using DMSMicroService.QuerySetting;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DMSMicroService.DMSFileCreator
{
    public class FileCreation : IDisposable
    {
        #region Variable Declaration
        public string UserId { get; set; }
        public string AgentCode { get; set; }
        private DBhandler dbManager;
        private AccessHelper accessHelper;
        #endregion

        #region Constructor
        public FileCreation()
        {
            accessHelper = new AccessHelper();
        }
        #endregion

        #region Properties
        public string RequestId { get; set; }
        public string ModuleCode { get; set; }
        #endregion

        #region Get All Folder
        public List<FolderModel> GetAllFolder(string Code, string connectionString)
        {
            FolderModel folderModel;
            List<FolderModel> folderModels = new List<FolderModel>();
            try
            {
                using (dbManager = new DBhandler())
                {
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();

                    string query = accessHelper.GetQueryDetail("GetAllFolder");
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        folderModel = new FolderModel();
                        folderModel.id = Convert.ToInt32(dr["Id"]);
                        folderModel.FolderCode = Convert.ToString(dr["FolderCode"]);
                        folderModel.value = Convert.ToString(dr["FolderName"]);
                        folderModel.PhysicallFolderName = Convert.ToString(dr["PhysicallFolderName"]);
                        folderModel.ParentId = Convert.ToInt32(dr["ParentId"]);
                        folderModel.IsVisible = Convert.ToBoolean(dr["IsVisible"]);
                        folderModel.CreatedBy = (dr["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["CreatedBy"]);
                        folderModel.CreatedOn = (dr["CreatedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        folderModel.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        folderModel.ModifiedOn = (dr["Modifiedon"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dr["Modifiedon"].ToString());
                        folderModels.Add(folderModel);
                    }
                    this.dbManager.CloseReader();
                    return folderModels;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception();
                //throw new RepositoryException("GETFOLDERFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public List<FolderModel> GetAllFolderById(string Code, int Id, string connectionString)
        {
            FolderModel folderModel;
            List<FolderModel> folderModels = new List<FolderModel>();
            try
            {
                using (dbManager = new DBhandler())
                {
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    string query = accessHelper.GetQueryDetail("GetFolderById");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        folderModel = new FolderModel();
                        folderModel.id = Convert.ToInt32(dr["Id"]);
                        folderModel.FolderCode = Convert.ToString(dr["FolderCode"]);
                        folderModel.value = Convert.ToString(dr["FolderName"]);
                        folderModel.PhysicallFolderName = Convert.ToString(dr["PhysicallFolderName"]);
                        folderModel.ParentId = Convert.ToInt32(dr["ParentId"]);
                        folderModel.IsVisible = Convert.ToBoolean(dr["IsVisible"]);
                        folderModel.CreatedBy = (dr["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["CreatedBy"]);
                        folderModel.CreatedOn = (dr["CreatedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dr["CreatedOn"].ToString());
                        folderModel.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        folderModel.ModifiedOn = (dr["Modifiedon"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dr["Modifiedon"].ToString());
                        folderModels.Add(folderModel);
                    }
                    this.dbManager.CloseReader();
                    return folderModels;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion

        #region Create Folder
        public int CreateFolder(string Code, FolderModel folderModel, string connectionString)
        {
            try
            {
                int returnFolderId = 0;
                Guid guid = Guid.NewGuid();
                string FolderCodeguid = guid.ToString("N").Substring(0, 8);

                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    //dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    int folderId = 0;
                    if (folderModel.ParentId > 0)
                    {
                        folderId = folderModel.ParentId;
                    }
                    else if (!string.IsNullOrEmpty(Convert.ToString(folderModel.ParentFolderCode)))
                    {
                        folderId = accessHelper.GetFolderId(dbManager, folderModel.ParentFolderCode);
                    }

                    string addFolderquery = accessHelper.GetQueryDetail("AddFolder");
                    dbManager.CreateParameters(7);
                    if (!string.IsNullOrEmpty(Convert.ToString(folderModel.FolderCode)))
                    {
                        dbManager.AddParameters(0, "@FolderCode", folderModel.FolderCode.ToUpper());
                        dbManager.AddParameters(1, "@IsVisible", false);
                    }
                    else
                    {
                        dbManager.AddParameters(0, "@FolderCode", FolderCodeguid.ToUpper());
                        dbManager.AddParameters(1, "@IsVisible", true);
                    }

                    dbManager.AddParameters(2, "@FolderName", folderModel.FolderName.Trim());
                    string physicalFolderName = Convert.ToString(guid).ToUpper();
                    dbManager.AddParameters(3, "@PhysicallFolderName", physicalFolderName);



                    dbManager.AddParameters(4, "@ParentId", folderId);
                    dbManager.AddParameters(5, "@CreatedBy", folderModel.CreatedBy);
                    dbManager.AddParameters(6, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    object returnId = dbManager.ExecuteScalar(CommandType.Text, addFolderquery);
                    if (Convert.ToInt32(returnId) > 0)
                    {
                        string virtualPath = accessHelper.VirtualPath;
                        //string virtualPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString(); //done
                        string companyTenantPath = virtualPath + "/" + Code;
                        if (!Directory.Exists(companyTenantPath))
                        {
                            Directory.CreateDirectory(companyTenantPath);
                        }
                        string folderPath = companyTenantPath + "/" + physicalFolderName;
                        System.IO.Directory.CreateDirectory(folderPath);
                        returnFolderId = Convert.ToInt32(returnId);
                    }
                    //dbManager.Transaction.Commit();
                    return returnFolderId;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion

        #region Modify Folder
        public bool ModifyFolder(string Code, int Id, string FolderName, int ModifiedBy, string connectionString)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    string modifyFolderQuery = accessHelper.GetQueryDetail("ModifyFolder");
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@Id", Id);
                    dbManager.AddParameters(1, "@FolderName", FolderName);
                    dbManager.AddParameters(2, "@ModifiedBy", ModifiedBy);
                    dbManager.AddParameters(3, "@Modifiedon", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, modifyFolderQuery);
                    if (returnId > 0)
                    {
                        status = true;
                    }
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
