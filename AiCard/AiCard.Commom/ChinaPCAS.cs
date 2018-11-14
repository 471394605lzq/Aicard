using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IO;
using System.Net.Http;

namespace AiCard.Common
{
    public static class ChinaPCAS
    {
        public static List<Pcas> Listpcas { get; set; }

        static List<Pcas> listpcas = new List<Pcas>();


        static ChinaPCAS()
        {
            string rootPath = string.Empty;
            if (HttpContext.Current !=null)
            {
                rootPath = HttpContext.Current.Request.PhysicalApplicationPath;
            }
            else {
                rootPath = AppDomain.CurrentDomain.BaseDirectory;
            }
             
            var path = $@"{rootPath}\Scripts\ChinaPCAS\pcas-code.json";
            var sourceContent = File.ReadAllText(path);
            var sourceobjects = JArray.Parse(sourceContent);
            Action<List<Pcas>, JArray, int> setChild = null;
            setChild = (child, cdata, i) =>
            {
                foreach (var data in cdata)
                {
                    var item = new Pcas();
                    item.Name = data["name"].ToString();
                    //item.Code = data["code"].ToString();
                    //item.Type = Enums.PcasType.Province;
                    item.Child = new List<Pcas>();
                    child.Add(item);
                    var tempchildren = data["children"]?.Value<JArray>() ?? null;
                    if (tempchildren != null)
                    {
                        if (i <= 1 && tempchildren.Count > 1)
                        {
                            setChild(item.Child, data["children"]?.Value<JArray>(), i + 1);
                        }
                        else if (i <= 1 && tempchildren.Count == 1)
                        {
                            setChild(item.Child, data["children"].Value<JArray>()[0]["children"]?.Value<JArray>(), i + 1);
                        }
                    }

                }
            };
            setChild(listpcas, sourceobjects, 0);

            //foreach (var item in sourceobjects)
            //{
            //    Pcas np = new Pcas();
            //    np.Name = item["name"].ToString();
            //    np.Code = item["code"].ToString();
            //    np.Type = Enums.PcasType.Province;
            //    np.ParentName = item["name"].ToString();
            //    listpcas.Add(np);
            //    var cityobject = item["children"];
            //    foreach (var item2 in cityobject)
            //    {
            //        Pcas nc = new Pcas();
            //        nc.Name = item2["name"].ToString();
            //        nc.Code = item2["code"].ToString();
            //        nc.Type = Enums.PcasType.City;
            //        nc.ParentName = item["name"].ToString();
            //        listpcas.Add(nc);
            //        var countyobject = item2["children"];
            //        foreach (var item3 in countyobject)
            //        {
            //            Pcas ncn = new Pcas();
            //            ncn.Name = item3["name"].ToString();
            //            ncn.Code = item3["code"].ToString();
            //            ncn.Type = Enums.PcasType.City;
            //            ncn.ParentName = item2["name"].ToString();
            //            listpcas.Add(ncn);
            //        }
            //    }
            //}
        }
        //根据上级名称查询
        //public static List<Pcas> getp(string preantname)
        //{
        //    List<Pcas> returnlist = listpcas.Where(s => s.ParentName == preantname).ToList();
        //    return returnlist;
        //}

        public static List<string> GetP()
        {
            return listpcas.Select(s => s.Name).ToList();
        }

        public static List<string> GetC(string p)
        {
            return listpcas.FirstOrDefault(s => s.Name == p).Child.Select(s => s.Name).ToList();
        }


        public static List<string> GetA(string p, string c)
        {
            return listpcas.FirstOrDefault(s => s.Name == p)
                .Child.FirstOrDefault(s => s.Name == c)
                .Child.Select(s => s.Name).ToList();
        }
    }

    public class Pcas
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Enums.PcasType Type { get; set; }

        public string ParentName { get; set; }

        public List<Pcas> Child { get; set; }

    }
}