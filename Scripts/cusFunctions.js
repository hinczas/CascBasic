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

    var _url = "/Account/ConfirmRole";
    var _data = { "role": role, "returnUrl": retUrl}
    $.ajax({
        url: _url,
        type: 'post',
        data: _data,
        success: function () {
            
        }
    });
}