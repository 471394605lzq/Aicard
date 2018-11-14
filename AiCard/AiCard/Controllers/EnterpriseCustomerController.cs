using AiCard.Common.Enums;
using AiCard.DAL.Models;
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
        public ActionResult GetEnterpriseCustomerList(int enterpriseID, string filter, int page = 1, int pageSize = 20)
        {
            var query = from ec in db.EnterpriseCustomers
                        from e in db.Enterprises
                        from us in db.Users
                        where e.ID == enterpriseID
                            && ec.EnterpriseID == e.ID
                            && ec.UserID == us.Id
                        select new
                        {
                            ID = ec.ID,
                            Name = ec.RealName,
                            Avatar = us.Avatar
                        };
            //根据搜索框字符匹配
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter));
            }
            var paged = query.OrderBy(s => s.ID).ToPagedList(page, pageSize);
            return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据标签查询客户信息列表
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="tabsid"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetEnterpriseCustomerListByTabs(int? tabsID, int page = 1, int pageSize = 20)
        {
            var query = from ct in db.EnterpriseCustomerTabs
                        from cus in db.EnterpriseCustomers
                        from us in db.Users
                        where ct.CustomerID == cus.ID && us.Id == cus.UserID
                        select new
                        {
                            ID = cus.ID,
                            Name = cus.RealName,
                            Avatar = us.Avatar,
                            TabsID = ct.ID
                        };

            //根据标签匹配
            if (tabsID != null)
            {
                query = query.Where(s => s.TabsID == tabsID);
            }
            var paged = query.OrderBy(s => s.ID).ToPagedList(page, pageSize);
            return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询标签客户总数
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetEnterpriseCustomerTabCountList(string ownerID, int page = 1, int pageSize = 20)
        {
            var query = (from ct in db.EnterpriseCustomerTabs
                         join c in db.EnterpriseCustomers on ct.CustomerID equals c.ID into ec
                         where ct.OwnerID == ownerID
                         select new
                         {
                             ID = ct.ID,
                             Name = ct.Name,
                             Count = ec.Count(),
                             Users = ec.Select(s => s.RealName).Take(10)
                         });
            var paged = query.OrderBy(s => s.ID).ToPagedList(page, pageSize);
            return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置客户自定义标签
        /// </summary>
        /// <param name="customerID">标签所属客户id</param>
        /// <param name="userID">客户对应用户表id</param>
        /// <param name="tabsname">客户自定义标签名称</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult SetCustomerTabs(int customerID, string ownerID, string tabsName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tabsName))
                {
                    return Json(Comm.ToJsonResult("Error", "标签内容不能为空"), JsonRequestBehavior.AllowGet);
                }
                if (!db.EnterpriseCustomers.Any(s => s.ID == customerID))
                {
                    return Json(Comm.ToJsonResult("CustomerNoFound", "客户不存在"));
                }
                if (db.EnterpriseCustomerTabs.Any(s => s.CustomerID == customerID && s.Name == tabsName))
                {
                    return Json(Comm.ToJsonResult("HadAdd", $"{tabsName}已经存在"));
                }
                var randam = Comm.Random.Next(3);
                var tab = new EnterpriseCustomerTab { CreateDateTime = DateTime.Now, Name = tabsName, Style = (CardTabStyle)randam, CustomerID = customerID, OwnerID = ownerID };
                db.EnterpriseCustomerTabs.Add(tab);
                db.SaveChanges();
                var returndata = new
                {
                    ID = tab.ID,
                    Style = tab.Style
                };
                return Json(Comm.ToJsonResult("Success", "成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Ai雷达 客户详情
        /// </summary>
        /// <param name="custID"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustInfo(int custID,string userID)
        {
            try
            {
                var query = (from c in db.EnterpriseCustomers
                             from u in db.Users
                             join cut in db.EnterpriseCustomerTabs.Where(s=>s.OwnerID== userID) on c.ID equals cut.CustomerID  into cuta
                             where c.ID == custID
                             select  new {
                                 Name=c.RealName,
                                 Avater=u.Avatar,
                                 CustTabs = cuta.Select(s=>s.Name).Take(3)
                             }).FirstOrDefault();
                return Json(Comm.ToJsonResult("Success", "成功", query), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Ai雷达 编辑客户资料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditCustInfo(EnterpriseCustomer model)
        {
            try
            {
                var t = db.EnterpriseCustomers.FirstOrDefault(s => s.ID == model.ID);
                if (t == null)
                {
                    return Json(Comm.ToJsonResult("Error", "客户不存在"), JsonRequestBehavior.AllowGet);
                }
                if (model.RealName != null)
                {
                    t.RealName = model.RealName;
                }
                if (model.Email != null)
                {
                    if (string.Empty != model.Email.Trim() && !Reg.IsEmail(model.Email))
                    {
                        return Json(Comm.ToJsonResult("Error", "邮箱格式不正确"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        t.Email = model.Email;
                    }
                }
                if (model.Mobile != null)
                {
                    if (string.Empty != model.Mobile.Trim() && !Reg.IsMobile(model.Mobile))
                    {
                        return Json(Comm.ToJsonResult("Error", "手机号格式不正确"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        t.Mobile = model.Mobile;
                    }
                }
               
                if (model.Position != null)
                {
                    t.Position = model.Position;
                }

                t.Gender = model.Gender;
                if (model.Birthday!=null)
                {
                    t.Birthday = model.Birthday;
                }

                if (model.Company != null)
                {
                    t.Company = model.Company;
                }
                if (model.Address != null)
                {
                    t.Address = model.Address;
                }
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}