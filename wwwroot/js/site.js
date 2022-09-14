// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var theme = localStorage.getItem('theme');
if (theme == 'dark') {
    $('html').addClass('darkmode');
    $('#colorTheme').prop('checked', false);
    $('.lightDarkMode').fadeOut(function () {
        $(this).html("<i class='fas fa-moon'></i>")
    }).fadeIn();
    localStorage.setItem('theme', 'dark');
}
$('#colorTheme').on('click', function () {
    theme = localStorage.getItem('theme');
    if (theme == null) {
        $('html').addClass('darkmode');
        $('#colorTheme').prop('checked', false);
        $('.lightDarkMode').fadeOut("fast", function () {
            $(this).html("<i class='fas fa-moon'></i>")
        }).fadeIn("fast");
        localStorage.setItem('theme', 'dark');
    }
    else {
        $('html').removeClass('darkmode');
        $('.lightDarkMode').fadeOut("fast", function () {
            $(this).html("<i class='fas fa-sun'></i>")
        }).fadeIn("fast");
        localStorage.removeItem('theme');
    }
});