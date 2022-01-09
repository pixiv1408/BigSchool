using BigSchoolLancuoi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.Mvc;

namespace BigSchoolLancuoi.Controllers
{

    public class HomeController : Controller
    {
        DataSchoolContext db = new DataSchoolContext();
        public ActionResult Index()
        {
            //List<Course> listCourse = db.Courses.Where(p => p.DateTime > DateTime.Now).OrderBy(p => p.DateTime).ToList();
            //ViewBag.UserID = User.Identity.GetUserId();
            //return View(listCourse);
            var upcommingCourse = db.Courses.Where(p => p.DateTime.ToString("yyyy/MM/dd") ==
                DateTime.Now.ToString("yyyy/MM/dd")).OrderBy(p => p.DateTime).ToList();
            //lấy user login hiện tại
            var userID = User.Identity.GetUserId();
            foreach (Course i in upcommingCourse)
            {
                //tìm Name của user từ lectureid
                ApplicationUser user =
                System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(
                ).FindById(i.LecturerId);
                i.Name = user.Name;
                //lấy ds tham gia khóa học
                if (userID != null)
                {
                    i.isLogin = true;
                    //ktra user đó chưa tham gia khóa học
                    Attendance find = db.Attendances.FirstOrDefault(p =>
                    p.CourseId == i.Id && p.Attendee == userID);
                    if (find == null)
                        i.isShowGoing = true;
                    //ktra user đã theo dõi giảng viên của khóa học ?
                    Following findFollow = db.Followings.FirstOrDefault(p =>
                    p.FollowerId == userID && p.FolloweeId == i.LecturerId);
                    if (findFollow == null)
                        i.isShowFollow = true;
                }
            }
            return View(upcommingCourse);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}