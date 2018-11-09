
////sdk登录

function webimLogin() {
    webim.login(
        loginInfo, listeners, options,
        function(resp) {
            loginInfo.identifierNick = resp.identifierNick; //设置当前用户昵称
            loginInfo.headurl = resp.headurl; //设置当前用户头像
            //initDemoApp();
            // webim.getPendencyGroup({
            //         StartTime: 0,
            //         Limit: 10
            //     },
            //     function() {

            //     })
            webim.syncGroupMsgs({},function(data){
                console.debug( data );
            },function(data){
                console.debug( data );
            });
        },
        function(err) {
            alert(err.ErrorInfo);
        }
    );
}
