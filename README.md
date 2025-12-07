# Order Processing System (Fall 2025 Database Project)

This project implements a simplified online bookstore system using a modern full-stack architecture. The repository is structured as a **Monorepo** containing the React frontend, the layered ASP.NET Core backend, and all necessary database scripts, orchestrated via Docker.

## 1. 🚀 Tech Stack & Core Architecture

| Component | Technology | Role in Architecture |
| :--- | :--- | :--- |
| **Frontend** | React | Presentation Layer (User Interface) |
| **Backend** | ASP.NET Core (C#) | Layered Architecture (API, Application, Core, Infrastructure) |
| **Database** | MySQL | Persistence Layer (Enforcing constraints & triggers) |
| **Orchestration** | Docker / Docker Compose | Containerization for consistent setup |

The backend adheres to a strict **Clean/Onion Layered Architecture** 

<p align="center">
<img src="https://raw.githubusercontent.com/NilavPatel/dotnet-onion-architecture/main/docs/dotnet-onion-architecture.png">
</p>

## 2. 🏛️ Backend Workflow & Separation of Folders

The backend is split into four distinct projects (layers) that govern the flow of control and data. This structure strictly enforces the **Dependency Rule**: **Domain** is the innermost layer (no dependencies), and **Infrastructure** and **Application** depend only on **Domain**.

### A. Project Layers & Contents

| Layer | Project Name | Type of Logic | Key Contents | Dependencies |
| :--- | :--- | :--- | :--- | :--- |
| **Presentation** | `OrderProcessing.Api` | API Endpoints, Configuration | **Controllers** (e.g., `ShoppingCartController`), `appsettings.json`, Program Startup. | Application, Domain |
| **Application** | `OrderProcessing.Application` | **Business Logic** (Orchestration, Use Cases) | Service **Implementations** (`BookService.cs`, `ReportService.cs`), **DTOs** (Input/Output Models). | Domain |
| **Domain** | `OrderProcessing.Domain` | **Domain Contracts** (Entities, Interfaces) | **Entities** (`Book.cs`, `Customer.cs`), **Interfaces** (`IBookRepository.cs`, `IReportService.cs`). | None |
| **Infrastructure**| `OrderProcessing.Infrastructure`| **Data Access**, External I/O | Repository **Implementations** (`BookRepository.cs`), `SqlFiles/` (complex reports), DB Context setup. | Domain |

### B. Workflow: How a Request is Processed (Example: Customer Checkout)

1.  **Api Layer:** The `ShoppingCartController` receives the request and calls the **Application Layer interface** (`_checkoutService.ExecuteCheckout(...)`).
2.  **Application Layer:** The `CheckoutService.cs` executes the **Business Logic**, orchestrating the steps:
    * It checks stock levels (using `IBookRepository`).
    * It enforces business rules (e.g., checking for negative stock).
    * It calls `IOrderRepository` to record the sale transaction.
3.  **Infrastructure Layer:** The concrete `BookRepository.cs` and `OrderRepository.cs` contain the actual code to connect to the MySQL database and execute parameterized SQL commands.
4.  **Database Layer:** MySQL executes the transaction, utilizing the defined **Triggers** to automatically deduct stock and potentially place a replenishment order if the stock falls below the threshold.

---

## 3. 📁 Repository Folder Structure

This is the overall layout of the repository, showing the placement of source code and Docker setup files.

```
order-processing-system/
├── src/
│   ├── Frontend/                          # React Application Source Code
│   │   ├── Dockerfile                     # Build instructions for the React app
│   │   └── src/                           # Components, pages, API service calls
│   │
│   ├── Backend/                           # .NET Solution Projects
│   │   ├── Dockerfile                     # Build instructions for the .NET API
│   │   ├── OrderProcessing.Api/           # API Endpoints, Startup, Controllers
│   │   ├── OrderProcessing.Application/   # Business Logic, Services, DTOs
│   │   ├── OrderProcessing.Domain/        # Entities and Interfaces
│   │   └── OrderProcessing.Infrastructure/
│   │       ├── Data/                      # Repository Implementations (MySQL logic)
│   │       └── SqlFiles/                  # Complex, externalized SQL queries
│   │
│   └── Database/                          # MySQL Setup Scripts
│       ├── 1_schema_creation.sql          # All CREATE TABLE statements
│       ├── 2_triggers.sql                 # Stock replenishment, Negative stock protection
│       └── 3_sample_data.sql              # Data for demonstration
│
├── Project_DB_Fall2025.pdf                # TA instructions and project task description
├── docker-compose.yml                     # Orchestrates Frontend, Backend, and DB services
└── README.md                              # This document
```