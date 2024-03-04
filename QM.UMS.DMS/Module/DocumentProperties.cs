using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class DocumentProperties : BaseModel
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
        public string DocumentTypeCode { get; set; }

    }
}
