// ---------------------------------------------------------------------------------------------------------------
// <copyright file="UserLogModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using System.ComponentModel.DataAnnotations;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserLogModel>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class LoginModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        //[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,15}", ErrorMessage = "Must be between 8 and 15 characters, should contains at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character")]
        [RegularExpression("^(((?=.*[a-z])(?=.*[A-Z])(?=.*[\\d]))|((?=.*[a-z])(?=.*[A-Z])(?=.*[\\W]))|((?=.*[a-z])(?=.*[\\d])(?=.*[\\W]))|((?=.*[A-Z])(?=.*[\\d])(?=.*[\\W]))).{8,15}$", ErrorMessage = "Password, Must be between 8 to 15 characters and should contain at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character.")]
        public string Password { get; set; }

        public bool FirstLogin { get; set; }

        public bool RememberMe { get; set; }        
    }

    public class ChangePassword
    {
        public string OrgnizationLogo { get; set; }

        public string UserName { get; set; }

        public string OldPassword { get; set; }

        [RegularExpression("^(((?=.*[a-z])(?=.*[A-Z])(?=.*[\\d]))|((?=.*[a-z])(?=.*[A-Z])(?=.*[\\W]))|((?=.*[a-z])(?=.*[\\d])(?=.*[\\W]))|((?=.*[A-Z])(?=.*[\\d])(?=.*[\\W]))).{8,50}$", ErrorMessage = "New Password, Must be between 8 to 50 characters and should contain at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character.")]
        public string NewPassword { get; set; }

        public string PasswordResetCode { get; set; }

        public string CompanyCode { get; set; }
    }
}