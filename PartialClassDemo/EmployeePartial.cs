using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialClassDemo
{
    public partial class EmployeePartial
    {
        private String _firstName ="";
        private String _lastName ="";
        private String _address="";
        private Double _salary = 0;

        public String FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        public string Address 
        { get => _address; set => _address = value; }
        public string LastName { get => _lastName; set => _lastName = value; }
        public double Salary { get => _salary; set => _salary = value; }
    }
}
