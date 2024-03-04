using System.Collections.Generic;

namespace QM.UMS.Models
{
    public class AttributeValue
    {
        public int UserId { get; set; }

        public int SectionId { get; set; }

        public int RowCollectionId { get; set; }

        public string RecordStatus { get; set; }

        public List<AttributeRecordSet> AttributeRecords { get; set; }
       
    }

    public class AttributeRecordSet
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user attribute mapping identifier
        /// </summary>
        public int UserAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the user attribute option mapping identifier
        /// </summary>
        public int UserAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the attribute value type identifier
        /// </summary>
        public string AttributeVal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the Action
        /// </summary>
        public string Action { get; set; }
    }

    
}
