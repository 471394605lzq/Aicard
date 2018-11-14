using AiCard.DAL.Models;
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
                            ProductID = p.ID,
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
                s.ProductID,
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
                        orderby k.Sort
                        select new ProductKindViewModel
                        {
                            Name = k.Name,
                            KindID = k.ID,
                            Sort = k.Sort
                        };
            var data = query.ToList();
            data.Insert(0, new ProductKindViewModel { KindID = null, Name = "全部", Sort = -1 });
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
                        select new
                        {
                            Name = e.Name,
                            Logo = e.Logo,
                            EnterpriseID = e.ID
                        };
            return Json(Comm.ToJsonResult("Success", "获取成功", query), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="enterpriseid">企业id</param>
        /// <param name="productid">商品id</param>
        /// <returns>商品信息json集合</returns>
        [AllowCrossSiteJson]
        public ActionResult GetProductDetails(int enterpriseid, int productid)
        {
            try
            {
                var product = (from p in db.Products
                               where p.EnterpriseID == enterpriseid && p.ID == productid && p.Release
                               select new
                               {
                                   p.ID,
                                   Images = p.Images,
                                   Name = p.Name,
                                   Nowprice = p.Price,
                                   DetailContent = p.Info,
                                   Originalprice = p.OriginalPrice,
                                   p.EnterpriseID,
                               }).FirstOrDefault();
                var model = new
                {
                    ProductID = product.ID,
                    Images = product.Images.SplitToArray<string>(),
                    Name = product.Name,
                    Nowprice = product.Nowprice,
                    DetailContent = product.DetailContent,
                    Originalprice = product.Originalprice,
                    product.EnterpriseID
                };

                return Json(Comm.ToJsonResult("Success", "获取成功", model), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message));
            }
        }
    }
}