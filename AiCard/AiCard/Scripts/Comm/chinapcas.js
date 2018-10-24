var chinaPcas = function (p, c, a) {
    var $p = $(p);
    var $c = $(c);
    var $a = $(a);

    //选择省份事件
    $p.change(function () {
        var selectValue = $p.children('option:selected').val();
        $c.children("option").remove()
        $a.children("option").remove()
        $.ajax({
            type: "POST",
            url: comm.action("Getc", "ChinaPCAS"),
            data: { pname: selectValue },
            dataType: "json",
            success: function (data) {
                if (data.State == "Success") {
                    var rs = data.Result.data;
                    for (var i = 0; i < rs.length; i++) {
                        $c.append("<option value='" + rs[i] + "'>" + rs[i] + "</option>");
                    }
                    var cname = $c.children('option:selected').val();
                    setdistrict(selectValue, cname);
                }
            }
        });
    });
    //选择城市事件
    $c.change(function () {
        var pname = $p.children('option:selected').val();
        var cname = $c.children('option:selected').val();
        $a.children("option").remove()
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
                        $a.append("<option value='" + rs[i] + "'>" + rs[i] + "</option>");
                    }
                }
            }
        });
    }
}