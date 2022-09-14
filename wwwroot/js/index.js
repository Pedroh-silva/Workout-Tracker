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
        //animação
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'scale-up 0.6s forwards')
        setTimeout(function () {
            conteudoModal.fadeIn();
        }, 310);
        //-----------------------------------------
        
    });
    $('#closeModal').on('click', function () {
        modal.css('animation', 'scale-down 0.4s forwards');
        conteudoModal.fadeOut("fast");
        conteudoModalExercicio.fadeOut("fast");
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');
        }, 310);
        
        
    });
    $('.exercicios').on('click', function () {
        var id = $(this).attr('itemid');
        var lista = $('#exerciseList_' + id).html();
        $('#exercisesSelectedWorkout').html(lista);
        //animação
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'scale-up 0.6s forwards')
        setTimeout(function () {
            conteudoModalExercicio.fadeIn();
        }, 310);
        //-----------------------------------------
    });
});
