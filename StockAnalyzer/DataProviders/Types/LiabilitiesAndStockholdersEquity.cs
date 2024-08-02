﻿using System.Collections.Generic;

namespace StockAnalyzer.DataProviders.Types
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class LiabilitiesAndStockholdersEquity
    {
        public int cik { get; set; }
        public string taxonomy { get; set; }
        public string tag { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string entityName { get; set; }
        public Units units { get; set; }
    }

    public class Units
    {
        public List<USD> USD { get; set; }
    }

    public class USD
    {
        public string end { get; set; }
        public object val { get; set; }
        public string accn { get; set; }
        public int fy { get; set; }
        public string fp { get; set; }
        public string form { get; set; }
        public string filed { get; set; }
        public string frame { get; set; }
    }


}
