using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseMover.Models
{
    public class HomeViewModel
    {
        public string CanvasAccountId { get; set; }
        public bool Authorized { get; set; }

        [Display(Name = "Courses Data CSV")]
        public string CoursesDataFile { get; set; }
        public bool Notify { get; set; }
    }
}