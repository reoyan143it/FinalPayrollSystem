$(document).ready(function () {
    $("#tblLeave").DataTable({
        processing: true,
        serverSide: true,
        autoWidth: true,
        orderMulti: true,
        ordering: true,
        search: {
            "smart": true,
            "caseInsensitive": true
        },
        ajax: {
            method: "POST",
            url: "/Records/LeaveList",
            dataType: "json",
        },
        columns: [
            { "data": "leavehistid", "name": "ID" },
            { "data": "employeeid", "name": "Employee ID" },
            { "data": "employeename", "name": "Employee Name" },
            { "data": "appliedleave", "name": "Applied Leave" },
            { "data": "rempaidleave", "name": "Remain Paid Leave" },
            { "data": "isdeductible", "name": "Is Deductible" },
            { "data": "leavedatefrom", "name": "Leave Date from" },
            { "data": "leavedateto", "name": "Leave Date to" },
            {
                "render": function (data, type, row, meta) {
                    
                    return "<a type='button' class='btn btn-primary' href='/Records/EditLeave?leavehistID=" + row.leavehistid + "'>EDIT</a> | " +
                        "<a type='button' class='btn btn-danger' href='/Records/DeleteLeave?leavehistID='" + row.leavehistid + ">DELETE</a>";

                }, "name":"Action"
            }
        ],
        columnDefs: [
            { "visible": false, "targets": [0]},
            { "width": "15%", "searchable": false, "orderable": false, "targets": [8] },
        ]
    });

});
