using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class BaseModel
    {
        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public string Creator { get; set; }

        public string Modifier { get; set; }

    }
}
