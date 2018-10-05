
//#region 地图
var editMap = function (option) {
    var windowsArr = [];
    var marker = null;
    var clickListener;
    var selectlat = $("#UserAddress_Lat").val();
    var selectlng = $("#UserAddress_Lng").val();
    var selectaddress = $("#UserAddress_Address").val();

    if (option == null) {
        option = { search: "True" };
    }
    else {
        if (option.search == null)
        {
            option = { search: "True" };
        }
    }

    var mapObj = new AMap.Map("mapContainer", {
        resizeEnable: true,
        center: [113.7518044037, 23.0206594065],//地图中心点
        zoom: 12,//地图显示的缩放级别
        keyboardEnable: false
    });

    if (option.search == "True") {
        if (selectaddress != "")
        {
            $("#mapKeyWord").val(selectaddress);
        }
        AMap.plugin(['AMap.Autocomplete', 'AMap.PlaceSearch'], function () {
            var autoOptions = {
                city: "东莞", //城市，默认全国
                //input: "mapKeyWord"//使用联想输入的input的id
            };
            autocomplete = new AMap.Autocomplete(autoOptions);
            var placeSearch = new AMap.PlaceSearch({
                city: '东莞',
                map: mapObj
            });
            $("#mapKeyWord").keyup(function (e) {
                var $this = $(this);
                autocomplete.search($this.val(), function (status, result) {
                    $("#mapKeyTip li").remove();
                    if (status == "complete") {
                        $.each(result.tips, function (i, n) {
                            if (n.location != "") {
                                var $item = $("<li>");
                                $item.text(n.name);
                                $item.data("location", { lat: n.location.lat, lng: n.location.lng });
                                $this.next("ul").append($item);
                            }
                        });
                        $("#mapKeyTip li").click(function () {
                            var $li = $(this);
                            selectaddress = $li.text();
                            var loc = $li.data("location");
                            selectlat = loc.lat;
                            selectlng = loc.lng;
                            updateMarker(loc.lng, loc.lat);
                            $("#btnMap").text("已绑定");
                            $("#mapKeyTip li").remove();
                        });
                    }
                    //TODO:开发者使用result自己进行下拉列表的显示与交互功能
                });
            });
        });

        var _onClick = function (e) {
            updateMarker(e.lnglat.lng, e.lnglat.lat);
        }

        var bind = function () {
            clickListener = AMap.event.addListener(mapObj, "click", _onClick);
        }

        bind();

        $(".map_submit").click(function (e) {
            console.log("sdfsfsd");
            $("#UserAddress_Lng").val(selectlng);
            $("#UserAddress_Lat").val(selectlat);
            $("#UserAddress_Address").val(selectaddress);
            $('#map_modal').modal('hide')
        });
    }

    if (selectlat != "" && selectlng != "") {
        updateMarker(Number(selectlng), Number(selectlat));
    }

    function updateMarker(lng, lat) {
        if (marker == null) {
            marker = new AMap.Marker({
                position: [lng, lat],
                map: mapObj
            });
        } else {
            marker.setPosition([lng, lat]);
        }
        selectlng = lng;
        selectlat = lat;
        mapObj.setCenter(marker.getPosition());
        mapObj.setZoom(18);
    }
}
//#endregion

var map = new editMap({ search: $(".userAddress").data("search") });
