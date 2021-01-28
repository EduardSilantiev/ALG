# ALG WEB.API test project

# Coding task:
Develop an ASP.Net Core API for managing promo codes.
Please use this layout as a reference
https://www.figma.com/file/6J7oriX3K4zPLF2lrhfIvJ/front-end-test-prototype

API should support the following functionality:

● Ability to search services by name 

● Ability to Activate bonus for a Service for the current user

● Infinite scroll for the Services list

● An API user should be authorized.

● Store data in any relational database, use EF Core to access data.

● Use tests in your project.

● The project should include a README.md with instructions on how to run it locally.


# System Requirements

1. .Net SDK 3.1.402 should be installed
2. Visual Studio 2019 should be installed (optionally)
3. MSSQL Server should be installed (any modern version).

# How to run the application from command line

1. Open CMD or PowerShell
2. Select a folder you want it to be installed

    cd ‘some folder’
    
3. Pull the source code from Github

    git clone https://github.com/EduardSilantiev/ALG.git
    
4. Select a ALG.Web.Host folder

    cd ALG\src\ALG.Web.Host
    
5. Run the application

    dotnet run

The application will be built and started. Then it can be accessed in a Browser by the address:
https://localhost:5001/

To stop the application, press Ctrl+C

# How to run the application from Visual Studio 2019

1. Open CMD or PowerShell
2. Select a folder you want it to be installed

    cd ‘some folder’
    
3. Pull the source code from Github

    git clone https://github.com/EduardSilantiev/ALG.git
    
4. Find and click a ALG.sln file
5. When Visual Studio opens, press Ctrl+F5 or select in menu Debug\Start Without Debugging

The application will be built and started. Then it can be accessed in a Browser by the address:
https://localhost:44315/index.html

*When the App starts for the first time it creates an ALG database in MSSQL server (if not created) and seeds the database with a test data.
If you want to use another database name, please change the connection string in an appsettings.json file.*

*Three users are created by default:*

*Email: john.dow@gmail.com*
*Password: 111*

*Email: adell.sansone@gmail.com*
*Password: 222*

*Email: gordon.brundage@gmail.com*
*Password: 333*

*Seven services are created by default:*
*Sitecostructor.io, Appvision.com, Analytics.com, Logotype, Google.com, Microsoft.com, Amazon.com*

*The promocode for all services is* ***itpromocode***

# Features

1. Entity Framework Core – Code First. With automatic database creation, model migration and seeding with a test data
2. Repository Pattern
3. Logging with Serilog in files
4. OpenAPI documentation, Swagger UI
5. Generic pagination mechanism
6. Identity with JWT Authentication
7. Custom Exception Handling Middlewares
8. API Versioning
9. Fluent Validation
10. Automapper
11. Unified error messages format

# How to use SwaggerUI with JWT Authentication

https://docs.google.com/document/d/1z5pdNfC3Sa1VRd-kKgpbWDRHywe5p_OsD2pNVCYLU4c/edit?usp=sharing
