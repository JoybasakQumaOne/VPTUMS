namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class UserAttributeMaster : BaseModel
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [Required(ErrorMessage = "Label is required")]
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the IsActive property
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets the CategoryId
        /// </summary>
        public int? GroupId { get; set; }  
        /// <summary>
        /// Gets or sets the attributeControl
        /// </summary>
        [Required(ErrorMessage = "Control type is required")]
        public Item AttributeControl { get; set; }
        /// <summary>
        /// Gets or sets the IsRepeated
        /// </summary>
        [Required(ErrorMessage = "Repeat view is required")]
        public bool IsRepeated { get; set; }  
        /// <summary>
        /// Gets or sets the IsRequired
        /// </summary>
        public bool IsRequired { get; set; }  
        /// <summary>
        /// Gets or sets the DispalyOrder
        /// </summary>
        public int DispalyOrder { get; set; }
    }
}
