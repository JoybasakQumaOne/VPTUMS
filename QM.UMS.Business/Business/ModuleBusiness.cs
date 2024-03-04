// ----------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>06-09-2017</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------


namespace QM.UMS.Business.Business
{

    #region Namespace
    using CommonApplicationFramework.Common;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ModuleBusiness>
    ///   Description:  <Contains CRUD>
    ///   Author:       <Purvi Pandya>                    
    /// -----------------------------------------------------------------
    
    public class ModuleBusiness : IModuleBusiness
    {
         #region Variable Declaration
        /// <summary>
        /// This is Private Variable
        /// </summary>
        private readonly IModuleRepository _IModuleRepository;
        #endregion

        #region Constructor
        public ModuleBusiness(IModuleRepository _iModuleRepository)
        {
            this._IModuleRepository = _iModuleRepository;
        }
        #endregion

        public List<ItemCode> GetAllModule(string code)
        {
            List<ItemCode> modules = new List<ItemCode>();
            return modules = this._IModuleRepository.GetAllModule(code);
        }

        public List<ItemCode> GetModulesByEmail(string code, string emailId)
        {
            List<ItemCode> modules = new List<ItemCode>();
            return modules = this._IModuleRepository.GetModulesByEmail(code, emailId);
        }

        public List<DisplayUserModule> GetAllModuleLinktoUsers(string code)
        {
            List<DisplayUserModule> userModules = new List<DisplayUserModule>();
            return userModules = this._IModuleRepository.GetAllModuleLinktoUsers(code);
        }


        public bool AddModuleToUser(string code, CommonModel userModule)
        {
            return this._IModuleRepository.AddModuleToUser(code, userModule);
        }

        public bool RemoveModuleFromUser(string code, CommonModel user)
        {
            return this._IModuleRepository.RemoveModuleFromUser(code, user);
        }
    }
}
