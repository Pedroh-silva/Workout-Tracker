$(document).ready(function () {
    var fade = $("#fade");
    var modal = $("#modal")
    var conteudo = $("#conteudo");
    var treinoSelecionadoModal = $("#modalTreinoSelecionado");
    var conteudoTreinoSelecionado = $("#conteudoTreinoSelecionado");
    var dateTime = new Date();
    var dataFormatada = dateTime.toISOString().substring(0, 11);
    var horaFormatada = dateTime.toLocaleTimeString().substring(0, 5);

    //Cores da categoria 
    $(".corCategoria").each(function () {
        var color = $(this).siblings('input').val();
        $(this).css('background', color);
    })
    //Selecionar Categoria
    $(".boxCategoria").on('click', function () {
        animacaoAbrir(conteudo)
        var htmlTreino = $(this).children().eq(3).html()
        $(".treinos").html(htmlTreino);
    });
    //Fechar Modal
    $('#closeModal').on('click', function () {
        fecharModal();
    });
    fade.on('click', function (e) {
        var divModal = document.querySelector("div#modal");
        var divModalTreino = document.querySelector("div#modalTreinoSelecionado");
        var fora = !divModal.contains(e.target) && !divModalTreino.contains(e.target);
        if (fora) fecharModal();
    });
    //Selecionar Treino
    $(document).on('click', '.boxTreino', function () {
        var dontHaveClass = !$(this).hasClass('boxChecked');
        if (dontHaveClass) {
            $('.boxTreino').each(function () {
                $(this).removeClass('boxChecked')
            });
            $(this).addClass('boxChecked')
            $(this).addClass('boxChecked');
            $("input#formId").val($(this).attr('itemId'))
            $("input#name").val($(this).children().eq(0).text()).css('borderBottom', '1px solid green').css('color', 'green');
            $("input#dateTime").val(dataFormatada + horaFormatada);
            $("input#duration").val($(this).children().eq(1).val());
            $('#exerciciosTreinoSelecionado').html($(this).children().eq(2).html());
            $("input#formCategoryId").val($(this).children().eq(3).val());
            animacaoAbrirTreinoSelecionado();
        }
        else {
            $(this).removeClass('boxChecked');
            fecharModalTreinoSelecionado();
        }
    });
    //validação
    $("#WorkoutAddedByCategoryForm").submit(function (e) {
        var duration = $("#duration").val();
        $("#ValidationformDuration").text("");
        if(duration == "00:00" || duration == "") {
            $("#ValidationformDuration").text("A duração do treino não pode ser 00:00");
            e.preventDefault();
        }
    });
    function animacaoAbrir() {
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'scale-up 0.4s forwards');
        setTimeout(function () {
            conteudo.fadeIn();
        }, 310);
    }
    function animacaoAbrirTreinoSelecionado() {
        treinoSelecionadoModal.css('display', 'block');
        treinoSelecionadoModal.css('animation', 'scale-right 0.4s forwards');
        setTimeout(function () {
            conteudoTreinoSelecionado.fadeIn();
        }, 310);
    }
    function fecharModalTreinoSelecionado() {
        treinoSelecionadoModal.css('animation', 'scale-left 0.4s forwards');
        conteudoTreinoSelecionado.fadeOut("fast");
        setTimeout(function () {
            treinoSelecionadoModal.css('display', 'none');
        }, 310);
    }
    function fecharModalConteudo() {
        modal.css('animation', 'scale-down 0.4s forwards');
        conteudo.fadeOut("fast");
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');
        }, 310);
    }
    function fecharModal() {
        $("#ValidationformDuration").text("");
        $("#dateTime-error").text("");
        var isTreinoSelecionado = treinoSelecionadoModal.css('display') == 'block';
        if (isTreinoSelecionado) {
            fecharModalTreinoSelecionado();
            setTimeout(function () {
                fecharModalConteudo();
            }, 400);

        }
        else {
            fecharModalConteudo();
        }
    }
});