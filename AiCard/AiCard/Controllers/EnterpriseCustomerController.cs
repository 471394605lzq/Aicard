using AiCard.Common;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public ActionResult GetEnterpriseCustomerList(string userID, string filter, int page = 1, int pageSize = 20)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("NoFound", "用户不存在"));
            }
            var query = from euc in db.EnterpriseUserCustomer
                        from ec in db.EnterpriseCustomers
                        from us in db.Users
                        where ec.ID == euc.CustomerID
                            && ec.UserID == us.Id && euc.OwnerID == userID
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
        /// 今日新增客户总数、未跟进客户数
        /// </summary>
        /// <param name="userID">名片所属用户ID</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustCount(string userID)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("NoFound", "用户不存在"));
            }
            SqlParameter[] parms = {
                new SqlParameter("@userid",SqlDbType.NVarChar),
                new SqlParameter("@statetype",SqlDbType.Int)
            };
            parms[0].Value = userID;
            parms[1].Value = Common.Enums.EnterpriseUserCustomerState.NoFllow;
            string sqlstr = string.Format(@"GetCustCount @userid,@statetype");
            List<GetCustCountModel> data = db.Database.SqlQuery<GetCustCountModel>(sqlstr, parms).ToList();
            return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据类型获取客户信息
        /// </summary>
        /// <param name="userID">名片所属用户ID</param>
        /// <param name="type">类型 0：今日新增客户 1：未跟进客户</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustInfoByType(string userID, int type)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("NoFound", "用户不存在"));
            }
            SqlParameter[] parms = {
                new SqlParameter("@userid",SqlDbType.NVarChar),
                new SqlParameter("@type",SqlDbType.Int),
                new SqlParameter("@statetype",SqlDbType.Int)
            };
            parms[0].Value = userID;
            parms[1].Value = type;
            parms[2].Value = Common.Enums.EnterpriseUserCustomerState.NoFllow;
            string sqlstr = string.Format(@"GetCustInfoByType @userid,@statetype,@statetype");
            List<CustInfoModel> data = db.Database.SqlQuery<CustInfoModel>(sqlstr, parms).ToList();
            return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
        }
        private class CustInfoModel
        {
            //id
            public int ID { get; set; }
            //姓名
            public string Name { get; set; }
            //头像
            public string Avatar { get; set; }
        }
        private class GetCustCountModel
        {
            //今日新增客户数
            public int newcustcount { get; set; }
            //未跟进客户数
            public int nofollwcount { get; set; }
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
        public ActionResult GetCustInfo(int custID, string userID)
        {
            try
            {
                var query = (from c in db.EnterpriseCustomers
                             from u in db.Users
                             join cut in db.EnterpriseCustomerTabs.Where(s => s.OwnerID == userID) on c.ID equals cut.CustomerID into cuta
                             join uscut in db.EnterpriseUserCustomer.Where(s=>s.OwnerID==userID) on c.ID equals uscut.CustomerID into uscuta
                             where c.ID == custID
                             select new
                             {
                                 Name = c.RealName,
                                 Avatar = u.Avatar,
                                 CustTabs = cuta.Select(s => s.Name),
                                 Position=c.Position,
                                 Email=c.Email,
                                 Mobile=c.Mobile,
                                 Gender=c.Gender,
                                 Birthday=c.Birthday,
                                 Company=c.Company,
                                 Address=c.Address,
                                 Remark=uscuta.Select(s=>s.Remark)
                             }).FirstOrDefault();
                var data = new
                {
                    Name = query.Name,
                    Avatar = query.Avatar,
                    CustTabs = query.CustTabs,
                    Position = query.Position,
                    Email = query.Email,
                    Mobile = query.Mobile,
                    Gender = query.Gender,
                    Birthday =Convert.ToDateTime(query.Birthday).ToString("yyyy-MM-dd"),
                    Company = query.Company,
                    Address = query.Address,
                    Remark = query.Remark
                };
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
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
                    if (string.Empty != model.Email.Trim() && !Common.Reg.IsEmail(model.Email))
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
                    if (string.Empty != model.Mobile.Trim() && !Common.Reg.IsMobile(model.Mobile))
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
                if (model.Birthday != null)
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

        /// <summary>
        /// 修改用户所属客户备注
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="custid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditCustRemark(string ownerid, int custid, string remark)
        {
            try
            {
                var t = db.EnterpriseUserCustomer.FirstOrDefault(s => s.CustomerID == custid && s.OwnerID == ownerid);
                if (t == null)
                {
                    return Json(Comm.ToJsonResult("Error", "客户不存在"), JsonRequestBehavior.AllowGet);
                }
                t.Remark = remark;
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 获取客户标签
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="custid"></param>
        /// <param name="enterpriseid"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustTabs(string userid, int custid, int enterpriseid)
        {
            try
            {
                //客户自定义标签
                var custtab = db.EnterpriseCustomerTabs.Where(s => s.OwnerID == userid && s.CustomerID == custid).ToList();
                //企业共用模板标签
                var comTab = (from g in db.CustomerTabGroups
                              join t in db.CustomerTabs on g.ID equals t.GroupID into gt
                              where g.EnterpriseID == enterpriseid
                              orderby g.Sort
                              select new
                              {
                                  g.Name,
                                  g.Sort,
                                  g.Style,
                                  Tabs = gt
                              }).ToList();

                var ects = db.EnterpriseCustomerTabs.Where(s => s.CustomerID == custid && s.OwnerID == userid);
                //添加到自定义标签中的模板标签
                var models = comTab.Select(group =>
                {
                    var child = group.Tabs.Select(tab => new
                    {
                        tab.Name,
                        tab.ID,
                        tab.Sort,
                        Style = group.Style,
                        Selected = ects.Any(z => z.Name == tab.Name)
                    }
                     );
                    return new
                    {
                        group.Name,
                        group.Sort,
                        group.Style,
                        Tabs = child
                    };
                });
                var returndata = new
                {
                    custtabs = custtab,
                    comtabs = models
                };
                return Json(Comm.ToJsonResult("Success", "成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 删除客户标签
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="tabID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult DeleteCustTabs(int tabID, string owerID, int custID)
        {
            try
            {
                var tab = db.EnterpriseCustomerTabs.FirstOrDefault(s => s.CustomerID == custID && s.ID == tabID && s.OwnerID == owerID);
                if (tab == null)
                {
                    return Json(Comm.ToJsonResult("TabNoFound", "标签不存在"));
                }
                db.EnterpriseCustomerTabs.Remove(tab);
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "删除成功"));
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message));
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