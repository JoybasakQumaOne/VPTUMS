// ---------------------------------------------------------------------------------------------------------------
// <copyright file="StateProvinceModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
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
    ///   Class:        <StateProvinceModel>
    ///   Description:  <StateDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class StateProvinceModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public int Published { get; set; }

        public int DisplayOrder { get; set; }

        //public List<CountryModel> Country { get; set; }

        public int CountryId { get; set; }
    }
}