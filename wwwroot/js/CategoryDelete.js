$(document).ready(function () {
    var hasWorkout = $("#hasWorkout").val()
    if (hasWorkout != 0) {
        $(".one").text("Esta categoria já está cadastrada em algum treino, seus treino(s) relacionados irão perder a categoria atual.");
        $(".two").text("Gostaria de continuar?");
    }
});