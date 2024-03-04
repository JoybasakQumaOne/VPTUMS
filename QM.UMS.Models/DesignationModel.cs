using CommonApplicationFramework.Common;
using System.ComponentModel.DataAnnotations;

namespace QM.UMS.Models
{
  public  class DesignationModel:BaseModel
    {
      public int DesignationId { get; set; }

        [Required(ErrorMessage = "Designation Name is required.")]
        //  [StringLength(100, MinimumLength = 1, ErrorMessage = "Department Name, Must be between 1 and 100 characters.")]
        [RegularExpression("^([^<>'!@$%^*]){1,100}$", ErrorMessage = "Designation Name, Special characters are not allowed.")]
        public string DesignationName { get; set; }
        public ItemCode Module { get; set; }
        public bool Status { get; set; }
    }
}
