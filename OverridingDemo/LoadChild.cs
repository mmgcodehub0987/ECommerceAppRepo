using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverridingDemo
{
    internal class LoadChild : LoadParent
    {
        public override void GetData()
        {
            Console.WriteLine("100child: This is child class get Data method");
        }

        public override void GetTime() //overloading
        {
            Console.WriteLine("Child - Current time: " + DateTime.UtcNow);
        }

        public new void SetTime(DateTime time) //methid hiding or Shadowing
        {
            Console.WriteLine("Child - Place: Bengaluru, Date has been Set: "+ time);
        }

    }
}
