$(function () {

    $('.list-group-item').on('click', function () {
        $('.fas', this)
            .toggleClass('fa-angle-right')
            .toggleClass('fa-angle-down');
    });

});