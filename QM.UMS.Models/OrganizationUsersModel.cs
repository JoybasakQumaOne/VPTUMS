// ---------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationUsersModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>24-10-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using System.Collections.Generic;
    using CommonApplicationFramework.Common;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <OrganizationUsersModel>
    ///   Description:    
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationUsersModel : BaseModel
    {
             
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public List<int> Users{ get; set; }
    }
}

