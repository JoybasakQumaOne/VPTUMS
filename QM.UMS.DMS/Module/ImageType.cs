using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class ImageType : BaseModel
    {
        public int ImageTypeId { get; set; }

        public string ImageTypeName { get; set; }

        public bool IsUserUploaded { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public int DocumentTypeId { get; set; }

        public int? ParentImageTypeId { get; set; }

        public int DocumentId { get; set; }

        public string DocumentTypeName { get; set; }

        public string DocumentTypeCode { get; set; }

        public string ImageFile { get; set; }
    }
}
