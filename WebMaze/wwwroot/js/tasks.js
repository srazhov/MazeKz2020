$(document).ready(function () {
    $("#submitCheckbox").on("change", "input:checkbox", function () {
        $("#submitCheckbox").submit();
    });
});