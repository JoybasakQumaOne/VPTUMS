namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IAttributeValueBusiness
    {
        bool AddAttributeValue(AttributeValue attributeValue);  
    }
}
