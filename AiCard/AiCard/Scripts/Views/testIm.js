
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
    for (var i in sessMap) {
        sess = sessMap[i];
        if (to.name != sess.id()) {//更新其他聊天对象的未读消息数
            updateSessDiv(sess.type(), sess.id(), sess.unread());
        }
    }
}

var app = {
    data: {
        Config: {
            accountMode: 0,
            accountType: 29887,
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
sdkLogin(this, app, to.name, {
    success: function () {
        getC2CHistoryMsgs(to.name, {
            adding: function (msg) {
                var user = msg.isSend ? from : to;
                selSess = msg.getSession();
                var onemsg = addMsg(msg, user);
                $message.append(onemsg);
            },
            complete: function () {
                $message.scrollTop($message.height());
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

var msgKey = '';
var lastMsgTime = 0;
//获取最新的 C2C 历史消息,用于切换好友聊天，重新拉取好友的聊天消息
function getC2CHistoryMsgs(toUserID, msgKey, lastMsgTime, callback) {
    if (!callback) {
        callback = {};
    }
    if (!callback.complete) {
        callback.complete = function () { };
    }
    if (!callback.error) {
        callback.error = function () { };
    }
    if (!callback.adding) {
        callback.adding = function () { };
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
    msgKey = '';
    var reqMsgCount = 5;
    var options = {
        'Peer_Account': toUserID, //好友帐号
        'MaxCnt': reqMsgCount, //拉取消息条数
        'LastMsgTime': lastMsgTime, //最近的消息时间，即从这个时间点向前拉取历史消息
        'MsgKey': msgKey
    };
    selSess = null;
    webim.MsgStore.delSessByTypeId(selType, toUserID);
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
            var msgList = resp.MsgList;
            for (var j in msgList) { //遍历新消息
                var msg = msgList[j];
                if (msg.getSession().id() == toUserID) { //为当前聊天对象的消息
                    callback.adding(msg);
                }
            }
            callback.complete(resp.MsgKey, resp.LastMsgTime);
        }
    )
}


// #endregion




