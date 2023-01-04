﻿using System.Collections.Generic;

namespace SsbHedger.Configuration
{
    public class Configuration : IConfiguration
    {
        private Dictionary<string, object> _configuration;

        public Configuration()
        {
            _configuration = new Dictionary<string, object>()
            {
                {"Host", "localhost" },
                {"Port", 4001 },
                {"ClientId", 1 },
            };
        }

        public void SetValue(string name, object value)
        {
            _configuration[name] = value;
        }

        public object GetValue(string name)
        {
            return _configuration[name];
        }
    }
}