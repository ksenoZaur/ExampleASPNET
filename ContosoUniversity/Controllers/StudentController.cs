using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ContosoUniversity.Auth;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext SchoolContext { get; set; } 
        
        [Authorize]// GET
        public ActionResult Index(string sortOrder, string searchWord, int? page)
        {
            ViewBag.NameOdrer = (String.IsNullOrEmpty(sortOrder) ? "name_desc" : "" );
            ViewBag.DateOrder = (sortOrder == "date" ? "date_desc" : "date");
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SearchWord = searchWord;
            
            SchoolContext = new SchoolContext();
            List<Student> students;

            using (ApplicationContext ac = new ApplicationContext())
            {
                var result = ac.Users.ToList();
                var res2 = ac.Roles.ToList();
            }
            
            if (Request.Cookies.AllKeys.Contains("test_cookie"))
            {
                DateTime dt = Convert.ToDateTime(Request.Cookies.Get("test_cookie").Value);
                Console.WriteLine(dt.ToString());
            } 
            else 
            {
                var cookie = new HttpCookie("test_cookie", DateTime.Now.ToString("MM/dd/yyyy"));
                Response.SetCookie(cookie);
            }
            if (String.IsNullOrEmpty(searchWord))
                students = SchoolContext.Students.ToList();
            else
                students = SchoolContext.Students
                    .Where(st => st.LastName.Contains(searchWord) || st.FirstMidName.Contains(searchWord)).ToList();
            
            switch (sortOrder)
            {
                case "name_desc" : 
                    students = students.OrderByDescending(s => s.LastName).ToList();
                    break;
                case "date" :
                    students = students.OrderBy(s => s.EnrollmentDate).ToList();
                    break;
                case "date_desc" :
                    students = students.OrderByDescending(s => s.EnrollmentDate).ToList();
                    break;
                default:
                    students = students.OrderBy(s => s.LastName).ToList();
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1 );
            
            return View(students.ToPagedList(pageNumber,pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolContext = new SchoolContext();
            Student student = SchoolContext.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")]Student student)
        {
            try
            {
                if (student.LastName == null || String.IsNullOrEmpty(student.LastName.Trim()))
                    ModelState.AddModelError("LastName","Укажите фамилию!");
                
                if (student.FirstMidName == null || string.IsNullOrEmpty(student.FirstMidName.Trim()))
                    ModelState.AddModelError("FirstMidName","Укажите имя!");
                
                if(student.EnrollmentDate == null || student.EnrollmentDate > DateTime.Now)
                    ModelState.AddModelError("EnrollmentDate","Введите верную дату!");
                
                if (ModelState.IsValid)
                {
                    SchoolContext = new SchoolContext();
                    SchoolContext.Students.Add(student);
                    SchoolContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("","Не удалось сохранить изменения, Повторите попытку" +
                                            ", если ошибка повторится снова, обратитесь к системному администратору!");
            }

            return View(student);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
          if (id == null)
              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
          SchoolContext = new SchoolContext();
          Student student = SchoolContext.Students.Find(id);
          return View(student);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult SaveChanges(int? id)
        {
            if (id == null)
              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SchoolContext = new SchoolContext();
            Student student = SchoolContext.Students.Find(id);
            if (TryUpdateModel(student, "", new string[] {"LastName", "FirstMidName", "EnrollmentDate"}))
            {
              try
              {
                  SchoolContext.SaveChanges();

                  return RedirectToAction("Index");
              }
              catch (DataException /* dex */)
              {
                  //Log the error (uncomment dex variable name and add a line here to write a log.
                  ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
              }
            }
          return View(student);
        }

        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
              return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (saveChangesError.GetValueOrDefault())
              ViewBag.ErrorMessage = "Delete filed. Try again or see your system administrator.";
                  
            SchoolContext = new SchoolContext();
            Student student = this.SchoolContext.Students.Find(id);

            if( student == null )
              return new HttpNotFoundResult();

            return View(student);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                SchoolContext = new SchoolContext();
                Student student = SchoolContext.Students.Find(id);
                SchoolContext.Students.Remove(student);
                SchoolContext.SaveChanges();
            }
            catch (Exception exc)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if( SchoolContext != null )
                    SchoolContext.Dispose();
            }
        base.Dispose(disposing);
        }
    }
}