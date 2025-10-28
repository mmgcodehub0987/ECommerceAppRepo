using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractionDemo
{
    public class AbsChild : AbsParent
    {
        public override int GetData(int a)
        {
            return a;
        }

        public override string getName(string Name)
        {
            return Name;
        }

    }
}
