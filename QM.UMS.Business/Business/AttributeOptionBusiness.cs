namespace QM.UMS.Business.Business
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class AttributeOptionBusiness : IAttributeOptionBusiness
    {
        #region variable Declaration
        private readonly IAttributeOptionRepository _AttributeOptionsRepository;    
        #endregion

        #region Constructor
        public AttributeOptionBusiness(IAttributeOptionRepository attributeOptionsRepository)    
        {
            this._AttributeOptionsRepository = attributeOptionsRepository;  
        }
        #endregion

        #region GetAllById
        public List<Item> GetAttributeOptions(int id)
        {
            return this._AttributeOptionsRepository.GetAttributeOptions(id);
        }
        #endregion

        #region Post
        public bool AddAttributeOptions(Item attributeOptions)
        {
            return this._AttributeOptionsRepository.AddAttributeOptions(attributeOptions);
        }
        #endregion

        #region Put
        public bool UpdateAttributeOptions(Item attributeOptions)
        {
            return this._AttributeOptionsRepository.UpdateAttributeOptions(attributeOptions); 
        }
        #endregion

        #region Delete
        public bool DeleteAttributeOptions(int id)
        {
            return this._AttributeOptionsRepository.DeleteAttributeOptions(id);
        }
        #endregion
    }
}
