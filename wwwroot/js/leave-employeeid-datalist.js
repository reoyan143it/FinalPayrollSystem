$(document).ready(function () {
    $("#lv-empID").on("input",function () {
        $("#lv-empName").val("");
        $.ajax({
            type: "GET",
            url: "/Records/lvEmpIDName?employeeID=" + $("#lv-empID").val() + "",
            dataType: "json",
            success: function (response) {
                console.log(response);
                $(response).each(function (index, emp) {
                    $("#lv-empName").val(emp.firstname + " " + emp.lastname);
                });
            }
        });
    })

});