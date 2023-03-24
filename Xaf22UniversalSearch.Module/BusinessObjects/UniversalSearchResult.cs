using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xaf22UniversalSearch.Module.BusinessObjects
{
    [ModelDefault("Caption", "Search")]
    [DomainComponent]
    [NavigationItem("Universal Search")]
    [ImageName("Action_Search")]
    public class UniversalSearchResult:NonPersistentBaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public string ObjectDisplayName { get; set; }
        public string Display { get; set; }

        [Browsable(false)]
        public Type ObjectType { get; set; }

        [Browsable(false)]
        public string ObjectKey { get; set; }

        [Browsable(false)]
        public object ObjectKeyAsObject { get; set; }
    }
}