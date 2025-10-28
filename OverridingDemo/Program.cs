using OverridingDemo;
using System;
namespace Poly
{
    class Program 
    {
        static void Main(string[] args)
        {
            LoadChild lc = new LoadChild();
            LoadParent lp = new LoadParent();
            lc.GetData();
            lc.GetTime();
            lc.SetTime(DateTime.Now);

            lp.GetData();
            lp.GetTime();
            lp.SetTime(DateTime.Now);

            //LoadParent Parent = new LoadChild();
            //Parent.GetData();
            //Parent.SetTime(DateTime.Now);

        }

        public void InvokeParentMethods()
        {
            
        }
    }
}
