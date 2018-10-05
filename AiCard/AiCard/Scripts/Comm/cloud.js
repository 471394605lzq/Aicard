var cloud = function (target, option) {
    var $this = $(target);
    var $btnClose = $this.find(".cloud_close");
    var $btnUpload = $this.find(".btnCloudUpload");
    var $file = $this.find("input[type=file]");
    var selecedItems = function () {
        var items = new Array();
        var ids = new Array();
        this.getItems = function () {
            return items;
        }

        this.getIDs = function () {
            return ids;
        }

        this.add = function (id, url) {
            ids.push(id);
            items.push(url);
        }
        this.remove = function (id) {

            var i = ids.indexOf(id);
            if (i > -1) {
                ids.slice(i, 1);
                items.slice(i, 1);
            }
        }
        this.clean = function () {
            items = new Array();
            ids = new Array();
            $cloudList.find("li").removeClass("active");
        }
    };
    var _selecedItems = new selecedItems();
    var $upload = $this.find(".cloud-upload");
    var $cloudList = $this.find(".cloud-list");
    var $page = $this.find(".cloud-page");
    var $btnAddSelected = $this.find(".btnAddSelected");
    var $btnDelSelected = $this.find(".btnDelSelected");

    if (option == undefined) {
        option = {
            added: function () { }
        };
    } else if (option.added == undefined) {
        option.added = function () { };
    }

    function _show() {
        $this.removeClass("hidden");
        comm.mask2.show();
    };

    this.show = function () {
        _show();
    };

    function _hide() {
        $this.addClass("hidden");
        comm.mask2.hide();
    };

    this.hide = function () {
        _hide();
    };

    $btnClose.click(function () {
        _hide();
    });

    $btnUpload.click(function () {
        $file.click();
    });


    $file.change(function (e) {
        $.each(this.files, function (i, item) {
            uploader(item);
        });

    });

    var uploadItemCout = {
        loading: 0,
        finsh: 0,
    }

    var paged = function () {
        var page = 1;
        var max = null;
        var $btnPrev = $page.find(".cloud-page-prev");
        var $btnNext = $page.find(".cloud-page-next");
        var $txtNumber = $page.find(".cloud-page-number");
        var $txtTotal = $page.find(".cloud-page-total");
        var $btnGo = $page.find(".cloud-page-go");

        $btnPrev.click(function () {
            page--;
            _loadList();
        });

        $btnNext.click(function () {
            page++;
            _loadList();
        });

        $btnGo.click(function () {
            var p = Number.parseInt($txtNumber.val());
            if (isNaN(p)) {
                p = 1;
            }
            page = p;
            _loadList();
        });

        function _set() {
            $btnPrev.prop("disabled", page == 1);
            $btnNext.prop("disabled", max == null || page == max);
            if (page > max) {
                page = max;
            } else if (page < 1) {
                page = 1;
            }
            $txtNumber.val(page);
            $txtTotal.text(max == null ? "" : "/" + max);
        }

        function _loadList() {
            $cloudList.children()
                .not(".cloud-list-item-temp")
                .remove();
            _set();
            $.ajax({
                type: "POST",
                url: comm.action("GetFileList", "Uploader"),
                data: { page: page },
                dataType: "json",
                success: function (data) {
                    if (data.State == "Success") {
                        $.each(data.Result.Data, function (i, n) {
                            new cloudItem(n);
                        });
                        max = data.Page.PageCount;
                        _set();
                    }
                }
            });
        }

        this.go = function (p) {
            if (p != undefined) {
                page = p;
            }
            _set();
            _loadList();
        }

        _loadList();
    }
    var _paged = new paged();

    var uploadItem = function (name, option) {

        var $item = $upload.find(".cloud-upload-item-temp")
            .clone()
            .removeClass("cloud-upload-item-temp");
        $item.find(".cloud-upload-title").text(name);
        var $p = $item.find(".cloud-upload-progress>div");

        if (option == undefined) {
            option = {
                finish: function () { },
                remove: function () { }
            };
        }
        if (option.finish == undefined) {
            option.finish = function () { }
        }
        if (option.remove == undefined) {
            option.remove = function () { }
        }

        this.remove = function () {
            _remove();
        }


        function _remove() {
            option.remove();
            $item.remove();
        }

        this.setValue = function (p) {
            $p.css("width", Math.floor(100 * p) + "%");
            if (p >= 1) {
                option.finish();
            };
        }

        $item.find(".cloud-upload-remove").click(function () {
            _remove();
        });

        $upload.append($item);
    }

    //上传文件
    var uploader = function (file) {
        uploadItemCout.loading++;
        var xhr = null;

        var form = new FormData();
        form.append("img", file, file.name);
        $.ajax({
            type: "POST",
            url: comm.action("UploadByCloud", "Uploader"),
            data: form,
            cache: false,
            dataType: "json",
            processData: false,
            contentType: false,
            xhr: function () {
                xhr = $.ajaxSettings.xhr();
                var item = new uploadItem(file.name, {
                    remove: function () {
                        xhr.abort();
                    },
                    finish: function () {
                        setTimeout(function () {
                            item.remove();
                        }, 1000)

                    }
                });
                if (xhr.upload) {
                    xhr.upload.addEventListener("progress", function (e) {
                        item.setValue(e.loaded / e.total);
                    }, false);
                }
                return xhr;
            },
            success: function (data) {
                uploadItemCout.finsh++;
                if (data.State == "Success") {
                    if (uploadItemCout.loading == uploadItemCout.finsh) {
                        _paged.go(1);
                        $file.val(null);
                    }
                } else {

                }
            },
            error: function (e) {
                console.log("error");
            }
        });
    }

    var cloudItem = function (data) {
        var $item = $cloudList.find(".cloud-list-item-temp")
            .clone()
            .removeClass("cloud-list-item-temp");
        var $itemMark = $item.find(".cloud-list-item-mark");

        var $img = $item.find(".cloud-list-item-img");
        var $name = $item.find(".cloud-list-item-name");

        $item.attrdata("item", data);
        $img.find("img").attr("src", data.Thumbnail);
        $name.text(data.Name);

        $img.click(function () {
            if ($item.hasClass("active")) {
                $item.removeClass("active");
                _selecedItems.remove(data.ID);
            } else {
                $item.addClass("active");
                _selecedItems.add(data.ID, data.Url);
            }

        });
        $cloudList.append($item);
    }



    $btnAddSelected.click(function () {
        option.added(_selecedItems.getItems());
        _selecedItems.clean();
        
    });

    $btnDelSelected.click(function () {
        $.ajax({
            type: "POST",
            url: comm.action("DeleteCloudFile", "Uploader"),
            data: { ids: _selecedItems.getIDs() },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    _paged.go();
                    _selecedItems.clean();
                }
            }
        });
    });

}