$(document).ready(function () {
    var dataFormatada = $("#dataFormatada").val();
    $("#DataOfWorkout").append("<input class='text-box single-line' data-val='true' data-val-required='Por favor, coloque a data e hora do treino' hidden id='DateTime' name='DateTime' type='datetime-local' value='" + dataFormatada + "' />");
    function validation(param) {
        $(".listaSets").each(function () {
            var listaHtml = $(this).html();
            if (!listaHtml.includes("li")) {
                $(this).html("<li class='text-danger'>Por favor, adicione as séries e repetições deste exercício</li>");
                param.preventDefault();
            }
            else {
                var isValidation = $(this).children().eq(0);
                if (isValidation.hasClass("text-danger")) {
                    param.preventDefault();
                }
            }
        })
    }
    $('#EditSetsAndRepsForm').submit(function (e) {
        validation(e);
    });
});
