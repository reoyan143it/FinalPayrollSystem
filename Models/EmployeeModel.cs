using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalPayrollSystem.Models
{
    public class EmployeeModel
    {

        [Key]
        [Required()]
        [RegularExpression(@"^\w[A-Z]{1,3}[-]\w[A-Z]{1,3}[-]\d{1,3}$", ErrorMessage = "Kindly follow format!")]
        public string employeeid { get; set; }

        [Required()]
        [StringLength(20, ErrorMessage ="It is not a name!")]
        public string firstname { get; set; }

        [Required()]
        [StringLength(20, ErrorMessage = "It is not a name!")]
        public string middlename { get; set; }

        [Required()]
        [StringLength(20, ErrorMessage = "It is not a name!")]
        public string lastname { get; set; }

        [Required()]
        public string gender { get; set; }

        [Required()]
        [DataType(DataType.Date)]
        public DateTime birthdate { get; set; }

        [Required()]
        [DataType(DataType.Date)]
        public DateTime datehired { get; set; }

        [Required()]
        public string employmentstatus { get; set; }

        [Required()]
        public string employeestatus { get; set; }

        [Required()]
        public string barangays { get; set; }

        public string towns { get; set; }

        public string cities { get; set; }

        [Required()]
        public string provinces { get; set; }

        [Required()]
        public string department { get; set; }

        [Required()]
        public string positions { get; set; }

        public string addedby { get; set; } = "Eden Pancho";
        public DateTime dateadded { get; set; } = DateTime.Now;
    }
}
