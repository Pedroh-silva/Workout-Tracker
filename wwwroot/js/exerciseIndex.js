$(document).ready(function () {
    var filter = $("#filtro")
    filter.on('change', function (e) {
        var option = $(this).val();
        if (option == "Filtar por músculo") {
            $(".muscleName").parent().show();
        }
        else {
            $(".muscleName").each(function () {
                var name = $(this).text();
                if (option == name) {
                    $(this).parent().show();
                }
                else {
                    $(this).parent().hide();
                }
            });
        }
    });
});