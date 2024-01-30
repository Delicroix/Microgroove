# Microgroove

**Challenge**

Build a C# Azure Functions v4 REST API, with an in-memory SQLite DB. Use .net 6.0.

3 endpoints / routes

|Route|Description|
|---|---|
|POST /api/customers|create a customer|
|GET /api/customers/GUID|return a customer by ID|
|GET /api/customers/int|return customers of a certain age|

Customer Model
|Property|C# Type|
|---|---|
|CustomerId|GUID|
|FullName|string|
|DateOfBirth|DateOnly|

**Implementation Requirements (things we want to see)**

- Separate Functions (i.e. front door / entry point), business logic and data manager projects / 
layers
- Validation and error messages returned to callers
- Dependency Injection


**Stretch Goal**

If time permits, add the following to the API:

- Whenever a new customer record is created (i.e. POST endpoint), call an API to generate a 
profile image for the customer
- Use the following API call: https://ui-avatars.com/api/?name=<FULL_NAME>&format=svg
- e.g. https://ui-avatars.com/api/?name=John+Doe&format=svg
- Store the returned SVG data in a new field in the customer record.

**Delivery**

- Locally runnable (i.e. does not require deployment or publishing) e.g. F5-able in Visual Studio
- Solution files zipped or via Github

***My Notes***

- I created the function project as a .NET 7 Isolated Function
- This is also my first stab at implementing Domain Driven Design & Clean Architecture (as I understand it)
- Project represents about 10 total hours of work