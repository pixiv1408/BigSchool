using BigSchoolLancuoi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchoolLancuoi.Controllers
{
    public class CourseController : Controller
    {

        DataSchoolContext db = new DataSchoolContext();
        [HttpGet]
        // GET: Course
        public ActionResult Create()
        {
            Course objCourse = new Course();
            objCourse.listCategory = db.Categories.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.listCategory = db.Categories.ToList();
                return View("Create", objCourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;
            objCourse.DateTime = DateTime.Parse(objCourse.DateTime.ToString("yyyy/MM/dd HH:mm:ss"));
            objCourse.Name = user.Name;
            db.Courses.Add(objCourse);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Attending()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = db.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var item in listAttendances)
            {
                Course objCourse = item.Course;
                objCourse.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }

        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = db.Courses.Where(p => p.LecturerId == currentUser.Id && p.DateTime > DateTime.Now).ToList();
            foreach (var item in courses)
            {
                item.Name = currentUser.Name;
            }
            return View(courses);
        }

        public ActionResult Edit(int id)
        {
            //get list category
            
            var userId = User.Identity.GetUserId();
            Course course = db.Courses.FirstOrDefault(c => c.Id == id && c.LecturerId == userId);
            
            if (course == null)
            {
                return RedirectToAction("Mine", "Course");
            }
            course.listCategory = db.Categories.ToList();
            return View(course);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Course objCourse)
        {
            //List<Book> listbook = context.Books.ToList();
            Course find = db.Courses.FirstOrDefault(p => p.Id == objCourse.Id);
            if (find == null)
            {
                return RedirectToAction("Mine", "Course");
            }


            find.Place = objCourse.Place;
            find.DateTime = objCourse.DateTime;
            find.CategoryId = objCourse.CategoryId;
            db.SaveChanges();

            return RedirectToAction("Mine", "Course");
        }

        public ActionResult Delete(string id)
        {
            int id1 = Int32.Parse(id);
            Course obj = db.Courses.FirstOrDefault(p => p.Id == id1);
            db.Courses.Remove(obj);
            db.SaveChanges();
            return RedirectToAction("Mine", "Course");
        }

        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = db.Courses.Where(p => p.DateTime > DateTime.Now).ToList();
            List<Course> listFollow = new List<Course>();
            foreach (var item in courses)
            {
                Following find = db.Followings.FirstOrDefault(p => p.FollowerId == currentUser.Id && p.FolloweeId == item.LecturerId);
                if (find != null)
                    listFollow.Add(item);
            }
            return View(listFollow);
        }


    }



}