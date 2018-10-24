using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace AiCard
{
    public static class ChinaPCAS
    {
       static List<Pcas> listpcas = new List<Pcas>();
        static ChinaPCAS()
        {
            var path = $@"{HttpRuntime.AppDomainAppPath}\Scripts\ChinaPCAS\pcas.json";
            //var jObject= new JObject()
            using (System.IO.StreamReader file = System.IO.File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    
                    //var alllist = o[0];
                    //if (alllist.Count() > 0) {
                    //    for (int i = 0; i < alllist.Count(); i++) {

                    //    }
                    //}
                    foreach (JObject e in o[0])
                    {
                        var province = e[0];
                        foreach (var p in province)
                        {
                            Pcas np = new Pcas();
                            np.Name = p.ToString();
                            np.Parent= p.ToString();
                            listpcas.Add(np);
                            var city = p;
                            foreach (var c in city)
                            {
                                Pcas nc = new Pcas();
                                nc.Name = c.ToString();
                                nc.Parent = p.ToString();
                                listpcas.Add(nc);
                                var county = c;
                                foreach (var item in county)
                                {
                                    Pcas ncounty = new Pcas();
                                    nc.Name = item.ToString();
                                    nc.Parent = c.ToString();
                                    listpcas.Add(ncounty);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static string getp() {
            return "";
        }
    }

    public class Pcas
    {
        public string Name { get; set; }

        public int Type { get; set; }

        public string Parent { get; set; }

    }
}