using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalPayrollSystem.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Your username please!")]
        [StringLength(20, ErrorMessage = "There's something wrong with you!")]
        public string username { get; set; }

        [Required(ErrorMessage = "Your password please!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}
