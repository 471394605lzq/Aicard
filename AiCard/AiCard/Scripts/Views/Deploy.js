//修改微信id按钮
$("#wecharworkidi").click(function () {
    showtemp("show", "hide");
});
$("#wecharworkidiqx").click(function () {
    showtemp("hide", "show");
});
//修改微信secret按钮
$("#wecharworksecreti").click(function () {
    showtemp2("show", "hide");
});
//修改微信secret按钮
$("#wecharworksecretiqx").click(function () {
    showtemp2("hide", "show");
});
function showtemp(class1,class2) {
    //隐藏lable标签
    $("#labwechatworkid").removeClass(class1);
    $("#labwechatworkid").addClass(class2);
    //显示input标签
    $("#WeChatWorkCorpid").removeClass(class2);
    $("#WeChatWorkCorpid").addClass(class1);
    //隐藏编辑按钮
    $("#wecharworkidi").removeClass(class1);
    $("#wecharworkidi").addClass(class2);
    //显示取消按钮
    $("#wecharworkidiqx").removeClass(class2);
    $("#wecharworkidiqx").addClass(class1);
}
function showtemp2(class1, class2) {
    //隐藏lable标签
    $("#labwechatworksecret").removeClass(class1);
    $("#labwechatworksecret").addClass(class2);
    //显示input标签
    $("#WeChatWorkSecret").removeClass(class2);
    $("#WeChatWorkSecret").addClass(class1);
    //隐藏编辑按钮
    $("#wecharworksecreti").removeClass(class1);
    $("#wecharworksecreti").addClass(class2);
    //显示取消按钮
    $("#wecharworksecretiqx").removeClass(class2);
    $("#wecharworksecretiqx").addClass(class1);
}