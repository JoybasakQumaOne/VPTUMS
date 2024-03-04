using CommonApplicationFramework.Common;
using System.ComponentModel.DataAnnotations;

namespace QM.UMS.Models
{
    public class ActionModel : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Action name is required")]
        [RegularExpression("^[a-zA-Z \\`_~!|@#$%^&*()\"':;><.,?*\\/]{1,50}$", ErrorMessage = "Action Name, Must be between 1 to 50 characters and should not contain number.")]
        public string ActionCode { get; set; }

        public string Description { get; set; }

        public int ModuleId { get; set; }

        public string Status { get; set; }

        public int? ParentId { get; set; }

        public int? OrderIndex { get; set; }
    }
}