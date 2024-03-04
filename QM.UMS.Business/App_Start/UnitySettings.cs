namespace QM.UMS.Business
{
    #region Namespace
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Microsoft.Practices.Unity;
    using Unity.WebApi;
    using QM.UMS.Business.Business;
    using QM.UMS.Business.IBusiness;
    using QM.UMS.Repository.IRepository;
    using QM.UMS.Repository.Repository;


    #endregion

    public class UnitySettings
    {
        /// <summary>
        /// Register Parameter with Method
        /// </summary>
        /// <param name="config">config is object of HttpConfiguration</param>
        public static void RegisterComponents(HttpConfiguration config)
        {
            try
            {
                var container = new UnityContainer();
                container.RegisterType<IActionBusiness, ActionBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IActionRepository, ActionRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IResourceBusiness, ResourceBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IResourceRepository, ResourceRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IRoleBusiness, RoleBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IRoleRepository, RoleRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IUserGroupBusiness, UserGroupBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IUserGroupRepository, UserGroupRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IUsersBusiness, UsersBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IUsersRepository, UsersRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IDepartmentBusiness, DepartmentBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IDepartmentRepository, DepartmentRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IDesignationBusiness, DesignationBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IDesignationRepository, DesignationRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IModuleBusiness, ModuleBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IModuleRepository, ModuleRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IOrganizationBusiness, OrganizationBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IOrganizationRepository, OrganizationRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IAttributeMasterBusiness, AttributeMasterBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IAttributeMasterRepository, AttributeMasterRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IAttributeOptionBusiness, AttributeOptionBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IAttributeOptionRepository, AttributeOptionRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IUserTypeBusiness, UserTypeBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IUserTypeRepository, UserTypeRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IMasterBusiness, MasterBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IMasterRepository, MasterRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IAttributeSectionBusiness, AttributeSectionBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IAttributeSectionRepository, AttributeSectionRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IInstanceBusiness, InstanceBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IInstanceRepository, InstanceRepository>(new HierarchicalLifetimeManager());

                //container.RegisterType<IOrganizationAttributeBusiness, OrganizationAttributeBusiness>(new HierarchicalLifetimeManager());
                //container.RegisterType<IOrganizationAttributeRepository, OrganizationAttributeRepository>(new HierarchicalLifetimeManager());

                //container.RegisterType<IOrganizationAttributeOptionBusiness, OrganizationAttributeOptionBusiness>(new HierarchicalLifetimeManager());
                //container.RegisterType<IOrganizationAttributeOptionRepository, OrganizationAttributeOptionRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IOrganizationTypeBusiness, OrganizationTypeBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IOrganizationTypeRepository, OrganizationTypeRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<IAttributeValueBusiness, AttributeValueBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IAttributeValueRepository, AttributeValueRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<ICustomerBusiness, CustomerBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<ICustomerRepository, CustomerRepository>(new HierarchicalLifetimeManager());


                container.RegisterType<IPhoneBusiness, PhoneBusiness>(new HierarchicalLifetimeManager());
                container.RegisterType<IPhoneRepository, PhoneRepository>(new HierarchicalLifetimeManager());

                container.RegisterType<ICommonHelperRepository, CommonHelperRepository>(new HierarchicalLifetimeManager());


                container.RegisterType<IDocumentProcessRepository, DocumentProcessRepository>(new HierarchicalLifetimeManager());
                config.DependencyResolver = new UnityDependencyResolver(container);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}