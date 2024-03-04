// ---------------------------------------------------------------------------------------------------------------
// <copyright file="RoleModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System.ComponentModel.DataAnnotations;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <RoleModel>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------
       
    public class RoleModel : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [RegularExpression("^[a-zA-Z \\`_~!|@#$%^&*()\"':;><.,?*\\/]{1,50}$", ErrorMessage = "Role Name, Must be between 1 to 50 characters and should not contain number.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public ItemCode Module { get; set; }

        public string Creator { get; set; }
    }
}