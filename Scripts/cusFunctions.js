function ChangeRole(e) {
    var role = e.options[e.selectedIndex].value;
    var fullUrl = window.location.href;             // http://domain.com/path1/path2?arg=val
    var protUrl = window.location.protocol + "//"; // http://
    var hostUrl = window.location.hostname;         // domain.com
    var pathUrl = window.location.pathname;         // /path1/path2

    var retUrl = "";
    if (pathUrl == "/" || pathUrl.length <= 1) {
        retUrl = "/";
    } else {
        var idx = fullUrl.indexOf(pathUrl);
        retUrl = fullUrl.substring(idx);
    }

    _changeRole(role, "/");
}

function _changeRole(role, retUrl) {
    var _url = "/Account/ConfirmRole";
    var _data = { "role": role, "returnUrl": retUrl }
    $.ajax({
        url: _url,
        type: 'post',
        data: _data,
        success: function () {
            //location.reload();
            window.location.href = retUrl;
        }
    });
}

// Menu assign page functions
function linkClicked(e) {
    var textId = $(e).attr('id');
    $(".role-link").removeClass("selected");
    $(e).addClass("selected");
    $(".move-button").removeAttr("disabled");
       
    $.ajax({
        url: '/Dashboard/GetRoleMenusAsync',
        type: 'get',
        data: {"roleId": textId},
        success: function (results) {
            feedSelected(results);
        }
    });
}

function feedSelected(e) {
    var gotResults = (typeof (e) !== "undefined") && ($.isArray(e)) && (e.length != 0);
    $("#menusSelected").empty();
    if (gotResults) {
        $(e).each(function (i, v) {
            var $opt = $('<option>');
            $opt.val(v.MenuItemId);
            $opt.append(v.MenuItemName);
            $("#menusSelected").append($opt);
            //$("#menusAvailable option[value='" + v.MenuItemId+"']").remove();
        });
    }
}

function moveMenusLeft() {
    var selectedOptions = $("#menusAvailable").children('option:selected');
    selectedOptions.each(function () {
        var opt = $(this).clone();
        var value = $(opt).attr('value');
        var exists = $("#menusSelected option[value='" + value + "']").length > 0;
        if (!exists) {
            $("#menusSelected").append(opt);
            $(".save-button").removeAttr("disabled");
        }
    });    
}

function moveMenusRight() {
    var selectedOptions = $("#menusSelected").children('option:selected');
    if (selectedOptions.length > 0) {
        $(".save-button").removeAttr("disabled");
    }
    selectedOptions.each(function () {
        var toRem = $(this).attr('value');
        //$("#menusSelected").remove(this);
        $("#menusSelected option[value='" + toRem + "']").remove();
    });
}

function submitMenus() {
    var roleId = $("#menusRolesList > .selected").attr('id');

    var options = $('#menusSelected option');
    var values = $.map(options, function (option) {
        return option.value;
    });

    $.ajax({
        url: '/Dashboard/ProcessMenus',
        type: 'post',
        data: { "roleId": roleId, "menus": values },
        success: function (response) {
            var today = new Date();
            var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();

            if (response.success) {
                if (response.reload) {
                    _changeRole(roleId, window.location.href);
                } else {
                    var $msg = $('<div class="result-success">').append(time+" - "+response.responseText)
                    $(".result-messages").prepend($msg);
                    $(".save-button").attr("disabled", true);
                }
            } else {
                var $msg = $('<div class="result-error">').append(time + " - " +response.responseText)
                $(".result-messages").prepend($msg);
            }
        },
        error: function (response) {
            alert("error!");  // 
        }
    });

}