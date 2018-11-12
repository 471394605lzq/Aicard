
// #region 初始化
var $send = $("#from");
var from = {
    userid: $send.data("id"),
    name: $send.data("name"),
    nick: $send.data("nick"),
    avatar: $send.data("avatar"),
    sign: $send.data("sign"),
}
var maxNameLen = 10;
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
    //var newMsg;
    //获取所有聊天会话

    for (var j in newMsgList) {//遍历新消息
        //newMsg = newMsgList[j];

        //selSess = newMsg.getSession();
        //handlderMsg(newMsg, false);
        //currentMsgsArray = currentMsgsArray.map((item, index) => {
        //    if (!item.isSelfSend) {
        //        item.avatar = myAvatar
        //    } else {
        //        item.avatar = friendAvatar
        //    }
        //    return item;
        //})
        //var myMessages = that.setDatas(currentMsgsArray);
        //that.setData({
        //    myMessages: myMessages,

        //})
        //setTimeout(function () {
        //    if (that.data.is_chat) {
        //        that.pageScrollToBottom()
        //    }
        //}, 100)

    }
    //getUnread()
}
//获取消息未读数并更新到会话列表
function getUnread() {
    var sess = {};
    var sessMap = webim.MsgStore.sessMap();
    //if (that.data.contactList) {//这里判断是否存在会话列表，或者在会话列表的页面
    // 更新消息的未读数
    console.log(sessMap);
    for (var i in sessMap) {
        sess = sessMap[i];
        //var contactList = that.data.contactList.map((item, index) => {
        //    if (item.To_Account == sess.id()) {
        //        item.UnreadMsgCount = sess.unread()
        //    }
        //    return item;
        //})
        //先把未读数赋值
        //that.setData({
        //    contactList: contactList
        //})
        // 获取最新的会话消息（把最新的一条赋值到会话列表）
        webim.getRecentContactList({
            'Count': 10 //最近的会话数 ,最大为 100
        }, function (resp) {
            var MsgShow = resp.SessionItem.filter((item, index) => {
                if (item.To_Account == sess.id())
                    return item;
            })

            //var contactList = that.data.contactList.map((item, index) => {
            //    if (item.To_Account == sess.id()) {
            //        // 获取最新消息
            //        if (MsgShow[0].MsgShow == '[其他]') {
            //            MsgShow[0].MsgShow = '[房源信息]'
            //        }
            //        item.MsgShow = MsgShow[0].MsgShow

            //    }
            //    return item;
            //})

            that.setData({
                contactList: contactList
            })

        })

    }
    //}
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
sdkLogin(this, app, {
    success: function () {
        //获取最近的聊天列表

    },
    error: function () {
        console.log();
    }
});

function loadUserList(option) {
    if (!option) {
        option = {};
    }
    if (!option.count) {
        option.count = 100;
    }
    if (!option.success) {
        option.success = function () { }
    }
    webim.getRecentContactList({
        'Count': option.count //最近的会话数 ,最大为 100
    }, function (resp) {
        var data = [];
        var tempSess, tempSessMap = {}; //临时会话变量
        if (resp.SessionItem && resp.SessionItem.length > 0) {

            for (var i in resp.SessionItem) {
                var item = resp.SessionItem[i];
                var type = item.Type; //接口返回的会话类型
                var sessType, typeZh, sessionId, sessionNick = '',
                    sessionImage = '',
                    senderId = '',
                    senderNick = '';
                if (type == webim.RECENT_CONTACT_TYPE.C2C) { //私聊
                    typeZh = '私聊';
                    sessType = webim.SESSION_TYPE.C2C; //设置会话类型
                    sessionId = item.To_Account; //会话id，私聊时为好友ID或者系统账号（值为@TIM#SYSTEM，业务可以自己决定是否需要展示），注意：从To_Account获取,

                    if (sessionId == '@TIM#SYSTEM') { //先过滤系统消息，，
                        webim.Log.warn('过滤好友系统消息,sessionId=' + sessionId);
                        continue;
                    }
                    senderNick = sessionNick = item.C2cNick;
                    sessionImage = item.C2cImage;

                    //var key = sessType + "_" + sessionId;
                    //var c2cInfo = infoMap[key];
                    //if (c2cInfo && c2cInfo.name) { //从infoMap获取c2c昵称
                    //    sessionNick = c2cInfo.name; //会话昵称，私聊时为好友昵称，接口暂不支持返回，需要业务自己获取（前提是用户设置过自己的昵称，通过拉取好友资料接口（支持批量拉取）得到）
                    //} else { //没有找到或者没有设置过
                    //    sessionNick = sessionId; //会话昵称，如果昵称为空，默认将其设成会话id
                    //}
                    //if (c2cInfo && c2cInfo.image) { //从infoMap获取c2c头像
                    //    sessionImage = c2cInfo.image; //会话头像，私聊时为好友头像，接口暂不支持返回，需要业务自己获取（前提是用户设置过自己的昵称，通过拉取好友资料接口（支持批量拉取）得到）
                    //} else { //没有找到或者没有设置过
                    //    sessionImage = friendHeadUrl; //会话头像，如果为空，默认将其设置demo自带的头像
                    //}
                    //senderId = senderNick = ''; //私聊时，这些字段用不到，直接设置为空

                } else if (type == webim.RECENT_CONTACT_TYPE.GROUP) { //群聊
                    //typeZh = '群聊';
                    //sessType = webim.SESSION_TYPE.GROUP; //设置会话类型
                    //sessionId = item.ToAccount; //会话id，群聊时为群ID，注意：从ToAccount获取
                    //sessionNick = item.GroupNick; //会话昵称，群聊时，为群名称，接口一定会返回

                    //if (item.GroupImage) { //优先考虑接口返回的群头像
                    //    sessionImage = item.GroupImage; //会话头像，群聊时，群头像，如果业务设置过群头像（设置群头像请参考wiki文档-设置群资料接口），接口会返回
                    //} else { //接口没有返回或者没有设置过群头像，再从infoMap获取群头像
                    //    var key = sessType + "_" + sessionId;
                    //    var groupInfo = infoMap[key];
                    //    if (groupInfo && groupInfo.image) { //
                    //        sessionImage = groupInfo.image
                    //    } else { //不存在或者没有设置过，则使用默认头像
                    //        sessionImage = groupHeadUrl; //会话头像，如果没有设置过群头像，默认将其设置demo自带的头像
                    //    }
                    //}
                    //senderId = item.MsgGroupFrom_Account; //群消息的发送者id

                    //if (!senderId) { //发送者id为空
                    //    webim.Log.warn('群消息发送者id为空,senderId=' + senderId + ",groupid=" + sessionId);
                    //    continue;
                    //}
                    //if (senderId == '@TIM#SYSTEM') { //先过滤群系统消息，因为接口暂时区分不了是进群还是退群等提示消息，
                    //    webim.Log.warn('过滤群系统消息,senderId=' + senderId + ",groupid=" + sessionId);
                    //    continue;
                    //}

                    //senderNick = item.MsgGroupFromCardName; //优先考虑群成员名片
                    //if (!senderNick) { //如果没有设置群成员名片
                    //    senderNick = item.MsgGroupFromNickName; //再考虑接口是否返回了群成员昵称
                    //    if (!senderNick) { //如果接口没有返回昵称或者没有设置群昵称，从infoMap获取昵称
                    //        var key = webim.SESSION_TYPE.C2C + "_" + senderId;
                    //        var c2cInfo = infoMap[key];
                    //        if (c2cInfo && c2cInfo.name) {
                    //            senderNick = c2cInfo.name; //发送者群昵称
                    //        } else {
                    //            sessionNick = senderId; //如果昵称为空，默认将其设成发送者id
                    //        }
                    //    }
                    //}

                } else {
                    typeZh = '未知类型';
                    sessionId = item.ToAccount; //
                }

                if (!sessionId) { //会话id为空
                    webim.Log.warn('会话id为空,sessionId=' + sessionId);
                    continue;
                }

                if (sessionId == '@TLS#NOT_FOUND') { //会话id不存在，可能是已经被删除了
                    webim.Log.warn('会话id不存在,sessionId=' + sessionId);
                    continue;
                }

                if (sessionNick.length > maxNameLen) { //帐号或昵称过长，截取一部分，出于demo需要，业务可以自己决定
                    sessionNick = sessionNick.substr(0, maxNameLen) + "...";
                }

                tempSess = tempSessMap[sessType + "_" + sessionId];
                if (!tempSess) { //先判断是否存在（用于去重），不存在增加一个
                    tempSessMap[sessType + "_" + sessionId] = true;
                    data.push({
                        SessionType: sessType, //会话类型
                        SessionTypeZh: typeZh, //会话类型中文
                        SessionId: webim.Tool.formatText2Html(sessionId), //会话id
                        SessionNick: webim.Tool.formatText2Html(sessionNick), //会话昵称
                        SessionImage: sessionImage, //会话头像
                        C2cAccount: webim.Tool.formatText2Html(senderId), //发送者id
                        C2cNick: webim.Tool.formatText2Html(senderNick), //发送者昵称
                        UnreadMsgCount: item.UnreadMsgCount, //未读消息数
                        MsgSeq: item.MsgSeq, //消息seq
                        MsgRandom: item.MsgRandom, //消息随机数
                        MsgTimeStamp: webim.Tool.formatTimeStamp(item.MsgTimeStamp), //消息时间戳
                        MsgShow: item.MsgShow //消息内容
                    });
                }
            }
        }
        //$('#get_recent_contact_list_table').bootstrapTable('load', data);
        //$('#get_recent_contact_list_dialog').modal('show');

        //var MsgShow = resp.SessionItem.filter((item, index) => {
        //    if (item.To_Account == sess.id())
        //        return item;
        //})

        //var contactList = that.data.contactList.map((item, index) => {
        //    if (item.To_Account == sess.id()) {
        //        // 获取最新消息
        //        if (MsgShow[0].MsgShow == '[其他]') {
        //            MsgShow[0].MsgShow = '[房源信息]'
        //        }
        //        item.MsgShow = MsgShow[0].MsgShow

        //    }
        //    return item;
        //})

        //that.setData({
        //    contactList: contactList
        //})
        option.success(data);
        initList(data);
    })
}

function initList(data) {
    var $list = $("#userList");
    $list.children().not("#temp").remove();
    $.each(data, function (i, n) {
        var $item = $list.find("#temp").clone().removeAttr("id").removeClass("hidden");
        $item.find(".userList-item-avatar").attr("src", n.SessionImage);
        $item.find(".userList-item-nick").html(decodeURI(n.SessionNick));
        $item.find(".userList-item-count").text(n.UnreadMsgCount);
        $item.find(".userList-item-lastMsg").text(n.MsgShow);
        $list.append($item);
    });

}
// #endregion




