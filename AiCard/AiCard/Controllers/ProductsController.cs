using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        /// <summary>
        /// 获取企业商品列表
        /// </summary>
        /// <param name="enterpriseid">企业id</param>
        /// <param name="filter">搜索关键字</param>
        /// <param name="kindname">分类名称</param>
        /// <param name="page">分页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>商品列表json集合</returns>
        [AllowCrossSiteJson]
        public ActionResult GetProductsList(int enterpriseid, string filter, int? kindid, int page = 1, int pageSize = 20)
        {
            var query = from p in db.Products
                        from e in db.Enterprises
                        from k in db.ProductKinds
                        where e.ID == enterpriseid
                            && p.EnterpriseID == e.ID
                            && p.KindID == k.ID
                            && p.Release
                        select new
                        {
                            ID = p.ID,
                            Images = p.Images,
                            Name = p.Name,
                            Price = p.Price,
                            p.KindID,
                            KindName = k.Name,
                            p.Sort
                        };
            //根据搜索框字符匹配
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter));
            }
            //根据分类名称匹配
            if (kindid != null)
            {
                query = query.Where(s => s.KindID == kindid);
            }
            var paged = query.OrderBy(s => s.Sort).ToPagedList(page, pageSize);
            var data = paged.Select(s => new
            {
                s.ID,
                Image = s.Images.SplitToArray<string>()?[0],
                s.KindID,
                s.KindName,
                s.Name,
                s.Price,
                s.Sort
            });
            return Json(Comm.ToJsonResultForPagedList(paged, data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取企业商品分类列表
        /// </summary>
        /// <param name="enterpriseid">企业id</param>
        /// <returns>商品分类列表json集合</returns>
        [AllowCrossSiteJson]
        public ActionResult GetProductKindsList(int enterpriseid)
        {
            var query = from k in db.ProductKinds
                        from e in db.Enterprises
                        where k.EnterpriseID == e.ID && k.EnterpriseID == enterpriseid
                        select new ProductKindsViewModels
                        {
                            Name = k.Name,
                            ID = k.ID
                        };
            var data = query.OrderBy(s => s.Sort);
            return Json(Comm.ToJsonResult("Success", "获取成功", data), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="enterpriseid">企业id</param>
        /// <returns>企业信息json集合</returns>
        [AllowCrossSiteJson]
        public ActionResult GetEnterpriseInfo(int enterpriseid)
        {
            var query = from e in db.Enterprises
                        where e.ID == enterpriseid && e.Enable
                        select new Enterprise
                        {
                            Name = e.Name,
                            Logo = e.Logo,
                            ID = e.ID
                        };
            return Json(Comm.ToJsonResult("Success", "获取成功", query), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductDetails(int enterpriseid) {
            var query = from p in db.Products
                        where p.EnterpriseID == enterpriseid
                        select new
                        {
                            Image = p.Images,
                            Name = p.Name,
                            Price = p.Price,
                            DetailContent = p.Info
                        };
            return Json(Comm.ToJsonResult("Success", "获取成功", query),JsonRequestBehavior.AllowGet);
        }
    }
}