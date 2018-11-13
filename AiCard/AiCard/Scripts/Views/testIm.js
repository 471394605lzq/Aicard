
// #region 初始化
var $send = $("#from");
var $to = $("#to");
var selType = webim.SESSION_TYPE.C2C;
var from = {
    userid: $send.data("id"),
    name: $send.data("name"),
    nick: $send.data("nick"),
    avatar: $send.data("avatar"),
    sign: $send.data("sign"),
}
var to = {
    userid: $to.data("id"),
    name: $to.data("name"),
    nick: $to.data("nick"),
    avatar: $to.data("avatar"),
    sign: null,
}
var $content = $("#txtContent");
var $message = $(".msg");
// #endregion

// #region 调用代码
//1v1单聊的话，一般只需要 'onConnNotify' 和 'onMsgNotify'就行了。
//监听连接状态回调变化事件
var onConnNotify = function (resp) {
    switch (resp.ErrorCode) {
        case webim.CONNECTION_STATUS.ON:
            //webim.Log.warn('连接状态正常...');
            break;
        case webim.CONNECTION_STATUS.OFF:
            webim.Log.warn('连接已断开，无法收到新消息，请检查下你的网络是否正常');
            break;
        default:
            webim.Log.error('未知连接状态,status=' + resp.ErrorCode);
            break;
    }
};
///监听聊天
var onMsgNotify = function (newMsgList) {
    //console.warn(newMsgList);
    var sess, newMsg;
    //获取所有聊天会话
    var sessMap = webim.MsgStore.sessMap();
    for (var j in newMsgList) {//遍历新消息
        newMsg = newMsgList[j];
        if (newMsg.getSession().id() == to.name) {//为当前聊天对象的消息
            selSess = newMsg.getSession();
            //在聊天窗体中新增一条消息
            //console.warn(newMsg);
            var onemsg = addMsg(newMsg, to);
            $message.append(onemsg);
        }
    }
    //消息已读上报，以及设置会话自动已读标记
    webim.setAutoRead(selSess, true, true);
    //for (var i in sessMap) {
    //    sess = sessMap[i];
    //    if (to.name != sess.id()) {//更新其他聊天对象的未读消息数
    //        updateSessDiv(sess.type(), sess.id(), sess.unread());
    //    }
    //}
}

var app = {
    data: {
        Config: {
            accountMode: 0,
            accountType: 36862,
            sdkappid: 1400157072,
        },
        userInfo: from,
        //监听事件（1V1监听这两个事件就够了）
        listeners: {
            "onConnNotify": onConnNotify, //监听连接状态回调变化事件,必填
            "onMsgNotify": onMsgNotify
        },
    }
};
sdkLogin(this, app, {
    success: function () {
        //加载第一页的聊天记录
        getC2CHistoryMsgs({
            toUserID: to.name,
            complete: function (res) {
                //反转数组
                var msgList = res.MsgList.reverse();

                for (var j in msgList) { //遍历新消息
                    var msg = msgList[j];
                    if (msg.getSession().id() == to.name) { //为当前聊天对象的消息
                        var user = msg.isSend ? from : to;
                        var onemsg = addMsg(msg, user);

                        if ($message.children().length > 0) {
                            $($message.children()[0]).before(onemsg);
                        }
                        else {
                            $message.append(onemsg);
                        }
                    }
                }

                $message.scrollTop($message.prop("scrollHeight"));
                //如果还有历史记录
                if (!res.IsComplete) {
                    var lastMsgTime = res.LastMsgTime;
                    var msgKey = res.MsgKey;

                    //滚动到顶部就加载历史记录
                    $message.scroll(function () {
                        //滚动到顶部就加载历史记录
                        if ($message.scrollTop() == 0) {
                            var height = 0;
                            getC2CHistoryMsgs({
                                toUserID: to.name,
                                lastMsgTime: lastMsgTime,
                                msgKey: msgKey,
                                complete: function (res) {
                                    var $top = $($message.children()[0]);
                                    //反转数组
                                    var msgList = res.MsgList.reverse();
                                    for (var j in msgList) { //遍历新消息
                                        var msg = msgList[j];
                                        if (msg.getSession().id() == to.name) { //为当前聊天对象的消息
                                            var user = msg.isSend ? from : to;
                                            var onemsg = addMsg(msg, user);
                                            $top.before(onemsg);
                                            height += $(onemsg).height();
                                            //console.log($top.offset().top);
                                        }
                                    }
                                    //debugger;
                                    //console.log($message.height()+$message.sr );
                                    //$message.scrollTop($top.offset().top);
                                    //更新lastMsgTime和msgKey
                                    $message.scrollTop(height);
                                    lastMsgTime = res.LastMsgTime;
                                    msgKey = res.MsgKey;
                                    if (res.IsComplete) {
                                        //如果已经加载完就注销滚动事件
                                        $message.unbind("scroll");
                                    }
                                }
                            })
                        }
                    });
                }
            }
        });
        //注册发送按钮
        $("#btnSend").click(function (e) {
            //发消息
            onSendMsg(to, selType, $content.val(), {
                success: function (msg) {
                    var onemsg = addMsg(msg, app.data.userInfo);
                    $message.append(onemsg);
                    $content.val("");
                    $message.scrollTop($message.prop("scrollHeight"));
                },
                error: function (error) {
                    console.log(error);
                }
            })
        });
    },
    error: function () {
        console.log();
    }
});
function setHistoryMegs(msgList) {

}
//获取最新的 C2C 历史消息,用于切换好友聊天，重新拉取好友的聊天消息
//option
//--toUserID 收消息的对象的用户名
//--complete({MsgKey,LastMsgTime,IsComplete})加载完成事件
//--error(err) 错误事件
//--reqMsgCount[int] 加载数量
//--lastMsgTime[int] 最后加载的消息时间结点默认为0
//--msgKey[string] 最后加载的消息的KEY，默认为空
function getC2CHistoryMsgs(option) {
    if (!option.complete) {
        option.complete = function () { };
    }
    if (!option.error) {
        option.error = function () { };
    }
    if (!option.adding) {
        option.adding = function () { };
    }
    if (!option.toUserID) {
        throw "toUserID不能为空";
    }
    if (!option.reqMsgCount) {
        option.reqMsgCount = 5;
    }
    if (!option.lastMsgTime) {
        option.lastMsgTime = 0;
    }
    if (!option.msgKey) {
        option.msgKey = "";
    }
    currentMsgsArray = [];
    if (selType == webim.SESSION_TYPE.GROUP) {
        alert('当前的聊天类型为群聊天，不能进行拉取好友历史消息操作');
        return;
    }

    if (selType == webim.SESSION_TYPE.GROUP) {
        alert('当前的聊天类型为群聊天，不能进行拉取好友历史消息操作');
        return;
    }
    //第一次拉取好友历史消息时，必须传0
    //var msgKey = wx.getStorageSync('msgKey') || '';
    var reqMsgCount = 5;
    var options = {
        'Peer_Account': option.toUserID, //好友帐号
        'MaxCnt': option.reqMsgCount, //拉取消息条数
        'LastMsgTime': option.lastMsgTime, //最近的消息时间，即从这个时间点向前拉取历史消息
        'MsgKey': option.msgKey//消息的ID第一次取时候传空
    };
    //selSess = null;
    webim.MsgStore.delSessByTypeId(selType, option.toUserID);
    webim.getC2CHistoryMsgs(
        options,
        function (resp) {
            var complete = resp.Complete; //是否还有历史消息可以拉取，1-表示没有，0-表示有

            if (resp.MsgList.length == 0) {
                return
            }
            //拉取消息后，要将下一次拉取信息所需要的东西给存在缓存中
            //wx.setStorageSync('lastMsgTime', resp.LastMsgTime);
            //wx.setStorageSync('msgKey', resp.MsgKey);
            option.complete({
                MsgKey: resp.MsgKey,
                LastMsgTime: resp.LastMsgTime,
                IsComplete: resp.Complete == 1,
                MsgList: resp.MsgList
            });
        }
    )
}


// #endregion




