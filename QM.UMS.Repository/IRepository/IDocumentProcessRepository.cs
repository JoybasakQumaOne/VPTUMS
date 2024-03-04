
namespace QM.UMS.Repository.IRepository
{
    using QM.eBook.DMS.Module;
    using QM.UMS.DMS.Module;
    #region Namespace
    using QM.UMS.Model;
    using System;
    using System.Collections.Generic;
    #endregion

    public interface IDocumentProcessRepository
    {
        DocumentModel GetDocumentByDocumentguid(string documentguid, string connectinString, bool isLocalpathRequired = false);
        DocumentModel GetDocumentByObjectId(int objectId,string ObjectName,string documentType, string connectionString);
        List<DocumentViewModel> GetImagesByType(int objectId, string imageType, string documentType, int index, int range, string objectType);

        List<DocumentViewModel> GetImagesByDocumentType(string documentType);

        List<DocumentModel> GetDocumentsOnDocumentType(string documentType);
        List<ImageDocumentUpload> GetDocumentListImages(int objectId, string imageType, string documentType, bool? isDefault);
		
        dynamic GetImageGalleryDetails(int objectId, string documentType);

        int ImageProcess(ImageDocumentUpload imageDocumentUpload);

		List<DocumentModel> GetDocByDocIdList(string code, List<ItemId> documentId, string imageType, string connectionString);
		
		List<DocumentFileMap> GetDocumentByIdList(int objectId, string imageType, string DocumentType, bool? isDefault);

        int UploadImage(string code, DocumentModel documentModel);
        string UploadDocument(string code, DocumentModel documentModel, string connectionString);
        int InsertDocument(DocumentModel documentUpload, string connection);
        int InsertDocumentFromFile(string filePath, int mappingId, string fileName, string documentTypeCode, string objectName, string folderCode, string connection);
        bool DeleteImage(DocumentModel documentModel, string connectionString,string code);

		bool DeleteImage(int docId, string connectionString);

		bool DeleteDocument(int objectId, string objectType, string documentType, bool IsDefault);

		bool DeleteDocument(string DocumentId, string connectionString);
        bool DeleteImage(string DocumentId, string connectionString);

		bool DeleteFolder(string folderCode, string connectionString);
			
		DocumentModel BuildDocumentModel(dynamic ImageDocModel);
        DocumentModel BuildDocumentModel(dynamic ImageDocModel, DocumentModel document);
        DMSDocumentServiceModel GenerateDocumentModel(dynamic ImageDocModel);
		int CopyImage(int documetId, string folderCode, string connectionString);
		List<DocumentFileMap> GetDocumentFileMaps(int objectId, string imageType, string documentType, bool? isDefault);

		string GetFilePath(int objectId, string imageType, string documentType, bool? isDefault, string connectionString="");

		string GetFilePath(int docId, string connectionString);
        string GetFilePath(string docId, string connectionString);

        int UploadSettingsImage(ImageDocumentUpload imageUploadDocument, Guid bookGuid, string documentType, string connectionString);


        #region Post Method
        string UploadBookBackgroudImage(ImageDocumentUpload image);
        #endregion

        #region Create Folder
        int CreateFolder(FolderModel folderModel, string connectionString, string Code);
        #endregion

        #region Document Mapping
        DocumentMapping MappingModelBuild(int documentId, int objectId, string documentTypeCode, bool isDefault, string objectType);

        int UpdateDocumentMapping(DocumentMapping documentMapping);
        #endregion
    }
}
