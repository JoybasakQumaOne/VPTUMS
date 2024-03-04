using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.DMS.Module
{
    public class FolderModel : FolderDetails
    {
        public string FolderName { get; set; }
        public string FolderCode { get; set; }
        public string ParentFolderCode { get; set; }
        public string PhysicallFolderName { get; set; }
        public int ParentId { get; set; }
        public bool IsVisible { get; set; }
        public List<FolderDetails> children { get; set; }
        public FolderModel()
        {
            children = new List<FolderDetails>();
        }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }

    public class FolderDetails
    {
        public int id { get; set; }
        public string value { get; set; }

    }

    public class BaseFolderModel
    {
        /// <summary>
        /// Gets or sets the  folder name
        /// </summary>
        public string FolderName { get; set; }
        /// <summary>
        /// Gets or sets the  folder code
        /// </summary>
        public string FolderCode { get; set; }
        /// <summary>
        /// Gets or sets the parent id
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// Gets or sets the parent folder code
        /// </summary>
        public string ParentFolderCode { get; set; }

        public int? CreatedBy { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
