// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizationRepository.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>17-10-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Repository.IRepository
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;   
    using System.Globalization;
    using System.Configuration;
    using CommonApplicationFramework.Common;
    using QM.UMS.Models; 
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IOrganizationRepository>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IOrganizationRepository
    {
        string RequestId { get; set; }

        #region GET Methods
        List<OrganizationModel> GetAllOrganizations();

        OrganizationModel GetOrganizationDetails(Guid Id);
        OrganizationModel GetOrganizationInfo(string Id);

        #endregion

        #region POST Methods

        bool AddOrganization(OrganizationModel organization);

        bool AddOrganizationUsers(OrganizationUsersModel organizationUser);

        bool AddOrgTypeMapping(List<OrganizationType> organizationTypeDetail, int OrganizationId);
        bool RegisterOrganization(OrganizationModel organization);

        #endregion

        #region PUT Methods

        bool Updateorganization(OrganizationModel userGroup);

        #endregion

        #region DELETE Methods

        bool RemoveOrganizationDetails(Guid Id);

        bool RemoveOrgTypeMapping(int id);

        #endregion
    }
}