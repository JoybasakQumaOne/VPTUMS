// ---------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationType.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>12-12-2017</createdOn>
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
    ///   Class:        <OrganizationType>
    ///   Description:  <OrganizationType Details>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationType
    {

        public int Id { get; set; }

        [Required(ErrorMessage ="Name cannot be empty.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code cannot be empty.")]
        public string Code { get; set; }

        public int TypeId { get; set; }
    }
}