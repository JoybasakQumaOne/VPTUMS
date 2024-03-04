// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>17-10-2017</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.Repository
{
    #region Namespace
    using CommonApplicationFramework.Caching;
    using CommonApplicationFramework.Common;
    using CommonApplicationFramework.ConfigurationHandling;
    using CommonApplicationFramework.DataHandling;
    using CommonApplicationFramework.ExceptionHandling;
    using CommonApplicationFramework.Logging;
    using Newtonsoft.Json;
    using QM.UMS.DMS.Module;
    using QM.UMS.Models;
    using QM.UMS.Repository.Helper;
    using QM.UMS.Repository.IRepository;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <OrganizationRepository>
    ///   Description:  <Contains CRUD, Active Directory, Login Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public class OrganizationRepository : RequestHeader,IOrganizationRepository, IDisposable
    {
        #region Variable Declaration
        public string AgentCode { get; set; }
        private DBManager dbManager;
        private readonly IMasterRepository _MasterRepository;
        private readonly IPhoneRepository _IPhoneRepository;
        private readonly IDocumentProcessRepository _IDocumentProcessRepository;
        #endregion

        #region Constructor
        public OrganizationRepository(IMasterRepository _masterRepository, IPhoneRepository _iPhoneRepository, IDocumentProcessRepository _IDocumentProcessRepository)
        {
            this._MasterRepository = _masterRepository;
            this._IPhoneRepository = _iPhoneRepository;
            this._IDocumentProcessRepository = _IDocumentProcessRepository;
        }
        #endregion

        #region Properties
        public string RequestId { get; set; }
        #endregion

        #region GET Methods

        /// <summary>Get Organization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <returns>List of Organization Details</returns>
        public List<OrganizationModel> GetAllOrganizations()
        {
            string query = string.Empty;
            OrganizationModel OrganisationModel = new OrganizationModel();
            List<OrganizationModel> organisationModels = new List<OrganizationModel>();
            try
            {
				using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();// dbManager.GetDBConnectionString(code);
					dbManager.Open();
                    UserContext userContext = JsonConvert.DeserializeObject<UserContext>(GlobalCacheManager.Instance.Get("usercontext_" + this.RequestId));

                    if(userContext.IsSuperUser)
                    {
                        query = QueryConfig.OrganizationQuerySettings["GetAllorganizations"].ToString();
                    }
                    else
					//{
					//    query = QueryConfig.OrganizationQuerySettings["GetAdminOrganizationList"].ToString();
					//    dbManager.CreateParameters(1);
					//    dbManager.AddParameters(0, "@OrganizationGUID", userContext.CurOrganization);
					//}
					{
						query = QueryConfig.OrganizationQuerySettings["GetAdminOrganizationList"].ToString();
						dbManager.CreateParameters(1);
						dbManager.AddParameters(0, "@UserId", userContext.UserId);
					}
					IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);

                    while (dr.Read())
                    {
                        OrganisationModel = new OrganizationModel();
                        OrganisationModel.Id = Convert.ToString(dr["OrgGUID"]);
                        OrganisationModel.Name = dr["Name"].ToString();
                        OrganisationModel.Email = dr["Email"].ToString();
                        OrganisationModel.OrgGUID = Guid.Parse(dr["OrgGUID"].ToString());
                        OrganisationModel.Description = dr["Description"].ToString();
                        OrganisationModel.Logo = dr["Logo"].ToString();
                        OrganisationModel.Website = dr["Website"].ToString();
                        OrganisationModel.AdminId = Convert.ToInt32(dr["AdminId"]);
                        OrganisationModel.IsActive = Convert.ToInt32(dr["IsActive"]);
                        OrganisationModel.IsDeleted = Convert.ToInt32(dr["IsDeleted"]);
                        OrganisationModel.IsTaxExcempt = Convert.ToBoolean(dr["IsTaxExcempt"]);
                        OrganisationModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        OrganisationModel.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        OrganisationModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        OrganisationModel.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                        OrganisationModel.OrganizationCode = Convert.ToString(dr["OrganizationCode"]);
                        OrganisationModel.HasDeliveryCharge = Convert.ToBoolean(dr["HasDeliveryCharge"]);
                        OrganisationModel.BackgroundColor = Convert.ToString(dr["BackgroundColor"]);
                        organisationModels.Add(OrganisationModel);
                    }
                    return organisationModels.OrderBy(x => x.Name).ToList();
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                OrganisationModel = null;
            }
        }

        /// <summary>Get Organization Address Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <return>List of Organization Fields</return>
        public OrganizationModel GetOrganizationDetails(Guid OrganizationId) {

            OrganizationModel OrganisationModel = new OrganizationModel();
            OrganizationType organizationType = new OrganizationType();
            List<OrganizationType> OrgTypeList = new List<OrganizationType>();
            int OrganizationIntId = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString(); //dbManager.GetDBConnectionString(code);
                    dbManager.Open();
                    string query = QueryConfig.OrganizationQuerySettings["GetAllOrganizationInfoById"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrganisationId", OrganizationId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {

                        OrganisationModel = new OrganizationModel();
                        OrganisationModel.Id = Convert.ToString(dr["OrgGUID"]);
                        OrganisationModel.OrgGUID = Guid.Parse(dr["OrgGUID"].ToString());
                        OrganisationModel.Name = dr["Name"].ToString();
                        OrganisationModel.Email = dr["Email"].ToString();
                        OrganisationModel.Description = dr["Description"].ToString();
                        OrganisationModel.Logo =!string.IsNullOrEmpty(ConvertData.ToString(dr["UploadedFileName"]))? Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/'+ ConfigurationManager.AppSettings["ProjectName"].ToString()+"/"+ ConvertData.ToString(dr["PhysicallFolderName"]) + "/" + ConvertData.ToString(dr["UploadedFileName"]) : null;
                        OrganisationModel.Website = dr["Website"].ToString();
                        OrganisationModel.AdminId = Convert.ToInt32(dr["AdminId"]);
                        OrganisationModel.IsActive = Convert.ToInt32(dr["IsActive"]);
                        OrganisationModel.IsDeleted = Convert.ToInt32(dr["IsDeleted"]);
                        OrganisationModel.IsTaxExcempt = Convert.ToBoolean(dr["IsTaxExcempt"]);
                        OrganisationModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        OrganisationModel.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        OrganisationModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        OrganisationModel.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                        OrganisationModel.OrganizationCode = Convert.ToString(dr["OrganizationCode"]);
                        OrganizationIntId = Convert.ToInt32(dr["Id"]);
                        OrganisationModel.HasDeliveryCharge = Convert.ToBoolean(dr["HasDeliveryCharge"]);
                        OrganisationModel.BackgroundColor = Convert.ToString(dr["BackgroundColor"]);
                        OrganisationModel.OrgMessage = Convert.ToString(dr["OrganizationMessage"]);
                        dr.Close();
                    }

                    query = QueryConfig.AddressQuerySettings["GetAddressByOrgGuid"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrgGUID", OrganizationId);

                    IDataReader drOrganisation = dbManager.ExecuteReader(CommandType.Text, query);
                    if (drOrganisation.Read())
                    {
                        OrganisationModel.Address = new AddressModel();
                        OrganisationModel.Address.CountryId = new Item();

                        OrganisationModel.Address.Id = Convert.ToInt32(drOrganisation["Id"]);
                        OrganisationModel.Address.FirstName = drOrganisation["FirstName"].ToString();
                        OrganisationModel.Address.LastName = drOrganisation["LastName"].ToString();
                        OrganisationModel.Address.CountryId.Id = Convert.ToInt32(drOrganisation["CountryId"]);
                        OrganisationModel.Address.CountryId.Value = Convert.ToString(drOrganisation["CountryName"]);
                        OrganisationModel.Address.StateProvinceId = Convert.ToString(drOrganisation["StateProvinceName"]);
                        OrganisationModel.Address.City = drOrganisation["City"].ToString();
                        OrganisationModel.Address.ZipPostalCode = drOrganisation["ZipPostalCode"].ToString();
                        OrganisationModel.Address.Address1 = drOrganisation["Address1"].ToString();
                        OrganisationModel.Address.Address2 = drOrganisation["Address2"].ToString();
                        OrganisationModel.Address.Latitude = (drOrganisation["Latitude"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drOrganisation["Latitude"]);
                        OrganisationModel.Address.Longitude = (drOrganisation["Longitude"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drOrganisation["Longitude"]);
                        OrganisationModel.CreatedBy = Convert.ToInt32(drOrganisation["CreatedBy"]);
                        OrganisationModel.ModifiedBy = (drOrganisation["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drOrganisation["ModifiedBy"]);
                        OrganisationModel.CreatedOn = (drOrganisation["CreatedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        OrganisationModel.ModifiedOn = (drOrganisation["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                    }
                    drOrganisation.Close();

                    OrganisationModel.Phone = this._IPhoneRepository.GetAllPhoneList("organization", OrganizationIntId);

                    string orgTypeQuery = QueryConfig.OrganizationQuerySettings["GetAllMappedTypes"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrganizationId", OrganizationIntId);

                    IDataReader orgTypeReader = dbManager.ExecuteReader(CommandType.Text, orgTypeQuery);
                    while (orgTypeReader.Read())
                    {
                        organizationType = new OrganizationType();
                        //organizationType.Id = Convert.ToInt32(orgTypeReader["Id"]);
                        organizationType.Name = Convert.ToString(orgTypeReader["Name"]);
                        organizationType.Code = Convert.ToString(orgTypeReader["Code"]);
                        organizationType.TypeId = Convert.ToInt32(orgTypeReader["typeId"]);
                        OrgTypeList.Add(organizationType);
                    }
                    OrganisationModel.OrganizationType = OrgTypeList;
                    
                }
                return OrganisationModel;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                OrganisationModel = null;
            }
        }

        public OrganizationModel GetOrganizationInfo(string OrganizationId)
        {
            OrganizationModel OrganisationModel = new OrganizationModel();
            OrganizationType organizationType = new OrganizationType();
            List<OrganizationType> OrgTypeList = new List<OrganizationType>();
            int OrganizationIntId = 0;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GenerateConnectionString(this.ConnectionId); //dbManager.GetDBConnectionString(code);
                    dbManager.Open();
                    string query = QueryConfig.OrganizationQuerySettings["GetAllOrganizationInfoByIdOrCode"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrganisationId", OrganizationId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, query);
                    if (dr.Read())
                    {

                        OrganisationModel = new OrganizationModel();
                        OrganisationModel.Id = Convert.ToString(dr["OrgGUID"]);
                        OrganisationModel.OrgGUID = Guid.Parse(dr["OrgGUID"].ToString());
                        OrganisationModel.Name = dr["Name"].ToString();
                        OrganisationModel.Email = dr["Email"].ToString();
                        OrganisationModel.Description = dr["Description"].ToString();
                        OrganisationModel.Logo = !string.IsNullOrEmpty(ConvertData.ToString(dr["Logo"])) ? Environments.Configurations.Settings.Find(x => x.Key.ToString().Equals("FileServerURL")).Value.ToString() + '/' + ConfigurationManager.AppSettings["ProjectName"].ToString() + '/' + ConvertData.ToString(dr["PhysicallFolderName"]) + '/' + ConvertData.ToString(dr["UploadedFileName"]) : null;
                        OrganisationModel.Website = dr["Website"].ToString();
                        OrganisationModel.AdminId = Convert.ToInt32(dr["AdminId"]);
                        OrganisationModel.IsActive = Convert.ToInt32(dr["IsActive"]);
                        OrganisationModel.IsDeleted = Convert.ToInt32(dr["IsDeleted"]);
                        OrganisationModel.IsTaxExcempt = Convert.ToBoolean(dr["IsTaxExcempt"]);
                        OrganisationModel.CreatedBy = Convert.ToInt32(dr["CreatedBy"]);
                        OrganisationModel.ModifiedBy = (dr["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(dr["ModifiedBy"]);
                        OrganisationModel.CreatedOn = string.IsNullOrEmpty(dr["CreatedOn"].ToString()) ? new DateTime(0) : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        OrganisationModel.ModifiedOn = (dr["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                        OrganisationModel.OrganizationCode = Convert.ToString(dr["OrganizationCode"]);
                        OrganizationIntId = Convert.ToInt32(dr["Id"]);
                        OrganisationModel.HasDeliveryCharge = Convert.ToBoolean(dr["HasDeliveryCharge"]);
                        OrganisationModel.BackgroundColor = Convert.ToString(dr["BackgroundColor"]);
                        OrganisationModel.HasPrivateEvent =!string.IsNullOrEmpty( Convert.ToString(dr["HasPrivateEvent"]))?true:false;
                        dr.Close();
                    }

                    query = QueryConfig.AddressQuerySettings["GetAddressByOrgGuid"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrgGUID", OrganisationModel.OrgGUID);

                    IDataReader drOrganisation = dbManager.ExecuteReader(CommandType.Text, query);
                    if (drOrganisation.Read())
                    {
                        OrganisationModel.Address = new AddressModel();
                        OrganisationModel.Address.CountryId = new Item();

                        OrganisationModel.Address.Id = Convert.ToInt32(drOrganisation["Id"]);
                        OrganisationModel.Address.FirstName = drOrganisation["FirstName"].ToString();
                        OrganisationModel.Address.LastName = drOrganisation["LastName"].ToString();
                        OrganisationModel.Address.CountryId.Id = Convert.ToInt32(drOrganisation["CountryId"]);
                        OrganisationModel.Address.CountryId.Value = Convert.ToString(drOrganisation["CountryName"]);
                        OrganisationModel.Address.StateProvinceId = Convert.ToString(drOrganisation["StateProvinceName"]);
                        OrganisationModel.Address.City = drOrganisation["City"].ToString();
                        OrganisationModel.Address.ZipPostalCode = drOrganisation["ZipPostalCode"].ToString();
                        OrganisationModel.Address.Address1 = drOrganisation["Address1"].ToString();
                        OrganisationModel.Address.Address2 = drOrganisation["Address2"].ToString();
                        OrganisationModel.Address.Latitude = (drOrganisation["Latitude"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drOrganisation["Latitude"]);
                        OrganisationModel.Address.Longitude = (drOrganisation["Longitude"] == DBNull.Value) ? (double?)null : Convert.ToDouble(drOrganisation["Longitude"]);
                        OrganisationModel.CreatedBy = Convert.ToInt32(drOrganisation["CreatedBy"]);
                        OrganisationModel.ModifiedBy = (drOrganisation["ModifiedBy"] == DBNull.Value) ? (int?)null : Convert.ToInt32(drOrganisation["ModifiedBy"]);
                        OrganisationModel.CreatedOn = (drOrganisation["CreatedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["CreatedOn"].ToString());
                        OrganisationModel.ModifiedOn = (drOrganisation["ModifiedOn"] == DBNull.Value) ? (DateTimeOffset?)null : DateTimeOffset.Parse(dbManager.DataReader["ModifiedOn"].ToString());
                    }
                    drOrganisation.Close();

                    //OrganisationModel.Phone = this._IPhoneRepository.GetAllPhoneList("organization", OrganizationIntId);
                    OrganisationModel.Phone = null;

                    string orgTypeQuery = QueryConfig.OrganizationQuerySettings["GetAllMappedTypes"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrganizationId", OrganizationIntId);

                    IDataReader orgTypeReader = dbManager.ExecuteReader(CommandType.Text, orgTypeQuery);
                    while (orgTypeReader.Read())
                    {
                        organizationType = new OrganizationType();
                        //organizationType.Id = Convert.ToInt32(orgTypeReader["Id"]);
                        organizationType.Name = Convert.ToString(orgTypeReader["Name"]);
                        organizationType.Code = Convert.ToString(orgTypeReader["Code"]);
                        organizationType.TypeId = Convert.ToInt32(orgTypeReader["typeId"]);
                        OrgTypeList.Add(organizationType);
                    }
                    OrganisationModel.OrganizationType = OrgTypeList;

                }
                return OrganisationModel;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            finally
            {
                OrganisationModel = null;
            }
        }

        #endregion

        #region POST Methods

        /// <summary>Add Organizations</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Organization">Object of Organization Model</param>
        /// <returns>Insert Status</returns>
        public bool AddOrganization(OrganizationModel organization)
        {
            bool retval = false;
            int organizationId = 0;
			try
			{

				using (dbManager = new DBManager())
				{
					dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
					dbManager.Open();
					dbManager.Transaction = this.dbManager.Connection.BeginTransaction();

					string checkDuplicate = QueryConfig.OrganizationQuerySettings["CheckDuplicateOrgCode"].ToString();
					dbManager.CreateParameters(1);
					dbManager.AddParameters(0, "@OrganizationCode", organization.OrganizationCode);
					IDataReader idr = dbManager.ExecuteReader(CommandType.Text, checkDuplicate);

					if (idr.Read())
					{
						idr.Close();
						throw new DuplicateException();
					}
					else
                    {
                        idr.Close();
                        UserContext userContext = JsonConvert.DeserializeObject<UserContext>(CacheManager.Instance.Get("usercontext_" + this.RequestId));

                        FolderModel folderDetails = new FolderModel()
                        {
                            FolderName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(organization.Name.ToLower()),
                            FolderCode = Convert.ToString(organization.OrgGUID),
                            ParentFolderCode = ConfigurationManager.AppSettings["ProjectName"].ToString(),
                            CreatedBy = Convert.ToInt32(organization.CreatedBy),
                            CreatedOn = DateTime.Now,
                            PhysicallFolderName = "Organization/" + Convert.ToString(organization.OrgGUID)
                        };

                        retval = _IDocumentProcessRepository.CreateFolder(folderDetails, dbManager.GetControlDBConnectionString(), ConfigurationManager.AppSettings["ProjectName"].ToString()) > 0 ? true : false; ;
                        if (retval)
                        {
                            int addressId = 0;
                            string addressTransaction = QueryConfig.AddressQuerySettings["AddAddressDetails"].ToString();
                            dbManager.CreateParameters(13);
                            dbManager.AddParameters(0, "@FirstName", organization.Address.FirstName);
                            dbManager.AddParameters(1, "@LastName", organization.Address.LastName);
                            dbManager.AddParameters(2, "@CountryId", organization.Address.CountryId.Id);
                            dbManager.AddParameters(3, "@StateProvinceId", organization.Address.StateProvinceId);
                            dbManager.AddParameters(4, "@City", organization.Address.City);
                            dbManager.AddParameters(5, "@Address1", organization.Address.Address1);
                            dbManager.AddParameters(6, "@Address2", organization.Address.Address2);
                            dbManager.AddParameters(7, "@ZipPostalCode", organization.Address.ZipPostalCode);
                            dbManager.AddParameters(8, "@Latitude", organization.Address.Latitude);
                            dbManager.AddParameters(9, "@Longitude", organization.Address.Longitude);
                            dbManager.AddParameters(10, "@CustomAttributes", organization.Address.CustomAttributes);
                            dbManager.AddParameters(11, "@CreatedBy", Convert.ToInt32(organization.CreatedBy));
                            dbManager.AddParameters(12, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                            object address = dbManager.ExecuteScalar(CommandType.Text, addressTransaction);
                            addressId = Convert.ToInt32(address);

                            if (addressId > 0)
                            {
                                organizationId = Convert.ToInt32(AddOrganizationDetails(organization, addressId));
                            }

                            if (Convert.ToInt32(organizationId) > 0)
                            {
                                AddOrgUser(Convert.ToInt32(userContext.UserId), organizationId);
                                AddOrgTypeMapping(organization.OrganizationType, organizationId);
                                this._IPhoneRepository.AddPhoneDetails(organization.Phone, organizationId, dbManager);
                            }

                            if (organizationId > 0)
                            {
                                dbManager.Transaction.Commit();
                                retval = true;
                            }
                            else
                            {
                                dbManager.Transaction.Rollback();
                                retval = false;
                            }
                        }
                    }
                    return retval;
				}
			}
			catch (SqlException sqlEx)
			{
				if (dbManager.Transaction != null)
					dbManager.Transaction.Rollback();
				LogManager.Log(sqlEx);
				throw new RepositoryException("ADDORGANIZATIONFAILED", MessageConfig.MessageSettings["ADDORGANIZATIONFAILED"].ToString(), sqlEx.StackTrace);
			}
			catch (DuplicateException dEx)
			{
				if (dbManager.Transaction != null)
					dbManager.Transaction.Rollback();
				LogManager.Log(dEx);
				throw new DuplicateException("Organization Code", " " + MessageConfig.MessageSettings["MAPPED"], "");
			}
			catch (Exception ex)
			{
				if (dbManager.Transaction != null)
					dbManager.Transaction.Rollback();
				LogManager.Log(ex);
				throw new RepositoryException("ADDORGANIZATIONFAILED", MessageConfig.MessageSettings["ADDORGANIZATIONFAILED"].ToString(), ex.StackTrace);
			}
        }

        public bool RegisterOrganization(OrganizationModel organization)
        {
            bool result = false;
            try
            {
                if (RegisterControlOranization(organization))
                {
                    using (dbManager = new DBManager())
                    {
                        dbManager.SynchronizeWithRedisConnectionInfo(ConfigurationManager.AppSettings["HostName"].ToString() + "_" + ConfigurationManager.AppSettings["ProjectName"].ToString() + "_" + "ConnectionString");
                        dbManager.GenerateConnectionString(organization.OrgGUID.ToString().Trim().ToUpper());
                        dbManager.Open();
                        this.ConnectionId = organization.OrgGUID.ToString().Trim().ToUpper();
                        int id = AddOrganizationDetails(organization);
                        if (id > 0)
                        {
                            result = true;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            return result;
        }
        
        private bool RegisterControlOranization(OrganizationModel organization)
        {
            bool result = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = dbManager.Connection.BeginTransaction();
                    string query = QueryConfig.OrganizationQuerySettings["RegisterControlOranization"].ToString();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@CompanyName", organization.Name);
                    dbManager.AddParameters(1, "@Code", organization.OrgGUID.ToString().ToUpper().Substring(0, 3));
                    dbManager.AddParameters(2, "@CompanyId", organization.OrgGUID);
                    int organisationId = ConvertData.ToInt(dbManager.ExecuteScalar(CommandType.Text, query));
                    if (organisationId > 0)
                    {
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(dbManager.ConnectionString);
                        query = QueryConfig.OrganizationQuerySettings["CreateAppForOrganization"].ToString();
                        dbManager.CreateParameters(8);
                        dbManager.AddParameters(0, "@orgId", organisationId);
                        dbManager.AddParameters(1, "@DBServerName", builder.DataSource);
                        dbManager.AddParameters(2, "@DBName", "Founders");
                        dbManager.AddParameters(3, "@DBUserName", builder.UserID);
                        dbManager.AddParameters(4, "@DBPassword", builder.Password);
                        dbManager.AddParameters(5, "@ConnectionId", organization.OrgGUID);
                        dbManager.AddParameters(6, "@CompanyModuleName", organization.Name);
                        dbManager.AddParameters(7, "@AppId", Apps.EKAHSALES.ToString());
                        int appId = ConvertData.ToInt(dbManager.ExecuteScalar(CommandType.Text, query));
                        if (appId > 0)
                        {
                            dbManager.Transaction.Commit();
                            result = true;
                        }
                        else
                        {
                            dbManager.Transaction.Rollback();
                            result = false;
                        }
                    }
                    else
                    {
                        dbManager.Transaction.Rollback();
                        result = false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ORGANIZATIONFOUNDFAIL", MessageConfig.MessageSettings["ORGANIZATIONFOUNDFAIL"].ToString(), ex.StackTrace);
            }
            return result;
        }

        private int AddOrganizationDetails(OrganizationModel organization,int addressId = 0)
        {
            string query = QueryConfig.OrganizationQuerySettings["AddOrganizationDetail"].ToString();
            dbManager.CreateParameters(18);
            dbManager.AddParameters(0, "@Name", organization.Name);
            dbManager.AddParameters(1, "@Description", organization.Description);
            dbManager.AddParameters(2, "@OrgGUID", organization.OrgGUID);
            dbManager.AddParameters(3, "@Logo", organization.Logo);
            dbManager.AddParameters(4, "@Email", organization.Email);
            dbManager.AddParameters(5, "@Website", organization.Website);
            dbManager.AddParameters(6, "@AdminId", Convert.ToInt32(organization.AdminId));
            dbManager.AddParameters(7, "@IsActive", Convert.ToInt32(organization.IsActive));
            dbManager.AddParameters(8, "@CreatedBy", Convert.ToInt32(organization.CreatedBy));
            dbManager.AddParameters(9, "@CreatedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
            dbManager.AddParameters(10, "@IsDeleted", organization.IsDeleted);
            dbManager.AddParameters(11, "@IsTaxExcempt", organization.IsTaxExcempt);
            dbManager.AddParameters(12, "@OrganizationCode", organization.OrganizationCode.ToUpper());
            dbManager.AddParameters(13, "@SequnceNumber", Convert.ToInt32(0));
            dbManager.AddParameters(14, "@HasDeliveryCharge", Convert.ToBoolean(organization.HasDeliveryCharge));
            dbManager.AddParameters(15, "@BackgroundColor", Convert.ToString(organization.BackgroundColor));
            dbManager.AddParameters(16, "@InstanceId", organization.InstanceId);
            dbManager.AddParameters(17, "@AddressId", addressId);
            object organisationId = dbManager.ExecuteScalar(CommandType.Text, query);
            return Convert.ToInt32(organisationId);
        }


        /// <summary>Mapping User to Organization</summary>
        /// <param name="organizationUser">Organization Users</param>
        /// <returns>Insert Status</returns>
        public bool AddOrganizationUsers(OrganizationUsersModel organizationUser)
        {

            try
            {
                int controlOrganisationUserId = 0;
                List<int> Users = organizationUser.Users;
                int totalCount = Users.Count();

                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();
                    foreach (int user in Users)
                    {
                        controlOrganisationUserId += AddOrgUser(user, organizationUser.OrganizationId);
                    }
                    if (Convert.ToInt32(controlOrganisationUserId) == totalCount)
                    {
                        dbManager.Transaction.Commit();
                        return true;
                    }
                    else
                    {
                       dbManager.Transaction.Rollback();
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDORGANIZATIONUSERFAILED", MessageConfig.MessageSettings["ADDORGANIZATIONUSERFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                LogManager.Log(ex);
                throw new RepositoryException("ADDORGANIZATIONUSERFAILED", MessageConfig.MessageSettings["ADDORGANIZATIONUSERFAILED"].ToString(), ex.StackTrace);
            }

        }

        private int AddOrgUser(int userId,int orgId)
        {
            string query1 = QueryConfig.OrganizationQuerySettings["AddorganizationUserDetailsControl"].ToString();
            dbManager.CreateParameters(2);
            dbManager.AddParameters(0, "@UserId", userId);
            dbManager.AddParameters(1, "@OrganizationId", orgId);
            return dbManager.ExecuteNonQuery(CommandType.Text, query1);
        }
        /// <summary>Add Organization Type</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Organization">Object of Organization Type Model</param>
        /// <returns>Insert Status</returns>
        public bool AddOrgTypeMapping(List<OrganizationType> orgTypeDetails, int OrganizationId)
        {
            bool retVal = false;
            int mappingValue = 0;

            try
            {
                foreach (OrganizationType items in orgTypeDetails)
                {
                    string checkDuplicate = QueryConfig.OrganizationQuerySettings["checkDuplicateOrgType"].ToString();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@OrgTypeId", items.Id);
                    dbManager.AddParameters(1, "@OrganizationId", OrganizationId);
                    IDataReader dr = dbManager.ExecuteReader(CommandType.Text, checkDuplicate);

                    if (dr.Read())
                    {
                        dr.Close();
                        throw new DuplicateException();
                    }
                    else 
                    {
                        dr.Close();
                        string mappingOrgType = QueryConfig.OrganizationQuerySettings["AddOrgTypeMapping"].ToString();
                        dbManager.CreateParameters(2);
                        dbManager.AddParameters(0, "@OrgTypeId", items.Id);
                        dbManager.AddParameters(1, "@OrganizationId", OrganizationId);
                        mappingValue = dbManager.ExecuteNonQuery(CommandType.Text, mappingOrgType);
                    }
                }

                if (Convert.ToInt32(mappingValue) > 0)
                {
                    retVal = true;
                }
                return retVal;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("ADDORGANIZATIONMAPFAIL", MessageConfig.MessageSettings["ADDORGANIZATIONMAPFAIL"].ToString(), sqlEx.StackTrace);
            }
            catch (DuplicateException dEx)
            {
                LogManager.Log(dEx);
                throw new DuplicateException("MAPPED", " " + MessageConfig.MessageSettings["MAPPED"], "");
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("ADDORGANIZATIONMAPFAIL", MessageConfig.MessageSettings["ADDORGANIZATIONMAPFAIL"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region PUT Methods

        /// <summary>Update Organization Details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="Group">Object of Organization Model</param>
        /// <returns>Update Organization Details</returns>
        public bool Updateorganization(OrganizationModel organization)
        {
            try
            {
                bool status = false;
                int returnId = 0;

                using (dbManager = new DBManager())
                {
                  
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();  
                    dbManager.Open();
                    dbManager.Transaction = this.dbManager.Connection.BeginTransaction();

                    string query = QueryConfig.OrganizationQuerySettings["UpdateOrganizationDetail"].ToString();
                    dbManager.CreateParameters(19);
                    dbManager.AddParameters(0, "@Name", organization.Name);
                    dbManager.AddParameters(1, "@Description", organization.Description);
                    dbManager.AddParameters(2, "@Logo", organization.Logo);
                    dbManager.AddParameters(3, "@Email", organization.Email);
                    dbManager.AddParameters(4, "@Website", organization.Website);
                    dbManager.AddParameters(5, "@AdminId", Convert.ToInt32(organization.AdminId));
                    dbManager.AddParameters(6, "@IsActive", Convert.ToInt32(organization.IsActive));
                    dbManager.AddParameters(7, "@ModifiedBy", Convert.ToInt32(organization.ModifiedBy));
                    dbManager.AddParameters(8, "@ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                    dbManager.AddParameters(9, "@organisationId", organization.Id);
                    dbManager.AddParameters(10, "@IsDeleted", organization.IsDeleted);
                    dbManager.AddParameters(11, "@IsTaxExcempt", organization.IsTaxExcempt);
                    dbManager.AddParameters(12, "@HasDeliveryCharge", Convert.ToBoolean(organization.HasDeliveryCharge));
                    dbManager.AddParameters(13, "@BackgroundColor", Convert.ToString(organization.BackgroundColor));
                    dbManager.AddParameters(14, "@InstanceId", organization.InstanceId);
                    dbManager.AddParameters(15, "@AddressId", organization.Address.Id);
                    dbManager.AddParameters(16, "@OrgHomeRedirect", organization.OrgHomeRedirect);
                    dbManager.AddParameters(17, "@BookDisplayMode", organization.BookDisplayMode);
                    dbManager.AddParameters(18, "@OrgMessage", organization.OrgMessage);

                    returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                
                    if (returnId > 0)
                    {
                        int addressId = 0;
                        string addressQuery = QueryConfig.AddressQuerySettings["UpdateAddressDetails"].ToString();
                        dbManager.CreateParameters(14);
                        dbManager.AddParameters(0, "@FirstName", organization.Address.FirstName);
                        dbManager.AddParameters(1, "@LastName", organization.Address.LastName);
                        dbManager.AddParameters(2, "@CountryId", organization.Address.CountryId.Id);
                        dbManager.AddParameters(3, "@StateProvinceId", organization.Address.StateProvinceId);
                        dbManager.AddParameters(4, "@City", organization.Address.City);
                        dbManager.AddParameters(5, "@Address1", organization.Address.Address1);
                        dbManager.AddParameters(6, "@Address2", organization.Address.Address2);
                        dbManager.AddParameters(7, "@ZipPostalCode", organization.Address.ZipPostalCode);
                        dbManager.AddParameters(8, "@Latitude", organization.Address.Latitude);
                        dbManager.AddParameters(9, "@Longitude", organization.Address.Longitude);
                        dbManager.AddParameters(10, "@CustomAttributes", organization.Address.CustomAttributes);
                        dbManager.AddParameters(11, "@ModifiedBy", Convert.ToInt32(organization.ModifiedBy));
                        dbManager.AddParameters(12, "@ModifiedOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz"));
                        dbManager.AddParameters(13, "@AddressId", organization.Address.Id);
                        addressId = dbManager.ExecuteNonQuery(CommandType.Text, addressQuery);
                    }                   
                    dbManager.Transaction.Commit();
                    status = true;
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
                 
                LogManager.Log(sqlEx);
                throw new RepositoryException("UPDATEORGANIZATIONFAILED", MessageConfig.MessageSettings["UPDATEORGANIZATIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                if (dbManager.Transaction != null)
                    dbManager.Transaction.Rollback();
               
                LogManager.Log(ex);
                throw new RepositoryException("UPDATEORGANIZATIONFAILED", MessageConfig.MessageSettings["UPDATEORGANIZATIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        #region DELETE Methods

        ///<summary>Remove Organization from Ecom and Control</summary>
        ///<param name="Code">Code in Entity</param>
        ///<return>Status</return>
        public bool RemoveOrganizationDetails(Guid OrgGUID)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString(); 
                    dbManager.Open();
                    string query = QueryConfig.OrganizationQuerySettings["RemoveOrganizationInfo"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrgGUID", OrgGUID);
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
                throw new RepositoryException("DELETEORGANIZATIONFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEORGANIZATIONFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        /// <summary>Deactivate Group details</summary>
        /// <param name="code">Code in Entity</param>
        /// <param name="groupId">Group Id</param>
        /// <returns>Deactivate Status</returns>
        public bool RemoveOrganization(int organizationId)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.OrganizationQuerySettings["RemoveOrganization"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrganizationId", organizationId);

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
                throw new RepositoryException("DELETEORGANIZATIONFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEORGANIZATIONFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        ///<summary>Remove Organization Type</summary>
        /// <param name="id">Organization Type Mapped Id</param>
        ///<return>Status</return>
        public bool RemoveOrgTypeMapping(int Id)
        {
            try
            {
                bool status = false;
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();

                    string removeOrgTypeMap = QueryConfig.OrganizationQuerySettings["RemoveOrgTypeMapping"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@Id", Id);

                    int returnControlId = dbManager.ExecuteNonQuery(CommandType.Text, removeOrgTypeMap);
                    if (returnControlId > 0)
                    {
                        status = true;
                    }
                    return status;
                }
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEORGANIZATIONTYPEMAPFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONTYPEMAPFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEORGANIZATIONTYPEMAPFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONTYPEMAPFAILED"].ToString(), ex.StackTrace);
            }
        }

        public bool RemoveInteruptedOrg(int organizationId)
        {
            bool status = false;
            try
            {
                using (dbManager = new DBManager())
                {
                    dbManager.ConnectionString = dbManager.GetControlDBConnectionString();
                    dbManager.Open();
                    string query = QueryConfig.OrganizationQuerySettings["RemoveErrorOrganization"].ToString();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@OrganizationId", organizationId);

                    int returnId = dbManager.ExecuteNonQuery(CommandType.Text, query);
                    if (returnId > 0)
                    {
                        status = true;
                    }
                }
                return status;
            }
            catch (SqlException sqlEx)
            {
                LogManager.Log(sqlEx);
                throw new RepositoryException("DELETEORGANIZATIONFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONFAILED"].ToString(), sqlEx.StackTrace);
            }
            catch (Exception ex)
            {
                LogManager.Log(ex);
                throw new RepositoryException("DELETEORGANIZATIONFAILED", MessageConfig.MessageSettings["DELETEORGANIZATIONFAILED"].ToString(), ex.StackTrace);
            }
        }

        #endregion

        /// <summary>
        /// Register void method to dispose object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}