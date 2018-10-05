var imageModule = function (imgs, option) {
    if (option == undefined) {
        option = {};
    }
    if (option.initialSlide == undefined) {
        option.initialSlide = 0;
    }
    if (option.show == undefined) {
        option.show = function () { }
    }
    if (option.hide == undefined) {
        option.hide = function () { }
    }
    var showImg = $(".showImgsBox");
    var windowH = $(window).height();
    var a = $(window).scrollTop();
    if (a != 0) {
        t = a;
    }
    showImg.children().remove();
    var $main = $(".main");

    var $module = $("<div>").addClass("showImgsModule");
    var $container = $("<div>").addClass("swiper-container");
    var $wrapper = $("<div>").addClass("swiper-wrapper");
    var $pagination = $("<div>").addClass("swiper-pagination");
    var $del = $("<img>")
        .addClass("showImgsBox-remove")
        .attr("src", comm.webPath + "/Content/Images/view/deleteimage.png");

    var sw = $(window).height() / $(window).width();

    $.each(imgs, function (i, n) {
        var $slide = $("<div>").addClass("swiper-slide");

        var $img = $("<img>").addClass("swiper-lazy").attrdata("src", n);
        var $preloader = $("<div>")
                .addClass("swiper-lazy-preloader")
                .addClass("swiper-lazy-preloader-white");
        $slide.append($img).append($preloader);
        $wrapper.append($slide);
        $img.load(function () {
            var $this = $(this);
            var iSw = $this.height() / $this.width();
            if ($this.height() == $(window).height() && iSw > sw) {
                $img.click(function () {
                    $img.toggleClass("imgType01")
                });
            }

        });

    });
    $container.append($wrapper);
    $container.append($pagination);
    $container.append($del);
    $module.append($container);
    showImg.append($module);

    function _show() {
        $main.addClass("hidden");
        showImg.removeClass("hidden");
        var swiper = new Swiper($container, {
            pagination: $pagination,
            paginationClickable: true,
            preloadImages: false,
            lazyLoading: true,
            initialSlide: option.initialSlide
        });
        option.show();

    }
    this.show = function () {
        _show();

    }

    function _hide() {
        showImg.addClass("hidden").children().remove();
        $main.removeClass("hidden");
        $(window).scrollTop(t);

        option.hide();
    }

    this.hide = function () {
        _hide();
    }

    var imgH = $(".showImgsModule img");

    $.each(imgH, function (i, n) {
        if ($(n).height() > windowH) {
            $(n).addClass("imgType02");
        } else {
            $(n).addClass("imgType01");
            $(n).parent().height(windowH);
        }
    });


    //setTimeout(function () {
    //    var imgH = $(".showImgsModule img");

    //    $.each(imgH, function (i, n) {
    //        if ($(n).height() > windowH) {
    //            $(n).addClass("imgType02");
    //        } else {
    //            $(n).addClass("imgType01");
    //            $(n).parent().height(windowH);
    //        }
    //    });
    //}, 100);

    $del.click("click", function () {
      
        _hide();
    });
}