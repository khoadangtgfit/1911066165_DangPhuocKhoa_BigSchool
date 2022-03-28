using _1911066165_DangPhuocKhoa_BigSchool.Models;
using _1911066165_DangPhuocKhoa_BigSchool.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.ModelBinding;
using System.Globalization;
using System.Threading;

namespace _1911066165_DangPhuocKhoa_BigSchool.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CoursesController()
        {
            _dbContext = new ApplicationDbContext();
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "dd-MMM-yyyy";
            culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;
        }
        //
        // GET: /Courses/Create
        public ActionResult Create()
        {
            var viewModel = new CourseViewModel()
            {
                Categories = _dbContext.Categories.ToList(),
                Heading = "Add Course"
            };
            return View(viewModel);
        }
        //
        // POST: /Courses/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _dbContext.Categories.ToList();
                return View("Create", viewModel);
            }
            var course = new Course()
            {
                LecturerId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                CategoryId = viewModel.Category,
                Place = viewModel.Place
            };
            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbContext.Attendances
                .Where(a => a.AttendeeId == userId)
                .Select(a => a.Course)
                .Include(l => l.Lecturer)
                .Include(l => l.Category)
                .Where(a => a.IsCanceled == false)
                .ToList();

            var viewModel = new CoursesViewModel
            {
                UpcommingCourses = courses,
                ShowAction = User.Identity.IsAuthenticated
            };

            return View(viewModel);
        }
        [Authorize]
        public ActionResult Following()
        {
            var userId = User.Identity.GetUserId();
            var followings = _dbContext.Followings
                .Where(a => a.FolloweeId == userId)
                .Select(a => a.Follower)
                .ToList();

            var viewModel = new FollowingViewModel
            {
                Followings = followings,
                ShowAction = User.Identity.IsAuthenticated
            };

            return View(viewModel);
        }
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbContext.Courses.Where(c => c.LecturerId == userId && c.DateTime > DateTime.Now).Include(l => l.Lecturer).Include(c => c.Category).ToList();
            return View(courses);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();

            var course = _dbContext.Courses.Single(c => c.Id == id && c.LecturerId == userId);

            var viewModel = new CourseViewModel
            {
                Categories = _dbContext.Categories.ToList(),
                Date = course.DateTime.ToString("dd/M/yyyy"),
                Time = course.DateTime.ToString("HH:mm"),
                Category = course.CategoryId,
                Place = course.Place,
                Heading = "Edit Course",
                Id = course.Id
            };

            return View("Create", viewModel);

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _dbContext.Categories.ToList();
                return View("Create", viewModel);
            }

            var userId = User.Identity.GetUserId();
            var course = _dbContext.Courses.Single(c => c.Id == viewModel.Id && c.LecturerId == userId);

            course.Place = viewModel.Place;
            course.DateTime = viewModel.GetDateTime();
            course.CategoryId = viewModel.Category;

            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult FollowingMeList()
        {
            var userId = User.Identity.GetUserId();
            var followings = _dbContext.Followings
                .Where(a => a.FollowerId == userId)
                .Select(a => a.Followee)
                .ToList();

            var viewModel = new FollowingViewModel
            {
                Followings = followings,
                ShowAction = User.Identity.IsAuthenticated
            };

            return View(viewModel);
        }
        public ActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var course = _dbContext.Courses.Single(c => c.Id == id && c.LecturerId == userId);
            _dbContext.Courses.Remove(course);
            _dbContext.SaveChanges();

            return RedirectToAction("Mine", "Courses");
        }
        [Authorize]
        public ActionResult FollowNotification()
        {
            var viewModel = new FollowNotificationViewModel
            {
                Notifications = _dbContext.FollowingNotifications.ToList()
            };

            return View(viewModel);
        }
        public ActionResult UnFollow(string followeeId, string followerId)
        {
            var follow = _dbContext.Followings
                .Where(x => x.FolloweeId == followeeId && x.FollowerId == followerId)
                .Include(x => x.Followee)
                .Include(x => x.Follower).SingleOrDefault();

            var followingNotification = new FollowingNotification()
            {
                Id = 0,
                Logger = follow.Follower.Name + " unfollow " + follow.Followee.Name
            };

            _dbContext.FollowingNotifications.Add(followingNotification);

            _dbContext.Followings.Remove(follow);
            _dbContext.SaveChanges();
            return RedirectToAction("Following", "Courses");
        }


    }
}