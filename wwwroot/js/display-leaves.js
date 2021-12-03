$(document).ready(function () {
    $("#tblLeave").DataTable({
        processing: true,
        serverSide: true,
        autoWidth: true,
        ajax:{
            method: "POST",
            url: "/Records/LeaveList",
            dataType: "json",
        },
        columns: [
            { "data": "leavehistid", "name": "ID"},
            { "data": "employeeid", "name": "Employee ID" },
            { "data": "employeename", "name": "Employee Name" },
            { "data": "appliedleave", "name": "Applied Leave" },
            { "data": "rempaidleave", "name": "Remain Paid Leave" },
            { "data": "isdeductible", "name": "Is Deductible" },
            { "data": "leavedatefrom", "name": "Leave Date from" },
            { "data": "leavedateto", "name": "Leave Date to" }
        ]
    });

});