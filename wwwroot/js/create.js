$(document).ready(function () {
    //pré preenchimento forms
    $("#Workout_Duration").val("00:00");
    var dateTime = new Date();
    var dataFormatada = dateTime.toISOString().substring(0, 11);
    var horaFormatada = dateTime.toLocaleTimeString().substring(0, 5);
    $("#Workout_DateTime").val(dataFormatada + horaFormatada);

    //------pop-up explicação -------------------------------------------------------------------------
    var moveLeft = 20;
    var moveDown = 10;
    var gatilhoPopUpCategoria = $('#gatilhoPopUp');
    var categoriaSelecionada = $('#categoriaSelecionada');
    var popUp = $('#pop-up-Categoria');
    gatilhoPopUpCategoria.hover(function (e) {
        if (categoriaSelecionada.text() != " Nenhuma categoria selecionada...") {
            return;
        }
        else {
            popUp.show(50);
        }
    }, function () {

        popUp.hide(50);
    }
    );
    gatilhoPopUpCategoria.mousemove(function (e) {
        popUp.css('top', e.pageY + moveDown).css('left', e.pageX + moveLeft);
    });

    //--------Pop up explicação para remover categoria -----------------------------------------------------------
    var gatilhoPopUpRemover = $('#categoriaSelecionada');

    var popUpRemover = $('#pop-up-remover');
    gatilhoPopUpRemover.hover(function (e) {
        var texto = $('#categoriaSelecionada').text();
        if (texto == ' Nenhuma categoria selecionada...') {
            return;
        }
        else {
            popUpRemover.show(50);
        }
    }, function () {

        popUpRemover.hide(50);
    }
    );
    gatilhoPopUpRemover.mousemove(function (e) {
        popUpRemover.css('top', e.pageY + moveDown).css('left', e.pageX + moveLeft);
    });

    //------------------Modal Categoria  + Modal Exercicios --------------------------------------------------------------------------------------------
    var fade = $('#fade');
    var modal = $('#modal');
    var conteudoCategoria = $('#conteudoCategoria');
    var conteudoExercicio = $('#conteudoExercicio');
    $('#btnModalCategoria').on('click', function () {
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'modalScale-up 0.4s forwards')
        setTimeout(function () {
            conteudoCategoria.fadeIn();
        }, 310);
        categoriaSelecionada.hide(200);
    });
    var clickBtnExercicio = 0; //serve para identificar e posteriormente fechar o conteúdo de exercícios do Modal
    $('#btnModalExercicio').on('click', function () {
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'ExercicioModalScale-up 0.4s forwards')
        setTimeout(function () {
            conteudoExercicio.fadeIn();
        }, 310);
        clickBtnExercicio = 1;
        $('#ListaExercicios').hide(200);
        $('#ListaExercicios').html(''); //Lista zerada para caso uma nova adição/remoção de exercícios
    });
    //Fechar modal com x ou clicando fora --------------------------------------------------------------------------
    $('#closeModal').on('click', function () {
        fechamentoModal();
    });
    fade.on('click', function (e) {
        var divModal = document.querySelector("div#modal");
        var fora = !divModal.contains(e.target);
        if (fora) fechamentoModal();
    });
    //---------Preenchimento de Cores das Categorias -----------------------------------------
    $('.codigoCor').each(function () {
        var codigoCor = $(this).val();
        var id = $(this).attr('itemid')
        $('#corCategoria_' + id).css("background", codigoCor);
    });
    //---------- Selecionar a Categoria ---------------------------------------

    $('.categoria').on('click', function () {
        var name = $(this).children().eq(0).text();
        var id = $(this).children().eq(1).attr('itemid');
        var color = $(this).children().eq(1).val();
        conteudoCategoria.fadeOut("fast");
        setTimeout(function () {
            modal.css('animation', 'modalScale-down 0.3s forwards');
        }, 170);
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');
        }, 350);
        categoriaSelecionada.html("<i style='color:" + color + ";font-weight:bold' class='fa fa-angle-right'></i>" + " " + name + " " + "<input type='checkbox' name='selectedCategory' id='selectedCategory' class='selectedCategory' checked='checked' value='" + id + "'>");
        categoriaSelecionada.show(200);
    });
    //remover categoria com clique ---------------------------------------------------------------------------
    categoriaSelecionada.on('click', function () {
        removerCategoria();
    });
    //Selecionar Exercício ----------------------------------------------------
    $('.boxExercicio').on('click', function () {
        var checked = $(this).children().eq(2);
        if (checked.val() == "") {
            checked.val('checked')
            $(this).addClass('boxChecked')
        }
        else {
            checked.val('')
            $(this).removeClass('boxChecked');
        }
    });
    //------ Verificação do forms  -----------------------------------------
    $("#WorkoutForm").submit(function (e) {
        $("#validacaoTimer").html("");
        $("#ValidacaoExercicios").html("");
        var duration = $("#Workout_Duration").val();
        var exercises = $("#ListaExercicios");
        if (exercises.is(":empty")) {
            $("#ValidacaoExercicios").text("Por favor, adicione exercícios")
            e.preventDefault();
        }
        if (duration == "00:00") {
            $("#validacaoTimer").text("A duração do treino não pode ser 00:00");
            e.preventDefault();

        }
    });
    //Filtro por músculo-------------------------------------------
    $("#muscleFilter").change(function () {
        var choice = $(this).val();
        if (choice == "Todos") {
            $('.boxExercicio').css('display', 'block')
        }
        else if (choice == "Selecionados") {
            $('.boxExercicio').each(function () {
                if (!$(this).hasClass("boxChecked")) {
                    $(this).css('display', 'none')
                }
                else {
                    $(this).css('display', 'block')
                }
            })
        }
        else{
            $('.boxExercicio').each(function () {
                var muscleName = $(this).children().eq(3).text()
                if (muscleName != choice) {
                    $(this).css('display', 'none')
                }
                else {
                    $(this).css('display', 'block')
                }
            })
        }
    })
    //-------------------------------------------------------------
    function fechamentoModal() {
        if (clickBtnExercicio != 0) {
            $('.boxExercicio').each(function () {
                var checked = $(this).children().eq(2);
                if (checked.val() == 'checked') {
                    var name = $(this).children().eq(0).text();
                    var id = $(this).children().eq(2).attr('itemid');
                    $('#ListaExercicios').append("<li>" + name + "<input type='checkbox' name='selectedExercise' id='selectedExercise' class='selectedExercise' checked='checked' value='" + id + "'></li>")
                }
            });
            $('#ListaExercicios').show(200);
            conteudoExercicio.fadeOut("fast");
            clickBtnExercicio = 0;
            //melhorar efeito ao fechar
            modal.animate({
                opacity: 0.1
            }, 350);
            fade.animate({
                opacity: 0.1
            }, 350);
            //------------------------
        }
        else {
            conteudoCategoria.fadeOut("fast");
            setTimeout(function () {
                modal.css('animation', 'modalScale-down 0.3s forwards');
            }, 170);
            categoriaSelecionada.show(200);
        }
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');
        }, 350);
        //Voltar opacidade para caso um novo clique
        modal.animate({
            opacity: 1
        }, 0);
        fade.animate({
            opacity: 1
        }, 0);
    }
    function removerCategoria() {
        if (categoriaSelecionada.text() == " Nenhuma categoria selecionada...") {
            return;
        }
        else {
            categoriaSelecionada.hide(200, function () {
                categoriaSelecionada.html("<i style='font-weight:bold' class='fa fa-angle-right'></i>" + " Nenhuma categoria selecionada...");
            })
            categoriaSelecionada.show(200);
        }
    }
});