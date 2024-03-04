// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ModulesModel.cs" company="Pentechs">Copyright (c) Pentechs . All rights reserved.</copyright>
// <author>Debabrata</author>
// <createdOn>16-11-2016</createdOn>
// <comment></comment>
// ---------------------------------------------------------------------------------------------------------------

namespace QM.UMS.Models
{
    #region Namespace
    using CommonApplicationFramework.Common;
    using System.Collections.Generic;

    #endregion

    /// -----------------------------------------------------------------
    ///   Namespace:    <UserManagementSystem>
    ///   Class:        <ModulesModel>
    ///   Description:    
    ///   Author:       <Debabrata>                    
    /// -----------------------------------------------------------------

    public class Modules : Element
    {        
        public List<Page> Pages { get; set; }

        public List<Section> Sections { get; set; }

        public List<DefaultSection> DefaultSections { get; set; }

        public List<Element> Elements { get; set; } 
    }

    public class Page : Element
    {
        public string AutoTrigger { get; set; }

        public List<Section> Sections { get; set; }

        public List<Element> Elements { get; set; } 
    }

    public class Section : Element
    {
        public List<Element> Elements { get; set; } 
    }

    public class DefaultSection : Element
    {
        public List<Element> Elements { get; set; } 
    }

    public class Element 
    {
        public string Name { get; set; }

        public string FieldName { get; set; }

        public bool? IsDefault { get; set; }

        public string Permission { get; set; }
    }

    public class UserModule
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public Item Modules { get; set; }
    }

    public class DisplayUserModule
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public List<Item> ModuleList { get; set; }
    }



}