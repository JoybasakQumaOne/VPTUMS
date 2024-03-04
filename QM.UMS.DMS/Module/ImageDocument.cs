using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class ImageDocument
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }

        public string ImageName { get; set; }

        public int ImageTypeId { get; set; }

        public int DocumentVersionId { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public string FilePath { get; set; }

        public string ImageTypeName { get; set; }
    }
}
