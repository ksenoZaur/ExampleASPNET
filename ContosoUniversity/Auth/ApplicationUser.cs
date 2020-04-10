using Microsoft.AspNet.Identity.EntityFramework;

namespace ContosoUniversity.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public int Year { get; set; }
        public ApplicationUser()
        {
        }
    }
}