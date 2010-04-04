function getLeaf(test) {
    return test.childFor('SampleTable').children[0].childFor('rows');
}

function getTableColumns(test) {
    var table = $('#templateTable').tableColumns().get(0);
    table.chooseColumns(getLeaf(test));

    return table;
}

function getTable(test) {
    var table = $('#templateTable').tableColumns().get(0);

    return table;
}

qUnitTesting(function() {
    module("Table Columns");

    test("when determining the active columns from a blank leaf", function() {
        var table = getTableColumns(blank);
        ok(table.columns['a'], "Mandatory column 'a' is shown");
        ok(!table.columns['b'], "Optional column 'b' is hidden");
        ok(table.columns['c'], "Mandatory column 'c' is shown");
        ok(!table.columns['d'], "Optional column 'b' is hidden");
        ok(!table.columns['e'], "Optional column 'b' is hidden");
        ok(!table.columns['f'], "Optional column 'b' is hidden");

    });

    test("the table headers contain a cell for each active column", function() {
        var table = getTableColumns(blank);

        var header = table.headerRow();

        ok(header.getCell('a'), "Mandatory column 'a' is shown");
        ok(!header.getCell('b'), "Optional column 'b' is hidden");
        ok(header.getCell('c'), "Mandatory column 'c' is shown");
        ok(!header.getCell('d'), "Optional column 'b' is hidden");
        ok(!header.getCell('e'), "Optional column 'b' is hidden");
        ok(!header.getCell('f'), "Optional column 'b' is hidden");
    });

    test("the table body rows contain a cell for each active column", function() {
        var table = getTableColumns(blank);

        var body = table.bodyRow(new Step('row'));

        ok(body.getCell('a'), "Mandatory column 'a' is shown");
        ok(!body.getCell('b'), "Optional column 'b' is hidden");
        ok(body.getCell('c'), "Mandatory column 'c' is shown");
        ok(!body.getCell('d'), "Optional column 'b' is hidden");
        ok(!body.getCell('e'), "Optional column 'b' is hidden");
        ok(!body.getCell('f'), "Optional column 'b' is hidden");
    });

    test("when determining the active columns from a leaf with rows", function() {
        var table = getTableColumns(test1);

        ok(table.columns['a'], "Mandatory column 'a' is shown");
        ok(table.columns['b'], "Optional column 'b' is shown because there is a value");
        ok(table.columns['c'], "Mandatory column 'c' is shown");
        ok(!table.columns['d'], "Optional column 'd' is hidden because it has no values");
        ok(table.columns['e'], "Optional column 'e' is shown because it has values");
        ok(!table.columns['f'], "Optional column 'b' is hidden");
    });

    test("add a column and recalculate", function() {
        var table = getTableColumns(test1);
        table.addColumn('d');

        ok(table.columns['d'], "Optional column 'd' is now shown");
    });

    test("remove a column and recalculate columns", function() {
        var table = getTableColumns(test1);
        table.removeColumn('b');

        ok(!table.columns['b'], "Optional column 'b' is now hidden");
    });

    test("open up a test with a blank table", function() {
        var editor = $("#testEditor2").testEditor(blank);

        equals($('.addb', editor).is(':visible'), true, "link for optional column 'b' is shown");
        equals($('.addd', editor).is(':visible'), true, "link for optional column 'd' is shown");
        equals($('.adde', editor).is(':visible'), true, "link for optional column 'e' is shown");
        equals($('.addf', editor).is(':visible'), true, "link for optional column 'f' is shown");

        equals($('.grid th.a', editor).is(':visible'), true, "mandatory a column is shown");
        equals($('.grid th.c', editor).is(':visible'), true, "mandatory c column is shown");
        equals($('.grid th.b', editor).is(':visible'), false, "optional b column is not shown");
        
        
    });


});