# SQLDynamicModel
This is a simple proof of concept repository that creates models in C# with 1 central DAL instance. New models can be created, extending the Model class and will use a table based on their class name by default.

## Usage
Create a new model in the Models folder, extending the base class Model.
```cs
internal class Customer: Model
{
  protected static string tableName = ""; // Can be used to set a tablename if it is not equal to the class name
  public int Id { get; private set; } // Id should only be set from the constructor

  // Specify the other properties of the model
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
}
```
That's all the mandatory code to be able to perform the following operations:
```cs
// Find a single model by id
Customer customer = Model.Find<Customer>(id);
// Print the model to the screen
Console.WriteLine(customer.ToString());
// Update a model
customer.Name = "New name";
customer.Save();
// Delete the model
customer.Delete();
// Get all models
List<Customer> customers = Model.All<Customer>();
```

Every model can have the following methods to take care of authorization and logging
```cs
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
```

Other possibilities can easily be implemented with the use of the DAL and QueryBuilder.
