using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractionDemo
{
    public abstract class AbsParent
    {

        public abstract int GetData(int a);
        public abstract String getName(String Name);

        public String GetMessage()
        {
            return "Concrete method returns message";
        }

        public static double ConvertDollarsToRupees(double Value)
        {
            return Value * 86.45;

        }
    }
}
