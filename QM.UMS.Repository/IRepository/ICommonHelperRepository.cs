using CommonApplicationFramework.Notification;
using QM.UMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.UMS.Repository.IRepository
{
    public interface ICommonHelperRepository
    {
        void UpdateActivity(string activityName, string message);

        void AdminPasswordResetMail(EmailSenderModel user);
        void ChangePasswordMail(EmailSenderModel user);
        void ForgotPasswordMail(EmailSenderModel user,string type);
        void StudentRegistrationMail(EmailSenderModel user);
        void AdminAddUserMail(EmailSenderModel user);
        void OrganizationRegistrationMail(EmailSenderModel user);
        void OrganizationRegistrationWithExistingUserMail(EmailSenderModel user);

        //void TriggerNewRegistrationEmail(RegistrationMailData data);
        //void TriggerExistsRegistrationEmail(RegistrationMailData data);
        //void TriggerForgotPasswordMail(string email);
        //void TriggerChangePasswordMail(string email);
        //void TriggerAdminResetMail(string email, string password);
    }
}
