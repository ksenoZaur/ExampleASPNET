using System.Net.Http;
using System.Security.Principal;
using System.Web;

namespace ContosoUniversity.Support
{
    public interface IAuthentication
    {
        HttpContext HttpContext { get; set; }
        User Login(string login, string password, bool isPersistent);
        User Login(string login);
        void LogOut();
        IPrincipal CurrentUser { get; };
    }
}