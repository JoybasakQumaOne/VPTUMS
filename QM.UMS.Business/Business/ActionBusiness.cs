// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

#region Namespace
using QM.UMS.Business.IBusiness;
using QM.UMS.Models;
using QM.UMS.Repository.IRepository;
using QM.UMS.Repository.Repository;
//using QM.UMS.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace QM.UMS.Business.Business
{
   /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ActionBusinessLogic>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------
    
    public class ActionBusiness : IActionBusiness, IDisposable
    {
        #region Variable Declaration
        /// <summary>
        /// This is Private Variable
        /// </summary>
        private readonly IActionRepository _IActionRepository;
        #endregion

        #region Constructor
        public ActionBusiness(IActionRepository _iIActionRepository)
        {
            this._IActionRepository = _iIActionRepository;
        }
        #endregion

        #region GET Methods

        /// <summary>Get Action Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>returns list of Action</returns>
        public List<ActionModel> GetActions(string code)
        {
            List<ActionModel> action = new List<ActionModel>();
            return action = this._IActionRepository.GetActions(code);          
        }

        /// <summary>Get Action Details by Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="actionId">Action Id</param>
        /// <returns>returns single entity of Action</returns>
        public ActionModel GetActionById(string code, int actionId)
        {
            ActionModel action = new ActionModel();
            return action = this._IActionRepository.GetActionById(code, actionId);           
        }

        /// <summary>Get Action Details by UserId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">User Id</param>
        /// <returns>returns list of Action</returns>
        public List<ActionModel> GetActionsByUserId(string code, int userId)
        {
            List<ActionModel> action = new List<ActionModel>();
            return action = this._IActionRepository.GetActionsByUserId(code, userId);
        }


        /// <summary>Get Action Details by RoleId</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userId">Role Id</param>
        /// <returns>returns list of Action</returns>
        public List<ActionModel> GetActionsByRoleId(string code, int RoleId)
        {
            List<ActionModel> action = new List<ActionModel>();
            return action = this._IActionRepository.GetActionsByRoleId(code, RoleId);           
        }

        #endregion

        #region POST Methods

        /// <summary>Add Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return add status</returns>
        public bool AddAction(string code, ActionModel action)
        {
            return this._IActionRepository.AddAction(code, action);            
        }

        /// <summary>Assign Action To Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Common Model</param>
        /// <returns>return assign status</returns>
        public bool AssignActionToRole(string code, CommonModel action)
        {
            return this._IActionRepository.AssignActionToRole(code, action);
        }


        /// <summary>Add Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return add status</returns>
        public List<AcessPermissionModel> GetActionPermission(string code,int UserId, List<string> actioncode)
        {
            List<AcessPermissionModel> accessPermisionModels = new List<AcessPermissionModel>();
            return accessPermisionModels = this._IActionRepository.GetActionPermission(code, UserId, actioncode);  
        }
        #endregion

        #region PUT Methods

        /// <summary>Update Action Detail</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Action Model</param>
        /// <returns>return update status</returns>
        public bool UpdateAction(string code, ActionModel action)
        {
           return this._IActionRepository.UpdateAction(code, action);
        }

        #endregion

        #region DELETE Methods

        /// <summary>Delete Action Detail By Id</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="actionId">Action Id</param>
        /// <returns>return delete status</returns>
        public bool RemoveAction(string code, int actionId)
        {
            return this._IActionRepository.RemoveAction(code, actionId);
        }

        /// <summary>Remove Action From Role</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="action">Object of Common Model</param>
        /// <returns>return remove status</returns>
        public bool RemoveActionFromRole(string code, CommonModel action)
        {
           return this._IActionRepository.RemoveActionFromRole(code, action);
        }

        #endregion

        /// <summary>
        /// Register Void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
