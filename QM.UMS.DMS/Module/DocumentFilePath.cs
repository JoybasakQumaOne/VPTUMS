using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class DocumentFilePath
    {
        public int DocumentId { get; set; }
        public string FilePath { get; set; }
        public string OriginalImageName { get; set; }
        public List<BuildPath> Image { get; set; }
    }

    public class BuildPath
    {
        public string ImageType { get; set; }
        public string ImageName { get; set; }
    }
}
