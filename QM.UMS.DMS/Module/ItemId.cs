using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.eBook.DMS.Module
{
    public class ItemId
    {
        public int Id { get; set; }
    }
    public class ItemCodeValue :ItemId
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }
}
