using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    public class Ingradient
    {
        public string Name { get; set; }
        public int Price { get; set; }
        
        public Ingradient(string name,int price)
        {
            Price = price;
            Name = name;
        }
    }
}
