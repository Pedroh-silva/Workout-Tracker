$(document).ready(function () {
    $(".selectedExercise").each(function () {
        var id = $(this).val();
        $(".boxExercicio").each(function () {
            var inputCheck = $(this).children().eq(2);
            if (inputCheck.attr('itemid') == id) {
                inputCheck.val('checked');
                $(this).addClass('boxChecked')
            }
        })
    })
});