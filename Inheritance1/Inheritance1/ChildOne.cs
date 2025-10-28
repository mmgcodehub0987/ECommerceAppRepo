using inheritanceProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance1
{
    class ChildOne : Parent
    {
        public static void Main(string[] args)
          {
            ChildOne Childone = new ChildOne(); //By default this invokes parent class constructor
            Parent p1 = new Parent();
            Console.WriteLine("Method 1 from parent class was inherite` d. Current Date time : " + Childone.GetCurrentDateTime());
            Console.WriteLine("Method 2 from parent class was inherited : "+ Childone.getParentMethodValue());
        }

        public void ChildOneMethodOne() //this method is not accessible by parent class
        {
            Console.WriteLine("Pure method of child one");
        }
    }
}
