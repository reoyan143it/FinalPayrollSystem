
$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/Records/DispRates",
        dataType: "json",
        success: function (response) {
            $(response).each(function (index, elem) {
                console.log(index + elem);
                $("#display-rates").append("<tr><td>" + elem.rateid +
                    "</td><td>" + elem.employeeid +
                    "</td><td>" + elem.salary +
                    "</td><td>" + elem.paytype +
                    "</td><td>" + elem.dividedby +
                    "</td><td>" + elem.multipliedby +
                    "</td><td> <a class='btn btn-success' href='#'>EDIT</a> | <a class='btn btn-info' href='#'>DETAILS</a> | <a class='btn btn-danger' href='#'>DELETE</a> </td></tr > ");
            });
        },
        error: function (response) {
            $("#error").append("ERROR FOUND: " + response.responseText);
        }
    });
});