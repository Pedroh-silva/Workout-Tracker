$(document).ready(function () {
    var fade = $('#fade');
    var modal = $('.modal');
    var conteudoModal = $('#conteudo');
    var conteudoModalExercicio = $('#conteudoExercicio');
    var botaoEditar = $('.editar');
    var botaoRemover = $('.remover');
    $('.fa-pen').on('click', function () {
        var id = $(this).attr('itemid');
        botaoEditar.attr('href',"/Workouts/Edit/" + id);
        botaoRemover.attr('href',"/Workouts/Delete/" + id);
        animacaoAbrir(conteudoModal);
    });

    $('#closeModal').on('click', function () {
        fecharModal();
    });
    var divModal = document.querySelector('div.modal')
    fade.on('click', function (e) {
        var fora = !divModal.contains(e.target)
        if (fora) fecharModal();
    })

    $('.exercicios').on('click', function () {
        var id = $(this).attr('itemid');
        var lista = $('#exerciseList_' + id).html();
        $('#exercisesSelectedWorkout').html(lista);
        animacaoAbrir(conteudoModalExercicio);
    });

    function animacaoAbrir(conteudoModal) {
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'scale-up 0.6s forwards')
        setTimeout(function () {
            conteudoModal.fadeIn();
        }, 310);
    }
    function fecharModal() {
        modal.css('animation', 'scale-down 0.4s forwards');
        conteudoModal.fadeOut("fast");
        conteudoModalExercicio.fadeOut("fast");
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');
        }, 310);
    }
});
