// ----------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Purvi Pandya</author>
// <createdOn>06-09-2017</createdOn>
// <comment></comment>
// -----------------------------------------------------------------------------------------------------------------


namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ModuleRepository>
    ///   Description:  <Contains CRUD>
    ///   Author:       <Purvi Pandya>                    
    /// -----------------------------------------------------------------
    public class ModuleRepository : IModuleRepository
    {
        #region Variable Declaration
        private DBManager dbManager;
        #endregion

        public List<ItemCode> GetAllModule(string code)
        {
            ItemCode module = new ItemCode();
            List<ItemCode> moduleList = new List<ItemCode>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ModuleQuerySettings["GetModuleDetails"].ToString();                    
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        module = new ItemCode();
                        module.Id = Convert.ToInt32(dr["Id"]);
                        module.Value = dr["Name"].ToString();
                        module.Code = dr["Code"].ToString();
                        moduleList.Add(module);
                    }
                    return moduleList;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), ex.StackTrace);
            }
        }


        public List<ItemCode> GetModulesByEmail(string code, string emailId)
        {
            ItemCode module = new ItemCode();
            List<ItemCode> moduleList = new List<ItemCode>();
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ModuleQuerySettings["GetModuleDetails"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Email", emailId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        module = new ItemCode();
                        module.Id = Convert.ToInt32(dr["Id"]);
                        module.Value = dr["Name"].ToString();
                        module.Code = dr["Code"].ToString();
                        moduleList.Add(module);
                    }
                    return moduleList;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), ex.StackTrace);
            }
        }


        public List<DisplayUserModule> GetAllModuleLinktoUsers(string code)
        {
            UserModule usermodule = new UserModule();
            List<UserModule> usermoduleList = new List<UserModule>();
            Item module = new Item();
            List<Item> moduleList = new List<Item>();


            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ModuleQuerySettings["GetModulesLinkedtoUser"].ToString();                    
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        usermodule = new UserModule();
                        usermodule.UserId = Convert.ToInt32(dr["Id"]);
                        usermodule.UserName = dr["UserName"].ToString();
                        usermodule.Modules = !string.IsNullOrEmpty(dr["ModuleId"].ToString()) ? new Item() { Id = Convert.ToInt32(dr["ModuleId"]), Value = dr["ModuleName"].ToString() } : new Item() { Id = 0, Value = null };
                        usermoduleList.Add(usermodule);
                    }

                    var moduleDetails = usermoduleList.GroupBy(x => x.UserId).Select(grp => grp.ToList()).ToList();
                    
                   
                    Item modulebind = new Item();
                    UserModule usermodules = new UserModule();
                    DisplayUserModule dusermodules = new DisplayUserModule();
                    List<DisplayUserModule> displayUserModule = new List<DisplayUserModule>();
                    List<Item> moduleListbind = new List<Item>();
                    foreach (var item in moduleDetails)
                    {
                        dusermodules = new DisplayUserModule();
                        moduleListbind = new List<Item>();
                        modulebind = new Item();

                        var moduleData = item.GroupBy(x => x.Modules.Id).Select(grp => grp.ToList()).ToList();
                        
                        foreach (var mo in moduleData)
                        {
                            modulebind = new Item();
                            modulebind.Id = mo[0].Modules.Id;
                            modulebind.Value = mo[0].Modules.Value;
                            moduleListbind.Add(modulebind);
                        }

                        //usermodules = new UserModule();
                        //usermodule.UserId = item[0].UserId;
                        //usermodule.UserName = item[0].UserName;
                        //dusermodules.User = usermodule;
                        dusermodules.UserId = item[0].UserId;
                        dusermodules.UserName = item[0].UserName;
                        dusermodules.ModuleList = moduleListbind;
                        displayUserModule.Add(dusermodules);
                    }
                    return displayUserModule;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("AGENCYCLIENTNOTFOUND", MessageConfig.MessageSettings["AGENCYCLIENTNOTFOUND"].ToString(), ex.StackTrace);
            }
        }


        /// <summary>Add Module To User</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="userModule">Object of Common Entity</param>
        /// <returns>Insert Status</returns>    
        public bool AddModuleToUser(string code, CommonModel userModule)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ModuleQuerySettings["AddModuleToUser"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", userModule.RelationalId);
                    dbManager.AddParameters(1, "@ModuleId", userModule.Items.Id);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        status = true;
                    }                    
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDROLETOGROUPFAILED", MessageConfig.MessageSettings["ADDROLETOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDROLETOGROUPFAILED", MessageConfig.MessageSettings["ADDROLETOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Remove Module From User</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="user">Object of Common Entity</param>
        /// <returns>Remove Status</returns>    
        public bool RemoveModuleFromUser(string code, CommonModel user)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.ModuleQuerySettings["RemoveModuleFromUser"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@UserId", user.RelationalId);
                    dbManager.AddParameters(1, "@ModuleId", user.Items.Id);
                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        status = true;
                    }
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEUSERTOGROUPFAILED", MessageConfig.MessageSettings["DELETEUSERTOGROUPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEUSERTOGROUPFAILED", MessageConfig.MessageSettings["DELETEUSERTOGROUPFAILED"].ToString(), ex.StackTrace);
            }
        }





    }
}
