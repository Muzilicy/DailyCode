using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Models
{
    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public float HeightInches { get; set; }

        public Dictionary<string,Address> Addresses { get; set; }
    }

    public class Address 
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get;set; }

        public string Zip { get; set; }
    }
}
