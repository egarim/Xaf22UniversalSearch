using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Xaf22UniversalSearch.Module.BusinessObjects;
using DevExpress.XtraRichEdit.Model.History;

namespace Xaf22UniversalSearch.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater
{
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion)
    {
    }
    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();
        //string name = "MyName";
        //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
        //if(theObject == null) {
        //    theObject = ObjectSpace.CreateObject<DomainObject1>();
        //    theObject.Name = name;
        //}
#if !RELEASE
        ApplicationUser sampleUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "User");
        if (sampleUser == null)
        {
            sampleUser = ObjectSpace.CreateObject<ApplicationUser>();
            sampleUser.UserName = "User";
            // Set a password if the standard authentication type is used
            sampleUser.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)sampleUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(sampleUser));
        }
        PermissionPolicyRole defaultRole = CreateDefaultRole();
        sampleUser.Roles.Add(defaultRole);

        ApplicationUser userAdmin = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "Admin");
        if (userAdmin == null)
        {
            userAdmin = ObjectSpace.CreateObject<ApplicationUser>();
            userAdmin.UserName = "Admin";
            // Set a password if the standard authentication type is used
            userAdmin.SetPassword("");

            // The UserLoginInfo object requires a user object Id (Oid).
            // Commit the user object to the database before you create a UserLoginInfo object. This will correctly initialize the user key property.
            ObjectSpace.CommitChanges(); //This line persists created object(s).
            ((ISecurityUserWithLoginInfo)userAdmin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(userAdmin));
        }
        // If a role with the Administrators name doesn't exist in the database, create this role
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if (adminRole == null)
        {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
        }
        adminRole.IsAdministrative = true;
        userAdmin.Roles.Add(adminRole);

        if(this.ObjectSpace.GetObjectsCount(typeof(Product),null)==0)
        {
            this.CreateCity();
            this.CreateProducts();

        }
        //var Product = this.ObjectSpace.CreateObject<Product>();
        //Product.Name = "Hamburger";
        //Product.Category = "Fast food";
        //Product.Description = "Double decker hamburger with cheese";

        //var City = this.ObjectSpace.CreateObject<City>();
        //City.Name = "San Salvador";
        //City.Country = "El Salvador";

        ObjectSpace.CommitChanges(); //This line persists created object(s).
#endif
    }
    void CreateCity()
    {


        // Define an array of 25 real cities and countries
        string[] cities = {
    "Paris, France",
    "New York City, United States",
    "Tokyo, Japan",
    "London, United Kingdom",
    "Berlin, Germany",
    "Sydney, Australia",
    "Toronto, Canada",
    "São Paulo, Brazil",
    "Moscow, Russia",
    "Madrid, Spain",
    "Bangkok, Thailand",
    "Rome, Italy",
    "Amsterdam, Netherlands",
    "Beijing, China",
    "Dubai, United Arab Emirates",
    "Stockholm, Sweden",
    "Vienna, Austria",
    "Zurich, Switzerland",
    "Seoul, South Korea",
    "Mumbai, India",
    "Cape Town, South Africa",
    "Helsinki, Finland",
    "Oslo, Norway",
    "Copenhagen, Denmark",
    "Buenos Aires, Argentina"
};

        // Create 25 new instances of the City class using the cities array
        for (int i = 0; i < 25; i++)
        {
            var newCity = this.ObjectSpace.CreateObject<City>();
            string[] cityData = cities[i].Split(", ");
            newCity.Name = cityData[0];
            newCity.Country = cityData[1];
        }

    }
    void CreateProducts()
    {
        string[] foodNames = { "Hamburger", "Pizza", "Tacos", "Sushi", "Fried chicken", "Spaghetti", "Burrito", "Steak", "Salmon", "Pad Thai" };
        string[] categories = { "Burgers", "Pizza", "Mexican", "Japanese", "Fried chicken", "Italian", "Mexican", "Steakhouse", "Seafood", "Thai" };

        // Create 25 new products using the food names and categories
        for (int i = 0; i < 25; i++)
        {
            var Product = this.ObjectSpace.CreateObject<Product>();
            Product.Name = foodNames[i % 10]; // Use the modulo operator to cycle through the food names
            Product.Category = categories[i % 10]; // Use the modulo operator to cycle through the categories
            Product.Description = "Delicious " + foodNames[i % 10] + " with " + categories[i % 10] + " toppings";
        }
    }
    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
    private PermissionPolicyRole CreateDefaultRole()
    {
        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
        if (defaultRole == null)
        {
            defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            defaultRole.Name = "Default";

            defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
            defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
        }
        return defaultRole;
    }
}
