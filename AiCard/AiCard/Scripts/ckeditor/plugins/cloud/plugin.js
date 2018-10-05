
CKEDITOR.plugins.add('cloud',
{
    lang: 'zh-cn',
    init: function (editor) {
        var pluginName = 'cloud';
        editor.ui.addButton('cloud',
           {
               label: editor.lang.cloud.cloud,
               command: 'show',
               icon: this.path + 'images/cloud.png'
           });
        var cmd = editor.addCommand('show', { exec: dalCloud });
    }
});

if (cloud != undefined) {
    var myCloud = new cloud("#cloud", {
        added: function (data) {
            var html = "";
            $.each(data, function (i, n) {
                html += "<p><img src='" + comm.imgFullUrl(n) + "' style='width:100%'/></p>";
            });
            html+="<p></p>"
            editor.insertHtml(html);
            myCloud.hide();
        }
    });

    function dalCloud(e) {

        //myCloud.show();
    }
}
