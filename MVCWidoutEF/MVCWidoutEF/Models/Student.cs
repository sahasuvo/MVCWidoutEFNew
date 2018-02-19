using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCWidoutEF.Models
{
    public class StudentModel
    {
        [Display (AutoGenerateField =true, Name="Id")]
        public int ID { get; set; }
        [Required (AllowEmptyStrings =false,ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please Enter City Name")]
        public string CityName { get; set; }
        [Required (AllowEmptyStrings =false,ErrorMessage ="Please Enter Your Address")]
        public string Address { get; set; }
    }
}