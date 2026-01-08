# Docosoft# Docosoft API

## Overview

Docosoft API is a .NET 8 Web API project following **Domain-Driven Design (DDD)** principles.  
It provides CRUD operations for users, including secure password hashing, DTOs, and global exception handling.

---

## Features

- Full **User CRUD** operations
- Passwords are securely hashed
- **DTOs** used for data transfer
- **Global exception handling** for unhandled exceptions
- Logging with configurable path and levels
- **Automatic database creation** if it doesn’t exist
- **Automatic Users table creation** if it doesn’t exist
- Flexible search with filters, skip, and take for paging

---

## Database Initialization

The API automatically handles database setup:

1. **Connection String**: Configure a valid connection string in `appsettings.json`.  
2. **Automatic Database Creation**:  
   - If the database specified in the `Initial Catalog` does not exist, it will be **created automatically**.  
   - The `Users` table will also be **checked and created automatically** if it does not exist, following the predefined schema.  
3. **Logging**: Database and table creation actions are logged to the path defined in `appsettings.json`.

**Example connection string in `appsettings.json`:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "LogsPath": "Logs/log-.txt",
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DocosoftStringConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=Docosoft1;Integrated Security=True;TrustServerCertificate=True;"
  }
}
