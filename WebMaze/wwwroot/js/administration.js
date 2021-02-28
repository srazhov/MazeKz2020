$(document).ready(function () {
    var editButtonPrototype = $('<button/>').append('Edit').addClass('btn btn-warning btn-sm btn-block');
    var deleteButtonPrototype = $('<button/>').append('Delete').addClass('btn btn-danger btn-sm btn-block');

    $("#addToggle").on("click",
        function () {
            $('#addForm form').slideToggle(500);
        });

    $.get(Uri).done(function (items) {
        displayItems(items);
    });

    $('#addForm form').on('submit', addItem);
    $('#updateForm form').on('submit', updateItem);

    function displayItems(items) {
        var tBody = $("#tBody");
        tBody.empty();

        displayCount(items.length);

        items.forEach(item => {
            var tr = $('<tr/>').appendTo(tBody);
            var td;

            $.each(item,
                function (name, value) {
                    td = $('<td/>').appendTo(tr);
                    if (name.includes('Date')) {
                        td.append(document.createTextNode(value.substring(0, 10)));
                    } else {
                        td.append(document.createTextNode(value));
                    }
                });

            var editButton = editButtonPrototype.clone().on("click", function () { displayUpdateForm(item.id); });
            td = $('<td/>').appendTo(tr);
            td.append(editButton);

            var deleteButton = deleteButtonPrototype.clone().on("click", function () { deleteItem(item.id); });
            td = $('<td/>').appendTo(tr);
            td.append(deleteButton);
        });
    }


    function addItem() {
        event.preventDefault();
        var itemObject = getFormData($(this));

        itemObject.Id = 0;

        $.ajax({
            url: Uri,
            type: 'POST',
            data: JSON.stringify(itemObject),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (result) {
                $.get(Uri).done(function (items) {
                    displayItems(items);
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.responseText);
                console.log(textStatus, errorThrown);
            }
        });

    }

    function getFormData($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};

        $.map(unindexed_array, function (val, i) {
            if (isFloat(val['value'])) {
                indexed_array[val['name']] = parseFloat(val['value']);
            } else {
                indexed_array[val['name']] = val['value'];
            }
        });

        return indexed_array;
    }

    function isFloat(val) {
        var floatRegex = /^-?\d+(?:[.,]\d*?)?$/;
        if (!floatRegex.test(val))
            return false;

        val = parseFloat(val);
        if (isNaN(val))
            return false;
        return true;
    }

    function deleteItem(id) {
        $.ajax({
            url: `${Uri}/${id}`,
            type: 'DELETE',
            success: function (result) {
                $.get(Uri).done(function (items) {
                    displayItems(items);
                });
            }
        }).catch(error => console.error('Unable to delete item.', error));
    }

    function updateItem() {
        var itemId = $('#updateForm form input[name ="Id"]').val();
        var itemObject = getFormData($(this));
        itemObject.Id = parseInt(itemId, 10);

        $.ajax({
            url: `${Uri}/${itemId}`,
            type: 'PUT',
            data: JSON.stringify(itemObject),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            success: function (result) {
                $.get(Uri).done(function (items) {
                    $('#updateForm form').hide();
                    displayItems(items);
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR.responseText);
                console.log(textStatus, errorThrown);
            }
        });
    }

    function displayCount(itemCount) {
        var name = (itemCount === 1) ? 'item' : 'items';

        $("#counter").empty();
        $("#counter").append(`${itemCount} ${name}`);
    }
});