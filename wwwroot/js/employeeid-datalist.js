

$(document).ready(function () {
    $("#empiddtlist").contents().empty();
    var data = new Array();
    $.ajax({
        type: "GET",
        url: "/Records/dtEmployeeID",
        dataType: "json",
        success: function (response) {
            console.log(response);
            $(response).each(function (index, emp) {
                $("#empiddtlist").append("<option value='"+emp.employeeid+"'></option>");
            });
        }
    });
    $("#employeeID").on("input", function (e) {
        $("#empname").val("");
        $.ajax({
            type: "GET",
            url: "/Records/valEmpName?employeeID=" + $("#employeeID").val() + "",
            dataType: "json",
            success: function (response) {
                console.log(response);
                $(response).each(function (index, emp) {
                    $("#empname").val(emp.firstname+" "+emp.lastname);
                });
            }
        });
    })
});