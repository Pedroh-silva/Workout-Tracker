$(document).ready(function () {
    setTimeout(function () {
        $("#donutchart_ svg").animate({
            opacity: 1
        }, 200);
    }, 300)

    $("#WorkoutsMonths").change(function () {
        var option = $(this).val()
        if (option == 'selecione') {
            $("#Treinos").animate({
                opacity: 0
            }, 300)
            setTimeout(function () {
                $("#Treinos").html('<h5 style="text-align:center">Selecione um mês...</h5>');
                $("#Treinos").animate({
                    opacity: 1
                }, 300)
            },305)
            
        }
        else {
            $("#Treinos").animate({
                opacity:0
            }, 300)
            setTimeout(function () {
                $.ajax({
                    url: '/Workouts/GetWorkoutByMonth/' + option,
                    type: 'GET',
                    dataType: 'html',
                    success: function (result) {
                        $("#Treinos").html(result);
                    }
                })
                $("#Treinos").animate({
                    opacity: 1
                }, 300)
            },305)
        }
    })

});