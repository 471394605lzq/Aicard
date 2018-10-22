$("#chkAll").click(function () {
    var checked = this.checked;
    $.each($(".chk"), function (i, n) {
        n.checked = checked;
    });
});

$(".chk").change(function (e) {
    var id = $(this).data("id");
});
$("#btnSubmit").click(function () {
    var a = new Array();
    $.each($(".chk:checked"), function (i, n) {
        var item = $(n).data("item");
        a.push(item);
    });
    var enterpriseid = $("#enterpriseid").data("enterpriseid");
    alert(enterpriseid);
    $.ajax({
        type: "POST",
        url: comm.action("CogradientWXUserInfo", "EnterpriseManage"),
        data: { listu: JSON.stringify(a), enterpriseid: enterpriseid },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                var rs = data.Result;

            }
        }
    });
});