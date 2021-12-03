using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalPayrollSystem.Models
{
    public class PayrollSettingsModel
    {
        // COMPANY BASIC INFORMATION
        public string Code { get; set; } = "CEB";
        public string Company { get; set; } = "Rollway Group of Companies";
        public string Address { get; set; } = "777 Rollway Bldg. A.del Rosario St. Guizo Mandaue City Cebu";
        public string Telephone { get; set; } = "345-7594";
        public string TINno { get; set; } = "000-000-000-000";
        public string SSSno { get; set; } = "123456789021";
        public string PhilHealthno { get; set; } = "01123456789";
        public string VATno { get; set; } = "TX123456789";

        // PAYROLL INFORMATION
        public string EmployeeClass { get; set; } = "Monthlies";
        public int DividedBy { get; set; } = 288;
        public int MultipliedBy { get; set; } = 12;

        // PREMIUMS
        public double RegularOT { get; set; } = 1.25;
        public double RestDay { get; set; } = 1.50;
        public double RestDayOT { get; set; } = 1.65;
        public double NightDifferential { get; set; } = .25;
        public double RegularHolidayWork { get; set; } = 1.25;
        public double RegularHolidayOT { get; set; } = 1.50;
        public double RDRHWork { get; set; } = 2.0;
        public double RDRHOT { get; set; } = 2.2;
        public double SpecialHolidayWork { get; set; } = 1.25;
        public double SpecialHolidayOT { get; set; } = 1.30;
        public double RDSHWork { get; set; } = 1.50;
        public double RDSHOT { get; set; } = 1.70;

        // PREFERENCES
        //Deduction Basis
        public string SSSDeductBasis { get; set; } = "BASICPAY";
        public string WTXDeductBasis { get; set; } = "BASICPAY";
        public string PHLTDeductBasis { get; set; } = "GROSSPAY";

        // WithHolding Contribution Type
        public string WTXContributionType { get; set; } = "MONTHLY";

        // Employee Minimum Take Home Pay
        public bool IsFixedMinTakeHome { get; set; } = false;
        public decimal MinimumTakeHomeFixed { get; set; } = 0;
        public double GrossPayPercentage { get; set; } = 1;

        // Pag-ibig Contribution Type
        public double PagibigPercentCont { get; set; } = 0.10;
        public string PagibigContBasis { get; set; } = "BASICPAY";
        public double MaximumAmount { get; set; } = 100;

        // Late Deduction
        public bool IsFixedAmountLateDeduct { get; set; } = false;
        public int RoundLateDeduct { get; set; } = 1;
        public int LateDeductFixed { get; set; } = 0;

        // Undertime Deduction
        public bool IsFixedAmountUndertime { get; set; } = false;
        public int RoundUndertime { get; set; } = 0;
        public int UndertimeFixed { get; set; } = 0;

        // Employer Contribution
        public bool IsEmployerContFixed { get; set; } = false;
        public double EmployerContFixed { get; set; } = 0;
        public double EmployerContPercentage { get; set; } = 0.02;
        public string EmployerContBasis { get; set; } = "BASICPAY";

        // Leave paid counts
        public int SickLeave { get; set; } = 5;
        public int VacationLeave { get; set; } = 7;
        public int MaternityLeave { get; set; } = 60;
        public int PaternityLeave { get; set; } = 7;
        public int StudyLeave { get; set; } = 45;
        public int RehabilitationLeave { get; set; } = 10;
    }
}
