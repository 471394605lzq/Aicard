$("#chkAll").click(function () {
    var checked = this.checked;
    $.each($(".chk"), function (i, n) {
        var item = $(n).data("item");
        if (!item.ishave) {
            n.checked = checked;
        }
    });
});



$("body").on("click", ".chk", function () {
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
        $.ajax({
            type: "POST",
            url: comm.action("Check", "ArticlesManage"),
            data: { listu: JSON.stringify(a)},
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    var rs = data.Result;
                    console.log(rs);
                    comm.alter(1, "审核成功");
                    setTimeout(function () {
                        //location = location;
                        window.location.reload();
                    }, 1000);
                }
                else if (data.State == "Fail") {
                    comm.alter(0, "审核失败" + data.message);
                }
                else {
                    comm.alter(0, "系统异常" + data.message);
                }
            }
        });
    }
    else {
        comm.alter(2, "请选择要审核的动态信息")
    }
});
