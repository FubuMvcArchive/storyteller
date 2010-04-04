$(document).ready(function() {
    $('#testName').change(function() {
        eval('var test = ' + $('#testName').val() + ';');
        $('#testEditor').testEditor(test);
    });

    $('#testName').change();

    
});