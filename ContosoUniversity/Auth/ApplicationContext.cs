using Microsoft.AspNet.Identity.EntityFramework;

namespace ContosoUniversity.Auth
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext() : base("IdentityDb") { }
       
        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }

}