using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalPayrollSystem.Models
{
    public class RatesModel
    {
        public int rateid { get; set; }

        [Required()]
        public string paytype { get; set; }

        [Required()]
        [DataType(DataType.Currency, ErrorMessage ="It should be number!")]
        public decimal salary { get; set; }

        [Required()]
        public string employeeid { get; set; }

        [Required()]
        public string employeename { get; set; } // NOT IN THE TABLE OF RATES, ONLY EMPLOYEE ID WILL BE SAVED

        public IEnumerable<RatesModel> rateslist { get; set; }
    }
}
