using System.Collections.Generic;

namespace QM.UMS.Models
{
    public class UserType
    {
        public int Id { get; set; }
        public string Name { get; set; }  
        public bool IsActive { get; set; }  

        public List<AttributeSection> AttrGroups { get; set; }
    }
}
