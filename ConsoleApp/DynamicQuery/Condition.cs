using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.DynamicQuery
{
    public class Condition
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Operator Operator { get; set; }
    }
}
