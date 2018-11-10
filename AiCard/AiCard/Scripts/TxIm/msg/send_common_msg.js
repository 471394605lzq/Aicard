//发送消息(文本或者表情)

function onSendMsg(to, selType, msg, option) {

    if (!option) {
        option.success = function () { };
        option.error = function () { };
    } else if (!option.success) {
        option.success = function () { };
    } else if (!option.error) {
        option.error = function () { };
    }
    //获取消息内容
    var msgtosend = msg;
    var msgLen = webim.Tool.getStrBytes(msg);
    // 创建会话对象
    var selSess = new webim.Session(selType, to.name, to.nick, to.avatar, Math.round(new Date().getTime() / 1000));
    var isSend = true;//是否为自己发送
    var seq = -1;//消息序列，-1表示sdk自动生成，用于去重
    var random = Math.round(Math.random() * 4294967296);//消息随机数，用于去重
    var msgTime = Math.round(new Date().getTime() / 1000);//消息时间戳
    var subType = webim.C2C_MSG_SUB_TYPE.COMMON;//消息子类型c2c消息时，参考c2c消息子类型对象：webim.C2C_MSG_SUB_TYPE 
    //loginInfo.identifier消息发送者账号,loginInfo.identifierNick消息发送者昵称
    var msg = new webim.Msg(selSess, isSend, seq, random, msgTime, to.name, subType, to.nick);
    //解析文本和表情
    var expr = /\[[^[\]]{1,3}\]/mg;
    var emotions = msgtosend.match(expr);
    if (!emotions || emotions.length < 1) {
        var text_obj = new webim.Msg.Elem.Text(msgtosend);
        msg.addText(text_obj);
    } else {//有表情

    }

    webim.sendMsg(msg, function (resp) {
        if (selType == webim.SESSION_TYPE.C2C) {//私聊时，在聊天窗口手动添加一条发的消息
            option.success(msg);
        }


    }, function (err) {
        //重发
        showReSend(msg);
        option.error(err);
        //webim.Log.error("发消息失败:" + err.ErrorInfo);
    });
}




