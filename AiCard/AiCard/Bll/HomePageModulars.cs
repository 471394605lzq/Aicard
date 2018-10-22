using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AiCard.Models;
namespace AiCard.Bll
{
    public class HomePageModulars : IDisposable
    {

        private int _enterpriseID;
        private ApplicationDbContext db = new ApplicationDbContext();
        public HomePageModulars(int enterpriseID)
        {
            _enterpriseID = enterpriseID;

        }

       

        public bool HasModulars
        {
            get
            {
                return db.HomePageModulars.Any(s => s.EnterpriseID == _enterpriseID);
            }
        }

        public List<HomePageModular> Init()
        {
            var modulars = new HomePageModular[] {
                 new HomePageModular { Title = "头部轮播",Type= Enums.HomePageModularType.Banner },
                 new HomePageModular { Title = "公司简介",Type= Enums.HomePageModularType.Images },
                 new HomePageModular { Title = "产品介绍",Type= Enums.HomePageModularType.Images },
                 new HomePageModular { Title = "联系方式" ,Type= Enums.HomePageModularType.Contact },
            };
            for (int i = 0; i < modulars.Length; i++)
            {
                modulars[i].Sort = i;
                modulars[i].EnterpriseID = _enterpriseID;
            }
            db.HomePageModulars.AddRange(modulars);
            db.SaveChanges();
            return modulars.ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}