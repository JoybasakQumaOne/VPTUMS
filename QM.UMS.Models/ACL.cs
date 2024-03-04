// -----------------------------------------------------------------------------------------------------
// <copyright file="ACL.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using System.Collections.Generic;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ACL>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class ACL
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<Groups> UserGroups { get; set; }
    }

    public class Groups
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public List<Roles> GroupRoles { get; set; }
    }

    public class Roles
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int resourceId { get; set; }
        public List<Permissions> RolePermissions { get; set; }
    }

    public class Permissions
    {
        public int Id { get; set; }
        public string AccessRight { get; set; }
    }
}