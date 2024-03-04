// ---------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>17-10-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System;
    using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <OrganizationModel>
    ///   Description:  <OrganizationDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationBaseModel:BaseModel
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public Guid OrgGUID { get; set; }
    }

    public class OrganizationModel : OrganizationBaseModel
    {
          
        public string Id { get; set; }

        public string Description { get; set; }

        public int InstanceId { get; set; }

        public string InstanceCode { get; set; }

        public string Logo { get; set; }

        [Required(ErrorMessage = "Enter a Valid Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        public int IsActive { get; set; }

        public int IsDeleted { get; set; }

        public bool IsTaxExcempt { get; set; }


        public int AdminId { get; set; }

        public int Address_Id { get; set; }

        public bool HasDeliveryCharge { get; set; }

        [Required(ErrorMessage="Organization Code can not be null")]
        [StringLength(3, ErrorMessage="Organization Code should be equal to 3 characters only", MinimumLength=3)]
        public string OrganizationCode { get; set; }

        public int SequnceNumber { get; set; }

        public AddressModel Address { get; set; }

        public List<PhoneModel> Phone { get; set; }

        public List<OrganizationType> OrganizationType { get; set; }

        public string BackgroundColor { get; set; }
        public bool HasPrivateEvent { get; set; }
        public bool OrgHomeRedirect { get; set; }
        public string BookDisplayMode { get; set; }
        public string OrgMessage { get; set; }
    }    

    public class RegisterOrganizationModel:OrganizationBaseModel
    {
        public UserProfileModel customerProfile { get; set; }
    }
}
