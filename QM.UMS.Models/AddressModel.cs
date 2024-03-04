// ---------------------------------------------------------------------------------------------------------------
// <copyright file="AddressModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>15-11-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

using CommonApplicationFramework.Common;
namespace QM.UMS.Models
{
    #region Namespace

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <AddressModel>
    ///   Description:  <AddressDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class AddressModel //: BaseModel
    {

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Item CountryId { get; set; }

        public string StateProvinceId { get; set; }

        public string City { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string ZipPostalCode { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string CustomAttributes { get; set; }

    }
}
