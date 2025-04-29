# ExternalUserServiceSolution

## Overview
This solution demonstrates a clean and professional .NET Core service component that interacts with an external API (https://reqres.in) to fetch, process, cache, and handle user data.

It simulates real-world scenarios where a platform needs to integrate with external systems reliably.

---

## Projects
- ExternalUserService — (.NET 6 Class Library)
  - Core business logic: API client, service layer, models, configuration handling, error handling, resilience (retry policies), caching.
- ExternalUserService.Console — (Console Application)
  - Demonstrates how to use the class library with Dependency Injection (DI).
- ExternalUserService.Tests — (Unit Test Project - optional)
  - Contains basic unit test examples using xUnit and Moq.

---

## Features
- ✅ Fetch single user details by ID from external API.
- ✅ Fetch all users across paginated results automatically.
- ✅ Retry transient network failures using Polly.
- ✅ In-memory caching with configurable expiration time.
- ✅ API Base URL and Cache Duration are configurable via `appsettings.json`.
- ✅ Full asynchronous (`async/await`) codebase.
- ✅ Clean architecture principles (separation of models, services, configuration).
- ✅ Proper error handling for HTTP failures and deserialization errors.
- ✅ Unit tests for service methods (xUnit + Moq).

---

## How to Run

1. Clone or Download the repository.

2. Open the solution `ExternalUserServiceSolution.sln` in Visual Studio 2022+.

3. Set Startup Project:  
   Right-click ExternalUserService.Console ➔ Set as Startup Project.

4. Check appsettings.json (already included):
   json
   {
     "ApiSettings": {
       "BaseUrl": "https://reqres.in/api",
       "CacheDurationMinutes": 5
     }
   }
