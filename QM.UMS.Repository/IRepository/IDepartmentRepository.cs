using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.IRepository
{
 public  interface IDepartmentRepository
    {
        #region GET Methods

        List<DepartmentModel> GetAllDepartment();

        DepartmentModel GetDepartment(string code, int departmentId);

        #endregion

        #region POST Methods

        int AddDepartment(string code, DepartmentModel department);

        #endregion

        #region PUT Methods

        bool UpdateDepartment(string code, DepartmentModel department);

        #endregion

        #region DELETE Methods

        bool DeleteDepartment(string code, int departmentId);

        #endregion
    }
}
