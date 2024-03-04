namespace QM.UMS.Business.Business
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class UserTypeBusiness : IUserTypeBusiness
    {
        #region variable Declaration
        private readonly IUserTypeRepository _UserTypeRepository;    
        #endregion

        #region Constructor
        public UserTypeBusiness(IUserTypeRepository userTypeRepository)    
        {
            this._UserTypeRepository = userTypeRepository;  
        }
        #endregion

        #region Get
        public List<UserType> Get()
        {
            return this._UserTypeRepository.Get();
        }
        #endregion

        #region Get
        public UserType Get(int id)
        {
            return this._UserTypeRepository.Get(id);
        }
        #endregion

        #region Post
        public bool AddUserType(UserType user)
        {
            return this._UserTypeRepository.AddUserType(user);
        }
        #endregion

        #region Put
        public bool UpdateUserType(UserType user)
        {
            return this._UserTypeRepository.UpdateUserType(user);
        }
        #endregion

        #region AttributeMapping
        public bool LinkAttribute(int userTypeId, int attributeId)
        {
            return this._UserTypeRepository.LinkAttribute(userTypeId, attributeId);
        }
        #endregion
    }
}
