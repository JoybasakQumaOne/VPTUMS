using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.IRepository
{
    public interface IUserAppRepository
    {
        bool MapControlUserApp(string email);
        bool MapUserApp(string email);
        AppModel GetAppInfo();
    }
}
