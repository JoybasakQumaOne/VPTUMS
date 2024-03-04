// ---------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>17-10-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using System.ComponentModel.DataAnnotations;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <PhoneModel>
    ///   Description:  <OrganizationDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class PhoneModelTest 
    {
        public int Id { get; set; }

        public int PhoneType { get; set; }

        [Required(ErrorMessage = "Enter a Valid Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set; }

        public int Extn { get; set; }

        public string ContactType { get; set; }

        public int ContactId { get; set; }

        public int IsPrimary { get; set; }

        public string Status { get; set; }
    }
}
