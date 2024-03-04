namespace QM.UMS.Business.Business
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion
    public class InstanceBusiness : IInstanceBusiness
    {
        #region Variable Declaration
        private readonly InstanceRepository _IInstanceRepository;  
        #endregion
        public InstanceBusiness(InstanceRepository _iInstanceRepository)  
        {
            this._IInstanceRepository = _iInstanceRepository;
        }

        #region GET
        public List<Item> GetInstance()
        {
            return this._IInstanceRepository.GetInstance();
        }
        #endregion

        #region MAPPING USER INSTANCE
        public bool MapUserInstance(int userId, int instanceId)
        {
            return this._IInstanceRepository.MapUserInstance(userId, instanceId);
        }

        #region UNMAPPING USER INSTANCE
        public bool UnMapUserInstance(int userId, int instanceId)
        {
            return this._IInstanceRepository.UnMapUserInstance(userId, instanceId);
        }
        #endregion

        #endregion
    }
}
