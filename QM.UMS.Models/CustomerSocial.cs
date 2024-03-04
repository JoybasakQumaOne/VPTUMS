using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Models
{
    public class CustomerSocial
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string SocialMediaTypeId { get; set; }
        public string SocialMediaTypeName { get; set; }
    }
}
