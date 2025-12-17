# 💻 Backend Project Setup Guide (Clean / Onion Architecture)

This guide shows the commands used to set up the four Clean Architecture projects and define the **correct dependency relationships** using the .NET CLI, based on the solution name: **`OrderProcessing.sln`**.

---

## 1. 📂 Initial Folder Structure & Solution Creation

We create the main backend directory and the solution file, ensuring the final solution file is named `OrderProcessing.sln`.

| Command                                 | Purpose                                              |
| :-------------------------------------- | :--------------------------------------------------- |
| `mkdir src/Backend`                     | Creates the main directory for the .NET solution.    |
| `cd src/Backend`                        | Navigates into the new directory.                    |
| **`dotnet new sln -n OrderProcessing`** | **Creates the `OrderProcessing.sln` solution file.** |

---

## 2. 🧱 Creating and Adding the Four Projects

We create the projects for each layer and immediately add them to the `OrderProcessing.sln`.

| Project              | Layer Role                                     | Command                                                 |
| :------------------- | :--------------------------------------------- | :------------------------------------------------------ |
| **Domain**           | Core Business Entities & Interfaces            | `dotnet new classlib -n OrderProcessing.Domain`         |
| **Application**      | Business Logic Orchestration & Use Cases       | `dotnet new classlib -n OrderProcessing.Application`    |
| **Infrastructure**   | Data Access & External Services Implementation | `dotnet new classlib -n OrderProcessing.Infrastructure` |
| **API/Presentation** | Web API Entry Point & Controllers              | `dotnet new webapi  -n OrderProcessing.Api`             |

Add projects to the solution:

```bash
dotnet sln add OrderProcessing.Domain/OrderProcessing.Domain.csproj
dotnet sln add OrderProcessing.Application/OrderProcessing.Application.csproj
dotnet sln add OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj
dotnet sln add OrderProcessing.Api/OrderProcessing.Api.csproj
```

---

## 3. 🔗 Defining Project References (Dependency Rule & Composition Root)

We enforce the **Dependency Rule** of Clean / Onion Architecture:

> **Dependencies always point inward toward the Domain.**  
> The **API project acts as the Composition Root**, responsible for wiring dependencies at application startup.

### Required References

```bash
dotnet add OrderProcessing.Application reference OrderProcessing.Domain
dotnet add OrderProcessing.Infrastructure reference OrderProcessing.Domain
dotnet add OrderProcessing.Api reference OrderProcessing.Application
dotnet add OrderProcessing.Api reference OrderProcessing.Infrastructure
```

---

## 4. 🧠 Architectural Clarification (Important)

Although the **API references Infrastructure**, this **does not violate Clean Architecture** because:

- The API **does NOT use Infrastructure directly in controllers**
- Infrastructure is referenced **only at startup** (`Program.cs`)
- API registers concrete implementations via **Dependency Injection**
- Controllers depend **only on Application and Domain abstractions**

Example (`Program.cs` – Composition Root):

```csharp
using OrderProcessing.Infrastructure;

builder.Services.AddInfrastructure(connectionString);
```

---

## 5. ✅ Final Architecture Guarantees

- ✔ Domain has **no dependencies**
- ✔ Application depends **only on Domain**
- ✔ Infrastructure depends **only on Domain**
- ✔ API acts as the **Composition Root**
- ✔ All data access uses **pure SQL with Dapper**
- ✔ No Entity Framework or ORM leakage into higher layers
