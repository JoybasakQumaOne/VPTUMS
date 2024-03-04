namespace QM.UMS.Models
{
    public class DBModel
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string Code { get; set; }

        public string DBServerName { get; set; }

        public string DBName { get; set; }

        public string DBUserName { get; set; }

        public string DBPassword { get; set; }

        public string AuthMode { get; set; }
    }

    public class DBViewModel
    {
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string Code { get; set; }
    }

    public class CompanyModuleModel : DBModel
    {
        public string ADConnection { get; set; }
        public string ActiveDirectoryUrl { get; set; }
        public string DomainName { get; set; }
    }
}