$("#chkAll").click(function () {
    var checked = this.checked;
    $.each($(".chk"), function (i, n) {
        var item = $(n).data("item");
        if (!item.ishave) {
            n.checked = checked;
        }
    });
});

$(".chk").change(function (e) {
    var id = $(this).data("id");
});
$("#btnSubmit").click(function () {
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
                    alert("同步成功！");
                    window.location.reload();
                }
                else if (data.State == "Fail") {
                    alert("同步失败！" + data.message);
                }
                else {
                    alert("系统异常！" + data.message);
                }
            }
        });
    }
    else {
        alert("请选择要同步的用户信息");
    }
});

var mychinaPcas = new chinaPcas("#Province", "#City", "#District");