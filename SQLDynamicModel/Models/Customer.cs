using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SQLDynamicModel.Models
{
    internal class Customer: Model
    {
        // Allow overwriting of the table name, otherwise the class name will be used
        //protected static string tableName = "Customer";

        // Id should be public and readonly
        public int Id { get; private set; }

        // Specify the properties of the model
        public string Name { get; set; }
        public int Age { get; set; }

        // Default constructor is required for performing Create operations
        public Customer()
        { }

        // Constructor for finding an existing customer by Id
        public Customer(int _id)
        {
            this.Id = _id;
            // Load the customer from the database
            this.Fill(_id);
        }

        public bool BeforeView()
        {
            // Perform checks before viewing a customer
            return true;
        }

        public void AfterView()
        {
            // Perform actions after viewing a customer
        }

        public static bool BeforeCreate()
        {
            // Perform checks before creating a new customer
            return true;
        }

        public void AfterCreate()
        {
            // Perform actions after creating a new customer
        }

        public bool BeforeUpdate()
        {
            // Perform checks before updating a customer
            return true;
        }

        public void AfterUpdate() 
        {
            // Perform actions after updating a customer
        }

        public bool BeforeDelete()
        {
            // Perform checks before deleting a customer
            return true;
        }

        public void AfterDelete()
        {
            // Perform actions after deleting a customer
        }
    }
}
