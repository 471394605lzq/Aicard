using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using AiCard;
using AiCard.Common;
using AiCard.Common.CommModels;

namespace AiCard.Controllers
{
    public class UploaderController : Controller
    {
        public string UserID
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        //[HttpPost]
        //[Authorize]
        //public ActionResult GetFileList(int page = 1)
        //{
        //    using (ApplicationDbContext db = new ApplicationDbContext())
        //    {
        //        Func<int, PagedList.PagedList<CloudFile>> load = null;
        //        load = p =>
        //        {
        //            var t = db.CloudFiles
        //              .Where(s => s.UserID == UserID)
        //              .OrderByDescending(s => s.CreateDateTime)
        //              .ToPagedList(p, 10);
        //            if (t.PageCount > 0 && p > t.PageCount)
        //            {
        //                t = load(t.PageCount);
        //            }
        //            return t;

        //        };


        //        var paged = load(page);
        //        var list = paged.Select(s =>
        //        {
        //            var t = new CloudFileViewModel(s);
        //            return new
        //            {
        //                t.Name,
        //                t.Size,
        //                t.ID,
        //                t.Thumbnail,
        //                t.Url,
        //                t.CreateDateTime,
        //                t.Type,
        //                t.Extension
        //            };
        //        }).ToList();

        //        return Json(Comm.ToMobileResultForPagedList(paged, list));
        //    }
        //}


        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Upload(BaseFileUpload model)
        {
            ICollection<string> filename = null;
            if (string.IsNullOrWhiteSpace(model.FilePath))
            {
                filename = this.UploadFile(isResetName: model.IsResetName);
            }
            else
            {
                filename = this.UploadFile(path: model.FilePath, isResetName: model.IsResetName);
            }

            if (filename == null || filename.Count <= 0)
            {
                return Json(Comm.ToJsonResult("Error", "上传失败"));
            }

            switch (model.Server)
            {

                default:
                case UploadServer.Local:
                    {
                        return Json(Comm.ToJsonResult("Success", "成功", new
                        {
                            FileUrls = filename,
                            FileFullUrls = filename.Select(s => Url.ContentFull(s))
                        }));
                    }
                case UploadServer.QinQiu:
                    {
                        List<string> fileList = new List<string>();
                        var qinniu = new Qiniu.QinQiuApi();
                        foreach (var item in filename)
                        {
                            var path = Server.MapPath(item);
                            fileList.Add(qinniu.UploadFile(path, true));
                        }
                        return Json(Comm.ToJsonResult("Success", "成功", new
                        {
                            FileUrls = fileList,
                            FileFullUrls = fileList
                        }));

                    }
            }

        }



        //[HttpPost]
        //[Authorize]
        //public ActionResult UploadByCloud()
        //{
        //    var filenames = this.UploadFile().ToList();
        //    if (filenames.Count > 0 && !string.IsNullOrWhiteSpace(UserID))
        //    {
        //        var file = Request.Files[0];
        //        CloudFileViewModel result;
        //        using (ApplicationDbContext db = new ApplicationDbContext())
        //        {
        //            CloudFile cFile = new CloudFile(UserID, file, filenames[0]);
        //            db.CloudFiles.Add(cFile);
        //            db.SaveChanges();
        //            result = new CloudFileViewModel(cFile);

        //        };
        //        return Json(Comm.ToMobileResult("Success", "成功", result));
        //    }
        //    return Json(Comm.ToMobileResult("Error", "失败"));

        //}

        public ActionResult DeleteFile(string file)
        {
            try
            {
                DeleteSeverFile(file);
                return Json(Comm.ToJsonResult("Success", "删除成功"));
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", "删除失败"));
            }
        }

        private void DeleteSeverFile(string file)
        {

            if (file.Contains(Qiniu.QinQiuApi.ServerLink))
            {
                string key = Qiniu.QinQiuApi.LinkToKey(file);
                try
                {
                    new Qiniu.QinQiuApi().DeleteFile(key);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
            else
            {
                file = Server.MapPath(file);
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (System.IO.IOException ex)
                {
                    throw ex;
                }
            }

        }



        //[HttpPost]
        //[Authorize]
        //public ActionResult DeleteCloudFile(int[] ids)
        //{
        //    using (ApplicationDbContext db = new ApplicationDbContext())
        //    {
        //        var cloudFile = db.CloudFiles
        //            .Where(s => ids.Contains(s.ID) && s.UserID == UserID)
        //            .ToList();
        //        if (cloudFile.Count > 0)
        //        {
        //            foreach (var item in cloudFile)
        //            {
        //                try
        //                {
        //                    DeleteSeverFile(item.Url);
        //                    db.CloudFiles.Remove(item);
        //                }
        //                catch (DirectoryNotFoundException)
        //                {
        //                    db.CloudFiles.Remove(item);
        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }
        //            db.SaveChanges();
        //            return Json(Comm.ToMobileResult("Success", "删除成功"));
        //        }
        //        else
        //        {
        //            return Json(Comm.ToMobileResult("FileNoFound", "文件不存在"));
        //        }
        //    }

        //}

        [HttpPost]
        public ActionResult ForCkEditor()
        {
            var filename = this.UploadFile().ToList();
            if (filename == null || filename.Count <= 0)
            {
                return Json(new { State = "Error", Message = "未找到上传文件" });
            }
            ViewBag.CKEditorFuncNum = Request["CKEditorFuncNum"];
            var name = $"{filename[0]}?404=default";
            ViewBag.File = Url.ContentNullEmpty(name);
            if (Request["responseType"] == "json")
            {
                return Json(new { fileName = new System.IO.FileInfo(filename[0]).Name, uploaded = 1, url = Url.Content(name) });
            }
            else
            {
                return View();
            }
        }
    }
}