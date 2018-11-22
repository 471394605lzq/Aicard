using Microsoft.VisualStudio.TestTools.UnitTesting;
using AiCard.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiCard.Bll;
using System.Web.Mvc;
using AiCard.Models.Vip;
using Newtonsoft.Json;

namespace AiCard.Controllers.Tests
{
    [TestClass()]
    public class VIPControllerTests
    {
        [TestMethod()]
        public void UpGradeVIPTest()
        {
            VIPController vip = new VIPController();
            //ActionResult result = vip.UpGradeVIP("aa", "011c7a5f-c201-4906-ac4c-6a0ee5372c7a");

            //OrderBLL bll = new OrderBLL();
            //string s = bll.CreateOrderCode("291afe55-6091-49ba-842b-de4b4aeffce7");
            //int leng = s.Length;
            //ActionResult obj = vip.Refund("2018112011422870401f1704d38996", "01f1704d-fd1f-409d-a3d1-eb23f2fd64");

            //ActionResult result = vip.CreateByWeChatPhone("f9df854b-24d5-4d09-bd9e-cf4147cec35e", "", "15521116670", "", "111111");

            ReqVipCardList req = new ReqVipCardList() {
                filter= "1562356",
                Page=1,
                PageSize=2
            };
            ActionResult result = vip.Index(JsonConvert.SerializeObject(req));
        }
    }
}