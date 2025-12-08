# 💻 Backend Project Setup Guide (Clean/Onion Architecture)

This guide shows the commands used to set up your four Clean Architecture projects and define the correct dependencies using the .NET Command Line Interface (CLI), based on your solution name: **`OrderProcessing.sln`**.

## 1. 📂 Initial Folder Structure & Solution Creation

We create the main backend directory and the solution file, ensuring the final solution file is named `OrderProcessing.sln`.

| Command | Purpose |
| :--- | :--- |
| `mkdir src/Backend` | Creates the main directory for the .NET solution. |
| `cd src/Backend` | Navigates into the new directory. |
| **`dotnet new sln -n OrderProcessing`** | **Creates the `OrderProcessing.sln` solution file.** |

---

## 2. 🧱 Creating and Adding the Four Projects

We create the projects for each layer and immediately add them to the new `OrderProcessing.sln`.

| Project | Layer Role | Command |
| :--- | :--- | :--- |
| **Domain** | Core Business Entities & Interfaces | `dotnet new classlib -n OrderProcessing.Domain` |
| **Application** | Business Logic Orchestration & Use Cases | `dotnet new classlib -n OrderProcessing.Application` |
| **Infrastructure** | Data Access & External Services Implementation | `dotnet new classlib -n OrderProcessing.Infrastructure` |
| **API/Presentation** | Web API Entry Point & Controllers | `dotnet new webapi -n OrderProcessing.Api` |

| Command to Add to Solution |
| :--- |
| `dotnet sln add OrderProcessing.Domain/OrderProcessing.Domain.csproj` |
| `dotnet sln add OrderProcessing.Application/OrderProcessing.Application.csproj` |
| `dotnet sln add OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj` |
| `dotnet sln add OrderProcessing.Api/OrderProcessing.Api.csproj` |

---

## 3. 🔗 Defining Project References (The Dependency Rule)

Enforce the **Dependency Rule**: Outer layers must reference inner layers, and the `Domain` layer must have no dependencies.

| Source Project (Dependent) | Target Project (Dependency) | Command Syntax |
| :--- | :--- | :--- |
| **Application** | **Domain** | `dotnet add OrderProcessing.Application/ reference OrderProcessing.Domain/` |
| **Infrastructure** | **Domain** | `dotnet add OrderProcessing.Infrastructure/ reference OrderProcessing.Domain/` |
| **API** | **Application** | `dotnet add OrderProcessing.Api/ reference OrderProcessing.Application/` |
| **API** | **Infrastructure** | `dotnet add OrderProcessing.Api/ reference OrderProcessing.Infrastructure/` |

***Note: The `Domain` project has no outgoing dependencies, preserving its status as the core of the application.***