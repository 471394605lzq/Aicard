$.ajax({
    type: "POST",
    url: "http://www.dtoao.com/CardPersonal/GetCardInfo",
    data: { PCardID: 1 },
    dataType: "json",
    success: function (data) {
        var checkKey = new Array("Name", "Avatar", "Position", "Industry", "Video");
        for (var i = 0; i < checkKey.length; i++) {
            for (var key in data.Result) {
                if (key == checkKey[i] && (data.Result[key] == "" || data.Result[key] == null)) {
                    console.log(key + "未完成");
                    return;
                }
            }
        }
    }
});