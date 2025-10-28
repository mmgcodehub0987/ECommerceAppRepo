using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialClassDemo
{
    internal partial class EmployeePartial
    {
        public void displayEmployeeName()
        {
            Console.WriteLine("First Name: "+ FirstName.ToString()+ " Last Name: "+LastName);
        }

        public void displayEmployeeDetails()
        { 
            Console.WriteLine("Employee Details: ");
            Console.WriteLine("Adress : "+Address);
            Console.WriteLine("Salary: "+Salary.ToString());
        }
    }
}
