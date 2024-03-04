using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Business.IBusiness
{
   public  interface IDesignationBusiness
    {
       List<DesignationModel> GetAllDesignation();
       DesignationModel GetDesignation(string code, int departmentId);
       int AddDesignation(string code, DesignationModel department);
       bool UpdateDesignation(string code, DesignationModel designation);
       bool DeleteDesignation(string code, int departmentId);
    }
}
