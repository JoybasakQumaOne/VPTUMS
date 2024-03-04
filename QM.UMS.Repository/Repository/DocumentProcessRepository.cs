namespace QM.UMS.Repository.Repository
{
    using CommonApplicationFramework.Caching;
    using CommonApplicationFramework.Common;
    #region Namespace
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using Newtonsoft.Json;
    using QM.eBook.DMS.DMSFileCreator;
    using QM.eBook.DMS.DMSService;
    using QM.eBook.DMS.Module;
    using QM.UMS.DMS.Module;
    using QM.UMS.Model;
    using QM.UMS.Models;
    using QM.UMS.Repository.Helper;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    #endregion

    public class DocumentProcessRepository : RequestHeader,IDisposable, IDocumentProcessRepository
    {
        #region Variable
        private DBManager dbManager;
		private readonly string logFilePath = Environments.Configurations.Settings.Find(z => z.Key.ToString().Equals("LogFilePath")).Value.ToString();

		Dictionary<string, string> documentTypeDictionary = new Dictionary<string, string>();
        #endregion

        #region DMS Method Call
        //     public int UploadImage(string code, DocumentModel documentModel, string connectionString)
        //     {
        //         string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString()+'/'+ code;
        //         string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;

        //int documentId = 0;
        //         using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
        //         {
        //             documentId = DMSHelper.DocumentUpload(documentModel, connectionString);
        //         }

        public int UploadImage(string code, DocumentModel documentModel)
        {
            int docId = 0;
            using (dbManager= new DBManager())
            {
                docId=UploadImage(ConfigurationManager.AppSettings["ProjectName"], documentModel, dbManager.GetControlDBConnectionString());
            }
            return docId;
        }


        private int UploadImage(string code, DocumentModel documentModel, string connectionString)
		{
			string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
			string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
			//int resizeByPercentage = ConvertData.ToInt(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResizeByPercentage")).Value);
			string resizeOrientation = string.Empty;
			int documentId = 0;
			using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
			{
                try
                {
                    documentModel.FolderPath = folderPath;
                    documentModel.BrowserPath = browserPath;
                    documentId = DMSHelper.DocumentUpload(documentModel, connectionString);
                }
                catch (Exception ex)
                {
                    LogManager.Log(ex);
                }
			}
			
			return documentId;
		}
        public string UploadDocument(string code, DocumentModel documentModel, string connectionString)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            //int resizeByPercentage = ConvertData.ToInt(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResizeByPercentage")).Value);
            string resizeOrientation = string.Empty;
            string documentId = string.Empty;
            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                documentModel.FolderPath = folderPath;
                documentModel.BrowserPath = browserPath;
                documentId = DMSHelper.UploadDocument(documentModel, connectionString); //, resizeOrientation, width,resizeByPercentage);
            }
            return documentId;
        }
        public int InsertDocumentFromFile(string filePath, int mappingId, string fileName, string documentTypeCode, string objectName, string folderCode,string connection)
        {
            try
            {
                UserContext userContext = JsonConvert.DeserializeObject<UserContext>(GlobalCacheManager.Instance.Get("usercontext_" + this.RequestId));
                DocumentModel document = new DocumentModel();

                if (File.Exists(filePath))
                {
                    document.FileName = fileName;
                    document.FilePath = string.Empty;
                    document.DocumentTypeCode = documentTypeCode;
                    document.CreatedBy = ConvertData.ToInt(userContext.UserId);
                    document.IsOverride = false;
                    document.FolderCode = folderCode;
                    document.IsNewVersion = false;
                    document.IsNewEntry = true;
                    document.FolderCode = folderCode;
                    return InsertDocument(document, connection);
                }
                else
                {
                    LogManager.Log("Upload no exists:" + filePath);
                    throw new FileNotFoundException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int InsertDocument(DocumentModel documentUpload, string connection)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
            //int resizeByPercentage = ConvertData.ToInt(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResizeByPercentage")).Value);
            string resizeOrientation = string.Empty;
            int documentId = 0;
            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                documentUpload.FolderPath = folderPath;
                documentUpload.BrowserPath = browserPath;
                documentId = DMSHelper.InsertDocument(documentUpload,documentUpload.FileName, connection); //, resizeOrientation, width,resizeByPercentage);
            }
            return documentId;
        }
        public int CopyImage(int documetId, string folderCode, string connectionString)
		{
			string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
			string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
			////int resizeByPercentage = ConvertData.ToInt(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResizeByPercentage")).Value);
			//string resizeOrientation = string.Empty;
			int documentId = 0;
			using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
			{
				documentId = DMSHelper.CopyDocument(documetId, folderCode, connectionString); //, resizeOrientation, width,resizeByPercentage);
				 
			}

			return documentId;
		}

		public bool DeleteImage(DocumentModel documentModel, string connectionString,string code)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            //int resizeByPercentage = ConvertData.ToInt(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResizeByPercentage")).Value);
            string resizeOrientation = string.Empty;
            bool documentId = false;
            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                //if (width != 0)
                //	resizeOrientation = "Width";
                documentModel.FolderPath = folderPath;
                documentModel.BrowserPath = browserPath;
                documentId = DMSHelper.DeleteImage(documentModel, connectionString); //, resizeOrientation, width,resizeByPercentage);
            }

            return documentId;
        }

		public bool DeleteImage(int docId, string connectionString)
		{
			bool result = false;

			if (docId > 0)
			{
				string code = ConfigurationManager.AppSettings["ProjectName"].ToString();
				string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
				string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
				//int resizeByPercentage = ConvertData.ToInt(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ResizeByPercentage")).Value);
				string resizeOrientation = string.Empty;

				using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
				{
					result = DMSHelper.DeleteDocument(docId, connectionString); //, resizeOrientation, width,resizeByPercentage);
				}
			}
			return result;
		}

		public bool DeleteDocument(int objectId, string objectType, string documentType, bool IsDefault)
		{
			bool result = false;
			string code = ConfigurationManager.AppSettings["ProjectName"].ToString();
			string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
			string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
			string connectionString = string.Empty;

			using (dbManager = new DBManager())
			{
				connectionString = dbManager.GetControlDBConnectionString();
			}
			List<DocumentFileMap> docs = GetDocumentId(objectId, documentType, IsDefault);

            if (docs!=null && docs.Count>0)
            {
                foreach (DocumentFileMap doc in docs)
                {
                    if (doc.documentId.Id > 0)
                    {

                        using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
                        {
                            result = DMSHelper.DeleteDocument(doc.documentId.Id, connectionString); //, resizeOrientation, width,resizeByPercentage);
                        }
                    }
                }

                result = DeleteDocumentMap(objectId, objectType, documentType, IsDefault); 
            }
            else
            {
                result = true;
            }
			return result;
		}

		public bool DeleteDocument(string DocumentId,string connectionString)
		{
			bool result = false;
			string code = ConfigurationManager.AppSettings["ProjectName"].ToString();
			string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
			string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;

			using (dbManager = new DBManager())
			{
				connectionString = connectionString;
			}
			List<DocumentModel> docs = GetListDocId(code, new List<ItemCodeValue>() { new ItemCodeValue { Code = DocumentId } }, null, connectionString);

			foreach (DocumentModel doc in docs)
			{
				if (doc.Id > 0)
				{
					using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
					{
						result = DMSHelper.DeleteDocument(doc.Id, connectionString); //, resizeOrientation, width,resizeByPercentage);
					}
				}
			}
			return result;
		}

        public bool DeleteImage(string DocumentId, string connectionString)
        {
            bool result = false;
            string code = ConfigurationManager.AppSettings["ProjectName"].ToString();
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;

            using (dbManager = new DBManager())
            {
                connectionString = connectionString;
            }
            List<DocumentModel> docs = GetListDocId(code, new List<ItemCodeValue>() { new ItemCodeValue { Code = DocumentId } }, null, connectionString);

            foreach (DocumentModel doc in docs)
            {
                if (doc.Id > 0)
                {
                    using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
                    {
                        result = DMSHelper.DeleteDocument(doc.Id, connectionString); //, resizeOrientation, width,resizeByPercentage);
                    }
                }
            }
            return result;
        }

		public bool DeleteFolder(string folderCode, string connectionString)
		{
			bool result = false;
			string code = ConfigurationManager.AppSettings["ProjectName"].ToString();
			string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
			string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
			 			 
			using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
			{
				result = DMSHelper.DeleteFolder(folderCode, connectionString);  
			}				
			
			return result;
		}

		private bool DeleteDocumentMap(int documentId)
		{
			bool result = false;
			using (dbManager = new DBManager())
			{
				dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
				dbManager.Open();

				string query = QueryConfig.DocumentQuerySettings["DeleteDocumentMapById"].ToString();
				dbManager.CreateParameters(1);
				dbManager.AddParameters(0, "@documentId", documentId);
				int count = dbManager.ExecuteNonQuery(CommandType.Text, query);
				if (count > 0)
					result = true;
			}

			return result;
		}

		private bool DeleteDocumentMap(int objectId, string objectType, string documentType, bool isDefault)
		{
			bool result = false;
			using (dbManager = new DBManager())
			{
				dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
				dbManager.Open();								 
				
				string	query = QueryConfig.DocumentQuerySettings["DeleteDocumentMap"].ToString();
				dbManager.CreateParameters(4);
				dbManager.AddParameters(0, "@ObjectId", objectId);
				dbManager.AddParameters(1, "@ObjectType", objectType);
				dbManager.AddParameters(2, "@DocumentType", documentType);
				dbManager.AddParameters(3, "@IsDefault", isDefault);
				
				int count = dbManager.ExecuteNonQuery(CommandType.Text, query);
				if (count > 0)
					result = true;
			}

			return result;
		}

		//public int UploadImage(string code, DMSDocumentServiceModel documentModel, string connectionString,string connectionType)
		//      {
		//          documentModel.FolderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
		//          documentModel.BrowserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
		//          int documentId = 0;
		//          //using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
		//          //{
		//          //    documentId = DMSHelper.DocumentUpload(documentModel, connectionString);
		//          //}
		//          HttpRequestMessage request = null;
		//          using (HttpClient con_httpClient = new HttpClient(new HttpClientHandler()))
		//          {
		//              LogManager.Log(JsonConvert.SerializeObject(documentModel));
		//              //string[] authorization = System.Web.HttpContext.Current.Request.Headers["authorization"].Split(' ');
		//              //con_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorization[0], authorization[1]);
		//              con_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		//              //con_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		//              con_httpClient.DefaultRequestHeaders.Add("BookGuid", connectionString);
		//              //con_httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
		//              request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("UploadDocumentURL")).Value.ToString()+ connectionType));
		//              //request = new HttpRequestMessage(new HttpMethod(HttpVerb.POST.ToString()), new Uri("http://localhost:49648/api/Request/AddProcessRequestFromExternalAPI"));
		//              request.Content = new StringContent(JsonConvert.SerializeObject(documentModel));
		//              request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
		//              var response = con_httpClient.SendAsync(request).Result;
		//              if (response.IsSuccessStatusCode)
		//              {
		//                  LogManager.Log(response.Content.ReadAsStringAsync().Result);
		//                  int responseData = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
		//                  LogManager.Log(responseData.ToString());
		//                  if (responseData > 0)
		//                  {
		//                      return responseData;
		//                  }
		//                  else
		//                  {
		//                      return 0;
		//                  }
		//              }
		//              else
		//              {
		//                  return 0;
		//              }
		//          }
		//          return documentId;
		//      }
		public List<DocumentModel> GetImageDocuments(string code, List<ItemId> documentId, string connectionString,string imageType=null)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            List<DocumentModel> documentModels = new List<DocumentModel>();
			try
			{
				using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
				{
                    if(string.IsNullOrEmpty(imageType))
                    {
                        documentModels = DMSHelper.GetDocumentList(documentId, connectionString);
                    }
                    else
                    {
                        documentModels = DMSHelper.GetDocumentListByImageType(imageType,documentId, connectionString);
                    }
                }
			}
			catch (Exception ex) { return null; }
            return documentModels;
        }
        public DocumentModel GetDocumentByDocumentguid(string documentguid, string connectinString, bool isLocalpathRequired = false)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
            DocumentModel document = null;
            try
            {
                using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
                {
                    if (!string.IsNullOrEmpty(documentguid))
                    {
                        document= DMSHelper.GetDocumentByDocumentguid(documentguid,connectinString,true);
                    }
                }
            }
            catch (Exception ex) { return null; }
            return document;
        }
        public DocumentModel GetDocumentByObjectId(int objectId, string ObjectName, string documentType,string connectionString)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString();
            DocumentModel document = null;
            try
            {
                using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
                {
                     document = DMSHelper.GetDocumentByObjectId(objectId,ObjectName,documentType,connectionString);
                }
            }
            catch (Exception ex) { return null; }
            return document;
        }
        
        public List<DocumentViewModel> GetImagesByType(int objectId, string imageType, string documentType, int index, int range, string objectType)
		{
			List<DocumentViewModel> images = null;
			try
			{
				List<dynamic> docMappings = GetDocumentMappings(objectId, documentType,index,range, objectType);
                List<ItemId> documentList = docMappings.Select(x => x.documentId).OfType<ItemId>().ToList();
				if (documentList.Count > 0)
				{
                    images = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], documentList, dbManager.GetControlDBConnectionString()
                        , imageType).Select(n=>new DocumentViewModel() {
                            DocumentId = n.DocumentGuid,
                            FileName = n.FileName,
                            FilePath = n.FilePath,
                            ObjectId = objectId
                        }).ToList();
				}
				return images;
			}
			catch (SqlException sqlEx)
			{
				LogManager.Log(sqlEx);
				throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
			}
			catch (Exception ex)
			{
				LogManager.Log(ex);
				throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
			}
		}
        public List<DocumentViewModel> GetImagesByDocumentType(string documentType)
        {
            List<DocumentViewModel> images = new List<DocumentViewModel>();
            try
            {
                List<ItemId> documentList = new List<ItemId>();
                List<DocumentModel> documentModels = new List<DocumentModel>();
                if (documentList.Count > 0)
                {

                    documentModels = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], documentList, dbManager.GetControlDBConnectionString());

                    foreach (DocumentModel ImageDocModel in documentModels)
                    {
                        images.Add(new DocumentViewModel()
                        {
                            DocumentId = ImageDocModel.DocumentGuid,
                            FileName = ImageDocModel.FileName,
                            FilePath = ImageDocModel.FilePath,
                            IsDefault = false
                        });
                    }
                }
                return images;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }

        public List<DocumentModel> GetDocumentsOnDocumentType(string documentType)
        {
            try
            {
                string connectionString = string.Empty;
                using (dbManager=new DBManager())
                {
                    connectionString = dbManager.GetControlDBConnectionString();
                }
                string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"];
                string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"];
                List<DocumentModel> documentModels = new List<DocumentModel>();
                using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
                {
                    documentModels = DMSHelper.GetDocumentByDocumentType(documentType, connectionString);
                }
                return documentModels;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }


        private List<dynamic> GetDocumentMappings(int objectId, string documentType, int index, int range,string ObjectType)
		{
			List<dynamic> docMappings = new List<dynamic>();

			using (dbManager = new DBManager())
			{
				dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
				dbManager.Open();
				string query = QueryConfig.ControlMasterQuerySettings["GetDocumentIdOutDef"].ToString();
				 
				dbManager.CreateParameters(5);
				dbManager.AddParameters(0, "@ObjectId", objectId);
				dbManager.AddParameters(1, "@DocumentType", documentType);
				dbManager.AddParameters(2, "@index", index);
				dbManager.AddParameters(3, "@count", range);
				dbManager.AddParameters(4, "@ObjectType", ObjectType);

                IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
				dynamic docMap = null;
				while (dr.Read())
				{
					docMap = new ExpandoObject();

					docMap.objectId = ConvertData.ToInt(dr["ObjectId"]);
					docMap.documentId = new ItemId() { Id = ConvertData.ToInt(dr["DocumentId"]) };
					docMap.IsDefault = ConvertData.ToBoolean(dr["IsDefault"]);
                    docMap.DocumentGuid = ConvertData.ToString(dr["DocumentGuid"]);

                    docMappings.Add(docMap);
				}
				dr.Close();

			}

				return docMappings;

		}

		public List<DocumentModel> GetDocByDocIdList(string code, List<ItemId> documentId, string imageType, string connectionString)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            List<DocumentModel> documentModels = new List<DocumentModel>();

            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                documentModels = DMSHelper.GetDocumentListByImageType(imageType, documentId, connectionString);
            }
            return documentModels;
        }
        public List<DocumentModel> GetListDocId(string code, List<ItemCodeValue> documentId, string imageType, string connectionString)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            List<DocumentModel> documentModels = new List<DocumentModel>();

            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                documentModels = DMSHelper.GetDocumentListByImageTypeByDocGuid(imageType, documentId, connectionString);
            }
            return documentModels;
        }

        public List<DocumentModel> GetDocumentListByImageTypeList(string code, string documentId, List<ItemType> imageType, string connectionstring)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            List<DocumentModel> documentModels = new List<DocumentModel>();
            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                documentModels = DMSHelper.GetDocumentListByImageTypeList(documentId, imageType, connectionstring);
            }
            return documentModels;
        }

        public List<DocumentModel> GetDocumentWithImageDetails(string code, List<ItemId> documentId, List<ItemType> imageType, string connectionstring)
        {
            string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + code;
            string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + code;
            List<DocumentModel> documentModels = new List<DocumentModel>();
            using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
            {
                documentModels = DMSHelper.GetDocumentListByImageTypeListAndDocumentList(documentId, imageType, connectionstring);
            }
            return documentModels;
        }
        #endregion

        #region DMS Model Conversion

        public DocumentModel BuildDocumentModel(dynamic ImageDocModel)
        {
            DocumentModel docModel = new DocumentModel()
            {
                Id = ImageDocModel.Id,
                DocumentTypeCode = ImageDocModel.DocumentTypeCode,
                CreatedBy = ImageDocModel.CreatedBy,
                FileFormat = ImageDocModel.FileFormat,
                FileName = ImageDocModel.FileName,
                FilePath = ImageDocModel.FilePath,
                File = ImageDocModel.File,
                IsOverride = ImageDocModel.IsOverride,
                IsNewEntry = ImageDocModel.IsNewEntry,
                IsNewVersion = ImageDocModel.IsNewVersion,
                FolderCode = ImageDocModel.FolderCode,
                DocumentGuid= ImageDocModel.DocumentGuid
            };
            return docModel;
        }
        public DocumentModel BuildDocumentModel(dynamic ImageDocModel,DocumentModel document)
        {
            DocumentModel docModel = new DocumentModel()
            {
                Id = ImageDocModel.Id,
                DocumentTypeCode = ImageDocModel.DocumentTypeCode,
                CreatedBy = ImageDocModel.CreatedBy,
                FileFormat = ImageDocModel.FileFormat,
                FileName = ImageDocModel.FileName,
                FilePath = ImageDocModel.FilePath,
                File =FileToBase64( document.FilePath),
                IsOverride = ImageDocModel.IsOverride,
                IsNewEntry = ImageDocModel.IsNewEntry,
                IsNewVersion = ImageDocModel.IsNewVersion,
                FolderCode = ImageDocModel.FolderCode,
            };
            return docModel;
        }
        private string FileToBase64(string path)
        {
            Byte[] bytes = File.ReadAllBytes(path);
            return Convert.ToBase64String(bytes);
        }
        public DMSDocumentServiceModel GenerateDocumentModel(dynamic ImageDocModel)
        {
            DMSDocumentServiceModel docModel = new DMSDocumentServiceModel()
            {
                Id = ImageDocModel.Id,
                DocumentTypeCode = ImageDocModel.DocumentTypeCode,
                CreatedBy = ImageDocModel.CreatedBy,
                FileFormat = ImageDocModel.FileFormat,
                FileName = ImageDocModel.FileName,
                FilePath = ImageDocModel.FilePath,
                File = ImageDocModel.File,
                IsOverride = ImageDocModel.IsOverride,
                IsNewEntry = ImageDocModel.IsNewEntry,
                IsNewVersion = ImageDocModel.IsNewVersion,
                FolderCode = ImageDocModel.FolderCode,
            };
            return docModel;
        }
        public ImageDocumentUpload BuildImageUploadModel(DocumentModel documentModel)
        {
            ImageDocumentUpload imageDocUpload = new ImageDocumentUpload()
            {
                Id = documentModel.Id,
                DocumentTypeCode = documentModel.DocumentTypeCode,
                CreatedBy = documentModel.CreatedBy,
                FileFormat = documentModel.FileFormat,
                File = documentModel.File,
                FilePath = documentModel.FilePath,
                FileName = documentModel.FileName,
                IsOverride = documentModel.IsOverride,
                IsNewEntry = documentModel.IsNewEntry,
                IsNewVersion = documentModel.IsNewVersion,
                FolderCode = documentModel.FolderCode
            };
            return imageDocUpload;
        }

        #endregion        

        #region Update Image And Map Content
        public int ImageProcess(ImageDocumentUpload imageDocumentUpload)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    DocumentModel documentModel = this.BuildDocumentModel(imageDocumentUpload);
                    int contentUpdate = 0;
                    int documentId = UploadImage(ConfigurationManager.AppSettings["ProjectName"], documentModel, dbManager.GetControlDBConnectionString());
                    if (documentId > 0)
                    {
                        contentUpdate = UpdateDocumentMapping(MappingModelBuild(documentId, imageDocumentUpload.ObjectId, documentModel.DocumentTypeCode, imageDocumentUpload.IsDefault, imageDocumentUpload.ObjectType));
                    }
                    return documentId;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("INSERTIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("INSERTIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }
        #endregion

        #region Document Mapping
        public DocumentMapping MappingModelBuild(int documentId, int objectId, string documentTypeCode, bool isDefault, string objectType)
        {
            DocumentMapping documentMapping = new DocumentMapping()
            {
                DocumentId = Convert.ToInt32(documentId),
                DocumentType = Convert.ToString(documentTypeCode),
                ObjectType = (string.IsNullOrEmpty(objectType)) ? Convert.ToString(documentTypeMapping(documentTypeCode)) : objectType,
                ObjectId = Convert.ToInt32(objectId),
                IsDefault = Convert.ToBoolean(isDefault)
            };
            return documentMapping;
        }

        public int UpdateDocumentMapping(DocumentMapping documentMapping)
        {
            using (dbManager = new DBManager())
            {
                dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                dbManager.Open();

                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@DocumentId", documentMapping.DocumentId);
                dbManager.AddParameters(1, "@DocumentType", documentMapping.DocumentType);
                dbManager.AddParameters(2, "@ObjectId", documentMapping.ObjectId);
                dbManager.AddParameters(3, "@ObjectType", documentMapping.ObjectType);
                dbManager.AddParameters(4, "@IsDefault", documentMapping.IsDefault);
                string queryDocumentMap = QueryConfig.BookContentDataQuerySettings["AddDocumentMapping"].ToString();
                int returnValue = dbManager.ExecuteNonQuery(CommandType.Text, queryDocumentMap);
                return returnValue;
            }
        }
        #endregion

        #region Get Document Id List
        public List<DocumentFileMap> GetDocumentId(int objectId, string documentType, bool? isDefault)
        {
            List<ItemId> documentId = new List<ItemId>();
            List<DocumentFileMap> documentFileMap = new List<DocumentFileMap>();
            using (dbManager = new DBManager())
            {
                dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                //dbManager.GetControlDBConnectionString();
                dbManager.Open();
                string queryGetDocumentId = string.Empty;

                if (isDefault == null)
                {
                    queryGetDocumentId = QueryConfig.ControlMasterQuerySettings["GetDocumentIdOutDef"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@ObjectId", objectId);
                    dbManager.AddParameters(1, "@DocumentType", documentType);
                }
                else
                {
                    queryGetDocumentId = QueryConfig.ControlMasterQuerySettings["GetDocumentIdWithDef"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@ObjectId", objectId);
                    dbManager.AddParameters(1, "@DocumentType", documentType);
                    dbManager.AddParameters(2, "@IsDefault", isDefault);
                }
                IDataReader idr = dbManager.ExecuteReader(CommandType.Text, queryGetDocumentId);

                while (idr.Read())
                {
                    documentFileMap.Add(new DocumentFileMap()
                    {
                        objectId = Convert.ToInt32(idr["ObjectId"]),
                        documentId = (new CommonApplicationFramework.Common.DMS.ItemId() { Id = Convert.ToInt32(idr["DocumentId"]) }),
                        filePath = Convert.ToString(null),
                    });
                }
                idr.Close();

                return documentFileMap;
            }
        }
        #endregion

        public string UploadBookBackgroudImage(ImageDocumentUpload image)
        {
            using (dbManager=new DBManager())
            {
                DocumentModel document = BuildDocumentModel(image);
                string docId = UploadDocument(ConfigurationManager.AppSettings["ProjectName"].ToString(), document, dbManager.GetControlDBConnectionString());
                return docId; 
            }
        }
        //public List<DocumentGallery> GetImageGalleryDetails(int objectId, string documentType)
        //{
        //    try
        //    {
        //        #region Getting Image types
        //        string imageTypes = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("GalleryImageTypes")).Value.ToString();
        //        List<ItemType> itemTypes = new List<ItemType>();
        //        List<string> imageStringList = imageTypes.Split(',').ToList();
        //        foreach(string imageString in imageStringList)
        //        {
        //            itemTypes.Add(new ItemType()
        //            {
        //                Type = imageString,
        //            });
        //        }
        //        #endregion

        //        #region Get Document Id
        //        List<ItemId> documentId = GetDocumentId(objectId, documentType, null).Select(x => x.documentId).ToList();
        //        #endregion

        //        using (dbManager = new DBManager())
        //            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();

        //        List<DocumentModel> documentModels = GetDocumentWithImageDetails(ConfigurationManager.AppSettings["ProjectName"], documentId, itemTypes, dbManager.ConnectionString);

        //        List<DocumentGallery> documentGalleries = new List<DocumentGallery>();
        //        List<DocumentImageObject> documentImageObjects = new List<DocumentImageObject>();
        //        foreach (DocumentModel documentModel in documentModels)
        //        {
        //            documentImageObjects = new List<DocumentImageObject>();
        //            foreach (DMS.Module.ImageDocument imageDocument in documentModel.imageDocumentList)
        //            {
        //                documentImageObjects.Add(new DocumentImageObject()
        //                    {
        //                        FilePath = imageDocument.FilePath,
        //                        FileType = imageDocument.ImageTypeName,
        //                        Id = imageDocument.ImageTypeId,
        //                    });
        //            }

        //            documentGalleries.Add(new DocumentGallery()
        //            {
        //                DocumentId = documentModel.Id,
        //                FileName = documentModel.FileName,
        //                FilePath = documentModel.FilePath,
        //                FileType = "raw",
        //                ImageObjects = documentImageObjects,
        //            });
        //        }
        //        return documentGalleries;   
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        LogManager.Log(sqlEx);
        //        throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Log(ex);
        //        throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
        //    }
        //}


        public dynamic GetImageGalleryDetails(int objectId, string documentType)
        {
            try
            {
                #region Get Document Id
                List<ItemId> documentId = GetDocumentId(objectId, documentType, null).Select(x => new ItemId() { Id = x.documentId.Id }).ToList();
                #endregion
                string folderPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"];
                string browserPath = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"];
                using (dbManager = new DBManager())
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                using (DMSHelper DMSHelper = new DMSHelper(folderPath, browserPath, logFilePath))
                {
                  return  DMSHelper.GetDocumentListByImageType(ImageTypeEnum.THUMBNAIL.ToString(), documentId, dbManager.ConnectionString);
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }

        public List<DocumentFileMap> GetDocumentByIdList(int objectId, string imageType, string DocumentType, bool? isDefault)
        {
            try
            {
                List<ItemId> documentList = new List<ItemId>();
                List<DocumentFileMap> documentFileMap = new List<DocumentFileMap>();
                List<DocumentModel> documentModels = new List<DocumentModel>();

                documentFileMap = this.GetDocumentId(objectId, DocumentType, isDefault);
                documentList = documentFileMap.Select(x => new ItemId() { Id = x.documentId.Id }).ToList();

                if (documentList.Count > 0)
                {
                    documentModels = GetDocByDocIdList(ConfigurationManager.AppSettings["ProjectName"], documentList, imageType, dbManager.GetControlDBConnectionString());
                }

                foreach (DocumentFileMap documentFile in documentFileMap)
                {
                    documentFile.filePath = documentModels.Where(x => x.Id == Convert.ToInt32(documentFile.documentId.Id)).Select(x => x.FilePath).FirstOrDefault();
                }
                return documentFileMap;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }
        
        public List<ImageDocumentUpload> GetDocumentListImages(int objectId, string imageType, string documentType, bool? isDefault)
        {
            try
            {
                List<ItemId> documentList = new List<ItemId>();
                List<ImageDocumentUpload> imgDocUpload = new List<ImageDocumentUpload>();
                List<DocumentModel> documentModels = new List<DocumentModel>();
                List<DocumentFileMap> documentFileMap = this.GetDocumentId(objectId, documentType, isDefault);
                documentList = documentFileMap.Select(x => new ItemId() { Id=x.documentId.Id }).ToList();
                if (documentList.Count > 0)
                {
                    if (string.IsNullOrEmpty(imageType))
                    {
                        documentModels = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], documentList, dbManager.GetControlDBConnectionString());
                    }
                    else
                    {
                        List<ItemType> itemType = new List<ItemType>();
                        itemType.Add(new ItemType() { Type = imageType });
                        documentModels = GetDocumentWithImageDetails(ConfigurationManager.AppSettings["ProjectName"], documentList, itemType, dbManager.GetControlDBConnectionString());
                    }
                    foreach (DocumentModel docModel in documentModels)
                    {
                        imgDocUpload.Add(BuildImageUploadModel(docModel));
                    }
                }
                return imgDocUpload;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }

        public List<DocumentFileMap> GetDocumentFileMaps(int objectId, string imageType, string documentType, bool? isDefault)
        {
            try
            {
                List<ItemId> documentList = new List<ItemId>();
                List<DocumentFileMap> documentFileMap = this.GetDocumentId(objectId, documentType, isDefault);
                documentList = documentFileMap.Select(x => new ItemId() { Id = x.documentId.Id }).ToList();
                List<DocumentModel> documentModels = new List<DocumentModel>();

                if (documentList.Count > 0)
                {
                    if (string.IsNullOrEmpty(imageType))
                    {
                        using (dbManager = new DBManager())
                        {
                            documentModels = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], documentList, dbManager.GetControlDBConnectionString());
                        }
                    }
                    else
                    {
                        string[] imageTypeArr = imageType.Split(',');

                        if (documentList.Count == 1)
                        {
                            List<ItemType> itemType = new List<ItemType>();
                            itemType.Add(new ItemType() { Type = imageType });
                            documentModels = GetDocumentListByImageTypeList(ConfigurationManager.AppSettings["ProjectName"], "", itemType, dbManager.GetControlDBConnectionString());
                        }
                        else
                        {
                            List<ItemType> itemType = new List<ItemType>();
                            itemType.Add(new ItemType() { Type = imageType });
                            documentModels = GetDocumentWithImageDetails(ConfigurationManager.AppSettings["ProjectName"], documentList, itemType, dbManager.GetControlDBConnectionString());
                        }
                    }
                    foreach (DocumentFileMap docFileMap in documentFileMap)
                    {
                        docFileMap.filePath = documentModels.Where(x => x.Id == Convert.ToInt32(docFileMap.documentId.Id)).Select(x => x.FilePath).FirstOrDefault();
                    }
                }
                return documentFileMap;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("GETIMAGEDETAILFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("GETIMAGEDETAILFAILED", ex.Message, string.Empty);
            }
        }

		public string GetFilePath(int objectId, string imageType, string documentType, bool? isDefault, string connectionString)
		{

			List<ItemId> documentList = new List<ItemId>();
			List<DocumentFileMap> documentFileMap = this.GetDocumentId(objectId, documentType, isDefault);
			documentList = documentFileMap.Select(x => new ItemId() { Id = x.documentId.Id }).ToList();
			
			string filePath = string.Empty;
			if (string.IsNullOrEmpty(connectionString))
			{
				using (dbManager = new DBManager())
				{
					connectionString = dbManager.GetControlDBConnectionString();
				}
			}

			if (documentList.Count > 0)
			{
				List<DocumentModel> images = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], documentList, connectionString);
				filePath = (images != null && images.Count > 0) ? images[0].FilePath : "";
			}
			return filePath;
		}

		public string GetFilePath(int docId, string connectionString)
		{
			string filePath = string.Empty;
			if (docId > 0)
			{
				List<DocumentModel> images = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], new List<ItemId> { new ItemId { Id = docId } }, connectionString);
				filePath = (images != null && images.Count > 0) ? images[0].FilePath : "";
			}
			return filePath;
		}
        public string GetFilePath(string docId, string connectionString)
        {
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(docId))
            {
                List<DocumentModel> images = GetListDocId(ConfigurationManager.AppSettings["ProjectName"], new List<ItemCodeValue> { new ItemCodeValue { Code=docId} }, null,connectionString);
                //List<DocumentModel> images = GetImageDocuments(ConfigurationManager.AppSettings["ProjectName"], new List<ItemId> { new ItemId { Id = docId } }, connectionString);
                filePath = (images != null && images.Count > 0) ? images[0].FilePath : "";
            }
            return filePath;
        }

        #region Setting Utilities
        public int UploadSettingsImage(ImageDocumentUpload imageUploadDocument, Guid bookGuid, string documentType, string connectionString)
		{
			try
			{
				int docId = 0;
				using (dbManager = new DBManager())
				{
					DocumentModel documentModel = BuildDocumentModel(imageUploadDocument);

					if (imageUploadDocument.IsCopyRequired)
					{
						DocumentModel controlUploadModel = documentModel;
						if (imageUploadDocument.IsOverride)
						{
							controlUploadModel.Id = 0;
							controlUploadModel.IsNewEntry = true;
							controlUploadModel.IsOverride = false;
						}

						int gallaryImageId = UploadImage(ConfigurationManager.AppSettings["ProjectName"].ToString(), controlUploadModel, dbManager.GetControlDBConnectionString());
					}

					dbManager.ConnectionString = connectionString;
					dbManager.Open();

					documentModel = BuildDocumentModel(imageUploadDocument);
					documentModel.FolderCode = bookGuid + "_Book";
					documentModel.DocumentTypeCode = documentType;


					docId = UploadImage(ConfigurationManager.AppSettings["ProjectName"].ToString(), documentModel, dbManager.ConnectionString);
					//if (docId > 0)
					//{
					//	string queryUpdateSettings = QueryConfig.BookQuerySettings["UpdateSettingImage"].ToString();
					//	dbManager.CreateParameters(2);
					//	dbManager.AddParameters(0, "@BookId", imageUploadDocument.ObjectId);
					//	dbManager.AddParameters(1, "@ImageDocId", docId);
					//	int returnValue = dbManager.ExecuteNonQuery(CommandType.Text, queryUpdateSettings);
					//}
				}
				return docId;
			}
			catch (SqlException sqlEx)
			{
				LogManager.Log(sqlEx);
				throw new RepositoryException("UPLOADSETTINGIMAGEFAILED", MessageConfig.MessageSettings["UPLOADSETTINGIMAGEFAILED"].ToString(), string.Empty);
			}
			catch (Exception ex)
			{
				LogManager.Log(ex);
				throw new RepositoryException("UPLOADSETTINGIMAGEFAILED", MessageConfig.MessageSettings["UPLOADSETTINGIMAGEFAILED"].ToString(), string.Empty);
			}
		}
		#endregion
		#region Object Type Mapper
		private Dictionary<string, string> documentObjectTye()
        {
            documentTypeDictionary.Add(DocumentType.ORGANIZATION_BOOK_IMAGE.ToString(), EcommObject.CUSTOMERIMAGE.ToString());
            documentTypeDictionary.Add(DocumentType.CUSTOMER_BOOK_IMAGE.ToString(), EcommObject.CUSTOMERIMAGE.ToString());
            return documentTypeDictionary;
        }

        public string documentTypeMapping(string documentType)
        {
            Dictionary<string, string> documenyObjTypeMap = documentObjectTye();
            return documenyObjTypeMap.Where(x => x.Key.Equals(documentType)).Select(x => x.Value).FirstOrDefault();
        }
        #endregion

        #region Folder Creator
        public int CreateFolder(FolderModel folderModel, string connectionString, string Code)
        {
            int returnFolderId = 0;

			string folderUrl = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString() + '/' + Code;
			using (Folder folder = new Folder(folderUrl))
			{
				returnFolderId = folder.CreateFolder(folderModel, connectionString);
			}
			return returnFolderId;
        }


        #endregion

        #region Test Code
        //public string GetObjectType(string documentType)
        //{
        //    if (documentType == DocumentType.CUSTOMER_BOOK_IMAGE.ToString() || documentType == DocumentType.ORGANIZATION_BOOK_IMAGE.ToString())
        //    {
        //        return EcommObject.CUSTOMERIMAGE.ToString();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        #endregion

        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
