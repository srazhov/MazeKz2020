// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {
    // Form Validation
    $('.needs-validation').submit(function (event) {
        if ($(this)[0].checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }

        $(this).addClass('was-validated');
    });

    $('#form-validation-errors').children().each(function () {
        const attributeName = $(this).attr('data-valmsg-for').toLowerCase();
        const attibuteValue = $(this).text();

        const inputElement = $('.needs-validation .form-control[name="' + attributeName + '"]');
        inputElement.on('input propertychange', function () {
            $('#invalid-feedback-temp-' + attributeName).remove();
        });
        const clone = '<div class="invalid-feedback" id="invalid-feedback-temp-' + attributeName + '">' + attibuteValue + '</div>';
        inputElement.parent().append(clone);

        $('.needs-validation').addClass('was-validated');
    });

    // Prevent enter
    $('.prevent-enter').on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            return false;
        }
    });

    // CitizenUser name autocomplete
    // НЕ РАБОТАЕТ без скриптов, объявленные в _PoliceLayout.cshtml
    $('#get-citizen-user-names').autocomplete({
        source: function (request, respone) {
            $.ajax({
                type: 'GET',
                url: '/api/violation/SearchUsers/' + $('#get-citizen-user-names').val(),
                success: function (data) {
                    respone($.map(data, function (item) {
                        return {
                            value: item.name,
                            label: (item.name + ' (' + item.login + ' )'),
                            login: item.login
                        }
                    }))
                }
            });
        },
        select: function (event, ui) {
            $('#get-citizen-user-names').attr('readonly', 'true').attr('data-mmsg', ui.item.login).parent().toggleClass('col-md-8 col-md-6');
            $('#cancel-get-user').show();
            $('input[name="blamedUserLogin"]').val(ui.item.login);
        }
    });

    $('#cancel-get-user').hide().click(function () {
        // Cancel
        $('#get-citizen-user-names').removeAttr('readonly').attr('data-mmsg', 'null').val('').parent().toggleClass('col-md-8 col-md-6');
        $(this).hide();
    });

    // Создание экрана подтверждения для формы AddViolation
    $('#makeConfirmModalView').click(function (event) {
        const form = $(this).closest('form');
        const input = $('#get-citizen-user-names');
        if (input.attr('data-mmsg') == 'null' || input.attr('data-mmsg') != form.find('input[name="blamedUserLogin"]').val()) {
            input.attr('data-mmsg', 'null').val('');
        }

        if (form[0].checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }

        form.addClass('was-validated');
    });

    // Кнопка отправки формы AddViolation
    $('#confirm-add-violation').click(function () {
        const form = $('#add-violation');

        $.ajax({
            type: 'POST',
            url: form.attr('action'),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            data: SerializeForm(form),
            success: function (data) {
                window.location = data.redirectLink;
            },
            error: function () {
                alert('Произошла ошибка');
                location.reload();
            }
        });
    });

    $('#searchViolation :input').change(function (e) {
        const form = $(this).closest('form');
        e.preventDefault();

        $.ajax({
            type: 'POST',
            url: form.attr('action'),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            data: SerializeForm(form),
            success: function (data) {
                AddDataItem(data.violations, 'violation', ViolationInstructionsToExtractData);
                $('violation-counter').text('Найдено результатов: ' + data.foundCount + ' (' + data.foundOnThisPage + ' на этой странице)');
                if (data.violations.length == 0) {
                    $('#violation-counter').text('');
                    $('.violation-not-found').show();
                }
            },
            error: function (data) {
                $('.violation-not-found').show();
            }
        });
    });

    if ($('.violation-container .violation-item').length != 0) {
        $.get('/api/violation/', function (data) {
            AddDataItem(data, 'violation', ViolationInstructionsToExtractData);
        });
    }

    if ($('.declaration-container .declaration-item').length != 0) {
        $.get('/api/violation/Declarations/10', function (data) {
            AddDataItem(data, 'declaration', DeclarationInstruction);
        });
    }
});

function SerializeForm(form) {
    var unindexed_array = form.serializeArray();
    var indexed_array = {};
    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return JSON.stringify(indexed_array);
}

function AddDataItem(data, dataName, instruction) {
    let parent = $('.' + dataName + '-container');
    let cloneItem = parent.find('.' + dataName + '-item');

    $('.temp-item-' + dataName).remove();
    if (cloneItem.length != 0) {
        for (const i in data) {
            let clone = cloneItem.clone().toggleClass(dataName + '-item temp-item-' + dataName).removeAttr('style').appendTo(parent);
            if (clone.hasClass('clickable')) {
                clone.addClass('temp-clickable');
            }

            instruction(clone, data[i]);
        }

        if (data.length == 0) {
            $('#' + dataName + '-not-found').show();
        }
        else {
            $('#' + dataName + '-not-found').hide();
        }

        cloneItem.attr('style', 'display: none!important;');
    }
}

function ViolationInstructionsToExtractData(clone, data) {
    clone.find('.v-user').text(data.userName);
    clone.find('.v-policeman').text(data.policemanName);
    clone.find('.v-date').text(new Date(data.date).toLocaleDateString());
    clone.find('.v-link').attr('href', '/Police/Criminal/' + data.id);
    $('.temp-clickable').toggleClass('temp-clickable').click(function () {
        window.location = '/Police/Criminal/' + data.id;
    });
}

function DeclarationInstruction(clone, data) {
    clone.find('.dec-user').text(data.userLogin);
    clone.find('.dec-blamed').text(data.blamedUserLogin);
}
