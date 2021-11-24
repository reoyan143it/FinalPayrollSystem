using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalPayrollSystem.Models
{
    public class RatesModel
    {
        public object rateid { get; set; } = null;

        [Required()]
        public string paytype { get; set; }

        [Required()]
        public decimal salary { get; set; }

        [Required()]
        public string dividedby { get; set; }

        [Required()]
        public string multipliedby { get; set; }

        [Required()]
        public string employeeid { get; set; }

        [Required()]
        public string employeename { get; set; }
    }
}
