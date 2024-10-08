using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SQLDynamicModel.DAL
{
    internal class DALSql
    {
        // Make a connection to an SQL Server database
        private readonly string connectionString = "Server=localhost;Database=SQLDynamicModel;Trusted_Connection=True;";


        private SqlConnection connection;

        public DALSql()
        {
            this.connection = new SqlConnection(this.connectionString);
        }

        // Method to insert a new row into the database
        public int Insert(string _table, Dictionary<string, object> _values)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                // Create a new query using the QueryBuilder and the values
                var query = new QueryBuilder(_table).Insert(_values).Build();
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the values as parameters to the command
                    foreach (var kvp in _values)
                    {
                        command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                    }
                    // Execute the command and return the Id of the new row
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT CAST(@@Identity as INT);";
                    return (int)command.ExecuteScalar();
                }

            }
        }

        // Method to select one or more rows from the database
        public List<Dictionary<string, object>> Select(string _table, Func<QueryBuilder, QueryBuilder> _queryBuilder = null)
        {
            // Create a new list to store the rows
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                // Create a new query using the QueryBuilder
                var queryBuilder = new QueryBuilder(_table).Select();
                // If a custom query builder is provided, use it. Otherwise use the default (select all)
                var query = _queryBuilder != null ? _queryBuilder(queryBuilder).Build() : queryBuilder.Build();
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // For each row, create a new dictionary with the column names and values
                        while (reader.Read())
                        {
                            Dictionary<string, object> values = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                values.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            // Add the dictionary to the list of rows
                            rows.Add(values);
                        }
                    }
                }
            }
            return rows;
        }

        // Method to update one or more rows in the database
        public void Update(string _table, Dictionary<string, object> _values, Func<QueryBuilder, QueryBuilder> _queryBuilder)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                // Create a new query using the QueryBuilder and the values
                var queryBuilder = new QueryBuilder(_table).Update(_values);
                // If a custom query builder is provided, use it. Otherwise use the default (update all)
                var query = _queryBuilder != null ? _queryBuilder(queryBuilder).Build() : queryBuilder.Build();

                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the values as parameters to the command
                    foreach (var kvp in _values)
                    {
                        command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);
                    }
                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to delete one or more rows from the database
        public void Delete(string _table, Func<QueryBuilder, QueryBuilder> _queryBuilder)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                // Create a new query using the QueryBuilder
                var queryBuilder = new QueryBuilder(_table).Delete();
                // If a custom query builder is provided, use it. Otherwise use the default (delete all)
                var query = _queryBuilder != null ? _queryBuilder(queryBuilder).Build() : queryBuilder.Build();
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
