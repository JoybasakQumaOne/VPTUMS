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

    public interface IAttributeMasterRepository  
    {
        bool AddAttribute(AttributeMaster attribute);

        List<AttributeMaster> Get();

        AttributeMaster Get(int id);

        bool UpdateAttribute(AttributeMaster attribute);

        bool DeleteAttribute(int id);

        
    }
}
