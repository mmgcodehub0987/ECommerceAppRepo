using System;
using System.Security.Cryptography.X509Certificates;
namespace inheritanceProject;
class Parent
{
    public Parent() // this should be public 
    {
        Console.WriteLine("The parent class constructor is called");
    }
    public DateTime GetCurrentDateTime()
    {
        DateTime currentTime = DateTime.Now;
        return currentTime;
    }

    public String getParentMethodValue()
    {
        return "The second method was executed";
    }

}