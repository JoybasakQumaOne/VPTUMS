using System.Collections.Generic;

namespace QM.UMS.Models
{
    public class AttributeGroup
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }

        public List<UserAttribute> Atrributes { get; set; }
    }


}
