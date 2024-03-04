// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CountryModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>01-12-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <CountryModel>
    ///   Description:  <CountryDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class CountryModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int AllowsBilling { get; set; }

        public int AllowsShipping { get; set; }

        public string TwoLetterIsoCode { get; set; }

        public string ThreeLetterIsoCode { get; set; }

        public int NumericIsoCode { get; set; }

        public int DeliveryZoneId { get; set; }

        public int CurrencyId { get; set; }
    }
}
