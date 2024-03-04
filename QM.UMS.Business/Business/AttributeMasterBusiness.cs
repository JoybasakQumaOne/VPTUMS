namespace QM.UMS.Business.Business
{
    #region Namespace
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class AttributeMasterBusiness : IAttributeMasterBusiness  
    {
        #region variable Declaration
        private readonly IAttributeMasterRepository _AttributeMasterRepository;    
        #endregion

        #region Constructor
        public AttributeMasterBusiness(IAttributeMasterRepository attributeMasterRepository)    
        {
            this._AttributeMasterRepository = attributeMasterRepository;  
        }
        #endregion

        #region Post
        public bool AddAttribute(AttributeMaster attribute)
        {
            return this._AttributeMasterRepository.AddAttribute(attribute);
        }
        #endregion

        #region Get
        public List<AttributeMaster> Get()
        {
            return this._AttributeMasterRepository.Get();
        }
        #endregion

        #region GetById
        public AttributeMaster Get(int id)
        {
            return this._AttributeMasterRepository.Get(id);
        }
        #endregion

        #region Put
        public bool UpdateAttribute(AttributeMaster attribute)
        {
            return this._AttributeMasterRepository.UpdateAttribute(attribute);
        }
        #endregion

        #region Delete
        public bool DeleteAttribute(int id)
        {
            return this._AttributeMasterRepository.DeleteAttribute(id);
        }
        #endregion
        
        
    }
}
