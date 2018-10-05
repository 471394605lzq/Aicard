var comm = {
    ///<field name='webPath' type='String'>域名</field>
    webPath: location.protocol + '//' + location.host,

    action: function (action, control, param) {
        /// <summary>网址生成器</summary>
        /// <param name="action" type="String">Action名称</param>
        /// <param name="control" type="String">Control名称</param>
        /// <param name="param" type="Json">参数名称</param>
        /// <returns type="String">地址</returns>
        var url = comm.webPath + "/" + control + "/" + action;
        if (param != null) {
            url += "?" + $.param(param);
        }
        return url;
    },
    actionShort: function (action, control, param) {
        /// <summary>网址生成器</summary>
        /// <param name="action" type="String">Action名称</param>
        /// <param name="control" type="String">Control名称</param>
        /// <param name="param" type="Json">参数名称</param>
        /// <returns type="String">地址</returns>
        var url = "/" + control + "/" + action;
        if (param != null) {
            url += "?" + $.param(param);
        }
        return url;
    },
    imgFullUrl: function (url) {
        if (url == "" || url == null) {
            return null;
        }
        url = url.replace("~", "");
        if (url[0] == "/") {
            url = url.substr(1);
        }
        return comm.webPath + "/" + url.replace("~", "");
    },
    alter: function (type, message, strong, target) {
        /// <summary>提示框</summary>
        /// <param name="container" type="String">控件，提示框会在该控件前插入</param>
        /// <param name="type" type="Int">1：成功；0：危险；2：警告；默认：信息</param>
        /// <param name="strong" type="String">消息头</param>
        /// <param name="message" type="String">消息正文</param>
        /// <param name="target" type="String">弹出框对象</param>
        var c;
        var random = "a" + Math.round(Math.random() * 10000);
        if (target == undefined) {
            target = $(".alertList");
        } else {
            target = $(target);
        }
        switch (type) {
            case 1: {
                c = "alert-success";
                strong = strong == null ? "成功" : strong;
                setTimeout(function () {
                    $("#" + random).remove();
                }, 1000);
                break;
            }
            case 0: {
                c = "alert-danger";
                strong = strong == null ? "错误" : strong;
                break;
            }
            case 2: {
                c = "alert-warning";
                strong = strong == null ? "警告" : strong;
                break;
            }
            default: {
                c = "alert-info";
                strong = strong == null ? "提示" : strong;
                break;
            }
        }
        var dAlter = $("<div>")
            .attr("id", random)
            .addClass("alert")
            .addClass(c)
            .addClass("alert-dismissible")
            .attr("role", "alert");
        var btn = $("<button>")
            .attr("type", "button")
            .addClass("close")
            .attrdata("dismiss", "alert");
        btn.append('<span aria-hidden="true">&times;</span>');
        var dStrong = $("<strong>").text(strong);
        dAlter.append(btn);
        dAlter.append(dStrong);
        dAlter.append(message);
        target.append(dAlter);
        return dAlter;
        //target.append('<div id="' + random + '" class="alert ' + c + ' alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span></button><strong>' + strong + '</strong>' + message + '</div>');
    },
    getImage: function (fileControl) {
        /// <summary>从input file里面读取img的本地地址</summary>
        /// <param name="fileControl" type="input[type=file]">上传用的input file</param>
        /// <returns type="Array">图片地址</returns>
        fileControl = $(fileControl)[0];
        var imgArray = new Array();
        $(fileControl.files).each(function (i, n) {
            var objUrl = null;
            if (window.createObjectURL != undefined) { // basic
                objUrl = window.createObjectURL(n);
            } else if (window.URL != undefined) { // mozilla(firefox)
                objUrl = window.URL.createObjectURL(n);
            } else if (window.webkitURL != undefined) { // webkit or chrome
                objUrl = window.webkitURL.createObjectURL(n);
            }
            if (objUrl) {
                imgArray.push(objUrl);
            }
        });
        return imgArray;
    },
    getImageFromBlob: function (n) {
        if (window.createObjectURL != undefined) { // basic
            objUrl = window.createObjectURL(n);
        } else if (window.URL != undefined) { // mozilla(firefox)
            objUrl = window.URL.createObjectURL(n);
        } else if (window.webkitURL != undefined) { // webkit or chrome
            objUrl = window.webkitURL.createObjectURL(n);
        }
        return objUrl;
    },
    mask: function () {
        /// <summary>显隐遮罩</summary>
        /// <returns type="jQuery Object">$(".mask")</returns>
        $(".mask").toggleClass("hidden");
        return $(".mask");
    },
    mask2: {
        show: function () {
            $(".mask").removeClass("hidden");
        },
        hide: function () {
            $(".mask").addClass("hidden");
        },
        create: function (zIndex) {
            if (zIndex == undefined) {
                zIndex = 9999;
            }
            var mark = $("<div>")
                .addClass("hidden")
                .css("z-index", zIndex)
                .css("position", "fixed")
                .css("top", 0)
                .css("left", 0)
                .css("right", 0)
                .css("bottom", 0)
                .css("background-color", "rgba(0, 0, 0, 0.5)");

            mark.show = function () {
                mark.removeClass("hidden")
            }
            mark.hide = function () {
                mark.addClass("hidden");
            }

            return mark;
        }
    },

    mask3: function () {
        $(".mask.style02").toggleClass("hidden");
        return $(".mask.style02");
    },

    TextToHtml: function (text) {
        return text.replace(/\r\n|\n|\r/g, '<br />');
    },
    promptBox: function (dat) {
        $(".promptBox .promptText").text("");
        $(".promptBox").stop().fadeIn(1000);
        $(".promptBox .promptText").append(dat);
        $(".promptBox").fadeOut(4000);
    },
    lazyloadALL: function () {
        $("img[data-original]").lazyload({ threshold: 40 });
        $(window).trigger('scroll');
    },
    isWindowBottom: function () {
        return $("html").height() - $(window).height() < $(window).scrollTop() + 100
    },
    addHistory: function (data, url) {
        /// <summary>修改浏览器地址栏地址</summary>
        window.history.pushState(data, null, url);
    },
    checkboxList: function (parent, child) {
        var $parent = $(parent);
        var $child = $(child);
        $parent.click(function (e) {
            $child.prop("checked", $parent.prop("checked"))
        });
        function getSelectedIDs() {
            var array = new Array();
            $.each($child.filter(":checked"), function (i, n) {
                array.push($(n).val());
            });
            return array;
        }
        return {
            selectedValues: function () {
                return getSelectedIDs();
            },
            selectedValuesString: function (char) {
                if (char == undefined) {
                    char = ",";
                }
                return getSelectedIDs().join(char);
            }
        }

    },
    checkIsLogin: function checkLogin(message, option) {
        var result = $("#hidIsLogin").data("val");
        var defReturnUrl = location.pathname + location.search;
        if (option == undefined) {
            option = {};
            option.ok = function () { }
            option.returnUrl = defReturnUrl;
        }
        if (option.ok == undefined) {
            option.ok = function () { }
        }
        if (option.returnUrl == undefined) {
            option.returnUrl = defReturnUrl;
        }
        if (!result) {
            if (confirm(message)) {
                location = comm.action("login", "account", { returnUrl: option.returnUrl })
            }
        } else {
            option.ok();
        }
        return result;
    },
    shareTop: {
        show: function () {
            $(".shareTip").removeClass("hidden");

            $(".shareTip").one("click", function () {
                $(".shareTip").addClass("hidden");
            });
        },
        hide: function () {
            $(".shareTip").addClass("hidden");
        }
    }
}
var shareTip = function () {
    var $shareTip = $(".shareTip");
    $shareTip.click(function () {
        $shareTip.addClass("hidden");
    });
    this.show = function () {
        $shareTip.removeClass("hidden");
    };
    this.hide = function () {
        $shareTip.addClass("hidden");
    };
}



