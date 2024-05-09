using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.IRepository
{
    public interface IEmailHandler
    {
        void TriggerNewRegistrationEmail(RegistrationMailData data);
        void TriggerExistsRegistrationEmail(RegistrationMailData data);
        void TriggerForgotPasswordMail(string email);
        void TriggerChangePasswordMail(string email);
        void TriggerAdminResetMail(RegistrationMailData rdm);
    }
}
