// ---------------------------------------------------------------------------------------------------------------
// <copyright file="UserModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace    
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using CommonApplicationFramework.Common;
    using QM.eBook.DMS.Module;
    using BaseModel = CommonApplicationFramework.Common.BaseModel;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserModel>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UserProfileModel : BaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        //[RegularExpression("^[a-zA-Z0-9 \\`_~!|@#$%^&*()\"':;><.,?*\\/]{1,50}$", ErrorMessage = "First Name, Must be between 1 to 50 characters and should not contain number.")]
        public string FirstName { get; set; }

        [RegularExpression("^[a-zA-Z0-9 \\`_~!|@#$%^&*()\"':;><.,?*\\/]{1,50}$", ErrorMessage = "Middle Name, Must be between 1 to 50 characters and should not contain number.")]
        public string MiddleName { get; set; }

        //[RegularExpression("^[a-zA-Z0-9 \\`_~!|@#$%^&*()\"':;><.,?*\\/]{1,50}$", ErrorMessage = "Last Name, Must be between 1 to 50 characters and should not contain number.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        //[RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone number.")]
        public string Phone { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public ItemCode Country { get; set; }

        //[RegularExpression("^[0-9]{1,10}$", ErrorMessage = "Zipcode, Should contain Number only")]
        public string Zipcode { get; set; }

        public string Status { get; set; }

        public string Salt { get; set; }

        public DateTimeOffset ExpiresOn { get; set; }

        public bool IsFirstLogin { get; set; }
        public bool IsOTPVerified { get; set; }

        public string ModuleCode { get; set; }

        public DocumentModel userImage { get; set; }

        public Item ReportingTo { get; set; }

        public double? TotalYrsExp { get; set; }

        public DateTimeOffset? DOB { get; set; }

        public string Hobbies { get; set; }

        public DateTimeOffset? JoiningDate { get; set; }

        public Item Department { get; set; }

        public Item Designation { get; set; }

        public string EmployeeId { get; set; }

        public string JobProfile { get; set; }

        public string Accomplishments { get; set; }

        public int? OrderIndex { get; set; }
        
        public bool IsSuperUser { get; set; }

        public List<Guid> Organisations { get; set; }

        public List<int> Instances { get; set; }

        public List<int> Group { get; set; }

        public ItemCode UserType { get; set; }
        public string UserId { get; set; }
        public string OTP { get; set; }
        public bool IsForceChange { get; set; }

        public string ControlUserCode { get; set; }
        public AttachmentFile UserImage { get; set; }
        public string AuthenticationMode { get; set; }
        public string OrgCode { get; set; }
        public bool IsAdmin { get; set; }
    }
}