using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.IRepository
{
  public  interface IDesignationRepository
    {
      List<DesignationModel> GetAllDesignation();  
      DesignationModel GetDesignation(string code, int DesignationId);
      int AddDesignation(string code, DesignationModel department);
      bool UpdateDesignation(string code, DesignationModel designation);
      bool DeleteDesignation(string code, int departmentId);
    }
}
