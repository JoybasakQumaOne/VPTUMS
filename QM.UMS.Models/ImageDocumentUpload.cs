namespace QM.UMS.Model
{
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.Common.DMS;
    using System;
    #region Namespace
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class ImageDocumentUpload : BaseModel
    {
        //Start Used for Book Image Upload Purpose
        public bool IsCopyRequired { get; set; }
        public string ObjectType { get; set; }
        public bool IsDefault { get; set; }
        public int ObjectId { get; set; }
        public string ObjectGuid { get; set; }
        public int Id { get; set; }
        public string DocumentTypeCode { get; set; }
        public bool IsNewEntry { get; set; }
        public bool IsNewVersion { get; set; }
        public bool IsOverride { get; set; }
        public string FileName { get; set; }
        public int FileFormat { get; set; }
        public string FilePath { get; set; }
        public string File { get; set; }
        public string FolderCode { get; set; }
        public List<ImageContentDocument> ImageDocuemntList { get; set; }
        public string DocumentGuid { get; set; }

    }
    public class ImageContentDocument
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }

        public string ImageName { get; set; }

        public int ImageTypeId { get; set; }

        public string FilePath { get; set; }

        public string ImageTypeName { get; set; }
    }
    public class DocumentViewModel
	{
		/// <summary>
		/// Gets and sets Id
		/// </summary>
		public string DocumentId { get; set; }
		/// <summary>
		/// Gets and sets File Name
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// Gets and sets File Path
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// Gets and Sets File Type
		/// </summary>
		public string FileType { get; set; }
		/// <summary>
		/// Gets and Sets object details
		/// </summary>	 
		public bool IsDefault { get; set; }
		public int ObjectId { get; set; }
	}
    public class DocumentFileMap
    {
        public int objectId { get; set; }
        public ItemId documentId { get; set; }
        public string filePath { get; set; }

    }

    public class DocumentProperty : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }

        public string Keywords { get; set; }

        public string DocumentNumber { get; set; }

        public int VersionId { get; set; }

        public decimal Version { get; set; }

        public string DocumentVersionName { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string Comment { get; set; }

        public double Cost { get; set; }

        public string Currency { get; set; }

        public double? FileSize { get; set; }

        public int? ParentId { get; set; }

        public int DocumentTypeId { get; set; }
    }

    public class DMSDocumentServiceModel : DocumentProperty
    {
        public string DocumentType { get; set; }// Required

        public string DocumentTypeCode { get; set; }// Required

        public int FileFormat { get; set; }// Required

        public string FileName { get; set; }// Required

        public string ActivetedFileName { get; set; }

        public string File { get; set; }// Required

        public string FilePath { get; set; }

        public ObjectModel ObjectDetails { get; set; }

        public List<object> DocMetaData { get; set; } //MetaData

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        [DefaultValue(false)]
        public bool IsOverride { get; set; }

        public ImageDocument imageDocument { get; set; }

        public ImageType imageType { get; set; }

        public List<ImageDocument> imageDocumentList { get; set; }

        public decimal? MinImageWidth { get; set; }

        public decimal? MinImageHeight { get; set; }

        public decimal? MaxImageWidth { get; set; }

        public decimal? MaxImageHeight { get; set; }

        public int FolderId { get; set; } // Required
        public string FolderCode { get; set; }// Required

        [DefaultValue(false)]
        public bool IsNewVersion { get; set; }

        [DefaultValue(false)]
        public bool IsNewEntry { get; set; }

        public string ReviewStatus { get; set; }
        public bool? AllowReviewersToReassignReview { get; set; }
        public bool? NotifyInitiatorWhenReviewComplete { get; set; }
        public bool? UseDefaultWorkflow { get; set; }
        public string ReviewerInstruction { get; set; }

        public string FolderPath { get; set; }
        public string BrowserPath { get; set; }
    }
    public class ImageObjectDetails
    {
        public List<int> ObjectId { get; set; }
        public string DocumentTypeCode { get; set; }
        public string ObjectType { get; set; }
        public int DocumentId { get; set; }
    }
    public class DocumentMapping
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets DocumentId
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Gets or sets ObjectId
        /// </summary>
        public int ObjectId { get; set; }
        /// <summary>
        /// Gets or sets ObjectType
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// Gets or sets DocumentType
        /// </summary>
        public string DocumentType { get; set; }
        /// <summary>
        /// Gets or sets IsDefault
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// Gets or sets IsDefault
        /// </summary>
        public string Caption { get; set; }
    }
}
