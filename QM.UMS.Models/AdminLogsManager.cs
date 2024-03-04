
using CommonApplicationFramework.Common;

namespace QM.UMS.Models
{
    public class AdminLogsManager : BaseModel
    {
        public int UserId { get; set; }

        public string Module { get; set; }

        public int Activity { get; set; }

        public string Message { get; set; }

        public string IPAddress { get; set; }
    }
}