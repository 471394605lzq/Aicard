using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace AiCard.Models.CommModels
{
    public class ChinaPCASCom
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="cname"></param>
        public ChinaPCASCom(string pname = "北京市", string cname = "东城区")
        {
            Provincelist = ChinaPCAS.GetP();
            if (!string.IsNullOrWhiteSpace(pname))
            {
                Citylist = ChinaPCAS.GetC(pname);
            }
            if (!string.IsNullOrWhiteSpace(pname) && !string.IsNullOrWhiteSpace(cname))
            {
                Districtlist = ChinaPCAS.GetA(pname, cname);
            }
        }

        //省份
        [Display(Name ="省份")]
        public string Province { get; set; }
        //城市
        [Display(Name = "城市")]
        public string City { get; set; }
        //地区
        [Display(Name = "地区")]
        public string District { get; set; }
        public List<string> Provincelist { get; set; }

        public List<string> Citylist { get; set; }
        public List<string> Districtlist { get; set; }
    }
}