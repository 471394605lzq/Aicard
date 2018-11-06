var $list = $(".homePageModularsManageIndex-list ul");
$list.sortable({
    handle: ".btnSort",
    cancel: "[data-type=3]",
    stop: function (event, ui) {
        var ids = new Array();
        $.each($(".homePageModularsManageIndex-list li"), function (i, n) {
            ids.push($(n).data("id"));
        });
        console.log(ids.join(","));
    }
});

$("#btnSaveSort").click(function () {
    var ids = new Array();
    $.each($list.find("li"), function (i, n) {
        ids.push($(n).data("id"));
    });
    $.ajax({
        type: "POST",
        url: comm.action("Sort", "HomePageModularsManage"),
        data: { ids: ids.join(",") },
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                comm.alter(1, "修改成功");
            }
            else {
                comm.alter(0, data.Message);
            }
        }
    });
});

$(".btnDelete").click(function (e) {
    var $item = $(this).parent();
    var id = $item.data("id");
    var title = $item.find(">div").text();
    if (confirm("是否要删除“" + title + "”模块")) {
        $.ajax({
            type: "POST",
            url: comm.action("Delete", "HomePageModularsManage"),
            data: { id: id },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    comm.alter(1, "删除成功");
                    $item.remove();
                }
                else {
                    comm.alter(0, data.Message);
                }
            }
        });
    }

});