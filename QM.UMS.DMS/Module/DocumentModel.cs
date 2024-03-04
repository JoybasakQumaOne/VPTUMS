using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class DocumentModel : DocumentProperties
    {


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
        public ResizeImageModel ResizeProperty { get; set; }
        public string DocumentGuid { get; set; }
    }
    
    public class ResizeImageModel
    {
        public string ResizeOrientation { get; set; }
        public int ResizeDimension { get; set; }
        public int ResizeByPercentage { get; set; }
    }
}
