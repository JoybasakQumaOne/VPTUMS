namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IAttributeOptionBusiness
    {
        bool AddAttributeOptions(Item attributeOptions);

        List<Item> GetAttributeOptions(int id);

        bool UpdateAttributeOptions(Item attributeOptions);

        bool DeleteAttributeOptions(int id);
    }
}
