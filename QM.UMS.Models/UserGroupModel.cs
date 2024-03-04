// ---------------------------------------------------------------------------------------------------------------
// <copyright file="UserGroupModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <UserGroupModel>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class UserGroupModel : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Group Name is required")]
        //[RegularExpression("^[a-zA-Z \\`_~!|@#$%^&*()\"':;><.,?*\\/]{1,50}$", ErrorMessage = "Group Name, Must be between 1 to 50 characters and should not contain number.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public Item GroupType { get; set; }

        public string Module { get; set; }

        public List<int> Folders{get;set;}

        public List<int> Users { get; set; }

        public List<int> Roles { get; set; }

        public string Creator { get; set; }
    }
}