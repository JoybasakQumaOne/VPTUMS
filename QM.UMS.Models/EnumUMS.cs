#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace QM.UMS.Models
{    
    public enum GroupNames { DemoGroupss, SuperAdminGroup, OrgAdminGroup, CustomerGroup, GuestGroup,Registered,Subscribed }
    public enum UserTypes { Client,Admin,Moderator}
    public enum ActivityType
    {
        PASSWORDCHANGED,PASSWORDRESET
    }
}
