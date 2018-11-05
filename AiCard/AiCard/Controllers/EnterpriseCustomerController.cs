using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class EnterpriseCustomerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="enterpriseid">企业id</param>
        /// <param name="filter">搜索关键字</param>
        /// <param name="tabsid">标签id</param>
        /// <param name="page">分页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>商品列表json集合</returns>
        [AllowCrossSiteJson]
        public ActionResult GetEnterpriseCustomerList(int enterpriseid, string filter, int? tabsid, int page = 1, int pageSize = 20)
        {
            var query = from ec in db.EnterpriseCustomers
                        from e in db.Enterprises
                        from us in db.Users
                        where e.ID == enterpriseid
                            && ec.EnterpriseID == e.ID
                            &&ec.UserID==us.Id
                        select new
                        {
                            ID = ec.ID,
                            Name = ec.RealName,
                            Avatar=us.Avatar
                        };
            //根据搜索框字符匹配
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter));
            }
            //根据标签匹配
            //if (tabsid != null)
            //{
            //    query = query.Where(s => s.KindID == tabsid);
            //}
            var paged = query.OrderBy(s => s.ID).ToPagedList(page, pageSize);
            return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 设置客户自定义标签
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <param name="tabsname"></param>
        /// <returns></returns>

        public ActionResult SetCustomerTabs(int enterpriseid, int userid, string tabsname)
        {
            return Json(Comm.ToJsonResult("Success", "添加成功"), JsonRequestBehavior.AllowGet);
        }


        // GET: EnterpriseCustomer
        public ActionResult Index()
        {
            return View();
        }
    }
}