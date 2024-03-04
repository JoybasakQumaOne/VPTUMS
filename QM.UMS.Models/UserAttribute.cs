using System.Collections.Generic;
using CommonApplicationFramework.Common;

namespace QM.UMS.Models
{
    public class UserAttribute : BaseModel
    {
        public AttributeMaster Attribute { get; set; }
        //public List<UserAttributeValue> AttributeData { get; set; }
    }

    public class UserAttributeValue
    {
        public int AttributeId { get; set; }
        public int AttributeValueId  { get; set; }
        public int AttributeOptionId { get; set; }
        public string AttributeValue { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RecordSet
    {
        public long RowCollectionId { get; set; }
        public int  SectionId { get; set; }
        public List<UserAttributeValue> UserAttributeValues { get; set; }
    }

}
