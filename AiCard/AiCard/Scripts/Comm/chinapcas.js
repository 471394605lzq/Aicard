//选择省份事件
$("#Province").change(function () {
    var selectValue = $("#Province").children('option:selected').val();
    $("#City option").remove();
    $("#District option").remove();
    $.ajax({
        type: "POST",
        url: comm.action("Getc", "ChinaPCAS"),
        data: { pname: selectValue },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                var rs = data.Result.data;
                for (var i = 0; i < rs.length; i++) {
                    $("#City").append("<option value='" + rs[i] + "'>" + rs[i] + "</option>");
                }
                var cname = $("#City").children('option:selected').val();
                setdistrict(selectValue, cname);
            }
        }
    });
});
//选择城市事件
$("#City").change(function () {
    var pname = $("#Province").children('option:selected').val();
    var cname = $("#City").children('option:selected').val();
    $("#District option").remove();
    setdistrict(pname, cname);
});
//设置区域
function setdistrict(pname, cname) {
    $.ajax({
        type: "POST",
        url: comm.action("Geta", "ChinaPCAS"),
        data: { pname: pname, cname: cname },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                var rs = data.Result.data;
                for (var i = 0; i < rs.length; i++) {
                    $("#District").append("<option value='" + rs[i] + "'>" + rs[i] + "</option>");
                }
            }
        }
    });
}