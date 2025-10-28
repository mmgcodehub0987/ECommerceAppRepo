using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverridingDemo
{
    internal class LoadParent
    {
        public virtual void GetData()
        {
            Console.WriteLine("100pa This is parent class get Data method");
        }

        public virtual void GetTime()
        {
            Console.WriteLine("Parent: Date: 25/07/2025");
        }
        public void SetTime(DateTime time)
        {
            Console.WriteLine("Parent method: Time set.."+time);
        }
    }
}
