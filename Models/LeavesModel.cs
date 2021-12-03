using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FinalPayrollSystem.Models
{
    public class LeavesModel
    {
        public int leavehistid { get; set; }

        [Required()]
        [DataType(DataType.Text)]
        public string employeeid { get; set; }

        [Required()]
        [DataType(DataType.Text)]
        public string employeename { get; set; }

        [Required()]
        [DataType(DataType.Text)]
        public string appliedleave { get; set; }

        [Required()]
        public int rempaidleave { get; set; }

        public bool isdeductible { get; set; }

        [Required()]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "yyyy-MM-dd")]
        public string leavedatefrom { get; set; }

        [Required()]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "yyyy-MM-dd")]
        public string leavedateto { get; set; }


        public string addedby { get; set; }
        public DateTime dateadded { get; set; }

        public IEnumerable<LeavesModel> lvenum { get; set; }
        public IOrderedEnumerable<LeavesModel> sortdata { get; set; }
    }
}
