
var enumAccessLogType = {
    CompanyIndex: "CompanyIndex",
    CompanyProductIndex: "CompanyProductIndex",
    CompanyCustomerPostIndex: "CompanyCustomerPostIndex",
    CompanyMap: "CompanyMap",
    CompanyPhone: "CompanyPhone",
    CompanyTicket: "CompanyTicket",
    CompanyWifi: "CompanyWifi",
    CompanyPostDetail: "CompanyPostDetail",
    CompanyPostShare: "CompanyPostShare",

    ThirdPartyTicketIndex: "ThirdPartyTicketIndex",
    ThirdPartyTicketDetail: "ThirdPartyTicketDetail",
    ThirdPartyTicketPlatform: "ThirdPartyTicketPlatform",
    ThirdPartyTicketType: "ThirdPartyTicketType",
    ThirdPartyTicketSort: "ThirdPartyTicketSort",
    ThirdPartyTicketSearch: "ThirdPartyTicketSearch",
    ThirdPartyTicketToIndex: "ThirdPartyTicketToIndex",
    ThirdPartyTicketToOrderTicket: "ThirdPartyTicketToOrderTicket",
};
var accessLog = function () {

    function _add(type, data, source) {
        if (type == undefined) {
            return
        }
        if (data == undefined) {
            data = null;
        }
        if (source == undefined) {
            source = null;
        }

        $.ajax({
            type: "POST",
            url: comm.action("Create", "AccessLog"),
            data: { type: type, data: data, source: source },
            dataType: "json",
            success: function (result) {

            }
        });
    }

    this.add = function (type, data, source) {
        _add(type, data, source);
    }
}

