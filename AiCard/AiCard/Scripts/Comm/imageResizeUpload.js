
var imageResizeUpload = function (selector, option) {
    var isMoblieDevice = new check().isMoblieDevice();
    var sp = 50;

    var $iru = $(selector);
    var $panel = $iru.find(".imageResizeUpload_panel");
    var $pf = $iru.find(".preview_f");
    var $pb = $iru.find(".preview_b");
    var $p = $iru.find(".preview");
    var $rImage = $iru.find(".imageResizeUpload_result");
    var $val = $iru.find("[type=hidden]");
    //配置参数
    var config = {
        uploadUrl: comm.action("Upload", "Uploader")
    };

    if (option == undefined) {
        option = {
            width: $pf.width(),
            height: $pf.height(),
            uploaded: function (state, message, data) { }
        }
    } else {
        if (option.width == undefined) {
            option.width = $pf.width();
        }
        if (option.height == undefined) {
            option.height = $pf.height();
        }
        if (option.uploaded == undefined) {
            option.uploaded = function (state, message, data) { }
        }
    }

    $panel.width(option.width + 130);
    $pf.width(option.width).height(option.height);
    $pb.add($p).width(option.width + sp * 2).height(option.height + sp * 2);
    var $imgPf = $pf.find("img");
    var $imgPb = $pb.find("img");
    var $file = $iru.find("input[type=file]");
    var $btnResize = $iru.find(".imageResizeUpload_resize_button");

    var size = {
        width: option.width,
        height: option.height
    }
    var uploadImage = {
        width: 0,
        height: 0
    };
    if (!isMoblieDevice) {
        $imgPf.add($imgPb).on("dragstart", function (e) {
            return false;
        })
    }

    var image = {
        width: 0,
        height: 0,
        x: 0,
        y: 0,
        sp: sp,
        p: 0,
        minp: 0,
        maxp: 1,
        move: function (x, y) {
            image.x = x;
            image.y = y;
            var limit = {
                xMax: image.width / 2,
                xMin: size.width - image.width + image.width / 2,
                yMax: image.height / 2,
                yMin: size.height - image.height + image.height / 2,
            };

            if (image.x < limit.xMin) {
                image.x = limit.xMin;
            } else if (image.x > limit.xMax) {
                image.x = limit.xMax;
            }
            if (image.y < limit.yMin) {
                image.y = limit.yMin;
            } else if (image.y > limit.yMax) {
                image.y = limit.yMax;
            }
            $imgPf.width(image.width)
              .css("left", image.x)
              .css("top", image.y);
            $imgPb.width(image.width)
                .css("left", image.x + image.sp)
                .css("top", image.y + image.sp);
        },
        resize: function (p) {
            image.p = p;
            image.width = uploadImage.width * image.p;
            image.height = uploadImage.height * image.p;

            $imgPf.add($imgPb).width(image.width)
                .height(image.height);
        },
        init: function () {
            if (!isMoblieDevice) {
                $imgPf.unbind("mousedown");
                $imgPf.mousedown(function (e) {
                    var start = { x: e.pageX, ix: image.x, y: e.pageY, iy: image.y };
                    mousemove(start.x, start.y, function (x, y) {
                        image.move(x + start.ix, y + start.iy);
                    });
                });
            } else {
                $imgPf.unbind("touchstart");
                $imgPf.on("touchstart", function (e) {
                    var t = e.originalEvent.touches[0];
                    var start = { x: t.pageX, ix: image.x, y: t.pageY, iy: image.y };
                    touchmove($imgPf, start.x, start.y, function (x, y) {
                        image.move(x + start.ix, y + start.iy);
                    })
                });
            }



            if (size.height / uploadImage.height * uploadImage.width > size.width) {
                image.width = size.height / uploadImage.height * uploadImage.width;
                image.height = size.height;
            } else {
                image.width = size.width;
                image.height = size.width / uploadImage.width * uploadImage.height;
            }

            image.resize(image.width / uploadImage.width);
            image.move(image.width / 2, image.height / 2);
            image.minp = image.width / uploadImage.width;
            if (image.width > uploadImage.width || image.height > uploadImage.height) {
                image.maxp = image.minp + 0.5;
            }
        },
        getLeftTopPoint: function () {
            return {
                x: image.x - image.width / 2,
                y: image.y - image.height / 2
            }
        }
    }
    var resize = {
        init: function () {
            resize.x = 0;
            resize.pre = (image.maxp - image.minp) / ($btnResize.parent().width() - $btnResize.width());
            resize.min = image.minp;
            resize.value = image.minp;
            resize.move();
            if (!isMoblieDevice) {
                $btnResize.unbind("mousedown");
                $btnResize.mousedown(function (e) {
                    var start = { mx: e.pageX, x: resize.x };
                    mousemove(start.mx, 0, function (x, y) {
                        resize.x = x + start.x;
                        resize.move();
                    })
                });
            } else {
                $btnResize.unbind("touchstart");
                $btnResize.on("touchstart", function (e) {
                    var t = e.originalEvent.touches[0];
                    var start = { mx: t.pageX, x: resize.x };
                    touchmove($btnResize, start.mx, 0, function (x, y) {
                        resize.x = x + start.x;
                        resize.move();
                    })
                });
            }


        },
        x: 0,
        min: 0,
        max: 1,
        width: $btnResize.parent().width() - $btnResize.width(),
        value: 0,
        pre: 0,
        move: function () {
            if (resize.x < 0) {
                resize.x = 0;
            } else if (resize.x > resize.width) {
                resize.x = resize.width;
            }

            $btnResize.css("left", resize.x);

            var tp = resize.x * resize.pre;
            resize.value = resize.min + tp;
            image.resize(resize.value);
            image.move(image.x, image.y);
        }
    };
    $file.change(function (e) {
        var imageFile = comm.getImage(this)[0];
        var tempImage = new Image();
        tempImage.src = imageFile;

        tempImage.onload = function () {
            show();
            uploadImage.width = this.width;
            uploadImage.height = this.height;
            image.init();
            resize.init();
            $imgPf.add($imgPb).attr("src", imageFile);
        }
    });

    $iru.find(".btn-upload").click(function (e) {
        var canvas = $("<canvas>")[0];
        var point = image.getLeftTopPoint();
        var temp = {
            width: size.width / image.p,
            height: size.height / image.p,
            x: -1 * point.x / image.p,
            y: -1 * point.y / image.p
        };
        canvas.width = temp.width;
        canvas.height = temp.height;
        var ctx = canvas.getContext("2d");
        ctx.fillStyle = "#fff";
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        if (image.p < 1) {
            ctx.drawImage($imgPf[0], temp.x, temp.y, temp.width, temp.height, 0, 0, temp.width, temp.height);
            canvas = downSizeCanvas(canvas, size.width, size.height);
        } else {
            canvas.width = size.width;
            canvas.height = size.height;
            ctx.drawImage($imgPf[0], temp.x, temp.y, temp.width, temp.height, 0, 0, size.width, size.height);
        }

        var resizedImage = window.dataURLtoBlob(canvas.toDataURL());
        var data = new FormData();
        data.append("img", resizedImage, "resize.jpg");
        $.ajax({
            type: "POST",
            url: config.uploadUrl,
            data: data,
            cache: false,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.State == "Success") {
                    hide();
                    var result = {
                        url: data.Result.FileUrls[0],
                        blob: resizedImage,
                        dataUrl: comm.getImageFromBlob(resizedImage)
                    };
                    option.uploaded("Success", "上传完成", result);
                    $rImage.attr("src", result.dataUrl);
                    $val.val(result.url);
                } else {
                    option.uploaded("Error", "上传失败");
                }
            },
            error: function () {
                option.uploaded("Error", "上传失败");
            }
        });
    });
    $iru.find(".btn-cancel").click(function () {
        hide();
    })

    $rImage.click(function (e) {
        browse();
    })

    function mousemove(x, y, func) {
        $(window).mousemove(function (e) {
            func(e.pageX - x, e.pageY - y);
        });
        $(window).one("mouseup", function (e) {
            $(window).unbind("mousemove");
        });
    }

    function touchmove(target, x, y, func) {
        $(target).on("touchmove", function (e) {
            var t = e.originalEvent.touches[0];
            func(t.pageX - x, t.pageY - y);
        });
        $(target).one("touchend", function (e) {
            $(target).unbind("touchmove");
        });
    }


    function browse() {
        $file.click();
    }

    function show() {
        $panel.removeClass("hidden");
        comm.mask();
    }

    function hide() {
        $panel.addClass("hidden");
        comm.mask();
    }

    return {
        browse: function () { browse(); },
        show: function () { show(); },
        hide: function () {
            $file.val("");
            hide();
        }
    }
}



// scales the image by (float) scale < 1
// returns a canvas containing the scaled image.
function downScaleImage(img, scale) {
    var imgCV = document.createElement('canvas');
    imgCV.width = img.width;
    imgCV.height = img.height;

    var imgCtx = imgCV.getContext('2d');
    imgCtx.fillStyle = "#fff";
    imgCtx.fillRect(0, 0, imgCV.width, imgCV.height);
    imgCtx.drawImage(img, 0, 0);
    return downScaleCanvas(imgCV, scale);
}

function downSizeImage(img, w, h) {
    var imgCV = document.createElement('canvas');
    imgCV.width = img.width;
    imgCV.height = img.height;

    var imgCtx = imgCV.getContext('2d');
    imgCtx.fillStyle = "#fff";
    imgCtx.drawImage(img, 0, 0);
    return downSizeCanvas(imgCV, w, h);
}

// scales the canvas by (float) scale < 1
// returns a new canvas containing the scaled image.
function downScaleCanvas(cv, scale) {
    var sw = cv.width; // source image width
    var sh = cv.height; // source image height
    var tw = Math.floor(sw * scale); // target image width
    var th = Math.floor(sh * scale); // target image height
    return downSizeCanvas(cv, tw, th);
}

function downSizeCanvas(cv, w, h) {
    var scale = w / cv.width;
    if (!(scale < 1) || !(scale > 0)) throw ('scale must be a positive number <1 ');
    var sqScale = scale * scale; // square scale = area of source pixel within target
    var sw = cv.width; // source image width
    var sh = cv.height; // source image height
    var tw = w; // target image width
    var th = h; // target image height
    var sx = 0, sy = 0, sIndex = 0; // source x,y, index within source array
    var tx = 0, ty = 0, yIndex = 0, tIndex = 0; // target x,y, x,y index within target array
    var tX = 0, tY = 0; // rounded tx, ty
    var w = 0, nw = 0, wx = 0, nwx = 0, wy = 0, nwy = 0; // weight / next weight x / y
    // weight is weight of current source point within target.
    // next weight is weight of current source point within next target's point.
    var crossX = false; // does scaled px cross its current px right border ?
    var crossY = false; // does scaled px cross its current px bottom border ?
    var sBuffer = cv.getContext('2d').
    getImageData(0, 0, sw, sh).data; // source buffer 8 bit rgba
    var tBuffer = new Float32Array(3 * tw * th); // target buffer Float32 rgb
    var sR = 0, sG = 0, sB = 0; // source's current point r,g,b
    /* untested !
    var sA = 0;  //source alpha  */

    for (sy = 0; sy < sh; sy++) {
        ty = sy * scale; // y src position within target
        tY = 0 | ty;     // rounded : target pixel's y
        yIndex = 3 * tY * tw;  // line index within target array
        crossY = (tY != (0 | ty + scale));
        if (crossY) { // if pixel is crossing botton target pixel
            wy = (tY + 1 - ty); // weight of point within target pixel
            nwy = (ty + scale - tY - 1); // ... within y+1 target pixel
        }
        for (sx = 0; sx < sw; sx++, sIndex += 4) {
            tx = sx * scale; // x src position within target
            tX = 0 | tx;    // rounded : target pixel's x
            tIndex = yIndex + tX * 3; // target pixel index within target array
            crossX = (tX != (0 | tx + scale));
            if (crossX) { // if pixel is crossing target pixel's right
                wx = (tX + 1 - tx); // weight of point within target pixel
                nwx = (tx + scale - tX - 1); // ... within x+1 target pixel
            }
            sR = sBuffer[sIndex];   // retrieving r,g,b for curr src px.
            sG = sBuffer[sIndex + 1];
            sB = sBuffer[sIndex + 2];

            /* !! untested : handling alpha !! 
            sA = sBuffer[sIndex + 3];
            if (!sA) continue;
            if (sA != 0xFF) {
                sR = (sR * sA) >> 8;  // or use /256 instead ??
                sG = (sG * sA) >> 8;
                sB = (sB * sA) >> 8;
            } */

            if (!crossX && !crossY) { // pixel does not cross
                // just add components weighted by squared scale.
                tBuffer[tIndex] += sR * sqScale;
                tBuffer[tIndex + 1] += sG * sqScale;
                tBuffer[tIndex + 2] += sB * sqScale;
            } else if (crossX && !crossY) { // cross on X only
                w = wx * scale;
                // add weighted component for current px
                tBuffer[tIndex] += sR * w;
                tBuffer[tIndex + 1] += sG * w;
                tBuffer[tIndex + 2] += sB * w;
                // add weighted component for next (tX+1) px                
                nw = nwx * scale
                tBuffer[tIndex + 3] += sR * nw;
                tBuffer[tIndex + 4] += sG * nw;
                tBuffer[tIndex + 5] += sB * nw;
            } else if (crossY && !crossX) { // cross on Y only
                w = wy * scale;
                // add weighted component for current px
                tBuffer[tIndex] += sR * w;
                tBuffer[tIndex + 1] += sG * w;
                tBuffer[tIndex + 2] += sB * w;
                // add weighted component for next (tY+1) px                
                nw = nwy * scale
                tBuffer[tIndex + 3 * tw] += sR * nw;
                tBuffer[tIndex + 3 * tw + 1] += sG * nw;
                tBuffer[tIndex + 3 * tw + 2] += sB * nw;
            } else { // crosses both x and y : four target points involved
                // add weighted component for current px
                w = wx * wy;
                tBuffer[tIndex] += sR * w;
                tBuffer[tIndex + 1] += sG * w;
                tBuffer[tIndex + 2] += sB * w;
                // for tX + 1; tY px
                nw = nwx * wy;
                tBuffer[tIndex + 3] += sR * nw;
                tBuffer[tIndex + 4] += sG * nw;
                tBuffer[tIndex + 5] += sB * nw;
                // for tX ; tY + 1 px
                nw = wx * nwy;
                tBuffer[tIndex + 3 * tw] += sR * nw;
                tBuffer[tIndex + 3 * tw + 1] += sG * nw;
                tBuffer[tIndex + 3 * tw + 2] += sB * nw;
                // for tX + 1 ; tY +1 px
                nw = nwx * nwy;
                tBuffer[tIndex + 3 * tw + 3] += sR * nw;
                tBuffer[tIndex + 3 * tw + 4] += sG * nw;
                tBuffer[tIndex + 3 * tw + 5] += sB * nw;
            }
        } // end for sx 
    } // end for sy

    // create result canvas
    var resCV = document.createElement('canvas');
    resCV.width = tw;
    resCV.height = th;
    var resCtx = resCV.getContext('2d');
    var imgRes = resCtx.getImageData(0, 0, tw, th);
    var tByteBuffer = imgRes.data;
    // convert float32 array into a UInt8Clamped Array
    var pxIndex = 0; //  
    for (sIndex = 0, tIndex = 0; pxIndex < tw * th; sIndex += 3, tIndex += 4, pxIndex++) {
        tByteBuffer[tIndex] = Math.ceil(tBuffer[sIndex]);
        tByteBuffer[tIndex + 1] = Math.ceil(tBuffer[sIndex + 1]);
        tByteBuffer[tIndex + 2] = Math.ceil(tBuffer[sIndex + 2]);
        tByteBuffer[tIndex + 3] = 255;
    }
    // writing result to canvas.
    resCtx.putImageData(imgRes, 0, 0);
    return resCV;
}

$.each($(".imageResizeUpload:not(.imageResizeUpload-display,.imageResizeUpload-noInit)"), function (i, n) {
    new imageResizeUpload(n, {
        uploaded: function (state, message, data) {
        }
    });
});