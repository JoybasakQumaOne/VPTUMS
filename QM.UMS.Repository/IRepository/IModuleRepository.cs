// ----------------------------------------------------------------------------------------------------------------
// <copyright file="IModuleRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>06-09-2017</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------


namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IModuleRepository>
    ///   Description:  <Contains CRUD>
    ///   Author:       <Purvi Pandya>                    
    /// -----------------------------------------------------------------
    public interface IModuleRepository
    {
        List<ItemCode> GetAllModule(string code);

        List<ItemCode> GetModulesByEmail(string code, string emailId);

        List<DisplayUserModule> GetAllModuleLinktoUsers(string code);

        bool AddModuleToUser(string code, CommonModel userModule);

        bool RemoveModuleFromUser(string code, CommonModel user);
    }
}
