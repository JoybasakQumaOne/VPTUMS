namespace QM.UMS.Business.IBusiness
{
    #region Namepsace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IInstanceBusiness
    {
        List<Item> GetInstance();

        bool MapUserInstance(int userId, int instanceId);

        bool UnMapUserInstance(int userId, int instanceId);
    }
}
