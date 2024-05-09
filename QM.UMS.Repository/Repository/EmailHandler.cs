using CommonApplicationFramework.ConfigurationHandling;
using CommonApplicationFramework.ExceptionHandling;
using CommonApplicationFramework.Logging;
using QM.UMS.Models;
using QM.UMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QM.UMS.Repository.Repository
{
    public class EmailHandler : IEmailHandler
    {
        public void TriggerAdminResetMail(RegistrationMailData rdm)
        {
            try
            {

                if (true)
                {
                    System.Net.Mail.SmtpClient client;
                    System.Net.Mail.MailMessage mail;
                    string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString();
                    string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
                    client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
                    client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
                    client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
                    string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
                    mail = new System.Net.Mail.MailMessage();
                    mail.Subject = "Password has been reset";
                    var txtMsg = string.Empty; var templateFile = string.Empty;

                    if(rdm.ProductCode=="VPT")
                    {
                        templateFile = "~/Resources/AdminResetPasswrodVPT.html";
                    }
                    else
                    {
                        templateFile = "~/Resources/AdminResetPasswrodTranscript.html";
                    }
                    txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
                    txtMsg = txtMsg.Replace("[Name]", rdm.Name);
                    txtMsg = txtMsg.Replace("[Password]", rdm.Password);

                    mail.To.Add(new MailAddress(rdm.Email));
                    mail.Body = txtMsg;
                    if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                        mail.IsBodyHtml = true;
                    mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString(), userDisplayName);
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log(ex.Message, ex.StackTrace);
                        LogManager.Log(ex);
                        throw new BusinessException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (RepositoryException repEx)
            {
                LogManager.Log(repEx);
                throw new RepositoryException(repEx.ErrorCode, repEx.ErrorMessage, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", ex.Message, ex.StackTrace);
            }
        }

        public void TriggerChangePasswordMail(string email)
        {
            try
            {
                if (true)
                {
                    System.Net.Mail.SmtpClient client;
                    System.Net.Mail.MailMessage mail;
                    string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString();
                    string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
                    client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
                    client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
                    client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
                    string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
                    mail = new System.Net.Mail.MailMessage();
                    mail.Subject = "Successfully Changed Password";
                    var txtMsg = string.Empty; var templateFile = string.Empty;

                    templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EkahChangePassword")).Value.ToString();
                    txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
                    txtMsg = txtMsg.Replace("[Name]", "");

                    mail.To.Add(new MailAddress(email));
                    mail.Body = txtMsg;
                    if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                        mail.IsBodyHtml = true;
                    mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString(), userDisplayName);
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log(ex.Message, ex.StackTrace);
                        LogManager.Log(ex);
                        throw new BusinessException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (RepositoryException repEx)
            {
                LogManager.Log(repEx);
                throw new RepositoryException(repEx.ErrorCode, repEx.ErrorMessage, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", ex.Message, ex.StackTrace);
            }
        }

        public void TriggerExistsRegistrationEmail(RegistrationMailData rdm)
        {
            try
            {
                if (rdm != null)
                {
                    System.Net.Mail.SmtpClient client;
                    System.Net.Mail.MailMessage mail;
                    string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString();
                    string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
                    client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
                    client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
                    client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
                    client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
                    string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
                    mail = new System.Net.Mail.MailMessage();
                    var txtMsg = string.Empty; var templateFile = string.Empty;

                    if (rdm.ProductCode == "VPT")
                    {
                        mail.Subject = "Welcome to VPT Admin portal";
                        templateFile = "~/Resources/AdminExistingUserAdmin.html";
                    }
                    else
                    {
                        mail.Subject = "Welcome to VPT Transcipt portal";
                        templateFile = "~/Resources/AdminExistingUserTranscript.html";
                    }

                    txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
                    txtMsg = txtMsg.Replace("[NAME]", rdm.Name);
                    txtMsg = txtMsg.Replace("[PASSWORD]", rdm.Password);
                    txtMsg = txtMsg.Replace("[COMPANYNAME]", rdm.CompanyName);
                    txtMsg = txtMsg.Replace("[LoginURL]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("LoginURL")).Value);


                    mail.To.Add(new MailAddress(rdm.Email));
                    mail.Body = txtMsg;
                    if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                        mail.IsBodyHtml = true;
                    mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString(), userDisplayName);
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log(ex);
                        throw new BusinessException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (RepositoryException repEx)
            {
                LogManager.Log(repEx);
                throw new RepositoryException(repEx.ErrorCode, repEx.ErrorMessage, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", ex.Message, ex.StackTrace);
            }
        }

        public void TriggerForgotPasswordMail(string email)
        {
            try
            {

                if (true)
                {
                    System.Net.Mail.SmtpClient client;
                    System.Net.Mail.MailMessage mail;
                    string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString();
                    string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
                    client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
                    client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
                    client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
                    client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
                    string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
                    mail = new System.Net.Mail.MailMessage();
                    mail.Subject = "Reset Password Request";
                    var txtMsg = string.Empty; var templateFile = string.Empty;

                    templateFile = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EkahForgotPassword")).Value.ToString();
                    txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
                    txtMsg = txtMsg.Replace("[Name]", "");

                    mail.To.Add(new MailAddress(email));
                    mail.Body = txtMsg;
                    if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                        mail.IsBodyHtml = true;
                    mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString(), userDisplayName);
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        client.Send(mail);
                        LogManager.Log("email triggeres " + email);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log(ex);
                        LogManager.Log(ex.Message, ex.StackTrace);
                        throw new BusinessException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
                    }
                }
                else
                {
                    LogManager.Log("no users " + email);
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (RepositoryException repEx)
            {
                LogManager.Log(repEx);
                throw new RepositoryException(repEx.ErrorCode, repEx.ErrorMessage, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", ex.Message, ex.StackTrace);
            }
        }

        public void TriggerNewRegistrationEmail(RegistrationMailData rdm)
        {
            try
            {
                if (rdm != null)
                {
                    System.Net.Mail.SmtpClient client;
                    System.Net.Mail.MailMessage mail;
                    string userCredential = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderEmail")).Value.ToString();
                    string userCredentialPass = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderPassword")).Value.ToString();
                    client = new System.Net.Mail.SmtpClient(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPAddress")).Value.ToString());
                    client.Port = Convert.ToInt32(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("SMTPPort")).Value.ToString());
                    client.Credentials = new System.Net.NetworkCredential(userCredential, userCredentialPass);
                    client.EnableSsl = Convert.ToBoolean(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EnableSSL")).Value.ToString());
                    string userDisplayName = Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSenderDisplayName")).Value.ToString();
                    mail = new System.Net.Mail.MailMessage();
                    var txtMsg = string.Empty;
                    var templateFile = string.Empty;

                    if (rdm.ProductCode == "VPT")
                    {
                        mail.Subject = "Welcome to VPT Admin portal";
                        templateFile = "~/Resources/AdminUserCreatedVPT.html";
                    }
                    else
                    {
                        mail.Subject = "Welcome to VPT Transcript portal";
                        templateFile = "~/Resources/AdminUserCreatedTranscript.html";
                    }

                    txtMsg = ReadTXTFile(HttpContext.Current.Server.MapPath(templateFile));
                    txtMsg = txtMsg.Replace("[NAME]", rdm.Name);
                    txtMsg = txtMsg.Replace("[PASSWORD]", rdm.Password);
                    txtMsg = txtMsg.Replace("[EMAIL]", rdm.Email);
                    txtMsg = txtMsg.Replace("[LoginURL]", Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("LoginURL")).Value);


                    mail.To.Add(new MailAddress(rdm.Email));
                    mail.Body = txtMsg;
                    if (templateFile.ToLower().EndsWith(".html") || templateFile.ToLower().EndsWith(".htm"))
                        mail.IsBodyHtml = true;
                    mail.From = new System.Net.Mail.MailAddress(Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("EmailSender")).Value.ToString(), userDisplayName);
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log(ex);
                        throw new BusinessException("ERROROCCURE", MessageConfig.MessageSettings["EMAILFAILED"], "CREATED");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (RepositoryException repEx)
            {
                LogManager.Log(repEx);
                throw new RepositoryException(repEx.ErrorCode, repEx.ErrorMessage, repEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DOCUMENTUPLOADFAILED", ex.Message, ex.StackTrace);
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
    }
}
