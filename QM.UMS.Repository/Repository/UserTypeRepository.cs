namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using QM.UMS.Models;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    #endregion

    public class UserTypeRepository : IUserTypeRepository
    {
        #region Variable declaration
        private DBManager dbManager;
        #endregion

        #region Get
        public List<UserType> Get()
        {
            UserType userType;
            List<UserType> userTypes = new List<UserType>();
            try
            {
                using (dbManager = new DBManager())
                {

                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUserType"].ToString();
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    while (dr.Read())
                    {
                        userType = new UserType();
                        userType.Id = Convert.ToInt32(dr["Id"]);
                        userType.Name = Convert.ToString(dr["UserType"]);
                        userType.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        userTypes.Add(userType);
                    }
                    this.dbManager.CloseReader();
                    return userTypes;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERTYPEFOUNDFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERTYPEFOUNDFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Get By Id
        public UserType Get(int id)
        {
            UserType userType = new UserType();
            List<UserType> userTypes = new List<UserType>();
            try
            {
                using (dbManager = new DBManager())
                {

                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.UserQuerySettings["GetUserTypeById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@UserTypeId", id);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {
                        userType = new UserType();
                        userType.Id = id;
                        userType.Name = Convert.ToString(dr["UserType"]);
                        userType.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    }
                    return userType;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERTYPEFOUNDFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERTYPEFOUNDFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Post
        public bool AddUserType(UserType user)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (user != null)
                    {
                        using (dbManager = new DBManager())
                        {
                            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                            dbManager.Open();
                            string duplicateChkquery = QueryConfig.UserQuerySettings["DuplicateUserTypeCheck"].ToString();
                            dbManager.CreateParameters(1);
                            dbManager.AddParameters(0, "@UserType", user.Name);
                            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateChkquery);
                            if (dr.Read())
                            {
                                dr.Close();
                                throw new DuplicateException();
                            }
                            else
                            {
                                dr.Close();
                                string userQuery = QueryConfig.UserQuerySettings["AddUserType"].ToString();
                                dbManager.CreateParameters(2);
                                dbManager.AddParameters(0, "@UserType", user.Name);
                                dbManager.AddParameters(1, "@IsActive", user.IsActive);
                                int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, userQuery);
                                if (rowAffacted > 0)
                                    return true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDUSERTYPEFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", "UserType " + user.Name + " " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDUSERTYPEFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Put
        public bool UpdateUserType(UserType user)
        {
            try
            {
                bool result = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (user.Id > 0)
                    {
                        string userQuery = QueryConfig.UserQuerySettings["UpdateUserType"].ToString();
                        dbManager.CreateParameters(3);
                        dbManager.AddParameters(0, "@Id", user.Id);
                        dbManager.AddParameters(1, "@UserType", user.Name);
                        dbManager.AddParameters(2, "@IsActive", user.IsActive);
                        int rowsaffected = dbManager.ExecuteNonQuery(CommandType.Text, userQuery);
                        if (rowsaffected > 0)
                        {
                            result = true;
                        }
                    }
                    return result;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEUSERTYPEFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEUSERTYPEFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region AttributeLinking
        public bool LinkAttribute(int userTypeId, int attributeId)
        {
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    if (userTypeId > 0 || attributeId > 0)
                    {
                        using (dbManager = new DBManager())
                        {
                            dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                            dbManager.Open();
                            string duplicateChkquery = QueryConfig.UserQuerySettings["DuplicateUserTypeAttributeMap"].ToString();
                            dbManager.CreateParameters(2);
                            dbManager.AddParameters(0, "@UserTypeId", userTypeId);
                            dbManager.AddParameters(1, "@UserAttributeId", attributeId);
                            IDataReader dr = dbManager.ExecuteReader(CommandType.Text, duplicateChkquery);
                            if (dr.Read())
                            {
                                dr.Close();
                                throw new DuplicateException();
                            }
                            else
                            {
                                dr.Close();
                                string userQuery = QueryConfig.UserQuerySettings["UserTypeAttributeMap"].ToString();
                                dbManager.CreateParameters(2);
                                dbManager.AddParameters(0, "@UserTypeId", userTypeId);
                                dbManager.AddParameters(1, "@UserAttributeId", attributeId);
                                int rowAffacted = dbManager.ExecuteNonQuery(CommandType.Text, userQuery);
                                if (rowAffacted > 0)
                                    return true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("USERATTRIBUTEMAPPINGFAILED", sqlEx.Message, sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("DUPLICATE", "Mapping " + MessageConfig.MessageSettings["DUPLICATE"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("USERATTRIBUTEMAPPINGFAILED", ex.Message, ex.StackTrace);
            }
        }
        #endregion

    }
}
