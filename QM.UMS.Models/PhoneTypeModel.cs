// ---------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>17-10-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <PhoneTypeModel>
    ///   Description:  <OrganizationDetails>  
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class PhoneTypeModel12 : BaseModel
    {

        public int Id { get; set; }

        public Item PhoneType { get; set; }

    }
}
