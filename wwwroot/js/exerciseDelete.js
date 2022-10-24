$(document).ready(function () {
    var hasWorkout = $("#hasWorkout").val()
    if (hasWorkout != 0) {
        $(".btn").prop("disabled", true);
        $(".text-danger").text("Este exercício não pode ser excluido, ele já está cadastrado em algum treino.");
    }
});