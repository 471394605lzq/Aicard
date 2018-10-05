
$("[name=roleGroup]").click(function (e) {
    var $this = $(".selectRoleList li").has(this);
    $this.find(">ul>li").find(":checkbox").prop("checked", this.checked);
});

function getSelectedRole() {
    var names = new Array();
    $.each($("[name=role]:checked"), function (i, n) {
        names.push($(n).val());
    });
    return names;
}
var $selected = $("#SelectedRoles");
$("[name=roleGroup],[name=role]").click(function (e) {
    var $this = $(".selectRoleList li").has(this);
    $selected.val(getSelectedRole().join(","));
});

$(".dropdownType").click(function (e) {
    var $this = $(this);
    $this.parents(".selectType").find(".type").val($this.text());
});


$("#btnUpdate").click(function (e) {
    $.ajax({
        type: "POST",
        url: comm.action("UpdateRoles", "RoleManage"),
        dataType: "json",
        success: function (data) {
            if (data.State == "Success") {
                comm.alter(1, data.Message);
            }
        }
    });
});



$("#createUser").click(function (e) {
    $.ajax({
        beforeSend: function () {
            var image = $("#myModal_Image").find(".uploaderVal").val();
            image = image.replace("[", "").replace("]", "").replace(/"/g, "");
            $("#Image").val(image);
        },
    });
});

