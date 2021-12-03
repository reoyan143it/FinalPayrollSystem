// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    $('.dropright a.submenu').on("mouseover", function (e) {
        $('.dropright a.submenu').next('ul').slideUp(100);
        $(this).next('ul').slideDown(100);
        e.stopPropagation();
    });
    $('.dropright').mouseleave(function (e) {
        $(this).next('a.submenu').next('ul').slideUp(100);
        e.stopPropagation();
    });
    $('.dropdown a').click(function (e) {

        if($(this).hasClass("text-dark") == true) {
            $(this).removeClass("text-dark");
            $(this).addClass("text-white");
        }else if($(this).hasClass("text-white") == true) {
            $(this).removeClass("text-white");
            $(this).addClass("text-dark");
        }
    });

    $('.dropdown a').focusout(function (e) {
        $(this).addClass("text-dark");
    });

});
