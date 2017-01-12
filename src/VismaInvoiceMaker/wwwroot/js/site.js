var URL_SAVE_INVOICE = "/Invoices/SaveInvoice";
var URL_PRINT_INVOICE = "/Invoices/Print";
var URL_EDIT_INVOICE = "/Invoices/EditInvoice";
var URL_NEW_INVOICE = "/Invoices/EditInvoice";

$(document)
    .ready(function() {
        $("#menu").kendoMenu();
    });

function parameterWrapFunction(options, operation) {
    if (operation !== "read" && options) {
        return { InvoiceId: options.InvoiceId };
    }
}

function editInvoice(e) {
    var grid = $("#gridInvoices").data("kendoGrid");
    var invoiceID = grid.dataItem($(e.currentTarget).closest("tr")).InvoiceId;
    window.location = URL_EDIT_INVOICE + (invoiceID != undefined ? ("/" + invoiceID) : '');
}

function printFromGrid(e) {
    var grid = $("#gridInvoices").data("kendoGrid");
    var invoiceID = grid.dataItem($(e.currentTarget).closest("tr")).InvoiceId;
    window.open(URL_PRINT_INVOICE + "/" + invoiceID, '_blank');
}

function releaseInvoice(e) {
    alert("To Be Implemented!");
}

function newInvoice() {
    window.location = URL_NEW_INVOICE;
}

function onDataBoundGridTemplate(e) {
    e.sender.tbody.find(".k-button.k-grid-editItem").each(function (idx, element) {
        $(element).find("span").addClass("k-icon k-edit");
    });

    e.sender.tbody.find(".k-button.k-grid-release").each(function (idx, element) {
        $(element).find("span").addClass("k-icon k-i-lock");
    });

    e.sender.tbody.find(".k-button.k-grid-deleteItem").each(function (idx, element) {
        $(element).find("span").addClass("k-icon k-delete k-i-delete");
    });

    e.sender.tbody.find(".k-button.k-grid-viewItem").each(function (idx, element) {
        $(element).find("span").addClass("k-icon k-si-arrow-e");
    });

    e.sender.tbody.find(".k-button.k-grid-print").each(function (idx, element) {
        $(element).find("span").addClass("fa fa-print small-margin-right");
    });
}

function onDataBoundEditInvoice(e) {
    $("tr .k-grid-delete", "#grid").on("click", function (e) {
        setTimeout(
            function () {
                UpdateGridTotals();
            }, 100);
    });
}

function UpdateGridTotals() {
    //update total on invoice
    var VATAmount = 0, NetTotal = 0, AdvancePaymentTaxAmount = 0, TotalWithVAT = 0, TotalToPay = 0;

    $.each($("#grid").data("kendoGrid").dataSource.data(), function (i, row) {
        NetTotal += row.Total;
        TotalWithVAT += row.TotalPlusVat;
    });
    AdvancePaymentTaxAmount = TotalWithVAT * ($("#AdvancePaymentTax").data("kendoNumericTextBox").value() / 100);
    VATAmount = TotalWithVAT - NetTotal;
    TotalToPay = TotalWithVAT - AdvancePaymentTaxAmount;

    $("#VATAmount").html(kendo.toString(VATAmount, "n2") + " NOK");
    $("#TotalWithVAT").html(kendo.toString(TotalWithVAT, "n2") + " NOK");
    $("#TotalToPay").html(kendo.toString(TotalToPay, "n2") + " NOK");
}

function saveInvoiceTemplate(loadNew) {

    var dataInvoice = {
        "InvoiceNumber": $("#InvoiceNumber").html(),
        "InvoiceId": $("#invoiceId").val(),
        "Customer": $("#Customer").data("kendoDropDownList").dataItem(),
        "Store": $("#Store").data("kendoDropDownList").dataItem(),
        "TimeStamp": $("#TimeStamp").data("kendoDatePicker").value(),
        "DueDate": $("#DueDate").data("kendoDatePicker").value(),
        "AdvancePaymentTax": $("#AdvancePaymentTax").data("kendoNumericTextBox").value(),
        "Notes": "",
        "InvoiceDetails": []
    };

    if (dataInvoice.Customer.CustomerId === "")
        dataInvoice.Customer = null;

    if (dataInvoice.Store.StoreId === "")
        dataInvoice.Store = null;

    $.each($("#grid").data("kendoGrid").dataSource.data(), function (i, row) {
        if (row.InvoiceDetailsId === "")
            row.InvoiceDetailsId = guid();

        dataInvoice.InvoiceDetails.push({
            InvoiceDetailsId: row.InvoiceDetailsId,
            Article: row.Article,
            Price: row.Price,
            Qty: row.Qty,
            VAT: row.VAT,
            VatAmount: row.VatAmount,
            TotalPlusVat: row.TotalPlusVat
        });
    });
    $.ajax({
        url: URL_SAVE_INVOICE,
        type: "POST",
        contentType: 'application/json;',
        dataType: 'json',
        data: JSON.stringify(dataInvoice),
        //  data: dataInvoice,
        success: function (result) {
            if (result.success === false) {
                alert(result.message);
            }
            else if (loadNew)
                window.location = URL_NEW_INVOICE;
            else {
                alert("Invoice saved.");
                var printBtn = $("#PrintBtn").data("kendoButton");
                printBtn.enable(true);
                $("#invoiceId").val(result.id);
            }
        },
        error: function (result) {
            alert(result.error);
        }
    });
}

function onChangeArticle(e) {
    var grid = $("#grid").data("kendoGrid");
    var current = grid.tbody.find(".k-edit-cell");
    if (current.length === 0)
        current = grid.current();
    var model = grid.dataItem(current.closest("tr"));
    if (typeof model.Article != 'object') {
        model.Article = null;
        return;
    }
    setLastEditedCell(grid);

    model.Price = model.Article.Price;
    model.VAT = model.Article.VAT;
    UpdateRowAmounts(model);
    grid.refresh();
    if (CURRENT_CELL != null) {
        setTimeout(function () {
            JumpNextEditedCell(grid);
        }, 0);
    }
}

function model_change(e, grid) {
    var field = e.field;
    if (field === "Qty" || field === "Price") {
        setLastEditedCell(grid);
        UpdateRowAmounts(e.sender.source != null ? e.sender.source : e.sender);
        setTimeout(function () {
            grid.refresh();
            JumpNextEditedCell(grid);
        }, 0);
    }

}


function printInvoice(e) {
    var invoiceID = $("#invoiceId").val();
    window.open(URL_PRINT_INVOICE + "/" + invoiceID, '_blank');

}


function saveInvoice(e) {
    saveInvoiceTemplate(false);
}

function saveAndNewInvoice(e) {
    saveInvoiceTemplate(true);
}

function onChangeAdvancePayment(e) {
    UpdateGridTotals();
}

function cancelInvoice(e) {
    //clear all page
    location.reload();
}

function onEditGrid(e) {
    setTimeout(function () {
        e.container.find("input").select();
    }, 100);
    e.model.unbind("change", function (event) { model_change(event, e.sender); }).bind("change", function (event) { model_change(event, e.sender); });
}


function error_handler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
    }
}

function UpdateRowAmounts(model) {
    if (model.Article != null) {
        model.Total = model.Qty * model.Price;
        model.TotalPlusVat = (1 + model.VAT / 100) * model.Total;
        model.VatAmount = model.TotalPlusVat - model.Total;

        UpdateGridTotals();
    }
}

function actionOnRowItem(e, gridName, PropertyId, UrlAction) {
    var grid = $(gridName).data("kendoGrid");
    var propid = grid.dataItem($(e.currentTarget).closest("tr"))[PropertyId];
    window.location = UrlAction + (propid != undefined ? ("/" + propid) : '');
}