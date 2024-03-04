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
    public class AttributeValueBusiness : IAttributeValueBusiness
    {
        #region variable Declaration
        private readonly IAttributeValueRepository _AttributeValueRepository;    
        #endregion

        #region Constructor
        public AttributeValueBusiness(IAttributeValueRepository attributeValueRepository)    
        {
            this._AttributeValueRepository = attributeValueRepository;  
        }
        #endregion

        #region Post
        public bool AddAttributeValue(AttributeValue attributeValue)  
        {
            return this._AttributeValueRepository.AddAttributeValue(attributeValue);    
        }
        #endregion
    }
}
