using System.Collections.Generic;

namespace QM.UMS.Models
{
    public class AttributeSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsRepeated { get; set; }
        public List<UserAttribute> Atrributes { get; set; }
        public List<RecordSet> AttributeValue { get; set; }
    }

}
