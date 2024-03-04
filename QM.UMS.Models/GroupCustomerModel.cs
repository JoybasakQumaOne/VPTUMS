using CommonApplicationFramework.Common;
using System;
namespace QM.UMS.Models
{
    public class GroupCustomerModel : BaseModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int GroupId { get; set; }
    }
}
