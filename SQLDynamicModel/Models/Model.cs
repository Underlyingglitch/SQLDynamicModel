using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SQLDynamicModel.DAL;

namespace SQLDynamicModel.Models
{
    internal class Model
    {
        // Method to convert the object to a string containing all properties and their values
        public override string ToString()
        {
            var properties = this.GetType().GetProperties();
            Dictionary<string, object> values = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                values.Add(property.Name, property.GetValue(this));
            }

            return string.Join(", ", values.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }

        // Dynamic method to call a method by name if it exists on the class extending Model
        private static bool CallMethodIfExists<T>(T instance, string methodName)
        {
            MethodInfo method = typeof(T).GetMethod(methodName);
            if (method != null)
            {
                // Invoke the method and check if the result is a boolean, if so return the result
                var result = method.Invoke(instance, null);
                if (result is bool)
                {
                    return (bool)result;
                }
            }
            // If the method does not exist or does not return a boolean, return true
            return true;
        }

        // Method to create a new instance of a model and insert it into the database
        public static T Create<T>(Dictionary<string, object> values)
        {
            // Try to call the BeforeCreate method on the class extending Model
            if (!CallMethodIfExists((T)Activator.CreateInstance(typeof(T)), "BeforeCreate"))
            {
                // If it fails, return an empty instance of the model
                return default(T);
            }
            // Create a DAL instance
            DALSql dal = new DALSql();
            string tableName = typeof(T).GetField("tableName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString() ?? typeof(T).Name;

            // Run the Insert method on the DAL and get the Id of the new row
            int id = dal.Insert(tableName, values);

            // Create a new instance of the model with the Id
            T instance = (T)Activator.CreateInstance(typeof(T), id);
            // Try to call the AfterCreate method on the instance if it exists
            CallMethodIfExists(instance, "AfterCreate");
            // Return the new 
            return instance;
        }

        // Method to find an instance of a model by Id
        public static T Find<T>(int _id)
        {
            // Create a new instance of the model with the id provided
            T instance = (T)Activator.CreateInstance(typeof(T), _id);
            // Try to call the AfterView method on the instance if it exists
            CallMethodIfExists(instance, "AfterView");
            // Return the instance
            return instance;
        }

        // Method to fill the properties of the model with values from the database
        protected void Fill(int _id)
        {
            // Try to call the BeforeView method on the instance
            if (!CallMethodIfExists(this, "BeforeView"))
            {
                Console.WriteLine("BeforeView failed");
                return;
            }
            // Create a new DAL instance and get the table name from the property/class name
            DALSql dal = new DALSql();
            string tableName = this.GetType().GetField("tableName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString() ?? this.GetType().Name;

            // Get the values from the database where the Id matches the provided Id
            var values = dal.Select(tableName, qb => qb.Where("Id", "=", _id.ToString()))[0];
            if (values == null)
            {
                Console.WriteLine("No values found");
                return;
            }
            // Set the properties of the instance with the values from the database
            foreach (var kvp in values)
            {
                var property = this.GetType().GetProperty(kvp.Key);
                if (property != null)
                {
                    property.SetValue(this, kvp.Value);
                }
            }
            // Try to call the AfterView method on the instance if it exists
            CallMethodIfExists(this, "AfterView");
        }

        // Method to save the instance to the database (with changed properties)
        public void Save()
        {
            // Create a new DAL instance and get the table name from the property/class name
            DALSql dal = new DALSql();
            string tableName = this.GetType().GetField("tableName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString() ?? this.GetType().Name;

            // Get the properties of the instance and create a dictionary of values
            var properties = this.GetType().GetProperties();
            Dictionary<string, object> values = new Dictionary<string, object>();
            var id = this.GetType().GetProperty("Id");
            foreach (var property in properties)
            {
                // Never update the Id property
                if (property.Name == "Id") continue;
                values.Add(property.Name, property.GetValue(this));
            }
            // If the Id is null, insert a new row, otherwise update the existing row calling the Before/After methods if they exist
            if (id is null)
            {
                if (!CallMethodIfExists(this, "BeforeCreate"))
                {
                    Console.WriteLine("BeforeCreate failed");
                    return;
                }
                dal.Insert(tableName, values);
                CallMethodIfExists(this, "AfterCreate");
            }
            else
            {
                if (!CallMethodIfExists(this, "BeforeUpdate"))
                {
                    Console.WriteLine("BeforeUpdate failed");
                    return;
                }
                dal.Update(tableName, values, qb => qb.Where("Id", "=", id.ToString()));
                CallMethodIfExists(this, "AfterUpdate");
            }
        }

        // Method to delete the instance from the database
        public void Delete()
        {
            // Try to call the BeforeDelete method on the instance
            if (!CallMethodIfExists(this, "BeforeUpdate"))
            {
                Console.WriteLine("BeforeDelete failed");
                return;
            }
            // Create a new DAL instance and get the table name from the property/class name
            DALSql dal = new DALSql();
            string tableName = this.GetType().GetField("tableName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString() ?? this.GetType().Name;

            // Get the Id of the instance and delete the row from the database
            var id = this.GetType().GetProperty("Id");
            dal.Delete(tableName, qb => qb.Where("Id", "=", id.GetValue(this).ToString()));
            // Try to call the AfterDelete method on the instance if it exists
            CallMethodIfExists(this, "AfterDelete");
        }

        // Method to get all instances of a model from the database
        public static List<T> All<T>() where T: Model, new()
        {
            // Create a new list of the model type
            var list = new List<T>();
            // Create a new DAL instance and get the table name from the property/class name
            DALSql dal = new DALSql();
            string tableName = typeof(T).GetField("tableName", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString() ?? typeof(T).Name;

            // Get all values from the database and create a new instance of the model for each row
            var values = dal.Select(tableName);
            foreach (var value in values)
            {
                // Create a new instance of the model with the Id from the database
                int id = int.Parse(value["Id"].ToString());
                var instance = (T)Activator.CreateInstance(typeof(T), id);

                // Set the properties of the instance with the values from the database
                foreach (var kvp in value)
                {
                    var property = instance.GetType().GetProperty(kvp.Key);
                    if (property != null)
                    {
                        property.SetValue(instance, kvp.Value);
                    }
                }
                // Add the instance to the list
                list.Add(instance);
            }
            // Return the list of instances
            return list;
        }
    }
}
