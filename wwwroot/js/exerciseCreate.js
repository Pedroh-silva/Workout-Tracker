$(document).ready(function () {
    var fade = $('#fade');
    var modal = $('#modal');
    var conteudoModal = $('#conteudoModalMusculo');
    var conteudoCriarMuscle = $("#addNewMuscle");
    var conteudoListaMuscle = $(".listMuscle");
    //MODAL CRIAR MÚSCULO
    $('#btnCriarMuscle').on('click', function () {
        $("#muscleValidation").text("")
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'modalScale-up 0.4s forwards');
        conteudoModal.children().eq(0).text("Adicione o nome do músculo");
        setTimeout(function () {
            conteudoModal.fadeIn();
            conteudoCriarMuscle.css('display','flex');
        }, 310);
    });
    $("#btnCreateNewMuscle").on('click', function () {
        var name = $('input#muscleName').val();
        var name = name.trim();
        var hasAnyNumber = hasNumber(name);
        if (name.length == 0 || name == null || hasAnyNumber == true) {
            return alert("Por favor, preencha o campo corretamente.");
        }
        $("#CreatedMuscleOrSelectedText").text(name);
        $("#createOrSelectedMuscleIdName").val("Create-"+name+"");
        fecharModal();
    });
   
    //MODAL SELECIONAR MÚSCULO
    $('#btnLista').on('click', function () {
        $("#muscleValidation").text("")
        fade.css('display', 'flex');
        modal.css('display', 'block');
        modal.css('animation', 'modalScale-up 0.4s forwards');
        conteudoModal.children().eq(0).text("Selecione o músculo");
        setTimeout(function () {
            conteudoModal.fadeIn();
            conteudoListaMuscle.fadeIn();
        }, 310);
    });
    $("#btnGetMuscleinList").on('click', function () {
        var muscle = $('select').val();
        var id;
        $("option").each(function () {
            var name = $(this).text();
            if (name == muscle) {
                id = $(this).attr('id');
            }
        });
        $("#CreatedMuscleOrSelectedText").text(muscle);
        $("#createOrSelectedMuscleIdName").val(id + "-" + muscle);
        fecharModal();
    });
    //FECHAR MODAL
    $('#closeModal').on('click', function () {
        
        fecharModal();
    });
    //VALIDAÇÃO
    function hasNumber(name) {
        var numbers = /[0-9]/g;
        var result = numbers.test(name);
        if (result) return true;
        return false;
    }
    $("#ExerciseForm").submit(function (e) {
        var muscleCreated = $("#CreatedMuscleOrSelected");
        if (muscleCreated.is(":empty")) {
            $("#muscleValidation").text("Por favor, adicione ou crie um músculo")
            e.preventDefault();
        }
        
    });
    //
    function fecharModal() {
        conteudoModal.fadeOut("fast");

        setTimeout(function () {
            modal.css('animation', 'modalScale-down 0.3s forwards');
            conteudoListaMuscle.css('display', 'none');
            conteudoCriarMuscle.css('display', 'none');
        }, 170);
        setTimeout(function () {
            modal.css('display', 'none');
            fade.css('display', 'none');
        }, 350);
    };
});