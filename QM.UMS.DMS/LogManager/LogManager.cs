using QM.eBook.DMS.QuerySetting;
using System;
using System.IO;

namespace QM.eBook.DMS.LogManager
{
    public class BaseLogManager
    {
        private AccessHelper accessHelper;

        public void Log(string message, string Code)
        {
            try
            {
                if (!string.IsNullOrEmpty(accessHelper.LogFilePath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(accessHelper.LogFilePath, true))
                    {
                        streamWriter.WriteLine("{0}: {1} {2}:", Code, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        streamWriter.WriteLine("______________________________________________________");
                        streamWriter.WriteLine("{0}", message);
                        streamWriter.WriteLine("______________________________________________________");
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}
