
var tempUploader = null;
CKEDITOR.plugins.add('cyimage',
{
    lang: 'zh-cn',
    init: function (editor) {
        
        var pluginName = 'cyimage';
        editor.ui.addButton('cyimage',
           {
               label: editor.lang.cyimage.image,
               command: 'show',
               icon: this.path + 'images/image.png'
           });
        tempUploader = new uploader("#myModal_ckupload", {
            closed: function (data) {
                var html = "";
                $.each(data, function (i, n) {
                    $("img").attr("src", n)
                    html += "<img src='" + n + "' style='width:100%'/><br/>";
                });
                editor.insertHtml(html);
                tempUploader.clean();

            }
        });
        var cmd = editor.addCommand('show', { exec: dalcyimage });
    }
});


function dalcyimage(e) {
    tempUploader.show();
}