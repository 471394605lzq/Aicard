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
    public class UserSpeechController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();



        /// <summary>
        /// 获取用户话术分类列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>list</returns>
        [AllowCrossSiteJson]
        public ActionResult GetUserSpeechTypeList(string userId, int? page = 1, int? pageSize = 20)
        {
            try
            {
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@UserId", SqlDbType.NVarChar),
                        new SqlParameter("@starpagesize", SqlDbType.Int),
                        new SqlParameter("@endpagesize", SqlDbType.Int)
                    };
                parameters[0].Value = userId;
                parameters[1].Value = starpagesize;
                parameters[2].Value = endpagesize;
                //
                string sqlstr = string.Format(@"SELECT * FROM (SELECT CAST(ROW_NUMBER() over(order by COUNT(ust.ID) DESC) AS INTEGER) AS Ornumber,ust.Name as SpeechTypeName,COUNT(ust.ID) AS SpeechTypeCount,ust.ID FROM dbo.UserSpeechTypes ust
                                                    LEFT JOIN dbo.UserSpeeches us ON us.TypeID=ust.ID WHERE ust.UserID=@UserId
                                                    GROUP BY ust.Name,ust.ID) t WHERE t.Ornumber > @starpagesize AND t.Ornumber<=@endpagesize");
                List<UserSpeechTypeModel> data = db.Database.SqlQuery<UserSpeechTypeModel>(sqlstr,parameters).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取用户话术列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>list</returns>
        [AllowCrossSiteJson]
        public ActionResult GetUserSpeechList(string userId, int? page = 1, int? pageSize = 20)
        {
            try
            {
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@UserId", SqlDbType.NVarChar),
                        new SqlParameter("@starpagesize", SqlDbType.Int),
                        new SqlParameter("@endpagesize", SqlDbType.Int)
                    };
                parameters[0].Value = userId;
                parameters[1].Value = starpagesize;
                parameters[2].Value = endpagesize;
                string sqlstr = string.Format(@"select* from (SELECT CAST(ROW_NUMBER() over(order by COUNT(ust.ID) DESC) AS INTEGER) AS Ornumber,ust.Content,ust.ID 
                                                FROM dbo.UserSpeeches ust
                                                INNER  JOIN dbo.AspNetUsers us ON us.Id=ust.UserID WHERE ust.UserID=@UserId GROUP BY ust.Content,ust.ID) t 
                                                where t.Ornumber > @starpagesize AND t.Ornumber<=@endpagesize  ");
                List<UserSpeechModel> data = db.Database.SqlQuery<UserSpeechModel>(sqlstr, parameters).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 新增话术分类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult AddSpeechTypeInfo(UserSpeechType model)
        {
            try
            {

                var hasname = db.UserSpeechTypes.Any(s => s.Name == model.Name && s.UserID == model.UserID);
                //检验话术分类是否存在
                if (hasname)
                {
                    return Json(Comm.ToJsonResult("Error", "话术分类已存在"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var speechtype = new UserSpeechType
                    {
                        Name = model.Name,
                        UserID = model.UserID,
                        Sort = 0
                    };
                    db.UserSpeechTypes.Add(speechtype);
                    db.SaveChanges();
                    return Json(Comm.ToJsonResult("Success", "新增成功"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 新增话术
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult AddSpeechInfo(UserSpeech model)
        {
            try
            {
                var speech = new UserSpeech
                {
                    Content = model.Content,
                    TypeID = model.TypeID,
                    UserID = model.UserID,
                    Sort = 0
                };
                db.UserSpeechs.Add(speech);
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "新增成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 编辑话术分类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditSpeechTypeInfo(UserSpeechType model)
        {
            try
            {
                var t = db.UserSpeechTypes.FirstOrDefault(s => s.ID == model.ID && s.UserID == model.UserID);
                var hasname = db.UserSpeechTypes.Any(s => s.Name == model.Name && s.UserID == model.UserID);
                if (t == null)
                {
                    return Json(Comm.ToJsonResult("Error", "话术分类不存在"), JsonRequestBehavior.AllowGet);
                }
                //检验话术分类是否存在
                else if (hasname)
                {
                    return Json(Comm.ToJsonResult("Error", "话术分类已存在"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    t.ID = model.ID;
                    t.Name = model.Name;
                    db.SaveChanges();
                    return Json(Comm.ToJsonResult("Success", "设置成功"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 编辑话术
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditSpeechInfo(UserSpeech model)
        {
            try
            {
                var t = db.UserSpeechs.FirstOrDefault(s => s.ID == model.ID && s.UserID == model.UserID);
                if (t == null)
                {
                    return Json(Comm.ToJsonResult("Error", "话术不存在"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    t.ID = model.ID;
                    t.Content = model.Content;
                    db.SaveChanges();
                    return Json(Comm.ToJsonResult("Success", "设置成功"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除话术
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult DeleteSpeechInfo(int id)
        {
            try
            {
                UserSpeech speech = db.UserSpeechs.Find(id);
                db.UserSpeechs.Remove(speech);
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "删除成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        private class UserSpeechTypeModel
        {
            //排序编号
            public int Ornumber { get; set; }
            //话术分类名称
            public string SpeechTypeName { get; set; }
            //话术分类下的话术总数
            public int SpeechTypeCount { get; set; }
            public int ID { get; set; }

        }
        private class UserSpeechModel
        {
            //排序编号
            public int Ornumber { get; set; }
            //话术内容
            public string Content { get; set; }
            public int ID { get; set; }

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