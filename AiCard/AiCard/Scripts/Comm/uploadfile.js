
var uploader = function (target, option) {
    var $this = $(target);
    var $btnUpload = $this.find(".btnUpload");
    var $file = $this.find(":file");
    var $table = $this.find("table");
    var $val = $this.find(".uploaderVal");
    var $dragOver = $this.find(".DragOver");//拖拽区域 
    var $alldel = $this.find(".alldel");//全部删除按钮
    var $fileList = $this.find("ul");//列表全部
    var $alter = $this.find(".modal_alter");
    //Modal
    var $modal = $this.find(".uploader_modal");
    var $btnModal = $this.find(".btnModal")
    var $btnClose = $this.find(".uploader_modal_close");
    //控件属性
    var max = $this.data("max");
    var sortable = $this.data("sortable");
    var isResetName = $this.data("isresetname");
    var filePath = $this.data("filepath");
    var server = $this.data("server");
    var fileUrls = new Array();
    var mask = comm.mask2.create(10000);

    var uploadControl = comm.imgFullUrl("~/Uploader/Upload");
    var deleteControl = comm.imgFullUrl("~/Uploader/DeleteFile");
    $this.after(mask);
    if (option == undefined) {
        option = {
            showed: function () { },
            closed: function () { },
            uploaded: function () { },
        }
    } else {
        if (option.showed == undefined) {
            option.showed = function () { }
        }
        if (option.closed == undefined) {
            option.closed = function () { }
        }
        if (option.uploaded == undefined) {
            option.uploaded = function () { }
        }
    }

    function _show() {
        $modal.removeClass("hidden");
        mask.show();
        option.showed();
    }
    this.show = function () {
        _show();
    }

    function _clean() {
        fileUrls = new Array();
    }

    this.clean = function () {
        _clean();
    };

    function _close() {
        $modal.addClass("hidden");
        mask.hide();
        option.closed(fileUrls);
        getUploadList().remove();
        $file.val("");
    }



    this.close = function () {
        _close();
    }

    function getUploadList() {
        return $table.find("tr").not(".demo_list");
    }
    function getFileListItems() {
        return $fileList.find("li").not(".demo_li");
    }

    //打开Modal
    $btnModal.click(function () {
        if (getFileListItems().length < max) {
            _show();
        }
        else {
            comm.alter(2, "最多只能上传" + max + "个文件");
        }
    })
    //关闭Modal
    $btnClose.click(function () {
        _close();
    });

    //获取图片地址放到Value控件里面
    function setVal() {
        fileUrls = new Array();
        $.each(getFileListItems(), function (i, n) {
            var url = $(n).find("[data-img]").data("img");
            fileUrls.push(url);
        });
        $val.val(JSON.stringify(fileUrls));
    }
    setVal();
    //更新Value
    var uploaderVal = {
        add: function (url) {
            fileUrls.push(url);
            $val.val(JSON.stringify(fileUrls));
        },
        del: function (url) {
            var i = fileUrls.indexOf(url);
            if (i > -1) {
                fileUrls.splice(i, 1);
            }
            $val.val(JSON.stringify(fileUrls));
        }

    }

    //文件对象（已上传文件）
    var fileListItem = function (li) {
        li = $(li);
        li.find(".glyphicon").click(function () {
            var url = $(this).data("img");
            delFile(url, {
                done: function () {
                    li.remove();
                    setVal();
                }
            })
        });
        function getImageUrl() {
            return li.find("[data-img]").data("img");
        }
        this.getImageUrl = function () {
            return getImageUrl();
        }
        this.removeItem = function () {
            li.remove();
            uploaderVal.del(getImageUrl());
        }
        return li;
    }
    //创建
    function createFileListItem(url, loc) {
        if (loc == undefined) {
            loc = comm.imgFullUrl(url);
        }
        li = $this.find(".demo_li").clone().removeClass("hidden demo_li");
        //图片地址
        li.find("img").attr("src", fileType(loc, url));
        li.find("img").attr("title", url.substr(url.lastIndexOf("/") + 1));
        var $btnDel = li.find("span");
        $btnDel.attr("data-img", url).data("img", url);
        $this.find("ul").append(li);
        uploaderVal.add(url);
        return new fileListItem(li);
    }
    //上传文件按钮
    $btnUpload.click(function (e) {
        if (getFileListItems().length < max) {
            $file.click();
        }
        else {
            comm.alter(2, "最多只能上传" + max + "个文件", null, $alter);
        }
    });
    //上传单元（真正上传对象）
    var uploader = function (file) {
        //创建
        var demo = $table.find(".demo_list").clone().removeClass("hidden demo_list");
        var $fileName = demo.find(".filename");
        var $progress = demo.find(".progress-bar");
        var $btnDel = demo.find(".btnDelFile");
        var $preview = demo.find(".preview");
        $fileName.text(file.name);

        $progress.removeClass("demo_progress");

        demo.find(".demo_remove").removeClass("demo_remove");

        ///获取预览图
        var objUrl = null;
        if (window.createObjectURL != undefined) { // basic
            objUrl = window.createObjectURL(file);
        } else if (window.URL != undefined) { // mozilla(firefox)
            objUrl = window.URL.createObjectURL(file);
        } else if (window.webkitURL != undefined) { // webkit or chrome
            objUrl = window.webkitURL.createObjectURL(file);
        }

        $preview.attr("src", fileType(objUrl, file.name));

        $table.append(demo);

        var data = new FormData();
        data.append("img", file);
        data.append("max", max);
        data.append("sortable", sortable);
        data.append("isResetName", isResetName);
        data.append("server", server);
        if (filePath != undefined) {
            data.append("filePath", filePath);
        }

        var xhr = null;
        $.ajax({
            type: "POST",
            url: uploadControl,
            data: data,
            cache: false,
            dataType: "json",
            processData: false,
            contentType: false,
            xhr: function () {
                xhr = $.ajaxSettings.xhr();
                $btnDel.click(function () {
                    xhr.abort();
                    demo.remove();
                });
                if (onprogress && xhr.upload) {
                    xhr.upload.addEventListener("progress", function (e) {
                        var loaded = e.loaded;  //已经上传大小情况
                        var tot = e.total;  //附件总大小
                        var per = Math.floor(100 * loaded / tot);//已经上传的百分比
                        onprogress(per);
                    }, false);
                    return xhr;
                }
            },
            success: function (data) {
                if (data.State == "Success") {
                    var url = data.Result.FileUrls[0];
                    var a = createFileListItem(url, objUrl);
                    //重新注册点击事件
                    $btnDel.text("删除");
                    $btnDel.unbind("click");
                    $btnDel.click(function () {
                        delFile(url, {
                            done: function () {
                                demo.remove();
                                a.remove();
                            }
                        })
                    });
                    onprogress(100, "上传成功", 1);
                    option.uploaded(url);
                } else {
                    comm.alter(0, data.Message, null, $alter);
                }
            },
            error: function () {
                onprogress(100, "上传失败", 0);
            }
        });
        //修改进度进度条
        //state 0：危险 | 1：成功 | 2：警告
        function onprogress(per, text, state) {
            if (state == undefined) {
                state == -1;
            }
            var p = $progress;
            p.text(per + "%");
            p.css("width", per + "%");
            if (text != undefined) {
                p.text(text);
            }
            switch (state) {
                case 1:
                    p.addClass("progress-bar-success");
                    break;
                case 0:
                    p.addClass("progress-bar-danger");
                    break;
                case 2:
                    p.addClass("progress-bar-warning");
                    break;
                default:
                    break;
            }
        }

        return demo;
    }
    //判断文件类型
    var fileType = function (url, filename) {
        var previewSrc = url;
        var fileType = filename.substr(filename.lastIndexOf(".") + 1);
        if (fileType != "bmp" && fileType != "tif" && fileType != "png" && fileType != "jpge" && fileType != "jpg" && fileType != "gif") {
            previewSrc = "/Content/Images/filetype/file.png"
            if (fileType == "doc" || fileType == "docx" || fileType == "txt" || fileType == "wps") {
                previewSrc = "/Content/Images/filetype/doc.png";
            }
            if (fileType == "mp3") {
                previewSrc = "/Content/Images/filetype/mp3.png";
            }
            if (fileType == "mp4") {
                previewSrc = "/Content/Images/filetype/mp4.png";
            }
            if (fileType == "ppt") {
                previewSrc = "/Content/Images/filetype/ppt.png";
            }
            if (fileType == "xls" || fileType == "xlsx") {
                previewSrc = "/Content/Images/filetype/xls.png";
            }
            if (fileType == "zip") {
                previewSrc = "/Content/Images/filetype/zip.png";
            }
        }
        return previewSrc;
    }

    //阻止浏览器默认行。
    $(document).on({
        dragleave: function (e) {    //拖离 
            e.preventDefault();
        },
        drop: function (e) {  //拖后放 
            e.preventDefault();
        },
        dragenter: function (e) {    //拖进 
            e.preventDefault();
        },
        dragover: function (e) {    //拖来拖去 
            e.preventDefault();
        }
    });
    //拖拽事件
    $dragOver.on({
        drop: function (e) {
            e.preventDefault();
            var files = e.originalEvent.dataTransfer.files; //获取文件对象
            //检测是否是拖拽文件到页面的操作 
            if (files.length == 0) {
                return false;
            }
            uploadFile(files);
        }
    });

    //上传文件
    $file.change(function (e) {
        var files = $(this)[0].files;
        uploadFile(files);
    });
    //上传文件
    function uploadFile(files) {
        $.each(files, function (i, item) {
            uploader(item);
        });
    }

    //全部删除
    $alldel.click(function (e) {
        $.each(getUploadList(), function (i, n) {
            $(n).find(".btnDelFile").click();
        });
    });

    //删除服务器文件
    function delFile(url, option) {
        if (option == undefined) {
            option.done = function () { };
            option.error = function () { };
        }
        if (option.done == undefined) {
            option.done = function () { };
        }
        if (option.error == undefined) {
            option.error = function () { };
        }
        $.ajax({
            type: "POST",
            url: comm.action("DeleteFile", "Uploader"),
            data: { file: url },
            dataType: "json",
            success: function (data) {
                option.done();
            }
        });
    }

    //初始化相册
    $.each(getFileListItems(), function (i, n) {
        new fileListItem(n);
    });

    //排序
    if (sortable) {
        if ($fileList.sortable == undefined) {
            throw "未引用jquery ui";
        };
        $fileList.sortable({
            cursor: "img",
            update: function () {
                setVal();
            }
        })
    }

    this.getUploadFilePaths = function () {
        return JSON.parse($val.val());
    };

    this.setFiles = function (urls) {
        getFileListItems().remove();
        $.each(urls, function (i, n) {
            createFileListItem(n);
        });
        setVal();
    }
    this.addFile = function (url) {
        createFileListItem(url);
        setVal();
    }
    this.remove = function (url) {
        $.each(getFileListItems(), function (i, n) {
            $(n).has("[data-img=" + url + "]").remove();
        });
    }
}
//参考调用方式
$.each($(".uploader").not(".noInit"), function (i, n) {
    new uploader(n);
});
