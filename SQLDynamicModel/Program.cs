using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SQLDynamicModel.Models;

namespace SQLDynamicModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var values = new Dictionary<string, object>
            //{
            //    { "Name", "John" },
            //    { "Age", 26 }
            //};
            //Customer customer1 = Model.Create<Customer>(values);
            //Console.WriteLine("Created new customer: ");
            //Console.WriteLine(customer1.ToString());

            //Customer customer2 = Model.Find<Customer>(20);
            //Console.WriteLine("Found customer: ");
            //Console.WriteLine(customer2.ToString());

            //customer2.Name = "Test";
            //customer2.Save();
            //Console.WriteLine("Updated customer: ");
            //Console.WriteLine(customer2.ToString());
            //customer2.Delete();

            //List<Customer> list = Model.All<Customer>();
            //Console.WriteLine("All customers: ");
            //foreach (var customer in list)
            //{
            //    Console.WriteLine(customer.ToString());
            //}


            Console.WriteLine("Press any key to exit.");

            Console.ReadKey();
        }
    }
}
