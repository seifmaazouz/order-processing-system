# Order Processing System (Fall 2025 Database Project)

This project implements a simplified online bookstore system using a modern full-stack architecture. The repository is structured as a **Monorepo** containing the React frontend, the layered ASP.NET Core 10 backend, and all necessary database scripts, orchestrated via Docker. CI/CD pipelines are managed with **GitHub Actions**.

## 1. 🚀 Tech Stack & Core Architecture

| Component         | Technology              | Role in Architecture                                          |
| :---------------- | :---------------------- | :------------------------------------------------------------ |
| **Frontend**      | React                   | Presentation Layer (User Interface)                           |
| **Backend**       | ASP.NET Core 10 (C#)    | Layered Architecture (API, Application, Core, Infrastructure) |
| **Database**      | PostgreSQL              | Persistence Layer (Enforcing constraints & triggers)          |
| **Orchestration** | Docker / Docker Compose | Containerization for consistent setup                         |
| **CI/CD**         | GitHub Actions          | Build, test, and deployment pipelines                         |

The backend adheres to a strict **Clean/Onion Layered Architecture**

<p align="center">
<img src="https://raw.githubusercontent.com/NilavPatel/dotnet-onion-architecture/main/docs/dotnet-onion-architecture.png">
</p>

---

## 2. 🏛️ Backend Workflow & Separation of Folders

The backend is split into four distinct projects (layers) that govern the flow of control and data. This structure strictly enforces the **Dependency Rule**: **Domain** is the innermost layer (no dependencies), and **Infrastructure** and **Application** depend only on **Domain**.

### A. Project Layers & Contents

| Layer              | Project Name                     | Type of Logic                                 | Key Contents                                                                                                          | Dependencies        |
| :----------------- | :------------------------------- | :-------------------------------------------- | :-------------------------------------------------------------------------------------------------------------------- | :------------------ |
| **Presentation**   | `OrderProcessing.Api`            | API Endpoints, Configuration                  | **Controllers** (e.g., `ShoppingCartController`), `appsettings.json`, Program Startup.                                | Application, Domain |
| **Application**    | `OrderProcessing.Application`    | **Business Logic** (Orchestration, Use Cases) | Service **Implementations** (`BookService.cs`, `ReportService.cs`), **DTOs** (Input/Output Models).                   | Domain              |
| **Domain**         | `OrderProcessing.Domain`         | **Domain Contracts** (Entities, Interfaces)   | **Entities** (`Book.cs`, `Customer.cs`), **Interfaces** (`IBookRepository.cs`, `IReportService.cs`).                  | None                |
| **Infrastructure** | `OrderProcessing.Infrastructure` | **Data Access**, External I/O                 | Repository **Implementations** (`BookRepository.cs`), `SqlFiles/` (complex queries), DB Context setup for PostgreSQL. | Domain              |

### B. Workflow: How a Request is Processed (Example: Customer Checkout)

1. **Api Layer:** The `ShoppingCartController` receives the request and calls the **Application Layer interface** (`_checkoutService.ExecuteCheckout(...)`).
2. **Application Layer:** The `CheckoutService.cs` executes the **Business Logic**, orchestrating the steps:

   * It checks stock levels (using `IBookRepository`).
   * It enforces business rules (e.g., checking for negative stock).
   * It calls `IOrderRepository` to record the sale transaction.
3. **Infrastructure Layer:** The concrete `BookRepository.cs` and `OrderRepository.cs` contain the actual code to connect to the **PostgreSQL database** and execute parameterized SQL commands.
4. **Database Layer:** PostgreSQL executes the transaction, utilizing the defined **Triggers** to automatically deduct stock and potentially place a replenishment order if the stock falls below the threshold.

---

## 3. 📁 Repository Folder Structure

```
order-processing-system/
├── src/
│   ├── Frontend/                          # React Application Source Code
│   │   ├── Dockerfile                     # Build instructions for the React app
│   │   └── src/                           # Components, pages, API service calls
│   │
│   ├── Backend/                           # .NET 10 Solution Projects
│   │   ├── Dockerfile                     # Build instructions for the .NET API
│   │   ├── OrderProcessing.Api/           # API Endpoints, Startup, Controllers
│   │   ├── OrderProcessing.Application/   # Business Logic, Services, DTOs
│   │   ├── OrderProcessing.Domain/        # Entities and Interfaces
│   │   └── OrderProcessing.Infrastructure/
│   │       ├── Data/                      # Repository Implementations (PostgreSQL logic)
│   │       └── SqlFiles/                  # Complex, externalized SQL queries
│   │
│   └── Database/                          # PostgreSQL Setup Scripts
│       ├── 1_schema_creation.sql          # All CREATE TABLE statements
│       ├── 2_triggers.sql                 # Stock replenishment, Negative stock protection
│       └── 3_sample_data.sql              # Data for demonstration
│
├── .github/workflows/                     # GitHub Actions pipelines
│   ├── build-backend.yml                  # Build/test .NET backend
│   ├── build-frontend.yml                 # Build/test React frontend
│   ├── deploy.yml                         # Optional deployment workflow
│   ├── restrict-main.yml                   # Workflow to restrict merges to main
│   └── restrict-dev.yml                    # Workflow to restrict merges to dev
│
├── Project_DB_Fall2025.pdf                # TA instructions and project task description
├── docker-compose.yml                     # Orchestrates Frontend, Backend, and PostgreSQL services
└── README.md                              # This document
```

---

## 4. ⚡ CI/CD Pipelines (GitHub Actions)

* **Backend pipeline (`build-backend.yml`)**

  * Runs `dotnet build`, `dotnet test`
  * Publishes artifacts for deployment

* **Frontend pipeline (`build-frontend.yml`)**

  * Installs dependencies, runs `npm build` and tests

* **Deployment pipeline (`deploy.yml`)**

  * Builds Docker images for frontend, backend, and PostgreSQL
  * Pushes images to registry (optional)
  * Updates staging/production environment via Docker Compose

* **Merge restriction workflows:**

  * **`restrict-main.yml`**: Allows PRs only from `dev` or `hotfix/*` to `main`
  * **`restrict-dev.yml`**: Allows PRs only from `backend/feature|hotfix|bugfix/*`, `database/feature|hotfix|bugfix/*`, `frontend/feature|hotfix|bugfix/*`, or `misc/*` to `dev`

---

## 5. 📌 Notes

* Backend is developed using **.NET 10** with a **Clean/Onion Architecture**
* Database is **PostgreSQL** (instead of MySQL)
* CI/CD is managed with **GitHub Actions**
* Docker ensures consistent setup across environments
* All SQL scripts are designed for PostgreSQL compatibility
* GitHub Actions enforce branch protection, automated tests, and deployment
* Frontend and backend are containerized for consistent development and deployment
* Follow the folder structure conventions to maintain the layered architecture integrity
* Workflows `restrict-main.yml` and `restrict-dev.yml` enforce branch merge restrictions
