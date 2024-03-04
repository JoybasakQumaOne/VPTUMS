using Newtonsoft.Json;
using QM.eBook.DMS.DBConnection;
using QM.eBook.DMS.LogManager;
using QM.eBook.DMS.Module;
using QM.eBook.DMS.QuerySetting;
using QM.UMS.DMS.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace QM.eBook.DMS.DMSService
{
    public class DMSHelper : IDisposable
    {
        public DMSHelper()
        {
            accessHelper = new AccessHelper();
        }

        public DMSHelper(string displayPath, string browserPath)
        {
            accessHelper = new AccessHelper();
            accessHelper.PhysicalPath = displayPath;
            accessHelper.BrowseURL = browserPath;
        }

        public DMSHelper(string displayPath, string browserPath, string logPath)
        {
            accessHelper = new AccessHelper();
            accessHelper.PhysicalPath = displayPath;
            accessHelper.BrowseURL = browserPath;
            accessHelper.LogFilePath = logPath;
        }

        #region DB Manager
        //private DBManager dbManager;
        private DBhandler dbManager;
        private DBhandler dbManagerImgDoc;
        private AccessHelper accessHelper;
        private BaseLogManager logManager;
        #endregion

        #region Query Variable
        Dictionary<string, string> queryString = new Dictionary<string, string>();
        #endregion

        #region Image Upload
        public int DocumentUpload(DocumentModel documentUpload, string connectionString)
        {
            #region VariableDecalration
            int documentTypeId = 0;
            string docPath = string.Empty;
            Guid guid = Guid.NewGuid();
            int documentId = 0;
            string oldFileName = string.Empty;
            int returndocumentId = 0;
            #endregion
            string DocUploadedFileNameguid = guid.ToString("N").ToUpper();

            using (dbManager = new DBhandler())
            {
                try
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();

                    #region DocumentInsert
                    documentTypeId = GetDocumentTypeId(documentUpload.DocumentTypeCode);

                    string DocumentTypeFileSizeQuery = accessHelper.GetQueryDetail("GetDocumentTypeByDocumentTypeId");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@DocumentTypeId", documentTypeId);
                    IDataReader drFileSize = dbManager.ExecuteReader(CommandType.Text, DocumentTypeFileSizeQuery);
                    DataTable dtFileSize = new DataTable();
                    dtFileSize.Load(drFileSize);
                    drFileSize.Close();

                    bool AcceptSaveDocument = false;
                    //DMS.DocumentType.FileSize: this is using byte.. 
                    if (string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["FileSize"])) || Convert.ToDecimal(dtFileSize.Rows[0]["FileSize"]) == 0 ||
                        (!string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["FileSize"])) && Convert.ToDecimal(dtFileSize.Rows[0]["FileSize"]) * 1024 * 1024 >= Convert.ToDecimal(documentUpload.File.Length)))
                    {
                        AcceptSaveDocument = true;
                        if (!string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MinImageWidth"])) && !string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MinImageHeight"])) && !string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageWidth"])) && !string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageHeight"])))
                        {
                            // Original Image Width and Height
                            int UploadedImageWidth;
                            int UploadedImageHeight;
                            CalculateOriginalImageWidthHeight(documentUpload, out UploadedImageWidth, out UploadedImageHeight);

                            if ((string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MinImageWidth"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MinImageWidth"]) == 0 ||
                                    UploadedImageWidth >= Convert.ToDecimal(dtFileSize.Rows[0]["MinImageWidth"]) * 1024 * 1024 &&
                                string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageWidth"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) == 0 ||
                                    UploadedImageWidth <= Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) * 1024 * 1024
                                )
                                &&
                                (string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageWidth"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) == 0 ||
                                    UploadedImageHeight >= Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) * 1024 * 1024 &&
                                string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageHeight"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageHeight"]) == 0 ||
                                UploadedImageHeight <= Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageHeight"]) * 1024 * 1024
                                ))
                            {
                                AcceptSaveDocument = true;
                            }
                            else
                            {
                                AcceptSaveDocument = false;
                            }
                        }

                    }
                    else
                    {
                        AcceptSaveDocument = false;
                        throw new Exception("File size is bigger than mentioned file size");
                    }
                    if (AcceptSaveDocument)
                    {
                        if (documentUpload.IsOverride == true)
                        {
                            bool isOriginalDocumentExists = IfDocumentPresentInFolder(documentUpload.DocumentGuid, documentUpload.FolderCode);
                            if(!isOriginalDocumentExists)
                            {
                                documentUpload.IsOverride = false;
                                documentUpload.IsNewEntry = true;
                                return DocumentUpload(documentUpload, connectionString);
                            }
                            //gets the original document
                            DocumentModel originalDocument = GetDocumentOnDocumentguid(documentUpload.DocumentGuid, connectionString, true);
                            //gets variant documents
                            List<DocumentModel> childDocument = GetImageTypeDocumentList(documentUpload.DocumentGuid, new List<ItemType>(), connectionString);
                            
                            //Overrides the existing original Image
                            string newOriginalFileName = Guid.NewGuid().ToString().ToUpper().Substring(0, 1) + originalDocument.FileName;
                            docPath = SaveImageInDirectory(documentUpload, newOriginalFileName, originalDocument.FolderId, Path.GetDirectoryName(originalDocument.FilePath));
                            UpdateOriginalFileName(newOriginalFileName, originalDocument.Id);
                            if(isOriginalDocumentExists)
                            DeleteOriginalDocumentFromDirectory(originalDocument.FilePath);
                            originalDocument.FilePath = docPath;

                            if (childDocument != null && childDocument.Count > 0)
                            {
                                List<ImageDocument> imageModel = childDocument.Select(n => new ImageDocument() { Id = n.Id, ImageName = n.FileName, FilePath = n.FilePath, ImageTypeName = n.imageType.ImageTypeName }).ToList();
                                DataTable dtimages = GetImageTypes(documentTypeId);

                                if (dtimages.Rows.Count > 0)
                                {
                                    #region Replaces the child documents
                                    for (int i = 0; i < dtimages.Rows.Count; i++)
                                    {
                                        ImageDocument replaceAbleImage = imageModel.Where(n => n.ImageTypeName.Trim().ToUpper() == Convert.ToString(dtimages.Rows[i]["TypeName"]).Trim().ToUpper()).FirstOrDefault();
                                        if (replaceAbleImage != null)
                                        {
                                            string documentType = Convert.ToString(dtimages.Rows[i]["DocumentType"]);
                                            int imageTypeId = Convert.ToInt32(dtimages.Rows[i]["ImageTypeId"]);
                                            string typeName = Convert.ToString(dtimages.Rows[i]["TypeName"]);
                                            decimal? width = (dtimages.Rows[i]["Width"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dtimages.Rows[i]["Width"]);
                                            decimal? height = (dtimages.Rows[i]["Height"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dtimages.Rows[i]["Height"]);
                                            int ImageWidth;
                                            int ImageHeight;
                                            CalculateOriginalImageWidthHeight(originalDocument, out ImageWidth, out ImageHeight);
                                            int newImageWidth = 0;
                                            int newImageHeight = 0;
                                            CalculateCropImageWidthHeight(width, height, ImageWidth, ImageHeight, ref newImageWidth, ref newImageHeight);
                                            string newDocumentName = Guid.NewGuid().ToString().ToUpper().Substring(0, 1) + replaceAbleImage.ImageName;
                                            string newPath = Path.GetDirectoryName(replaceAbleImage.FilePath)+"/"+ newDocumentName;
                                            ResizeImage(originalDocument.FilePath, newPath, newImageWidth, newImageHeight);
                                            UpdateOriginalFileName(newDocumentName, replaceAbleImage.Id);
                                            if (isOriginalDocumentExists)
                                            DeleteOriginalDocumentFromDirectory(replaceAbleImage.FilePath);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            documentId = originalDocument.Id;
                            returndocumentId = originalDocument.Id;
                        }
                        else if (documentUpload.IsNewEntry == true)
                        {
                            #region New Document
                            Tuple<int, int, string, int, string> tupleDocumentResult = UploadNewDocument(documentUpload, documentTypeId, DocUploadedFileNameguid);
                            if (tupleDocumentResult.Item1 > 0)
                            {
                                documentId = tupleDocumentResult.Item1;
                                int VersionId = tupleDocumentResult.Item2;
                                //  Insert or update or delete in the EAVMetaData table 
                                //bool result = InserEavMetaData(dbManager, documentUpload, documentId); // check EavMetaData Table's data is insert and it will return true then next function will work
                                bool result = true;
                                //  upload file
                                if (result == true)
                                {
                                    string uploadedFilePath = SaveImageInDirectory(documentUpload, tupleDocumentResult.Item3, tupleDocumentResult.Item4);
                                    //string uploadedFilePath = FileUploadInTheFileServer(documentUpload, tupleDocumentResult.Item3, tupleDocumentResult.Item4);
                                    if (File.Exists(uploadedFilePath))
                                    {
                                        // Insert data in the ImageDocument Table, if documentTypeId is present in ImageType table[have to write the new logic]
                                        #region Save Data in the ImageDocument Table
                                        returndocumentId = ImageDocument_Save(documentUpload, documentTypeId, documentId, VersionId, uploadedFilePath);
                                        if (returndocumentId > 0)
                                        {
                                            returndocumentId = 1;
                                        }
                                        else
                                        {
                                            returndocumentId = 0;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        returndocumentId = 0;
                                    }
                                }
                                else
                                {
                                    returndocumentId = 0;
                                }
                            }
                            else
                            {
                                returndocumentId = 0;
                            }
                            #endregion
                        }
                    }
                    #endregion
                    if (returndocumentId > 0)
                    {
                        dbManager.Transaction.Commit();
                        returndocumentId = documentId;
                    }
                    else
                    {
                        dbManager.Transaction.Rollback();
                        returndocumentId = 0;
                    }
                }
                catch (Exception ex)
                {
                    dbManager.Transaction.Rollback();
                    documentUpload.File = null;
                    throw (ex);
                }
            }
            return returndocumentId;
        }

        private bool IfDocumentPresentInFolder(string documentId,string folderCode)
        {
            dbManager.CreateParameters(2);
            dbManager.AddParameters(0, "@documentId", documentId);
            dbManager.AddParameters(1, "@folderCode", folderCode);
            string query = accessHelper.GetQueryDetail("IfDocumentPresentInFolder");
            if(Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text,query))>0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private DataTable GetImageTypes(int documentTypeId)
        {
            #region gets document types
            DataTable dt = new DataTable();
            string imageTypeDetailsquery = accessHelper.GetQueryDetail("GetImageTypeWithDocumentTypeDetailsByDocumentTypeId");
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@DocumentTypeId", documentTypeId);
            IDataReader drimageTypeDetail = dbManager.ExecuteReader(CommandType.Text, imageTypeDetailsquery);
            dt.Load(drimageTypeDetail);
            drimageTypeDetail.Close();
            return dt;
            #endregion
        }
        public string UploadDocument(DocumentModel documentUpload, string connectionString)
        {
            try
            {
                #region VariableDecalration
                int documentTypeId = 0;
                string docPath = string.Empty;
                Guid guid = Guid.NewGuid();
                List<ImageType> imageTypes = null;
                int documentId = 0;
                string docGuid = string.Empty;
                string oldFileName = string.Empty;
                int returndocumentId = 0;
                #endregion
                string DocUploadedFileNameguid = guid.ToString("N").ToUpper();

                //TODO: Document data insert
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();

                    #region DocumentInsert
                    documentTypeId = GetDocumentTypeId(documentUpload.DocumentTypeCode);

                    string DocumentTypeFileSizeQuery = accessHelper.GetQueryDetail("GetDocumentTypeByDocumentTypeId");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@DocumentTypeId", documentTypeId);
                    IDataReader drFileSize = dbManager.ExecuteReader(CommandType.Text, DocumentTypeFileSizeQuery);
                    DataTable dtFileSize = new DataTable();
                    dtFileSize.Load(drFileSize);
                    drFileSize.Close();

                    bool AcceptSaveDocument = false;
                    //DMS.DocumentType.FileSize: this is using byte.. 
                    if (string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["FileSize"])) || Convert.ToDecimal(dtFileSize.Rows[0]["FileSize"]) == 0 ||
                        (!string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["FileSize"])) && Convert.ToDecimal(dtFileSize.Rows[0]["FileSize"]) * 1024 * 1024 >= Convert.ToDecimal(documentUpload.File.Length)))
                    {
                        AcceptSaveDocument = true;
                        if (!string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MinImageWidth"])) && !string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MinImageHeight"])) && !string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageWidth"])) && !string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageHeight"])))
                        {
                            // Original Image Width and Height
                            int UploadedImageWidth;
                            int UploadedImageHeight;
                            CalculateOriginalImageWidthHeight(documentUpload, out UploadedImageWidth, out UploadedImageHeight);

                            if ((string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MinImageWidth"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MinImageWidth"]) == 0 ||
                                    UploadedImageWidth >= Convert.ToDecimal(dtFileSize.Rows[0]["MinImageWidth"]) * 1024 * 1024 &&
                                string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageWidth"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) == 0 ||
                                    UploadedImageWidth <= Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) * 1024 * 1024
                                )
                                &&
                                (string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageWidth"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) == 0 ||
                                    UploadedImageHeight >= Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageWidth"]) * 1024 * 1024 &&
                                string.IsNullOrEmpty(Convert.ToString(dtFileSize.Rows[0]["MaxImageHeight"])) || Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageHeight"]) == 0 ||
                                UploadedImageHeight <= Convert.ToDecimal(dtFileSize.Rows[0]["MaxImageHeight"]) * 1024 * 1024
                                ))
                            {
                                AcceptSaveDocument = true;
                            }
                            else
                            {
                                AcceptSaveDocument = false;
                            }
                        }

                    }
                    else
                    {
                        AcceptSaveDocument = false;
                        throw new Exception("File size is bigger than mentioned file size");
                    }
                    if (AcceptSaveDocument)
                    {
                        //if(documentUpload.IsOverride == true && connectionType.Trim().ToUpper() == "CONTROL")
                        //{
                        //    documentUpload.IsOverride = false;
                        //    documentUpload.IsNewEntry = true;
                        //}
                        if (documentUpload.IsOverride == true)
                        {
                            //if(connectionType.Trim().ToUpper()!="CONTROL")
                            //{
                            ItemCodeValue physicalFolderName = GetFolderDetails(dbManager, documentUpload.FolderCode);
                            string fileNameQuery = accessHelper.GetQueryDetail("GetFileNameByDocumentId");
                            dbManager.CreateParameters(1);
                            dbManager.AddParameters(0, "@Id", documentUpload.Id);
                            IDataReader drfileName = dbManager.ExecuteReader(CommandType.Text, fileNameQuery);
                            DataTable dtfileName = new DataTable();
                            dtfileName.Load(drfileName);
                            drfileName.Close();
                            if (dtfileName.Rows.Count > 0)
                            {
                                oldFileName = Convert.ToString(dtfileName.Rows[0]["UploadedFileName"]);
                            }
                            string documentImageFile = documentUpload.FolderPath + "/" + physicalFolderName.Value + "/" + oldFileName;
                            List<DocumentModel> docModel = GetImageTypeDocumentList(documentUpload.DocumentGuid, new List<ItemType>(), connectionString);
                            List<ImageDocument> imageModel = new List<ImageDocument>();
                            if (docModel != null && docModel.Count > 0)
                            {
                                #region Delete Type Images
                                docModel.Select(n => new ImageDocument() { Id = n.Id, ImageName = n.FileName }).ToList();
                                DeleteImageDocumentFromDirectory(imageModel, documentUpload.FolderPath, physicalFolderName.Value);
                                #endregion

                                #region Re-Create the files
                                string imageTypeDetailsquery = accessHelper.GetQueryDetail("GetImageTypeWithDocumentTypeDetailsByDocumentTypeId");
                                dbManager.CreateParameters(1);
                                dbManager.AddParameters(0, "@DocumentTypeId", documentTypeId);
                                IDataReader drimageTypeDetail = dbManager.ExecuteReader(CommandType.Text, imageTypeDetailsquery);
                                DataTable dtimageTypeDetail = new DataTable();
                                dtimageTypeDetail.Load(drimageTypeDetail);
                                drimageTypeDetail.Close();
                                if (dtimageTypeDetail.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtimageTypeDetail.Rows.Count; i++)
                                    {
                                        string documentType = Convert.ToString(dtimageTypeDetail.Rows[i]["DocumentType"]);
                                        int imageTypeId = Convert.ToInt32(dtimageTypeDetail.Rows[i]["ImageTypeId"]);
                                        string typeName = Convert.ToString(dtimageTypeDetail.Rows[i]["TypeName"]);
                                        decimal? width = (dtimageTypeDetail.Rows[i]["Width"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dtimageTypeDetail.Rows[i]["Width"]);
                                        decimal? height = (dtimageTypeDetail.Rows[i]["Height"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dtimageTypeDetail.Rows[i]["Height"]);
                                        int ImageWidth;
                                        int ImageHeight;
                                        CalculateOriginalImageWidthHeight(documentUpload, out ImageWidth, out ImageHeight);
                                        int newImageWidth = 0;
                                        int newImageHeight = 0;
                                        CalculateCropImageWidthHeight(width, height, ImageWidth, ImageHeight, ref newImageWidth, ref newImageHeight);
                                        string imageName = docModel.Where(n => n.DocumentTypeCode.Trim() == documentType.ToString().Trim()).Select(n => n.FileName).FirstOrDefault();
                                        string strfileOutput = documentUpload.FolderPath + "\\" + GetFolderName(dbManager, physicalFolderName.Id) + "\\" + imageName;
                                        ResizeImage(documentUpload.FolderPath, strfileOutput, newImageWidth, newImageHeight);
                                    }
                                }
                                #endregion
                            }

                            DeleteOriginalDocumentFromDirectory(documentImageFile);
                            SaveImageInDirectory(documentUpload, oldFileName, physicalFolderName.Id);

                            documentId = documentUpload.Id;
                            returndocumentId = documentUpload.Id;
                        }
                        else if (documentUpload.IsNewEntry == true)
                        {
                            #region New Document
                            Tuple<int, int, string, int, string> tupleDocumentResult = UploadNewDocument(documentUpload, documentTypeId, DocUploadedFileNameguid);
                            if (tupleDocumentResult.Item1 > 0)
                            {
                                documentId = tupleDocumentResult.Item1;
                                docGuid = tupleDocumentResult.Item5;
                                int VersionId = tupleDocumentResult.Item2;
                                //  Insert or update or delete in the EAVMetaData table 
                                //bool result = InserEavMetaData(dbManager, documentUpload, documentId); // check EavMetaData Table's data is insert and it will return true then next function will work
                                bool result = true;
                                //  upload file
                                if (result == true)
                                {
                                    string uploadedFilePath = SaveImageInDirectory(documentUpload, tupleDocumentResult.Item3, tupleDocumentResult.Item4);
                                    //string uploadedFilePath = FileUploadInTheFileServer(documentUpload, tupleDocumentResult.Item3, tupleDocumentResult.Item4);
                                    if (File.Exists(uploadedFilePath))
                                    {
                                        // Insert data in the ImageDocument Table, if documentTypeId is present in ImageType table[have to write the new logic]
                                        #region Save Data in the ImageDocument Table
                                        returndocumentId = ImageDocument_Save(documentUpload, documentTypeId, documentId, VersionId, uploadedFilePath);
                                        if (returndocumentId > 0)
                                        {
                                            returndocumentId = 1;
                                        }
                                        else
                                        {
                                            returndocumentId = 0;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        returndocumentId = 0;
                                    }
                                }
                                else
                                {
                                    returndocumentId = 0;
                                }
                            }
                            else
                            {
                                returndocumentId = 0;
                            }
                            #endregion
                        }
                    }

                    #endregion

                    if (returndocumentId > 0)
                    {
                        dbManager.Transaction.Commit();
                        returndocumentId = documentId;
                    }
                    else
                    {
                        dbManager.Transaction.Rollback();
                        logManager.Log(JsonConvert.SerializeObject(documentUpload), "Request Json not exception log");
                        returndocumentId = 0;
                    }

                }
                return docGuid;
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                dbManager.Transaction.Rollback();
                documentUpload.File = null;
                logManager.Log(JsonConvert.SerializeObject(documentUpload), "Request Json");
                throw (ex);
            }
        }

        private void UpdateOriginalFileName(string fileName, int documentId)
        {
            dbManager.CreateParameters(2);
            dbManager.AddParameters(0, "@fileName", fileName);
            dbManager.AddParameters(1, "@documentId", documentId);
            string UpdateOriginalFileName = accessHelper.GetQueryDetail("UpdateOriginalFileName");
            dbManager.ExecuteNonQuery(CommandType.Text, UpdateOriginalFileName);
        }
        private string SaveImageInDirectory(DocumentModel documentUpload, string newFilename, int folderId, string folderPath = null)
        {
            string uploadedFilePath = string.Empty;
            string virtualPath = documentUpload.FolderPath;
            if (!string.IsNullOrEmpty(folderPath))
            {
                virtualPath = folderPath;
            }
            else
            {
                virtualPath = virtualPath + "/" + GetFolderName(dbManager, folderId);

            }
            uploadedFilePath = UploadFile(virtualPath, documentUpload.File, newFilename);
            if (documentUpload.ResizeProperty != null && !string.IsNullOrEmpty(documentUpload.ResizeProperty.ResizeOrientation))
            {
                var data = ResizeImage(documentUpload, uploadedFilePath, virtualPath, newFilename);
                dbManager.CreateParameters(3);
                dbManager.AddParameters(0, "@height", documentUpload.imageDocument.Height);
                dbManager.AddParameters(1, "@width", documentUpload.imageDocument.Width);
                dbManager.AddParameters(2, "@Id", documentUpload.Id);
                string query = accessHelper.GetQueryDetail("UpdateImageProperties").ToString();
                dbManager.ExecuteNonQuery(CommandType.Text, query);
            }
            return uploadedFilePath;
        }

        public DocumentModel ResizeImage(DocumentModel documentUpload, string fileLocation, string filepath, string fileName)
        {
            try
            {
                string fileEntension = Path.GetExtension(fileLocation);
                ImageFormat OutputFormat = GetImageFormat(fileEntension);
                string tempFileName = filepath + "/" + Guid.NewGuid().ToString("N") + fileEntension;
                File.Copy(fileLocation, tempFileName);

                int newWidth = 0;
                int newHeight = 0;
                documentUpload.imageDocument = documentUpload.imageDocument == null ? new ImageDocument() : documentUpload.imageDocument;
                //Image photo = Bitmap.FromFile(tempFileName);
                Image photo;
                using (photo = Bitmap.FromFile(tempFileName))
                {
                    ExifRotate(photo);
                    decimal aspectRatio = (decimal)photo.Width / photo.Height;
                    if (!string.IsNullOrEmpty(documentUpload.ResizeProperty.ResizeOrientation))
                    {
                        if (documentUpload.ResizeProperty.ResizeOrientation.Trim() == ImageOrientation.Height.ToString())
                        {
                            newHeight = (documentUpload.ResizeProperty.ResizeByPercentage * documentUpload.ResizeProperty.ResizeDimension) / 100;
                            newWidth = Convert.ToInt32(aspectRatio * newHeight);
                        }
                        else
                        {
                            newWidth = (documentUpload.ResizeProperty.ResizeByPercentage * documentUpload.ResizeProperty.ResizeDimension) / 100;
                            newHeight = Convert.ToInt32(newWidth / aspectRatio);
                        }
                    }

                    if (newWidth < photo.Width)
                    {
                        File.Delete(fileLocation);
                        using (Bitmap bmp = new Bitmap(newWidth, newHeight))
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.SmoothingMode = SmoothingMode.HighQuality;
                                g.CompositingQuality = CompositingQuality.HighQuality;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                g.DrawImage(photo, 0, 0, newWidth, newHeight);

                                if (ImageFormat.Png.Equals(OutputFormat))
                                {
                                    bmp.Save(filepath, OutputFormat);
                                }
                                else if (ImageFormat.Jpeg.Equals(OutputFormat))
                                {
                                    ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                                    EncoderParameters encoderParameters;
                                    using (encoderParameters = new System.Drawing.Imaging.EncoderParameters(1))
                                    {
                                        // use jpeg info[1] and set quality to 90
                                        encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                                        bmp.Save(fileLocation, info[1], encoderParameters);
                                    }
                                    documentUpload.imageDocument.Height = newHeight;
                                    documentUpload.imageDocument.Width = newWidth;
                                }
                            }
                        }
                    }
                }
                File.Delete(tempFileName);
                return documentUpload;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public ImageFormat GetImageFormat(string extension)
        {
            if (extension.Trim().ToLower() == ".jpg")
            {
                return ImageFormat.Jpeg;
            }
            else if (extension.Trim().ToLower() == ".jpg")
            {
                return ImageFormat.Png;
            }
            return ImageFormat.Jpeg;
        }
        private void ExifRotate(Image bmp)
        {
            PropertyItem pi = bmp.PropertyItems.Select(x => x)
                                       .FirstOrDefault(x => x.Id == 0x0112);
            if (pi == null) return;

            byte o = pi.Value[0];

            if (o == 2) bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            if (o == 3) bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            if (o == 4) bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            if (o == 5) bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
            if (o == 6) bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (o == 7) bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
            if (o == 8) bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
        }

        //DMSUtilities.CopyDocument(int documentId, string folderCode, string connectionString);


        public int CopyDocument(int documentId, string folderCode, string connectionString)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(folderCode)))
            {
                ItemCodeValue folder = new ItemCodeValue();
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    folder = GetFolderDetails(dbManager, folderCode);
                }
                if (folder.Id > 0)
                {
                    List<ItemId> documentIds = new List<ItemId>();
                    documentIds.Add(new ItemId() { Id = documentId });
                    List<DocumentModel> document = GetDocumentList(documentIds, connectionString);
                    var copyDoc = document.FirstOrDefault();
                    copyDoc.FolderCode = folderCode;
                    if (document != null & document.Count > 0)
                    {
                        string newFileName = Guid.NewGuid().ToString().Replace("-", string.Empty).ToUpper() + System.IO.Path.GetExtension(document.FirstOrDefault().BrowserPath);
                        MoveImage(document.FirstOrDefault().BrowserPath, accessHelper.PhysicalPath + "\\" + folder.Value + "//" + newFileName);
                        return InsertDocument(document.FirstOrDefault(), newFileName, connectionString);
                    }
                    return 0;
                }
                else
                {
                    throw new Exception("Invalid folder code");
                }
            }
            else
            {
                throw new Exception("FolderCode cannot be null");
            }
        }

        private bool MoveImage(string source, string target)
        {
            string targetDirectoryInfo = Path.GetDirectoryName(target);
            if (File.Exists(source))
            {
                if (Directory.Exists(targetDirectoryInfo))
                {
                    File.Copy(source, target);
                    return true;
                }
                throw new Exception("Destination Folder could Not found !!");
            }
            throw new Exception("Source Folder could not found !!");
        }
        #endregion

        #region Get Image
        public List<DocumentModel> GetDocumentList(List<ItemId> DocumentId, string connectinString)
        {
            DocumentModel documentUploadModel = null;
            List<DocumentModel> documentUploadModels = new List<DocumentModel>();
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectinString;
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    //dbManager.GetDatabaseConnectionString(code);
                    dbManager.Open();
                    foreach (var item in DocumentId)
                    {
                        IdList += item.Id + ",";
                    }
                    IdList = IdList.Remove(IdList.Length - 1, 1);

                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentList");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@IDs", IdList);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    //TODO : Get the Browse File Server
                    //string Fileserver = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.Width = (drdocuments["ImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageWidth"]);
                        documentUploadModel.Height = (drdocuments["ImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageHeight"]);
                        documentUploadModel.CreatedOn = (drdocuments["CreatedOn"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["CreatedOn"]);
                        documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);
                        //documentUploadModel.ActivetedFileName = Convert.ToString(drdocuments["ActivetedFileName"]);
                        documentUploadModel.FolderCode = Convert.ToString(drdocuments["FolderCode"]);
                        documentUploadModel.BrowserPath = accessHelper.PhysicalPath + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.DocumentTypeId = Convert.ToInt16(drdocuments["DocumentTypeId"]);

                        documentUploadModel.DocumentGuid = Convert.ToString(drdocuments["DocumentGuid"]);
                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModels;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }

        public DocumentModel GetDocumentByDocumentguid(string documentguid, string connectinString, bool isLocalpathRequired = false)
        {
            DocumentModel documentUploadModel = null;
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectinString;
                    dbManager.Open();
                    documentUploadModel = GetDocumentOnDocumentguid(documentguid, connectinString, isLocalpathRequired);
                }
                return documentUploadModel;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }

        public DocumentModel GetDocumentByObjectId(int objectId, string ObjectName, string documentType, string connectionString)
        {
            DocumentModel documentUploadModel = null;
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentByObjectId");
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@objectId", objectId);
                    dbManager.AddParameters(1, "@objectName", ObjectName);
                    dbManager.AddParameters(2, "@documentType", documentType);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    string Fileserver = accessHelper.BrowseURL;
                    if (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = accessHelper.PhysicalPath + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.Width = (drdocuments["ImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageWidth"]);
                        documentUploadModel.Height = (drdocuments["ImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageHeight"]);
                        documentUploadModel.CreatedOn = (drdocuments["CreatedOn"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["CreatedOn"]);
                        documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);
                        documentUploadModel.FolderCode = Convert.ToString(drdocuments["FolderCode"]);
                        documentUploadModel.BrowserPath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.DocumentTypeId = Convert.ToInt32(drdocuments["DocumentTypeId"]);
                        documentUploadModel.DocumentTypeCode = Convert.ToString(drdocuments["DocumentType"]);
                        documentUploadModel.DocumentGuid = Convert.ToString(drdocuments["DocumentGuid"]);
                        documentUploadModel.FolderPath = Convert.ToString(drdocuments["PhysicallFolderName"]);
                        documentUploadModel.FolderId = Convert.ToInt32(drdocuments["FOlderId"]);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModel;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
            }
        }
        public DocumentModel GetDocumentOnDocumentguid(string documentguid, string connectinString, bool isLocalpathRequired = false)
        {
            DocumentModel documentUploadModel = null;
            string IdList = string.Empty;
            try
            {
                string documentsquery = accessHelper.GetQueryDetail("GetDocumentByDocumentguid");
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@ID", documentguid);
                IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                string Fileserver = accessHelper.BrowseURL;
                if (drdocuments.Read())
                {
                    documentUploadModel = new DocumentModel();
                    documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                    documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                    documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                    documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                    documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                    documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                    documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                    documentUploadModel.FilePath = accessHelper.PhysicalPath + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                    documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                    documentUploadModel.Width = (drdocuments["ImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageWidth"]);
                    documentUploadModel.Height = (drdocuments["ImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageHeight"]);
                    documentUploadModel.CreatedOn = (drdocuments["CreatedOn"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["CreatedOn"]);
                    documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                    documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                    documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                    documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                    documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);
                    documentUploadModel.FolderCode = Convert.ToString(drdocuments["FolderCode"]);
                    documentUploadModel.BrowserPath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                    documentUploadModel.DocumentTypeId = Convert.ToInt32(drdocuments["DocumentTypeId"]);
                    documentUploadModel.DocumentTypeCode = Convert.ToString(drdocuments["DocumentType"]);
                    documentUploadModel.DocumentGuid = Convert.ToString(drdocuments["DocumentGuid"]);
                    documentUploadModel.FolderPath = Convert.ToString(drdocuments["PhysicallFolderName"]);
                    documentUploadModel.FolderId = Convert.ToInt32(drdocuments["FOlderId"]);
                }
                dbManager.CloseReader();
                return documentUploadModel;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }
        public List<DocumentModel> GetDocumentByDocumentType(string documentType, string connectinString)
        {
            DocumentModel documentUploadModel = null;
            List<DocumentModel> documentUploadModels = new List<DocumentModel>();
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectinString;
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    //dbManager.GetDatabaseConnectionString(code);
                    dbManager.Open();
                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentByDocumentType");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@documentType", documentType);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    //TODO : Get the Browse File Server
                    //string Fileserver = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        //documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.DocumentGuid = Convert.ToString(drdocuments["DocumentGuid"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.Width = (drdocuments["ImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageWidth"]);
                        documentUploadModel.Height = (drdocuments["ImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageHeight"]);
                        documentUploadModel.CreatedOn = (drdocuments["CreatedOn"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["CreatedOn"]);
                        documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);
                        documentUploadModel.ActivetedFileName = Convert.ToString(drdocuments["ActivetedFileName"]);
                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModels;
            }
            catch (SqlException sqlEx)
            {
                string createText = sqlEx.InnerException + "TT" + string.Empty;
                File.WriteAllText(@"E:/LogFile/LogFile.txt", createText);

                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
            return null;
        }
        public List<DocumentModel> GetDocumentListByImageType(string imageType, List<ItemId> DocumentId, string connectionString)
        {
            DocumentModel documentUploadModel = null;
            List<DocumentModel> documentUploadModels = new List<DocumentModel>();
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    //dbManager.GetDatabaseConnectionString(code);
                    dbManager.Open();
                    foreach (var item in DocumentId)
                    {
                        IdList += item.Id + ",";
                    }
                    IdList = IdList.Remove(IdList.Length - 1, 1);

                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentListByImageType");
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@IDs", IdList);
                    dbManager.AddParameters(1, "@ImagetTypeName", imageType);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    // DataTable dt = new DataTable();
                    // dt.Load(drdocuments);
                    //string Fileserver = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);
                        documentUploadModel.DocumentGuid = Convert.ToString(drdocuments["DocumentGuid"]);
                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModels;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }

        public List<DocumentModel> GetDocumentListByImageTypeByDocGuid(string imageType, List<ItemCodeValue> DocumentId, string connectionString)
        {
            DocumentModel documentUploadModel = null;
            List<DocumentModel> documentUploadModels = new List<DocumentModel>();
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    //dbManager.GetDatabaseConnectionString(code);
                    dbManager.Open();
                    foreach (var item in DocumentId)
                    {
                        IdList += item.Code + ",";
                    }
                    IdList = IdList.Remove(IdList.Length - 1, 1);

                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentListByImageTypeByDocGuid");
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@IDs", IdList);
                    dbManager.AddParameters(1, "@ImagetTypeName", imageType);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    // DataTable dt = new DataTable();
                    // dt.Load(drdocuments);
                    //string Fileserver = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);

                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModels;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }

        public List<DocumentModel> GetDocumentListByImageTypeList(string DocumentId, List<ItemType> itemTypeList, string connectionString)
        {
            using (dbManager = new DBhandler())
            {
                dbManager.ConnectionString = connectionString;
                dbManager.Open();
                return GetImageTypeDocumentList(DocumentId, itemTypeList, connectionString);
            }
        }
        private List<DocumentModel> GetImageTypeDocumentList(string DocumentId, List<ItemType> itemTypeList, string connectionString)
        {
            DocumentModel documentUploadModel = null;
            List<DocumentModel> documentUploadModels = new List<DocumentModel>();
            string imgTypelist = string.Empty;
            try
            {
                if (itemTypeList.Count > 0)
                {
                    foreach (var item in itemTypeList)
                    {
                        imgTypelist += item.Type + ",";
                    }
                    imgTypelist = imgTypelist.Remove(imgTypelist.Length - 1, 1);

                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentListByImageTypeList");
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@DocumentId", DocumentId);
                    dbManager.AddParameters(1, "@ImagetTypeName", imgTypelist);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    //string Fileserver = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.imageType = new ImageType();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        string filePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        //documentUploadModel.File = ConvertImageURLToBase64(filePath);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.Width = (drdocuments["Width"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["Width"]);
                        documentUploadModel.Height = (drdocuments["Height"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["Height"]);
                        documentUploadModel.DocumentTypeCode = Convert.ToString(drdocuments["Code"]);
                        documentUploadModel.imageType.ImageTypeId = Convert.ToInt32(drdocuments["ImageTypeId"]);
                        documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);

                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                else
                {
                    //string documentsquery = accessHelper.GetQueryDetail("GetDocumentImageByDocumentId");
                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentImageVariations");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@DocumentId", DocumentId);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    //string Fileserver = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.imageType = new ImageType();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.BrowserPath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        string filePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        //documentUploadModel.File = ConvertImageURLToBase64(filePath);
                        documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        documentUploadModel.FilePath = accessHelper.PhysicalPath + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.DocumentTypeCode = Convert.ToString(drdocuments["Code"]);
                        documentUploadModel.MinImageWidth = (drdocuments["MinImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["MinImageWidth"]);
                        documentUploadModel.MinImageHeight = (drdocuments["MinImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["MinImageHeight"]);
                        documentUploadModel.MaxImageWidth = (drdocuments["MaxImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["MaxImageWidth"]);
                        documentUploadModel.MaxImageHeight = (drdocuments["MaxImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["MaxImageHeight"]);
                        documentUploadModel.imageType.ImageTypeId = 0;
                        documentUploadModel.imageType.ImageTypeName = Convert.ToString(drdocuments["TypeName"]);
                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModels;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }

        public List<DocumentModel> GetDocumentListByImageTypeListAndDocumentList(List<ItemId> DocumentId, List<ItemType> itemTypeList, string connectinString)
        {
            DocumentModel documentUploadModel = null;
            List<DocumentModel> documentUploadModels = new List<DocumentModel>();
            List<ImageDocument> imageDocuments = new List<ImageDocument>();
            ImageDocument imageDocument = null;
            string imgTypelist = string.Empty;
            string IdList = string.Empty;
            try
            {
                using (dbManager = new DBhandler())
                {
                    //dbManager.GetDatabaseConnectionString(this.ModuleCode);
                    dbManager.ConnectionString = connectinString;
                    dbManager.Open();
                    foreach (var item in itemTypeList)
                    {
                        imgTypelist += item.Type + ",";
                    }
                    foreach (var item in DocumentId)
                    {
                        IdList += item.Id + ",";
                    }
                    imgTypelist = imgTypelist.Remove(imgTypelist.Length - 1, 1);
                    IdList = IdList.Remove(IdList.Length - 1, 1);

                    string documentsquery = accessHelper.GetQueryDetail("GetDocumentListByImageTypeListAndDocumentList");
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@IDs", IdList);
                    IDataReader drdocuments = dbManager.ExecuteReader(CommandType.Text, documentsquery);
                    string Fileserver = accessHelper.BrowseURL;
                    while (drdocuments.Read())
                    {
                        documentUploadModel = new DocumentModel();
                        documentUploadModel.imageDocument = new ImageDocument();
                        documentUploadModel.imageType = new ImageType();
                        documentUploadModel.Id = Convert.ToInt32(drdocuments["DocumentId"]);
                        documentUploadModel.Name = Convert.ToString(drdocuments["DocumentName"]);
                        documentUploadModel.FileName = Convert.ToString(drdocuments["UploadedFileName"]);
                        //documentUploadModel.Cost = Convert.ToDouble(drdocuments["CostFee"]);
                        documentUploadModel.CreatedBy = (drdocuments["CreatedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drdocuments["CreatedBy"]);
                        documentUploadModel.ExpiryDate = (drdocuments["ExpiredDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(drdocuments["ExpiredDate"]);
                        documentUploadModel.Width = (drdocuments["ImageWidth"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageWidth"]);
                        documentUploadModel.Height = (drdocuments["ImageHeight"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drdocuments["ImageHeight"]);
                        documentUploadModel.FileSize = (drdocuments["FileSize"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drdocuments["FileSize"]);
                        documentUploadModel.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]) + "/" + Convert.ToString(drdocuments["UploadedFileName"]);
                        //documentUploadModel.Type = Convert.ToString(drdocuments["MIMEType"]);
                        //documentUploadModel.ReviewStatus = Convert.ToString(drdocuments["ReviewStatus"]);
                        //documentUploadModel.AllowReviewersToReassignReview = (drdocuments["AllowReviewersToReassignReview"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["AllowReviewersToReassignReview"].ToString());
                        //documentUploadModel.NotifyInitiatorWhenReviewComplete = (drdocuments["NotifyInitiatorWhenReviewComplete"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["NotifyInitiatorWhenReviewComplete"].ToString());
                        //documentUploadModel.UseDefaultWorkflow = (drdocuments["UseDefaultWorkflow"] == DBNull.Value) ? (bool?)null : Boolean.Parse(drdocuments["UseDefaultWorkflow"].ToString());
                        //documentUploadModel.ReviewerInstruction = Convert.ToString(drdocuments["ReviewerInstruction"]);
                        //----------List of Image Document------------------------

                        ImageDocumentList(documentUploadModel, imageDocument, drdocuments, Convert.ToInt32(drdocuments["DocumentId"]), imgTypelist, connectinString);
                        documentUploadModels.Add(documentUploadModel);
                    }
                    dbManager.CloseReader();
                }
                return documentUploadModels;
            }
            catch (SqlException sqlEx)
            {
                logManager.Log(Convert.ToString(sqlEx.InnerException), string.Empty);
                throw (sqlEx);
                //LogManager.Log(sqlEx, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", sqlEx.Message, string.Empty);
            }
            catch (Exception ex)
            {
                logManager.Log(Convert.ToString(ex.InnerException), string.Empty);
                throw (ex);
                //LogManager.Log(ex, code);
                //throw new RepositoryException("GETDOCUMENTFAILED", ex.Message, string.Empty);
            }
        }

        private void ImageDocumentList(DocumentModel documentUploadModel, ImageDocument imageDocument, IDataReader drdocuments, int documentId, string imgTypelist, string connectinString)
        {
            using (dbManagerImgDoc = new DBhandler())
            {
                dbManagerImgDoc.ConnectionString = connectinString;
                dbManagerImgDoc.Open();
                string Fileserver = accessHelper.PhysicalPath;  // Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("BrowseFileServer")).Value.ToString();
                string imageDocumentquery = accessHelper.GetQueryDetail("GetImageDocumentList");
                dbManagerImgDoc.CreateParameters(2);
                dbManagerImgDoc.AddParameters(0, "@DocumentId", documentId);
                dbManagerImgDoc.AddParameters(1, "@ImagetTypeName", imgTypelist);
                IDataReader drimageDocument = dbManagerImgDoc.ExecuteReader(CommandType.Text, imageDocumentquery);
                documentUploadModel.imageDocumentList = new List<ImageDocument>();
                while (drimageDocument.Read())
                {
                    imageDocument = new ImageDocument();
                    imageDocument.Id = Convert.ToInt32(drimageDocument["Id"]);
                    imageDocument.FilePath = Fileserver + "/" + Convert.ToString(drdocuments["FilePath"]).ToUpper().Trim() + "/" + Convert.ToString(drimageDocument["ImageName"]);
                    imageDocument.Width = (drimageDocument["Width"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drimageDocument["Width"]);
                    imageDocument.Height = (drimageDocument["Height"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(drimageDocument["Height"]);
                    imageDocument.ImageTypeName = Convert.ToString(drimageDocument["TypeName"]);
                    documentUploadModel.imageDocumentList.Add(imageDocument);
                }
                dbManagerImgDoc.CloseReader();
            }
        }

        public String ConvertImageURLToBase64(String url)
        {
            StringBuilder _sb = new StringBuilder();
            // _sb.Append("data:image/png;base64,");
            try
            {
                Byte[] _byte = GetImage(url);
                _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
            }
            catch (Exception ex)
            {
            }
            return _sb.ToString();
        }

        private static byte[] GetImage(string url)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                stream = response.GetResponseStream();

                using (BinaryReader br = new BinaryReader(stream))
                {
                    int len = (int)(response.ContentLength);
                    buf = br.ReadBytes(len);
                    br.Close();
                }

                stream.Close();
                response.Close();
            }
            catch (Exception exp)
            {
                buf = null;
            }

            return (buf);
        }
        #endregion

        #region Main Utilities
        private int GetDocumentTypeId(string code)
        {
            int docTypeId = 0;
            string query = accessHelper.GetQueryDetail("GetDocumentTypeByCode");
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@Code", code);
            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
            if (dr.Read())
            {
                docTypeId = Convert.ToInt32(dr["Id"]);
            }
            dr.Close();
            return docTypeId;
        }

        private void CalculateOriginalImageWidthHeight(DocumentModel documentUpload, out int ImageWidth, out int ImageHeight)// this is used in upload API
        {
            if (!string.IsNullOrEmpty(documentUpload.FilePath))
            {
                Image photo = Bitmap.FromFile(documentUpload.FilePath);
                ImageWidth = photo.Width;
                ImageHeight = photo.Height;
            }
            else
            {
                byte[] imageBytes = Convert.FromBase64String(Convert.ToString(documentUpload.File));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // write the string to the stream 
                    memoryStream.Write(imageBytes, 0, imageBytes.Length);

                    // create the start Bitmap from the MemoryStream that contains the image  
                    Bitmap startBitmap = new Bitmap(memoryStream);
                    ImageWidth = startBitmap.Width;
                    ImageHeight = startBitmap.Height;
                }
            }
        }

        private int UpdateOldDocument(DocumentModel documentUpload, string newFilename)
        {
            int returndocumentId = 0;
            int documentId = documentUpload.Id;
            int VersionId = 0;
            string uploadedFileName = string.Empty;
            //---------------------------------------------------------------------------------------------------
            if (documentId > 0)
            {
                #region: Document version data insert
                string documentVersionQuery = accessHelper.GetQueryDetail("UpdateDocumentVersion");
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@DocumentId", documentId);
                dbManager.AddParameters(1, "@DocumentVersionName", newFilename);
                dbManager.AddParameters(2, "@ModifiedBy", documentUpload.CreatedBy);
                dbManager.AddParameters(3, "@Modifiedon", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                VersionId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, documentVersionQuery));
                #endregion
                if (VersionId > 0)
                {
                    returndocumentId = documentId;
                }
            }
            return returndocumentId;
        }

        private string FileUploadInTheFileServer(DocumentModel documentUpload, string newFilename, int folderId)// this is used in upload API
        {
            string virtualPath = accessHelper.PhysicalPath;
            //string virtualPath = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString();
            virtualPath = virtualPath + "/" + GetFolderName(dbManager, folderId);
            string uploadedFilePath = UploadFile(virtualPath, documentUpload.File, newFilename);
            return uploadedFilePath;
        }

        public string GetFolderName(DBhandler dbManager, int folderId)
        {
            string physicallFolderName = string.Empty;
            DMSHelper dMSUtilities = new DMSHelper();
            string query = dMSUtilities.accessHelper.GetQueryDetail("GetFolderNameById");
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@Id", folderId);
            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
            if (dr.Read())
            {
                physicallFolderName = Convert.ToString(dr["PhysicallFolderName"]);
            }
            dr.Close();
            return physicallFolderName;
        }

        public ItemCodeValue GetFolderDetails(DBhandler dBhandler, string FolderCode)
        {
            ItemCodeValue folderDetails = new ItemCodeValue();
            DMSHelper dMSUtilities = new DMSHelper();
            dBhandler.CreateParameters(1);
            dBhandler.AddParameters(0, "@FolderCode", FolderCode);
            string query = dMSUtilities.accessHelper.GetQueryDetail("GetFolderNameByFolderCode");
            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
            if (dr.Read())
            {
                folderDetails = new ItemCodeValue()
                {
                    Id = !string.IsNullOrEmpty(Convert.ToString(dr["Id"])) ? Convert.ToInt32(dr["Id"]) : 0,
                    Value = Convert.ToString(dr["PhysicallFolderName"])
                };
            }
            dr.Close();
            return folderDetails;
        }
        public string UploadFile(string filePath, string file, string fileName)
        {
            byte[] imageBytes = Convert.FromBase64String(file);
            //var path = Path.Combine(filePath, Path.GetFileName(fileName));
            System.IO.File.WriteAllBytes(filePath + "/" + fileName, imageBytes);
            return filePath + "/" + fileName;
        }

        private void CalculateCropImageWidthHeight(decimal? width, decimal? height, int ImageWidth, int ImageHeight, ref int newImageWidth, ref int newImageHeight) // this is used in upload API
        {
            if ((width == null && height == null) || (width == 0 && height == 0))//1
            {
                newImageWidth = ImageWidth;
                newImageHeight = ImageHeight;
            }
            else if ((width == null && height != null) || (width == 0 && height != 0))//2
            {
                newImageWidth = (int)(ImageWidth * height) / ImageHeight;
                newImageHeight = Convert.ToInt32(height); //ImageHeight;
            }
            else if ((width != null && height == null) || (width != 0 && height == 0))//3
            {
                newImageWidth = Convert.ToInt32(width); //ImageWidth;
                newImageHeight = (int)(width * ImageHeight) / ImageWidth;

            }
            else//4
            {
                newImageWidth = (int)width;
                newImageHeight = (int)height;
            }
        }

        public void ResizeImage(string FileNameInput, string FileNameOutput, int ResizeWidth, int ResizeHeight)
        {
            try
            {
                Image photo = null;
                using (photo = Bitmap.FromFile(FileNameInput))
                {
                    string OutputFormat = Path.GetExtension(FileNameInput).Replace(".", string.Empty).ToUpper();

                    decimal aspectRatio = (decimal)photo.Width / photo.Height;
                    decimal boxRatio = ResizeWidth / ResizeHeight;

                    int newWidth = ResizeWidth;
                    int newHeight = ResizeHeight;

                    using (Bitmap bmp = new Bitmap(newWidth, newHeight))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            g.DrawImage(photo, 0, 0, newWidth, newHeight);

                            if (ImageFormat.Png.ToString().ToUpper().Equals(OutputFormat))
                            {
                                bmp.Save(FileNameOutput, ImageFormat.Png);
                            }
                            else if (OutputFormat.ToUpper()=="JPG" || OutputFormat.ToUpper() == "JPEG")
                            {
                                ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                                EncoderParameters encoderParameters;
                                using (encoderParameters = new System.Drawing.Imaging.EncoderParameters(1))
                                {
                                    // use jpeg info[1] and set quality to 90
                                    encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                                    bmp.Save(FileNameOutput, info[1], encoderParameters);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private int ImageDocument_Save(DocumentModel documentUpload, int documentTypeId, int documentId, int VersionId, string uploadedFilePath)// this is used in upload API
        {
            //need to change
            int returndocumentId = 0;
            int folderId = 0;
            //----------------------------------FolderId/ Folder Code------------------------------------------------------------
            if (documentUpload.FolderId > 0)
            {
                folderId = documentUpload.FolderId;
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(documentUpload.FolderCode)))
            {
                folderId = accessHelper.GetFolderId(dbManager, documentUpload.FolderCode);
            }
            //---------------------------------------------------------------------------------------------------
            //-----------Original Image Name---------------------
            string[] arr = new string[] { };
            string originalFilename = Convert.ToString(documentUpload.FileName).Replace("_", "").Replace(" ", "").Replace("(", "").Replace(")", "").Trim(); ;
            //-------------- Physical File location------------
            //UpLoad file
            string virtualPath = accessHelper.PhysicalPath;
            //string virtualPath = DMSEnvironments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServer")).Value.ToString();//done
            string imageTypeDetailsquery = accessHelper.GetQueryDetail("GetImageTypeWithDocumentTypeDetailsByDocumentTypeId");
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@DocumentTypeId", documentTypeId);
            IDataReader drimageTypeDetail = dbManager.ExecuteReader(CommandType.Text, imageTypeDetailsquery);
            DataTable dtimageTypeDetail = new DataTable();
            dtimageTypeDetail.Load(drimageTypeDetail);
            drimageTypeDetail.Close();
            if (dtimageTypeDetail.Rows.Count > 0)
            {
                for (int i = 0; i < dtimageTypeDetail.Rows.Count; i++)
                {
                    string documentType = Convert.ToString(dtimageTypeDetail.Rows[i]["DocumentType"]);
                    int imageTypeId = Convert.ToInt32(dtimageTypeDetail.Rows[i]["ImageTypeId"]);
                    string typeName = Convert.ToString(dtimageTypeDetail.Rows[i]["TypeName"]);
                    decimal? width = (dtimageTypeDetail.Rows[i]["Width"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dtimageTypeDetail.Rows[i]["Width"]);
                    decimal? height = (dtimageTypeDetail.Rows[i]["Height"] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(dtimageTypeDetail.Rows[i]["Height"]);

                    //--------------------------------ImageDocument.Width  and ImageDocument.Height calculate--------------------------------------
                    int ImageWidth;
                    int ImageHeight;
                    CalculateOriginalImageWidthHeight(documentUpload, out ImageWidth, out ImageHeight);
                    //---------------------------------------------------------------------------------
                    int newImageWidth = 0;
                    int newImageHeight = 0;
                    CalculateCropImageWidthHeight(width, height, ImageWidth, ImageHeight, ref newImageWidth, ref newImageHeight);

                    //----------------------------------------------------------------------------------------------------------------------------------

                    //------------------8 characters--------------------------
                    Guid imgguid = Guid.NewGuid();
                    string imgguidDocguid = imgguid.ToString("N").Substring(0, 8);
                    string imgName = imgguidDocguid.ToUpper() + "_" + Convert.ToString(documentType) + "_" + Convert.ToString(typeName) + "_" + originalFilename;

                    string strfileOutput = virtualPath + "\\" + GetFolderName(dbManager, folderId) + "\\" + imgName;

                    #region Save ImageDocument Data---------------------------------------------------------------------------------
                    int returnaddImageDocument = SaveDataInImageDocument(documentUpload, documentId, VersionId, imgName, imageTypeId, newImageWidth, newImageHeight);
                    if (returnaddImageDocument > 0)
                    {
                        returndocumentId = 1;
                    }
                    else
                    {
                        dbManager.Transaction.Rollback();
                        // return 0;
                        return returndocumentId = 0;
                    }
                    ResizeImage(uploadedFilePath, strfileOutput, newImageWidth, newImageHeight);

                    #endregion-----------------------------------------------------------------------------------------------------
                }

            }
            else
            {
                returndocumentId = 1;
            }
            return returndocumentId;
        }

        private int SaveDataInImageDocument(DocumentModel documentUpload, int documentId, int VersionId, string imgName, int imageTypeId, int newImageWidth, int newImageHeight)// this is used in upload API
        {
            string addImageDocumentquery = accessHelper.GetQueryDetail("AddImageDocument");
            dbManager.CreateParameters(8);
            dbManager.AddParameters(0, "@DocumentId", documentId);
            dbManager.AddParameters(1, "@ImageName", imgName);
            dbManager.AddParameters(2, "@ImageTypeId", imageTypeId);
            dbManager.AddParameters(3, "@DocumentVersionId", VersionId > 0 ? VersionId : (int?)null);
            dbManager.AddParameters(4, "@CreatedBy", documentUpload.CreatedBy);
            dbManager.AddParameters(5, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            dbManager.AddParameters(6, "@Width", newImageWidth);
            dbManager.AddParameters(7, "@Height", newImageHeight);
            int returnaddImageDocument = dbManager.ExecuteNonQuery(CommandType.Text, addImageDocumentquery);
            return returnaddImageDocument;
        }

        private int ImageDocument_Save(DocumentModel documentUpload, int documentTypeId, int documentId, int VersionId, string uploadedFilePath, string imgName, int imageTypeId, int width, int height, string strfileOutput)
        {
            int returndocumentId = 0;
            #region Save ImageDocument Data---------------------------------------------------------------------------------
            int returnaddImageDocument = SaveDataInImageDocument(documentUpload, documentId, VersionId, imgName, imageTypeId, width, height);
            if (returnaddImageDocument > 0)
            {
                returndocumentId = 1;
            }
            else
            {
                dbManager.Transaction.Rollback();
                // return 0;
                returndocumentId = 0;
            }
            ResizeImage(uploadedFilePath, strfileOutput, width, height);

            #endregion-----------------------------------------------------------------------------------------------------
            return returndocumentId;
        }

        private Tuple<int, int, string, int> ReturnDocumentVersion(DocumentModel documentUpload, string DocUploadedFileNameguid)// this is used in upload API
        {
            Tuple<int, int, string, int> tupleDocumentResult = new Tuple<int, int, string, int>(0, 0, string.Empty, 0);
            int documentId = documentUpload.Id;
            int VersionId = 0;
            int resultId = 0;
            int folderId = 0;
            string uploadedFileName = string.Empty;
            //----------------------------------FolderId/ Folder Code------------------------------------------------------------
            if (documentUpload.FolderId > 0)
            {
                folderId = documentUpload.FolderId;
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(documentUpload.FolderCode)))
            {
                folderId = accessHelper.GetFolderId(dbManager, documentUpload.FolderCode);
            }
            //---------------------------------------------------------------------------------------------------
            if (documentId > 0)
            {
                //int LatestVersionId = ReturnLatestVersionId(documentId);
                int LatestVersionId = 1;
                // -----------------------------------create file Name-----------------------------------
                string fileName = documentUpload.FileName.Replace("_", "").Replace(" ", "").Replace("(", "").Replace(")", "").Trim();
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                string extension = System.IO.Path.GetExtension(fileName);
                uploadedFileName = DocUploadedFileNameguid.ToUpper() + "_" + fileNameWithoutExtension + "_" + "V" + LatestVersionId + extension;

                #region: Document version data insert
                string documentVersionQuery = accessHelper.GetQueryDetail("AddDocumentVersion");
                dbManager.CreateParameters(6);
                dbManager.AddParameters(0, "@DocumentId", documentId);
                dbManager.AddParameters(1, "@Version", LatestVersionId);
                dbManager.AddParameters(2, "@DocumentVersionName", uploadedFileName);
                dbManager.AddParameters(3, "@IsActive", true);
                dbManager.AddParameters(4, "@CreatedBy", documentUpload.CreatedBy);
                dbManager.AddParameters(5, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                VersionId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, documentVersionQuery));
                #endregion
                if (VersionId > 0)
                {
                    string activeCurrentVerionQuery = accessHelper.GetQueryDetail("ActiveCurrentVerion");
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@DocumentId", documentId);
                    dbManager.AddParameters(1, "@Id", VersionId);
                    resultId = Convert.ToInt32(dbManager.ExecuteNonQuery(CommandType.Text, activeCurrentVerionQuery));
                }
            }
            if (resultId > 0)
            {
                tupleDocumentResult = new Tuple<int, int, string, int>(documentId, VersionId, uploadedFileName, folderId);
            }
            return tupleDocumentResult;
        }

        private Tuple<int, int, string, int, string> UploadNewDocument(DocumentModel documentUpload, int documentTypeId, string DocUploadedFileNameguid)// this is used in upload API
        {

            Tuple<int, int, string, int, string> tupleDocumentResult = new Tuple<int, int, string, int, string>(0, 0, string.Empty, 0, string.Empty);
            int documentId = 0;
            int VersionId = 0;
            int resultId = 0;
            int folderId = 0;
            string uploadedFileName = string.Empty;
            // -----------------------------------create file Name-----------------------------------
            string fileName = documentUpload.FileName.Replace("_", "").Replace(" ", "").Replace("(", "").Replace(")", "").Trim();
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string extension = System.IO.Path.GetExtension(fileName);
            uploadedFileName = DocUploadedFileNameguid.ToUpper() + extension;
            //----------------------------Check this document is image or not------------------------------
            bool IsImage = IsImageCheck(documentTypeId);
            //----------------------------------FolderId/ Folder Code------------------------------------------------------------
            if (documentUpload.FolderId > 0)
            {
                folderId = documentUpload.FolderId;
            }
            else if (!string.IsNullOrEmpty(Convert.ToString(documentUpload.FolderCode)))
            {
                folderId = accessHelper.GetFolderId(dbManager, documentUpload.FolderCode);
            }
            //---------------------------------------------------------------------------------------------------
            string documentQuery = accessHelper.GetQueryDetail("AddDocument");
            dbManager.CreateParameters(24);
            dbManager.AddParameters(0, "@DocumentTypeId", documentUpload.DocumentTypeCode);
            dbManager.AddParameters(1, "@Name", documentUpload.Name);
            dbManager.AddParameters(2, "@UploadedFileName", uploadedFileName);
            dbManager.AddParameters(3, "@BaseDocumentType", documentUpload.FileFormat);
            dbManager.AddParameters(4, "@Title", documentUpload.Title);
            dbManager.AddParameters(5, "@DocumentSubject", documentUpload.Subject);
            dbManager.AddParameters(6, "@Keywords", documentUpload.Keywords);
            dbManager.AddParameters(7, "@DocumentNumber", documentUpload.DocumentNumber);
            dbManager.AddParameters(8, "@Comment", documentUpload.Comment);
            //dbManager.AddParameters(9, "@CostFee", Convert.ToDouble(documentUpload.Cost));
            dbManager.AddParameters(9, "@CostFee", Convert.ToDouble(0));
            if ((Convert.ToDateTime(documentUpload.ExpiryDate) < DateTime.Now))
            {
                dbManager.AddParameters(10, "@ExpiredDate", DBNull.Value);
            }
            else
            {
                dbManager.AddParameters(10, "@ExpiredDate", Convert.ToDateTime(documentUpload.ExpiryDate));
            }
            dbManager.AddParameters(11, "@CreatedBy", documentUpload.CreatedOn);
            dbManager.AddParameters(12, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            dbManager.AddParameters(13, "@FileSize", documentUpload.FileSize);// file size store in the database as a byte
            dbManager.AddParameters(14, "@ParentId", documentUpload.ParentId);
            if (IsImage)
            {
                int ImageWidth;
                int ImageHeight;
                CalculateOriginalImageWidthHeight(documentUpload, out ImageWidth, out ImageHeight);
                dbManager.AddParameters(15, "@ImageWidth", ImageWidth);
                dbManager.AddParameters(16, "@ImageHeight", ImageHeight);
            }
            else
            {
                dbManager.AddParameters(15, "@ImageWidth", null);
                dbManager.AddParameters(16, "@ImageHeight", null);
            }
            dbManager.AddParameters(17, "@FolderId", documentUpload.FolderCode);
            dbManager.AddParameters(18, "@ReviewStatus", documentUpload.ReviewStatus);
            dbManager.AddParameters(19, "@AllowReviewersToReassignReview", documentUpload.AllowReviewersToReassignReview);
            dbManager.AddParameters(20, "@NotifyInitiatorWhenReviewComplete", documentUpload.NotifyInitiatorWhenReviewComplete);
            dbManager.AddParameters(21, "@UseDefaultWorkflow", documentUpload.UseDefaultWorkflow);
            dbManager.AddParameters(22, "@ReviewerInstruction", documentUpload.ReviewerInstruction);
            dbManager.AddParameters(23, "@DocumentGuid", DocUploadedFileNameguid);
            documentId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, documentQuery));


            if (documentId > 0)
            {
                tupleDocumentResult = new Tuple<int, int, string, int, string>(documentId, VersionId, uploadedFileName, folderId, DocUploadedFileNameguid);
            }
            return tupleDocumentResult;
        }


        public int InsertDocument(DocumentModel documentUpload, string uploadedFileName, string connectionString)
        {

            try
            {
                using (dbManager = new DBhandler())
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    bool IsImage = IsImageCheck(documentUpload.DocumentTypeId);
                    string documentQuery = accessHelper.GetQueryDetail("AddDocument");
                    dbManager.CreateParameters(24);
                    dbManager.AddParameters(0, "@DocumentTypeId", documentUpload.DocumentTypeCode);
                    dbManager.AddParameters(1, "@Name", documentUpload.Name);
                    dbManager.AddParameters(2, "@UploadedFileName", uploadedFileName);
                    dbManager.AddParameters(3, "@BaseDocumentType", documentUpload.FileFormat);
                    dbManager.AddParameters(4, "@Title", documentUpload.Title);
                    dbManager.AddParameters(5, "@DocumentSubject", documentUpload.Subject);
                    dbManager.AddParameters(6, "@Keywords", documentUpload.Keywords);
                    dbManager.AddParameters(7, "@DocumentNumber", documentUpload.DocumentNumber);
                    dbManager.AddParameters(8, "@Comment", documentUpload.Comment);
                    //dbManager.AddParameters(9, "@CostFee", Convert.ToDouble(documentUpload.Cost));
                    dbManager.AddParameters(9, "@CostFee", Convert.ToDouble(0));
                    if ((Convert.ToDateTime(documentUpload.ExpiryDate) < DateTime.Now))
                    {
                        dbManager.AddParameters(10, "@ExpiredDate", DBNull.Value);
                    }
                    else
                    {
                        dbManager.AddParameters(10, "@ExpiredDate", Convert.ToDateTime(documentUpload.ExpiryDate));
                    }
                    dbManager.AddParameters(11, "@CreatedBy", documentUpload.CreatedBy);
                    dbManager.AddParameters(12, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    dbManager.AddParameters(13, "@FileSize", documentUpload.FileSize);// file size store in the database as a byte
                    dbManager.AddParameters(14, "@ParentId", documentUpload.ParentId);
                    if (IsImage)
                    {
                        int ImageWidth;
                        int ImageHeight;
                        CalculateOriginalImageWidthHeight(documentUpload, out ImageWidth, out ImageHeight);
                        dbManager.AddParameters(15, "@ImageWidth", ImageWidth);
                        dbManager.AddParameters(16, "@ImageHeight", ImageHeight);
                    }
                    else
                    {
                        dbManager.AddParameters(15, "@ImageWidth", null);
                        dbManager.AddParameters(16, "@ImageHeight", null);
                    }
                    dbManager.AddParameters(17, "@FolderId", documentUpload.FolderCode);
                    dbManager.AddParameters(18, "@ReviewStatus", documentUpload.ReviewStatus);
                    dbManager.AddParameters(19, "@AllowReviewersToReassignReview", documentUpload.AllowReviewersToReassignReview);
                    dbManager.AddParameters(20, "@NotifyInitiatorWhenReviewComplete", documentUpload.NotifyInitiatorWhenReviewComplete);
                    dbManager.AddParameters(21, "@UseDefaultWorkflow", documentUpload.UseDefaultWorkflow);
                    dbManager.AddParameters(22, "@ReviewerInstruction", documentUpload.ReviewerInstruction);
                    dbManager.AddParameters(23, "@DocumentGuid", Guid.NewGuid().ToString().Replace("-",string.Empty).ToUpper());
                    int documentId = Convert.ToInt32(dbManager.ExecuteScalar(CommandType.Text, documentQuery));
                    return documentId;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private bool IsImageCheck(int documentTypeId)// this is used in upload API
        {
            bool IsImage = false;
            string checkIsImageQuery = accessHelper.GetQueryDetail("CheckIsImage");
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@DocumentTypeId", documentTypeId);
            IDataReader drcheckIsImageQuery = dbManager.ExecuteReader(CommandType.Text, checkIsImageQuery);
            DataTable dtimageTypeDetail = new DataTable();
            dtimageTypeDetail.Load(drcheckIsImageQuery);
            drcheckIsImageQuery.Close();
            if (Convert.ToInt32(dtimageTypeDetail.Rows[0]["TotalCount"]) > 0)
            {
                IsImage = true;
            }

            return IsImage;
        }

        private void ActivityLog(string code, DocumentModel documentUpload)
        {
            //ActivityLogModel activityLogModel = new ActivityLogModel();
            //activityLogModel = activityLogModel.SetModelValue(documentUpload.VersionId, Convert.ToInt32(documentUpload.CreatedBy), "FILEUPLOAD", "Document uploaded.");
            //if (documentUpload.VersionId > 0)
            //{
            //    this._ActivityLogRepository.CreateActivityLog(code, activityLogModel);
            //}
        }
        #endregion

        #region Get Query Details
        //public string accessHelper.GetQueryDetail(string strQueryInput)
        //{
        //    try
        //    {
        //        if (!queryString.ContainsKey(strQueryInput))
        //        {
        //            QueryConfiguration queryConfig = new QueryConfiguration();
        //            queryString = queryConfig.GetAllQuery();
        //        }
        //        return queryString[strQueryInput];
        //    }
        //    catch (Exception e)
        //    {
        //        throw (e);
        //    }
        //}
        #endregion

        public bool DeleteImage(DocumentModel document, string connectionString)
        {
            if (document.Id > 0)
            {
                List<ImageDocument> imageDocument = new List<ImageDocument>();
                if (document.imageDocumentList != null && document.imageDocumentList.Count > 0)
                {
                    imageDocument = GetImageTypeDocumentList(document.DocumentGuid, document.imageDocumentList.Select(n => new ItemType { Type = n.ImageTypeName }).ToList(), connectionString)
                                                    .Select(n => new ImageDocument() { Id = n.Id, ImageName = n.FileName }).ToList();
                }
                using (dbManager = new DBhandler())
                {
                    try
                    {
                        dbManager.ConnectionString = connectionString;
                        dbManager.Open();
                        dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                        ItemCodeValue physicalFolderName = GetFolderDetails(dbManager, document.FolderCode);

                        string documentImageFile = document.FolderPath + "/" + physicalFolderName + "/" + document.FileName;


                        #region Deleting ImageType Documents
                        if (imageDocument.Count > 0)
                        {
                            if (DeleteImageDocuments(dbManager, document, physicalFolderName.Value, imageDocument))
                            {
                                DeleteOriginalDocument(dbManager, document.Id, documentImageFile);
                            }
                        }
                        else
                        {
                            DeleteOriginalDocument(dbManager, document.Id, documentImageFile);
                        }
                        #endregion
                        return true;
                    }
                    catch (Exception ex)
                    {
                        dbManager.Transaction.Rollback();
                        throw;
                    }
                }
            }
            return false;
        }
        public bool DeleteDocument(int docId, string connectionString)
        {
            List<ItemId> docIds = new List<ItemId>();
            docIds.Add(new ItemId() { Id = docId });
            var documents = GetDocumentList(docIds, connectionString);

            if (documents != null && documents.FirstOrDefault().Id > 0)
            {
                DocumentModel document = documents.FirstOrDefault();
                return DeleteDocument(document, connectionString);
            }
            return false;
        }
        private bool DeleteDocument(DocumentModel document, string connectionString)
        {
            List<ImageDocument> imageDocument = new List<ImageDocument>();
            bool returnStatus = false;
            if (document.imageDocumentList != null && document.imageDocumentList.Count > 0)
            {
                imageDocument = GetImageTypeDocumentList(document.DocumentGuid, document.imageDocumentList.Select(n => new ItemType { Type = n.ImageTypeName }).ToList(), connectionString)
                                                .Select(n => new ImageDocument() { Id = n.Id, ImageName = n.FileName }).ToList();
            }
            using (dbManager = new DBhandler())
            {
                try
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    ItemCodeValue physicalFolderName = GetFolderDetails(dbManager, document.FolderCode);

                    string documentImageFile = document.BrowserPath;


                    #region Deleting ImageType Documents
                    if (imageDocument.Count > 0)
                    {
                        if (DeleteImageDocuments(dbManager, document, physicalFolderName.Value, imageDocument))
                        {
                            returnStatus = DeleteOriginalDocument(dbManager, document.Id, documentImageFile);
                        }
                    }
                    else
                    {
                        returnStatus = DeleteOriginalDocument(dbManager, document.Id, documentImageFile);
                    }
                    #endregion
                    if (returnStatus)
                    {
                        dbManager.Transaction.Commit();
                        return true;
                    }
                    else
                    {
                        dbManager.Transaction.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    dbManager.Transaction.Rollback();
                    throw;
                }
            }
        }
        #region Delete ImageDocument
        private bool DeleteImageDocuments(DBhandler dbManager, DocumentModel document, string physicalFolderPath, List<ImageDocument> imageDocument)
        {
            #region Deleting ImageType Documents
            try
            {
                if (document.imageDocumentList != null && document.imageDocumentList.Count > 0)
                {
                    if (DeleteImageDocument(dbManager, imageDocument.Select(n => n.Id).ToList()))
                    {
                        DeleteImageDocumentFromDirectory(imageDocument, document.FolderPath, physicalFolderPath);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            #endregion
        }
        private void DeleteImageDocumentFromDirectory(List<ImageDocument> imageDocument, string folderPath, string physicalFolderPath)
        {
            foreach (var data in imageDocument)
            {
                try
                {
                    if (File.Exists(folderPath + "/" + physicalFolderPath + "/" + data.ImageName))
                    {
                        File.Delete(folderPath + "/" + physicalFolderPath + "/" + data.ImageName);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private bool DeleteImageDocument(DBhandler dbManager, List<int> imageIds)
        {
            DMSHelper dMSUtilities = new DMSHelper();
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@Ids", string.Join(",", imageIds));
            string query = dMSUtilities.accessHelper.GetQueryDetail("DeleteImageDocument");
            int effectedRows = dbManager.ExecuteNonQuery(CommandType.Text, query);
            if (effectedRows > 0)
            {
                return true;
            }
            return false;
        }
        #endregion
        private bool DeleteOriginalDocument(DBhandler dbManager, int docId, string documentImageFile)
        {
            DMSHelper dMSUtilities = new DMSHelper();
            dbManager.CreateParameters(1);
            dbManager.AddParameters(0, "@docId", docId);
            string query = dMSUtilities.accessHelper.GetQueryDetail("DeleteOriginalDocument");
            int effectedRows = dbManager.ExecuteNonQuery(CommandType.Text, query);
            if (effectedRows > 0)
            {
                return DeleteOriginalDocumentFromDirectory(documentImageFile);
            }
            return false;
        }
        private bool DeleteOriginalDocumentFromDirectory(string documentImageFile)
        {
            if (File.Exists(documentImageFile))
            {
                File.Delete(documentImageFile);
                return true;
            }
            return false;
        }

        public bool DeleteInActiveFolders(string connectionString)
        {
            bool isDeleted = false;
            using (dbManager = new DBhandler())
            {
                try
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    dbManager.Transaction = dbManager.Connection.BeginTransaction();
                    string deleteFolderQuery = accessHelper.GetQueryDetail("GetDeletedFolders");
                    if (dbManager.ExecuteNonQuery(CommandType.Text, deleteFolderQuery) > 0)
                    {
                        if (Directory.Exists(accessHelper.BrowseURL + "/" + ""))
                        {
                            try
                            {
                                Directory.Delete(accessHelper.BrowseURL + "/" + "");
                                dbManager.Transaction.Commit();
                                isDeleted = true;
                            }
                            catch (Exception)
                            {
                                dbManager.Transaction.Rollback();
                                isDeleted = false;
                            }
                        }
                        else
                        {
                            isDeleted = false;
                            dbManager.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        isDeleted = false;
                        dbManager.Transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    dbManager.Transaction.Rollback();
                    throw ex;
                }
            }
            return isDeleted;
        }
        public bool DeleteFolder(string folderCode, string connectionString)
        {
            bool isDeleted = false;
            using (dbManager = new DBhandler())
            {
                try
                {
                    dbManager.ConnectionString = connectionString;
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@folderCode", folderCode);
                    string deleteFolderQuery = accessHelper.GetQueryDetail("DeleteFolder");
                    if (dbManager.ExecuteNonQuery(CommandType.Text, deleteFolderQuery) > 0)
                    {
                        isDeleted = true;
                    }
                    else
                    {
                        isDeleted = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return isDeleted;
        }

        public int CreateFolder(FolderModel folderModel, string connectionString)
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
                    dbManager.AddParameters(3, "@PhysicallFolderName", folderModel.PhysicallFolderName);



                    dbManager.AddParameters(4, "@ParentId", folderId);
                    dbManager.AddParameters(5, "@CreatedBy", folderModel.CreatedBy);
                    dbManager.AddParameters(6, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    object returnId = dbManager.ExecuteScalar(CommandType.Text, addFolderquery);
                    if (Convert.ToInt32(returnId) > 0)
                    {
                        string virtualPath = accessHelper.PhysicalPath;
                        string companyTenantPath = virtualPath;
                        if (!Directory.Exists(companyTenantPath))
                        {
                            Directory.CreateDirectory(companyTenantPath);
                        }
                        string folderPath = companyTenantPath + "/" + folderModel.PhysicallFolderName;
                        System.IO.Directory.CreateDirectory(folderPath);
                        returnFolderId = Convert.ToInt32(returnId);
                    }
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}


