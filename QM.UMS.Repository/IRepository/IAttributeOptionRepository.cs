namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IAttributeOptionRepository
    {
        bool AddAttributeOptions(Item attributeOptions);

        bool UpdateAttributeOptions(Item attributeOptions);  

        List<Item> GetAttributeOptions(int Id);

        bool DeleteAttributeOptions(int id);  
    }
}
