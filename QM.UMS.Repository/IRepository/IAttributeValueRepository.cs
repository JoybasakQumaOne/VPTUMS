namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion
    public interface IAttributeValueRepository
    {
        bool AddAttributeValue(AttributeValue attributeValue);        
    }
}
