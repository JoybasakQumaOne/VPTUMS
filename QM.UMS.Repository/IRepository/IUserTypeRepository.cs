namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IUserTypeRepository
    {
        List<UserType> Get();

        UserType Get(int id);

        bool AddUserType(UserType user);

        bool UpdateUserType(UserType user);

        bool LinkAttribute(int userTypeId, int attributeId);
    }
}
