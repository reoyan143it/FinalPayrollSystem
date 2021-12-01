
$(document).ready(function () {
    $("#td-items > tr td").dblclick(function () {
        var par = $(this).parent().attr("id");
        var td = $(this);
        var subs = "";
        var tdattr = $(this).attr("id");
        
        if (tdattr == "salary") {
            var tdval = $(this).text().replace("₱", "").replace(".00", "").replace(",", "");
            $(this).html("");
            $(this).append("<input id='toModify' type='text' class='form-control' value='" + tdval + "' />");
            $("#toModify").focus();
            $("#toModify").focusout(function () {
                subs = $(this).val();
                if (subs == tdval) {
                    td.html(tdval);
                } else {

                    while (subs.charAt(0) == 0) {
                        subs = subs.substring(1, $(this).val().length);
                    }

                    var regEx = /\d{3,6}/;
                    if (regEx.test(subs)) {
                        $.ajax({
                            type: "POST",
                            url: "/Records/EditRate?rateID=" + par + "&colnm=" + tdattr + "&inpval=" + subs,
                            dataType: "text",
                            success: function (response) {
                                alert(response);
                                window.location.href = "/Records/AddRates";
                            },
                            error: function (response) {
                                alert(response);
                                window.location.href = "/Records/AddRates";
                            }
                        });
                    } else {
                        alert("It should be number! Or It should not be precede with 0");
                        window.location.href = "/Records/AddRates";
                    }
                }
                $(this).remove();
            });
        } else if (tdattr == "paytype") {
            var deftdval = $(this).text();
            $(this).html("");
            switch (deftdval) {
                case "Monthly":
                    $(this).append("<select id='toModify' class='form-control'>" +
                        "<option value='Monthly' selected > Monthly</option > " +
                        "<option value='Semi-Monthly'>Semi-Monthly</option>" +
                        "<option value='Daily Rate'>Daily Rate</option></select>");
                    break;
                case "Semi-Monthly":
                    $(this).append("<select id='toModify' class='form-control'>" +
                        "<option value='Monthly'>Monthly</option>" +
                        "<option value='Semi-Monthly' selected>Semi-Monthly</option>" +
                        "<option value='Daily Rate'>Daily Rate</option></select>");
                    break;
                case "Daily Rate":
                    $(this).append("<select id='toModify' class='form-control'>" +
                        "<option value='Monthly'>Monthly</option>" +
                        "<option value='Semi-Monthly'>Semi-Monthly</option>" +
                        "<option value='Daily Rate' selected>Daily Rate</option></select>");
                    break;
            }
            
            $("#toModify").focus();
            $("#toModify").focusout(function () {
                subs = $(this).val();
                $.ajax({
                    type: "POST",
                    url: "/Records/EditRate?rateID=" + par + "&colnm=" + tdattr + "&inpval=" + subs,
                    dataType: "text",
                    success: function (response) {
                        alert(response);
                        window.location.href = "/Records/AddRates";
                    },
                    error: function (response) {
                        alert(response);
                        window.location.href = "/Records/AddRates";
                    }
                });
                $(this).remove();
            });
        } else {alert("No selected!")}
    });

        
});