// ---------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationAttributeModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>07-12-2017</createdOn>
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
    ///   Class:        <OrganizationAttributeModel>
    ///   Description:  <OrganizationDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationAttribute : BaseModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage="Name Cannot be Empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description Cannot be Empty")]
        public string Description { get; set; }

        public string Label { get; set; }

        public int IsActive { get; set; }

        public int IsRequired { get; set; }

        public string AttributeCode { get; set; }

        public Item AttributeControlTypeId { get; set; }

        public int IsRepeated { get; set; }

        public string ObjectType { get; set; }

        public int DisplayOrder { get; set; }
    }
}