using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Models
{
    public class UserAttributeValue
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user attribute mapping identifier
        /// </summary>
        public int UserAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the attribute value type identifier
        /// </summary>
        public string AttributeValue { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Gets or sets the AttributeOptionId
        /// </summary>
        public int AttributeOptionMasterId { get; set; }
    }
}
