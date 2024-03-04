// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>05-04-2017</createdOn>
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
    ///   Class:        <ResourceModel>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------
    public class ResourceModel : BaseModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Module is required.")]
        public int? Module { get; set; }

        [Required(ErrorMessage = "ResourceType is required.")]
        public Item ResourceType { get; set; }

        [Required(ErrorMessage = "ResourceCode is required.")]
        public string ResourceCode { get; set; }

        [Required(ErrorMessage = "ResourceURL is required.")]
        public string ResourceURL { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public int? ParentId { get; set; }

        public int? OrderIndex { get; set; }

        public string Creator { get; set; }
    }
}