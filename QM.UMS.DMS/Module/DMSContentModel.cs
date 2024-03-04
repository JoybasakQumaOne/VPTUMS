using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class DMSContentModel
    {
        public string Code { get; set; }
        public string ConnectionString { get; set; }
        public string FileServerPath { get; set; }
        public string BrowerServicePath { get; set; }
        public DocumentModel DMSDocumentModel { get; set; }
    }
}
