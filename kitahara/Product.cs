﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace kitahara
{
    class Product
    {
        public long Hinban { get; set; }
        public string Shohinmei { get; set; }
        public string Ymd { get; set; }
        
        public string Nkbn { get; set; }
        public string Tancd { set; get; }
        public string Torimei { get; set; }
        public int Hatyusu { get; set; }
        public int Nyukasu { get; set; }
        public int Jutyusu { get; set; }
        public int Syukasiji { get; set; }
        
        public int Hzan { get; set; }
        public int Jzan { get; set; }
        public int Tzaiko { get; set; }
        

        public Product(long hinban, string shohinmei, string ymd, string nkbn, string tancd, string torimei, int hatyusu, int nyukasu, int jutyusu, int syukasiji
            , int hzan, int jzan, int tzaiko)
        {
            Hinban = hinban;
            Shohinmei = shohinmei;
            Ymd = ymd;
            Nkbn = nkbn;
            Tancd = tancd;
            Torimei = torimei;
            Nyukasu = nyukasu;
            Jutyusu = jutyusu;
            Syukasiji = syukasiji;
            Hzan = hzan;
            Jzan = jzan;
            Tzaiko = tzaiko;
            
        }
    }
    class ProductMapper : CsvHelper.Configuration.ClassMap<Product>
    {
        public ProductMapper()
        {
            Map(x => x.Hinban).Index(0).Name("コード");
            Map(x => x.Shohinmei).Index(1).Name("品名");
            Map(x => x.Ymd).Index(2).Name("日付");
            Map(x => x.Nkbn).Index(3).Name("区分");
            Map(x => x.Tancd).Index(4).Name("取引先名");
            Map(x => x.Torimei).Index(5).Name("発注数");
            Map(x => x.Nyukasu).Index(7).Name("入荷数");
            Map(x => x.Jutyusu).Index(8).Name("受注数");
            Map(x => x.Syukasiji).Index(9).Name("出荷指示数");
            Map(x => x.Hzan).Index(10).Name("発注残");
            Map(x => x.Jzan).Index(11).Name("受注残");
            Map(x => x.Tzaiko).Index(12).Name("取置在庫数");
        }
    }

}
