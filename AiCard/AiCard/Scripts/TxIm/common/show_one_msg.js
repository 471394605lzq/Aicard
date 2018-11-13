//聊天页面增加一条消息

function addMsg(msg, userInfo, prepend) {
    var isSelfSend, fromAccount, fromAccountNick, fromAccountImage, sessType, subType;


    //获取会话类型，目前只支持群聊
    //webim.SESSION_TYPE.GROUP-群聊，
    //webim.SESSION_TYPE.C2C-私聊，
    sessType = msg.getSession().type();

    isSelfSend = msg.getIsSend(); //消息是否为自己发的

    fromAccount = msg.getFromAccount();
    if (!fromAccount) {
        return;
    }
    fromAccountNick = userInfo.nick;
    fromAccountImage = userInfo.avatar;
    //if (isSelfSend) { //如果是自己发的消息
    //    fromAccountNick = userInfo.fromAccountNick;
    //    if (userInfo.nickname) {
    //        fromAccountNick = userInfo.nickname;
    //    } else {
    //        fromAccountNick = fromAccount;
    //    }
    //    //获取头像
    //    if (userInfo.agent_pic) {
    //        fromAccountImage = userInfo.agent_pic;
    //    } else {
    //        fromAccountImage = friendHeadUrl;
    //    }
    //} else { //如果别人发的消息
    //    var key = webim.SESSION_TYPE.C2C + "_" + fromAccount;
    //    var info = infoMap[key];
    //    if (info && info.name) {
    //        fromAccountNick = info.name;
    //    } else if (msg.getFromAccountNick()) {
    //        fromAccountNick = msg.getFromAccountNick();
    //    } else {
    //        fromAccountNick = fromAccount;
    //    }
    //    //获取头像
    //    if (info && info.image) {
    //        fromAccountImage = info.image;
    //    } else if (msg.fromAccountHeadurl) {
    //        fromAccountImage = msg.fromAccountHeadurl;
    //    } else {
    //        fromAccountImage = friendHeadUrl;
    //    }
    //}
    // #region 根据自己的项目去处理 



    // #endregion
    var onemsg = document.createElement("div");
    if (msg.sending) {
        onemsg.id = "id_" + msg.random;
        //发送中
        var spinner = document.createElement("div");
        spinner.className = "spinner";
        spinner.innerHTML = '<div class="bounce1"></div> <div class="bounce2"></div> <div class="bounce3"></div>';
        onemsg.appendChild(spinner);
    } else {
        $("#id_" + msg.random).remove();
    }

    onemsg.className = "onemsg";
    var msghead = document.createElement("p");
    var msgbody = document.createElement("p");
    var msgPre = document.createElement("pre");
    msghead.className = "msghead";
    msgbody.className = "msgbody";


    //如果是发给自己的消息
    if (!isSelfSend)
        msghead.style.color = "blue";
    //昵称  消息时间
    msghead.innerHTML = "<img class='headurlClass' src='" + fromAccountImage + "'>" + "&nbsp;&nbsp;" + webim.Tool.formatText2Html(fromAccountNick + "&nbsp;&nbsp;" + webim.Tool.formatTimeStamp(msg.getTime()));


    //解析消息

    //获取消息子类型
    //会话类型为群聊时，子类型为：webim.GROUP_MSG_SUB_TYPE
    //会话类型为私聊时，子类型为：webim.C2C_MSG_SUB_TYPE
    subType = msg.getSubType();
    switch (subType) {
        case webim.GROUP_MSG_SUB_TYPE.COMMON: //群普通消息
            msgPre.innerHTML = convertMsgtoHtml(msg);
            break;
        case webim.GROUP_MSG_SUB_TYPE.REDPACKET: //群红包消息
            msgPre.innerHTML = "[群红包消息]" + convertMsgtoHtml(msg);
            break;
        case webim.GROUP_MSG_SUB_TYPE.LOVEMSG: //群点赞消息
            //业务自己可以增加逻辑，比如展示点赞动画效果
            msgPre.innerHTML = "[群点赞消息]" + convertMsgtoHtml(msg);
            //展示点赞动画
            //showLoveMsgAnimation();
            break;
        case webim.GROUP_MSG_SUB_TYPE.TIP: //群提示消息
            msgPre.innerHTML = "[群提示消息]" + convertMsgtoHtml(msg);
            break;
    }

    msgbody.appendChild(msgPre);

    onemsg.appendChild(msghead);
    onemsg.appendChild(msgbody);

    return onemsg;

}
//把消息转换成Html

function convertMsgtoHtml(msg) {
    var html = "",
        elems, elem, type, content;
    elems = msg.getElems(); //获取消息包含的元素数组
    var count = elems.length;
    for (var i = 0; i < count; i++) {
        elem = elems[i];
        type = elem.getType(); //获取元素类型
        content = elem.getContent(); //获取元素对象
        switch (type) {
            case webim.MSG_ELEMENT_TYPE.TEXT:
                var eleHtml = convertTextMsgToHtml(content);
                //转义，防XSS
                html += webim.Tool.formatText2Html(eleHtml);
                break;
            case webim.MSG_ELEMENT_TYPE.FACE:
                html += convertFaceMsgToHtml(content);
                break;
            case webim.MSG_ELEMENT_TYPE.IMAGE:
                if (i <= count - 2) {
                    var customMsgElem = elems[i + 1]; //获取保存图片名称的自定义消息elem
                    var imgName = customMsgElem.getContent().getData(); //业务可以自定义保存字段，demo中采用data字段保存图片文件名
                    html += convertImageMsgToHtml(content, imgName);
                    i++; //下标向后移一位
                } else {
                    html += convertImageMsgToHtml(content);
                }
                break;
            case webim.MSG_ELEMENT_TYPE.SOUND:
                html += convertSoundMsgToHtml(content);
                break;
            case webim.MSG_ELEMENT_TYPE.FILE:
                html += convertFileMsgToHtml(content);
                break;
            case webim.MSG_ELEMENT_TYPE.LOCATION:
                html += convertLocationMsgToHtml(content);
                break;
            case webim.MSG_ELEMENT_TYPE.CUSTOM:
                var eleHtml = convertCustomMsgToHtml(content);
                //转义，防XSS
                html += webim.Tool.formatText2Html(eleHtml);
                break;
            case webim.MSG_ELEMENT_TYPE.GROUP_TIP:
                var eleHtml = convertGroupTipMsgToHtml(content);
                //转义，防XSS
                html += webim.Tool.formatText2Html(eleHtml);
                break;
            default:
                webim.Log.error('未知消息元素类型: elemType=' + type);
                break;
        }
    }
    return html;
}

//解析文本消息元素

function convertTextMsgToHtml(content) {
    return content.getText();
}
//解析表情消息元素

function convertFaceMsgToHtml(content) {
    var faceUrl = null;
    var data = content.getData();
    var index = webim.EmotionDataIndexs[data];

    var emotion = webim.Emotions[index];
    if (emotion && emotion[1]) {
        faceUrl = emotion[1];
    }
    if (faceUrl) {
        return "<img src='" + faceUrl + "'/>";
    } else {
        return data;
    }
}
//解析图片消息元素

function convertImageMsgToHtml(content, imageName) {
    var smallImage = content.getImage(webim.IMAGE_TYPE.SMALL); //小图
    var bigImage = content.getImage(webim.IMAGE_TYPE.LARGE); //大图
    var oriImage = content.getImage(webim.IMAGE_TYPE.ORIGIN); //原图
    if (!bigImage) {
        bigImage = smallImage;
    }
    if (!oriImage) {
        oriImage = smallImage;
    }
    return "<img name='" + imageName + "' src='" + smallImage.getUrl() + "#" + bigImage.getUrl() + "#" + oriImage.getUrl() + "' style='CURSOR: hand' id='" + content.getImageId() + "' bigImgUrl='" + bigImage.getUrl() + "' onclick='imageClick(this)' />";
}
//解析语音消息元素

function convertSoundMsgToHtml(content) {
    var second = content.getSecond(); //获取语音时长
    var downUrl = content.getDownUrl();
    if (webim.BROWSER_INFO.type == 'ie' && parseInt(webim.BROWSER_INFO.ver) <= 8) {
        return '[这是一条语音消息]demo暂不支持ie8(含)以下浏览器播放语音,语音URL:' + downUrl;
    }
    return '<audio id="uuid_' + content.uuid + '" src="' + downUrl + '" controls="controls" onplay="onChangePlayAudio(this)" preload="none"></audio>';
}

function convertCustomMsgToHtml(content) {
    var data = content.getData(); //自定义数据
    var desc = content.getDesc(); //描述信息
    var ext = content.getExt(); //扩展信息
    return "data=" + data + ", desc=" + desc + ", ext=" + ext;
}
