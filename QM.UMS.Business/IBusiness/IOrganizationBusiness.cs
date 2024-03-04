// ----------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizationBusiness.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Gurudeep</author>
// <createdOn>18-10-2017</createdOn>
// <comment></comment>
// ----------------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Business.IBusiness
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using CommonApplicationFramework.Common;
    using QM.UMS.Models;
    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <IOrganizationBusiness>
    ///   Description:  <Contains CRUD, Link-Delink, Deactivate Operations>
    ///   Author:       <Gurudeep>                    
    /// -----------------------------------------------------------------

    public interface IOrganizationBusiness
    {
        string RequestId { get; set; }

        void Init(string requestId);

        #region GET Method

        List<OrganizationModel> GetAllOrganizations();

        OrganizationModel GetOrganizationDetails(Guid Id);
        OrganizationModel GetOrganizationInfo(string Id);

        #endregion

        #region POST Method

        bool AddOrganization(OrganizationModel organization);
        bool Register(RegisterOrganizationModel Organization);
        bool AddOrganizationUsers(OrganizationUsersModel organizationUser);
        bool AddOrgTypeMapping(List<OrganizationType> orgTypeDetails, int organizationId);

        #endregion

        #region PUT Method

        bool Updateorganization(OrganizationModel organization);

        #endregion

        #region DELETE Method

        bool RemoveOrganizationDetails(Guid Id);

        bool RemoveOrgTypeMapping(int Id);

        #endregion

    }
}
