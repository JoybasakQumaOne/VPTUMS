using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace QM.UMS.Business
{ /// <summary>
    /// Register UnityResolver Class
    /// </summary>
    /// <param name="UnityResolver">UnityResolver inherit IDependencyResolver</param>
    public class UnityResolver : IDependencyResolver
    {
        /// <summary>
        /// Register protectedVariable
        /// </summary>
        /// <retuns>This will return IUnityContainer variable</retuns>
        private IUnityContainer container;
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityResolver"/> class.
        /// </summary>
        /// <param name="container">container is object of IUnityContainer</param>
        /// <returns>This will return containerList</returns>
        public UnityResolver(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }
        #endregion

        /// <summary>
        /// Register Object with GetMethod
        /// </summary>
        /// <param name="serviceType">serviceType is object of Type</param>
        /// <returns>This will return serviceType</returns>
      
        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Register IEnumerableObject with GetMethod
        /// </summary>
        /// <param name="serviceType">serviceType is object of Type</param>
        /// <returns>This will return serviceType</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Register IDependencyScope with BeginScopeMethod
        /// </summary>
        /// <returns>This will return UnityResolver</returns>
        public IDependencyScope BeginScope()
        {
            var child = this.container.CreateChildContainer();
            return new UnityResolver(child);
        }

        /// <summary>
        /// Register Void with DisposeMethod
        /// </summary>
        public void Dispose()
        {
            this.container.Dispose();
        }
    }
    
}