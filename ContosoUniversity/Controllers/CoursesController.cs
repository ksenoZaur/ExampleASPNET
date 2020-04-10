using System.Linq;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private SchoolContext db = new SchoolContext();
        // GET
        public ActionResult Index(int? page)
        {
            int currentPage = (page ?? 1);
            ViewBag.CurrentPage = currentPage;
            return View(db.Courses.ToList().ToPagedList(currentPage, 2));
        }
    }
}