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
    public class DepartmentBusiness : IDepartmentBusiness
    {
          #region Variable Declaration
        /// <summary>
        /// Variable Declaration
        /// </summary>
       private readonly IDepartmentRepository _IDepartmentRepository;
        #endregion

        #region Constructor
        /// <summary>
        /// Register Parametarized Constructor
        /// </summary>
        /// <param name="_prmIHttpClientBusiness">_prmIHttpClientBusiness</param>
       public DepartmentBusiness(IDepartmentRepository _iDepartmentRepository)
        {
            this._IDepartmentRepository = _iDepartmentRepository;
        }
        #endregion

        #region GET Methods

        /// <summary>Get list of Department Details</summary>
        /// <param name="code">code in Entity</param>
        /// <returns>Returns List of Department Details</returns>        
        public List<DepartmentModel> GetAllDepartment()
        {
            return this._IDepartmentRepository.GetAllDepartment();
        }

        /// <summary>Get Department Details by Id</summary>
        /// <param name="code">code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>Returns single entity of Department</returns>
        public DepartmentModel GetDepartment(string code, int departmentId)
        {
          
            return this._IDepartmentRepository.GetDepartment(code,departmentId);
        }

        #endregion

        #region POST Methods

        /// <summary>Add Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="dept">dept is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>        
        public int AddDepartment(string code, DepartmentModel department)
        {
            return this._IDepartmentRepository.AddDepartment(code, department);
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="department">department is object of DepartmentModel</param>
        /// <returns>Returns bool value</returns>        
        public bool UpdateDepartment(string code, DepartmentModel department)
        {
            return this._IDepartmentRepository.UpdateDepartment(code, department);
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Department Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="departmentId">Department Id</param>
        /// <returns>returns bool value</returns>        
        public bool DeleteDepartment(string code, int departmentId)
        {
            return this._IDepartmentRepository.DeleteDepartment(code, departmentId);
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
