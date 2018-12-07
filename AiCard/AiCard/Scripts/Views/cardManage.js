$("#sendcode").click(function () {
    var a = new Array();
    $.each($(".chk:checked"), function (i, n) {
        var item = $(n).data("item");
        if (!item.ishave) {
            a.push(item);
        }
    });
    if (a.length > 0) {
        var enterpriseid = $("#enterpriseid").data("enterpriseid");
        $.ajax({
            type: "POST",
            url: comm.action("CogradientWXUserInfo", "EnterpriseManage"),
            data: { listu: JSON.stringify(a), enterpriseid: enterpriseid },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    var rs = data.Result;
                    console.log(rs);
                    comm.alter(1, "同步成功！")
                    window.location.reload();
                }
                else if (data.State == "Fail") {
                    comm.alter(0, "同步失败！" + data.message);
                }
                else {
                    comm.alter(2, "系统异常！" + data.message);
                }
            }
        });
    }
    else {
        alert("请选择要同步的用户信息");
    }
});