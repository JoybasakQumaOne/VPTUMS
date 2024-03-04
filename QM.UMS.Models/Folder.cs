namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
    #endregion

    public class Foldermodel
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
