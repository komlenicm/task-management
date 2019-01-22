using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using TaskManagement.Models;

[assembly: OwinStartupAttribute(typeof(TaskManagement.Startup))]
namespace TaskManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        public void CreateRolesAndUsers() {

            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup im creating first Admin Role and creating a default Admin User 
            if (!roleManager.RoleExists("Administrator")) {

                // first we create Admin role
                var role = new IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website				

                var user = new ApplicationUser();
                user.UserName = "milosadmin";
                user.Email = "milosadmin@gmail.com";
                user.FirstName = "Milos";
                user.SurName = "Komlenic";

                string userPWD = "Milosadmin1!";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin
                if (chkUser.Succeeded) {
                    var result1 = UserManager.AddToRole(user.Id, "Administrator");

                }
            }

            // creating Creating Manager role 
            if (!roleManager.RoleExists("Project Manager")) {
                var role = new IdentityRole();
                role.Name = "Project Manager";
                roleManager.Create(role);

            }

            // creating Creating Developer role 
            if (!roleManager.RoleExists("Developer")) {
                var role = new IdentityRole();
                role.Name = "Developer";
                roleManager.Create(role);

            }
        }
    }
}
