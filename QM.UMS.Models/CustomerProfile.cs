using CommonApplicationFramework.Common;
using QM.eBook.DMS.Module;
using QM.UMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Models
{
    public class CustomerProfile
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public ImageDocumentUpload Image { get; set; }
        public DateTime? DOB { get; set; }
		public ItemCode Country { get; set; }

		public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
