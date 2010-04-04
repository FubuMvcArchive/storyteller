ST.paragraph = function(div, metadata, step) {
    div.step = step;

    $('.deleteStep', div).removable();

    ST.activateGrammars(div, step);

    div.update = function() {
        $(div).children('.step').each(function(i, child) {
            child.update();
        });

        return div.step;
    };
}

ST.registerGrammar('.paragraph', ST.paragraph);