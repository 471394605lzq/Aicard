(function ($) {
    $.fn.datetime = function () {
        this.datetimepicker({
            format: 'yyyy-mm-dd hh:ii',
            language: "zh-CN",
            todayHighlight: true,
            todayBtn: true
        });
        return this;
    };
    $.fn.date = function () {
        this.datetimepicker({
            format: 'yyyy-mm-dd',
            language: "zh-CN",
            todayHighlight: true,
            todayBtn: true,
            minView: 'month'
        });
        return this;
    };
    $.fn.attrdata = function (key, val) {
        this.data(key, val).attr("data-" + key, val);
        return this;
    };
    $.fn.numberInput = function (option) {
        var def = 0;
        var $this = $(this);
        function setvalue() {
            var val = Number($this.val());
            if (isNaN(val)) {
                val = def;
            }
            if (option.dec != undefined) {
                var temp = Math.pow(10, option.dec);
                var temp2 = Math.accMul(val, temp);
                if (temp2 % 1 > 0) {
                    val = (temp2 - temp2 % 1) / temp;
                }
            }
            if (option.max != undefined) {
                if (val > option.max) {
                    val = option.max;
                }
            }
            if (option.min != undefined) {
                if (val < option.min) {
                    val = option.min;
                }
            }
            if (option.fun != undefined) {
                option.fun();
            }
            if (Number($this.val()) != val) {
                $this.val(val);
            }

        }
        if (option != undefined) {
            if (option.def != undefined) {
                def = option.def;
            }
            $this.keyup(function (e) {
                if ((e.keyCode > 36 && e.keyCode < 41) || e.keyCode == 109 || e.keyCode == 110) {
                    return;
                }
                console.log(e.keyCode);
                setvalue();
            });
            $this.change(function () {
                setvalue();
            })

        }
        return $this;

    };

    $.fn.goback = function (url, option) {
        if (url == undefined || url == null) {
            throw "url 不能为空";
        }
        if (option == undefined) {
            option = {};
        }
        if (option.clicking == undefined) {
            option.clicking = function () { };
        }
        if (option.clicked == undefined) {
            option.clicked = function () { };
        }
        var $this = $(this);
        $this.click(function () {
            option.clicking();
            if (document.referrer == "") {
                location = url;
            } else {
                history.go(-1);
            }
            option.clicked();
        })

    }
}(jQuery));