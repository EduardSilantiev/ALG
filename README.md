# ALG test project

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

When the App starts for the first time it creates an ALG database in MSSQL server (if not created) and seeds the database with a test data.
It you want to use another database name, please change the connection string in an appsettings.json file.

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
