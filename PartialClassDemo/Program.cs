using PartialClassDemo;
using System;
public class Program
{
    public static void Main(string[] args)
    {
        EmployeePartial employeePartial= new EmployeePartial();
        employeePartial.FirstName = " Atharva";
        employeePartial.LastName = "Bhat";
        employeePartial.Address = "345 slippery road, Krakov, Poland";
        employeePartial.Salary = 70000;

        employeePartial.displayEmployeeName();
        employeePartial.displayEmployeeDetails();
    }
}