using CommonApplicationFramework.ConfigurationHandling;
using CommonApplicationFramework.DataHandling;
using CommonApplicationFramework.ExceptionHandling;
using CommonApplicationFramework.Logging;
using CommonApplicationFramework.Notification;
using QM.UMS.Models;
using QM.UMS.Repository.Helper;
using QM.UMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace QM.UMS.Repository.Repository
{
    public class CommonHelperRepository : RequestHeader,ICommonHelperRepository
    {
        private DBManager dbManager;

        public void UpdateActivity(string activityName, string message)
        {
            try
            {
                List<string> allowedActivities = Enum.GetNames(typeof(ActivityType)).ToList();

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (allowedActivities.Where(n => n.Equals(activityName)).Any())
                    {
                        string query_ = QueryConfig.ControlMasterQuerySettings["UpdateActivity"].ToString();
                        dbManager.CreateParameters(4);
                        dbManager.AddParameters(0, "@BookId", !string.IsNullOrEmpty(this.BookGuid) ? this.BookGuid : null);
                        dbManager.AddParameters(1, "@UserId", Context.UserId);
                        dbManager.AddParameters(2, "@Activity", activityName);
                        dbManager.AddParameters(3, "@UserAgent", this.UserAgent);
                        dbManager.ExecuteNonQuery(CommandType.Text, query_);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
            }
        }


        public void AdminPasswordResetMail(EmailSenderModel user)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = " Successfully changed Password";
            var txtMsg = string.Empty; var templateFile = string.Empty;
                templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AdminResetPassword")).Value.ToString();
                txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
                txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("LoginURL")).Value.ToString());
                    txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[email]", user.EmailId);
            txtMsg = txtMsg.Replace("[EmailAddress]", user.EmailId);
                txtMsg = txtMsg.Replace("[Password]", user.Password);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));

            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }

        public void ChangePasswordMail(EmailSenderModel user)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = " Successfully changed Password";
            var txtMsg = string.Empty; var templateFile = string.Empty;
            templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ChangedPasswordEmail")).Value.ToString();
            txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
            txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("LoginURL")).Value.ToString());
            txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[email]", user.EmailId);
            txtMsg = txtMsg.Replace("[EmailAddress]", user.EmailId);
            txtMsg = txtMsg.Replace("[Password]", user.Password);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));


            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }

        public void ForgotPasswordMail(EmailSenderModel user, string type)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = "Reset password";
            var txtMsg = string.Empty; var templateFile = string.Empty;
            templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("ForgotPasswordMailTemplate")).Value.ToString();
            txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));

            if(type=="STUDENT")
            {
                txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("CustomerPasswordReset")).Value.ToString());
            }
            else
            {
                txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AdminPasswordReset")).Value.ToString());
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(user.Salt);
            string Encoded = System.Convert.ToBase64String(plainTextBytes);
            Encoded = Encoded.Replace("=", "*");
            txtMsg = txtMsg.Replace("[passwordresetcode]", Encoded);

            txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[email]", user.EmailId);
            txtMsg = txtMsg.Replace("[EmailAddress]", user.EmailId);
            txtMsg = txtMsg.Replace("[Password]", user.Password);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));

            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }


        public void StudentRegistrationMail(EmailSenderModel user)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = "Registration successful";
            var txtMsg = string.Empty; var templateFile = string.Empty;
            templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("StudentRegistrationMailTemplate")).Value.ToString();
            txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
            txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("LoginURL")).Value.ToString());
            txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[email]", user.EmailId);
            txtMsg = txtMsg.Replace("[EmailAddress]", user.EmailId);
            txtMsg = txtMsg.Replace("[Password]", user.Password);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));


            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }

        public void AdminAddUserMail(EmailSenderModel user)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = "Added as admin user of "+ user.CompanyName;
            var txtMsg = string.Empty; var templateFile = string.Empty;
            templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("OrganizationAdminRegistration")).Value.ToString();
            txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
            txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AdminLoginURL")).Value.ToString());
            txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[Password]", user.Password);
            txtMsg = txtMsg.Replace("[CompanyName]", user.CompanyName);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));


            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }

        public void OrganizationRegistrationWithExistingUserMail(EmailSenderModel user)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = "Registration successful";
            var txtMsg = string.Empty; var templateFile = string.Empty;
            templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("OrganizationRegistrationWithExistingUserMail")).Value.ToString();
            txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
            txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AdminLoginURL")).Value.ToString());
            txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));


            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }

        public void OrganizationRegistrationMail(EmailSenderModel user)
        {
            System.Net.Mail.SmtpClient client;
            System.Net.Mail.MailMessage mail;
            string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString();
            string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
            client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
            client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
            client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
            string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
            mail = new System.Net.Mail.MailMessage();
            mail.Subject = "Registration successful";
            var txtMsg = string.Empty; var templateFile = string.Empty;
            templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("OrganizationRegistration")).Value.ToString();
            txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
            txtMsg = txtMsg.Replace("[Websiteurl]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("AdminLoginURL")).Value.ToString());
            txtMsg = txtMsg.Replace("[CompanyLogo]", user.CompanyLogo);
            mail.To.Add(new MailAddress(user.EmailId));
            //txtMsg = FormatMail(mail, txtMsg, emailTemplate);

            txtMsg = txtMsg.Replace("[Name]", user.FirstName);
            txtMsg = txtMsg.Replace("[newline]", "<br />");
            mail.Body = txtMsg;
            if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                mail.IsBodyHtml = true;
            mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString(), userDisplayName);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //client.Send(mail);
                System.Threading.Tasks.Task.Run(() => client.Send(mail));


            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
            }
        }

        private static string ReadTXTFile(string Path)
        {
            string line;
            var sb = new StringBuilder();
            var file = new System.IO.StreamReader(Path);
            while ((line = file.ReadLine()) != null)
            {
                sb.AppendLine(line);
            }
            file.Close();
            return sb.ToString();
        }
        private static string FormatMail(MailMessage mail, string txtMsg, string templatetype)
        {
            if (!string.IsNullOrEmpty(templatetype))
            {
                XmlReader reader = XmlReader.Create(HttpContext.Current.Server.MapPath(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailContent")).Value.ToString()));
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.EndElement && reader.Name == templatetype)
                    {
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if (reader.Name == "subject")
                            {
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    reader.Read();
                                    if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        mail.Subject = reader.Value;
                                    }
                                }
                                reader.Read();
                            }
                            if (reader.Name == "emailcontent")
                            {
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    reader.Read();
                                    if (reader.Name == "header")
                                    {
                                        while (reader.NodeType != XmlNodeType.EndElement)
                                        {
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                txtMsg = txtMsg.Replace("[header]", reader.Value);
                                            }
                                        }
                                        reader.Read();
                                    }
                                    if (reader.Name == "emailbody")
                                    {
                                        while (reader.NodeType != XmlNodeType.EndElement)
                                        {
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.CDATA)
                                            {
                                                txtMsg = txtMsg.Replace("[emailbody]", reader.Value);
                                            }
                                        }
                                        reader.Read();
                                    }
                                }
                            }
                        }
                    }
                }
                return txtMsg; 
            }
            return txtMsg;
        }
    }
}
