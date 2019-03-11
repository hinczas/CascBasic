$(document).ready(function () {
    $('#dash-users-tbl').DataTable();
});

$('#userFormBtn').click(function () {
    $('#userForm').toggle();
})

$('#userSubmit').click(function () {
    var form = $(this).parents('form:first');
    var url = form.attr('action');
    var fData = form.serialize();

    $.ajax({
        url: url,
        type: "POST",
        data: fData, // serializes the form's elements.
        success: function (data) {
            //alert(data); // show response from the php script.
            var link = '/Dashboard/Users';
            $.ajax({
                type: "get",
                url: link,
                success: function (d) {
                    /* d is the HTML of the returned response */
                    $('.main').html(d); //replaces previous HTML with action

                }
            });
            $('#alertPlaceholder').html(data);
        }
    });

    return false; // avoid to execute the actual submit of the form.
});