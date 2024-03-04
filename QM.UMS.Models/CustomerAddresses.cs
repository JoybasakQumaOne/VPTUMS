using CommonApplicationFramework.Common;
namespace QM.UMS.Models
{
    public class CustomerAddresses
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public string City { get; set; }
        public Item countryModel { get; set; }
        public string StateProvinceModel { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public bool isDelivery { get; set; }
        public bool isInvoice { get; set; }
        public string AddressNote { get; set; }

    }
}
