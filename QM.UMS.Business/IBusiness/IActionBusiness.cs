// -----------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionBusinessLogic.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using QM.UMS.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IActionBusinessLogic>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public interface IActionBusiness
    {
        List<ActionModel> GetActions(string code);

        ActionModel GetActionById(string code, int actionId);

        List<ActionModel> GetActionsByUserId(string code, int userId);

        bool AddAction(string code, ActionModel action);

        bool AssignActionToRole(string code, CommonModel action);

        bool UpdateAction(string code, ActionModel action);

        bool RemoveAction(string code, int actionId);

        bool RemoveActionFromRole(string code, CommonModel action);

        List<ActionModel> GetActionsByRoleId(string code, int RoleId);

        List<AcessPermissionModel> GetActionPermission(string Code, int userId, List<string> CodeList);
    }
}



