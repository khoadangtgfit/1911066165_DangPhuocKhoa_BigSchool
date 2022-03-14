using _1911066165_DangPhuocKhoa_BigSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _1911066165_DangPhuocKhoa_BigSchool.ViewModels
{
    public class CourseViewModel
    {
        public string Place { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public byte Category { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        //commit lan 2
        public DateTime GetDateTime()
        {
            return DateTime.Parse(string.Format("{0}{1}", Date, Time));
        }
    }
}