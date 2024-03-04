using QM.UMS.Business.IBusiness;
using QM.UMS.Models;
using QM.UMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Business.Business
{
   public  class DesignationBusiness:IDesignationBusiness
    {
         #region Variable Declaration
        /// <summary>
        /// Variable Declaration
        /// </summary>
       private readonly IDesignationRepository _IDesignationRepository;
        #endregion

        #region Constructor
        /// <summary>
        /// Register Parametarized Constructor
        /// </summary>
        /// <param name="_prmIHttpClientBusiness">_prmIHttpClientBusiness</param>
       public DesignationBusiness(IDesignationRepository _iDesignationRepository)
        {
            this._IDesignationRepository = _iDesignationRepository;
        }
        #endregion

        #region GET Methods

        /// <summary>Get list of Department Details</summary>
        /// <param name="code">code in Entity</param>
        /// <returns>Returns List of Department Details</returns>        
       public List<DesignationModel> GetAllDesignation()
        {
           
            return this._IDesignationRepository.GetAllDesignation();
        }

        /// <summary>Get Department Details by Id</summary>
        /// <param name="code">code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>Returns single entity of Department</returns>
       public DesignationModel GetDesignation(string code, int designationId)
        {
            return this._IDesignationRepository.GetDesignation(code, designationId);
        }

        #endregion

        #region POST Methods

        /// <summary>Add Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="dept">dept is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>        
        public int AddDesignation(string code, DesignationModel designation)
        {
            return this._IDesignationRepository.AddDesignation(code, designation);
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="department">department is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>        
        public bool UpdateDesignation(string code, DesignationModel designation)
        {
            return this._IDesignationRepository.UpdateDesignation(code, designation);
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>returns bool value</returns>        
        public bool DeleteDesignation(string code, int designationId)
        {
            return this._IDesignationRepository.DeleteDesignation(code, designationId);
        }

        #endregion

        /// <summary>
        /// Register Void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
