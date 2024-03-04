using CommonApplicationFramework.Common;
using System;
using System.Collections.Generic;

namespace QM.UMS.Models
{
    public class Customer : BaseModel
    {
        public int Id { get; set; }
        public Guid CustomerGuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string EmailToRevalidate { get; set; }
        public string AdminComment { get; set; }
        public string IsTaxExempt { get; set; }
        public string HasShoppingCartItems { get; set; }
        public string RequireReLogin { get; set; }
        public string FailedLoginAttempts { get; set; }
        public string CannotLoginUntilDate { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
        public string CustomAttributes { get; set; }
        public string LastIpAddress { get; set; }
        public bool? IsFirstLogin { get; set; }
		public string OTP { get; set; }
        public bool IsGuest { get; set; }
        public CustomerAddresses customerAddresses { get; set; }
        public CustomerProfile customerProfile { get; set; }
        public CustomerSocial customerSocial { get; set; }
        public string CompanyCode { get; set; }
        public string OrgnizationName { get; set; }
        public string OrgnizationLogo { get; set; }
        public bool IsOTPVerified { get; set; }
        public bool IsAdmin { get; set; }
        //public ItemCode Country { get; set; }
        public List<ItemCode> Groups { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class CustomerAddressDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public CustomerAddresses customerAddress { get; set; }
    }
}
