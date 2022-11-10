$(document).ready(function () {
    $("input[type='color']").change(function() {
        var cor = $(this).val();
        $("#categoryColor").val(cor);
    })
});