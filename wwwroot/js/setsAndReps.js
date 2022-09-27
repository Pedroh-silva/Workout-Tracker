$(document).ready(function () {
    var modal = $('.modal');
    var fade = $('#fade');
    var conteudo = $('.conteudoModal');
    $(".exercise_name").on('click', function () {
        var name = $(this).children().eq(1).text();
        var id = $(this).children().eq(1).attr('itemid');
        $("#idExercicio").val(id);
        fade.css('display', 'flex');
        modal.css('display', 'block')
        modal.css('animation', 'modalScale-up 0.4s forwards')
        setTimeout(function () {
            conteudo.fadeIn();
        }, 310);
        conteudo.children().eq(0).text(name);
    });
    //Fechar modal -----------------------------------------------------------------
    $("#closeModal").on('click', function () {
        fecharModal();
    });
    fade.on('click', function (e) {
        var divModal = document.querySelector('div#modal');
        var fora = !divModal.contains(e.target)
        if (fora) fecharModal();
    });
    //Adicionando séries e repetições -----------------------------------
    var serieAdicionada = $('#seriesAdicionadas');
    $('.add-btn').on('click', function () {
        var serie = $("#series");
        var reps = $("#repetiçoes");
        var idExercicio = $("#idExercicio").val();

        if (!IsNull(serie, reps)) {
            serieAdicionada.append("<li class='listaInput'> Série: " + serie.val() + " " + "Repetições: " + reps.val() +
                "<input type='checkbox' name='SetsReps' id='SetsReps' class='SetsReps' checked='checked' value='" + idExercicio + "-" + serie.val() + "-" + reps.val() +
                "'> <button style='vertical-align:baseline;' id='excluir' class='remove fa fa-close'></button></li>"
            );
        }
    });
    $(document).on('click', 'button#excluir', function () {
        $(this).closest('li.listaInput').remove();
    })
    //Validando  input --------------------------------------------
    $('.form__field').keypress(function (e) {
        var invalidChars = ["-", "e", "+", "E", "."];
        if (invalidChars.includes(e.key)) {
            e.preventDefault();
        }
    })
    $('.form__field').on('paste', function (e) {
        e.preventDefault();
    })

    //Validação do formulário --------------------------------------
    $('#SetsAndRepsForm').submit(function (e) {
        validation(e);
    });



    function validation(param) {
        $(".listaSets").each(function () {
            if ($(this).is(":empty")) {
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
    function fecharModal() {
        if (!$("#seriesAdicionadas").is(':empty')) {
            var htmlSeries = $("#seriesAdicionadas").html();
            var idAtual = $('#idExercicio').val();

            addSets(idAtual, htmlSeries);
        }

        conteudo.fadeOut(150, function () {

            modal.css('animation', 'modalScale-down 0.4s forwards')
        });
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');

        }, 310);
    }
    function IsNull(a, b) {


        if (a.val() == "" || a.val() == "0") {
            a.addClass('invalid');
            a.siblings().eq(0).addClass('invalid');
            if (b.val() == "") {
                b.addClass('invalid');
                b.siblings().eq(0).addClass('invalid');
            }
            setTimeout(function () {
                a.removeClass('invalid');
                a.siblings().eq(0).removeClass('invalid');
                b.removeClass('invalid');
                b.siblings().eq(0).removeClass('invalid');
            }, 1050)
            return true;
        }
        else if (b.val() == "" || b.val() == "0") {
            b.addClass('invalid');
            b.siblings().eq(0).addClass('invalid');
            setTimeout(function () {
                b.removeClass('invalid');
                b.siblings().eq(0).removeClass('invalid');
            }, 1050)
            return true;
        }
        else {
            return false;
        }
    };
    function addSets(idAtual, htmlSeries) {
        $(".listaSets").each(function () {
            var id = $(this).attr('id');
            if (idAtual == id) {
                var isValidation = $(this).children().eq(0);
                if (isValidation.hasClass("text-danger")) {
                    $(this).html("");
                }
                $(this).append(htmlSeries);
                $('#seriesAdicionadas').html("");
            }
        })
    }
});