﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Model
{
    internal class Position
    {
        public Position()
        {
        }

        public Position(List<string> splitted)
        {
            if (splitted == null) throw new Exception("Unexpected. splitted may not be null");

            NetBms = Convert.ToInt32(splitted[1]);
            ConId = splitted.Count > 2 ? Convert.ToInt32(splitted[2]) : null;
            Weight = splitted.Count > 3 ? Convert.ToInt32(splitted[3]) : null;
            Quantity = splitted.Count > 4 ? Convert.ToInt32(splitted[4]) : null;
            Margin = splitted.Count > 5 ? Convert.ToInt32(splitted[5]) : null;
        }

        public int NetBms { get; set; }
        public int? ConId { get; set; }
        public int? Weight { get; set; }
        public int? Quantity { get; set; }
        public int? Margin { get; set; }
    }
}