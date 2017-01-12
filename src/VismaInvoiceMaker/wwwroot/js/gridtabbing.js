var LAST_EDIT_CELL = null;
var CURRENT_CELL = null;

function setLastEditedCell(grid) {
    var current = grid.tbody.find(".k-edit-cell");
    if (current.length === 0)
        current = grid.current();
    if (current != null)
        CURRENT_CELL = {
            row: grid.tbody.find("tr[data-uid='" + current.closest("tr").data("uid") + "']").index(),
            column: current.index()
        };
    else
        CURRENT_CELL = null;
  //  console.log(CURRENT_CELL);
}

function JumpNextEditedCell(grid) {
   // console.log(CURRENT_CELL);
    if (($(document.activeElement).attr("id") === grid.tbody.closest("table").attr("id")) && (grid.tbody.find(".k-edit-cell").length === 0) && CURRENT_CELL != null) {

        var cell = grid.tbody.find("tr:eq(" + CURRENT_CELL.row + ") > td:eq(" + CURRENT_CELL.column + ")");
        // jump to next column
        var nextCell = cell.nextAll(".editable-cell:visible");
        if (!nextCell[0]) {
            //search the next row
            var nextRow = cell.parent().next();
            nextCell = nextRow.find(".editable-cell:visible");
        }
        cell = nextCell[0];
        if (cell !== undefined) {
            grid.editCell(cell);
            grid.current(cell);
            if (cell !== null)cell.focus();
        }
    }
}

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}