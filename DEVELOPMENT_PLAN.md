# 📚 Order Processing System Development Plan

This plan integrates the **Clean/Onion Architecture** development flow with the specific **Database Systems Project** requirements (Order Processing System).

---

## Phase 0: 🚀 Setup and Environment (1 Week)

The goal is to establish the basic structure, environment consistency, and developer workflow.

| Step | Task | Output/Goal | Responsibility |
| :--- | :--- | :--- | :--- |
| **0.1** | **Monorepo Structure** | Create the core folder structure (`src/Backend/`, `src/Frontend/`, `src/Database/`). | Backend Lead |
| **0.2** | **Project Scaffolding** | Create the four .NET projects (`.Api`, `.Application`, `.Domain`, `.Infrastructure`) and set up cross-project references enforcing the **Dependency Rule**. | Backend Lead |
| **0.3** | **Docker Compose Setup** | Create `docker-compose.yml` for **MySQL Database**, **Backend API**, and **React Frontend**. | DevOps |
| **0.4** | **Initial Database** | Write and test the initial `1_schema_creation.sql` script (basic tables like `Books`, `Publishers`, `Orders`, `Sales Transactions`, `User Accounts`, and `Shopping Carts`). Ensure the MySQL container runs and executes the script on startup. | Database Lead |
| **0.5** | **Git Workflow** | Finalize the **root-level** `.gitignore` and commit the initial empty structure. | All |

---

## Phase 1: 🧅 Backend Core & Data Design

This phase establishes the foundational data contracts and persistence mechanisms.

| Step | Task | Layer | Database Relations | Specific Operations |
| :--- | :--- | :--- | :--- | :--- |
| **1.0** | **Database Design (ERD & Schema)** | *Design* | Analyze all entities and relationships (1-to-many, many-to-many) required for Books, Authors, Publishers, Customers, Orders, Carts, and Sales. | Create the **ER Diagram** and derive the initial **Relational Schema**. |
| **1.1** | **Domain Entities** | `Domain` | Define core **Entities** for **Book** (with ISBN, title, author(s), price, threshold, category), **Publisher**, **Customer**, **Order**, **Sales Transaction**, and **Shopping Cart**. | Define the list of book categories: "Science", "Art", "Religion", "History", "Geography". |
| **1.2** | **Repository Interfaces** | `Domain` | Define interfaces for data access (e.g., `IBookRepository`, `ICustomerRepository`, `IOrderRepository`). | Interface method for **Admin: Add New Books**. |
| **1.3** | **DB Context Setup** | `Infrastructure` | Configure DB Context and map all Entities to the database tables, ensuring **full integrity validation**. | Implement methods for **Admin: Add New Books** including threshold. |
| **1.4** | **Repository Implementation** | `Infrastructure` | Implement the concrete repository classes. | Implement methods for **Customer Sign Up** (including all personal info) and general **Search for Books** by ISBN/Title, Category, Author, or Publisher. |
| **1.5** | **DI and Configuration** | `Api` | Configure Dependency Injection to map all Domain interfaces to Infrastructure implementations. | Load database connection and configuration settings. |

---

## Phase 2: 🛍️ Application Logic and Triggers (Checkout & Stock)

This phase builds the main business logic and crucial database integrity rules, adhering to the project's trigger requirements.

| Step | Task | Layer | Database Relations | Specific Operations |
| :--- | :--- | :--- | :--- | :--- |
| **2.1** | **DTO & Use Cases** | `Application` | Define DTOs for `CheckoutRequest`, `BookUpdateDto`, and `OrderConfirmDto`. | Implement use case for **Admin: Modify Existing Books**. |
| **2.2** | **Shopping Cart Service** | `Application` | Implement `ShoppingCartService.cs` methods to **Add/View/Remove items** from the cart and view prices/totals. | Implement method for **Customer: Edit personal information**. |
| **2.3** | **API Controller** | `Api` | Create `ShoppingCartController.cs` with `POST /api/checkout` endpoint. | Implement **Check out a shopping cart** logic, including the credit card placeholder check. |
| **2.4** | **Triggers/SQL Logic (Stock Management)** | `Database` | Implement `2_triggers.sql`: **Trigger BEFORE UPDATE** on books to prevent negative stock quantity. | Implement **Trigger AFTER UPDATE** on books to automatically place an order from publishers when stock drops below the defined threshold. |
| **2.5** | **Admin Order Logic** | `Application`/`Api` | Implement the logic for **Admin: Confirm Orders**. This must automatically add the ordered quantity to stock upon confirmation, changing order status to Confirmed. |

---

## Phase 3: 🌐 Frontend and Integration

This phase focuses on the user interface and connecting it to the backend contract using automated generation.

| Step | Task | Layer | Specific Operations | Output/Goal |
| :--- | :--- | :--- | :--- | :--- |
| **3.1** | **Client Generation Setup** | `Api` | Integrate **Swashbuckle.AspNetCore** and **NSwag.MSBuild** into `OrderProcessing.Api.csproj`. | Automated `ApiAgent.ts` generated into `src/Frontend/` after successful build. |
| **3.2** | **Book Search UI** | `Frontend` | Implement search fields and display book details/availability. | Functional search page for both Admin and Customer users. |
| **3.3** | **User Authentication UI** | `Frontend` | Implement **Sign Up**, **Log In**, and **Logout** (which removes current cart items). | Secured routes for Admin-only operations. |
| **3.4** | **Checkout Integration** | `Frontend` | Implement the final review screen and secure submission of checkout data to the API. | Successful end-to-end checkout flow resulting in stock deduction. |
| **3.5** | **Docker Finalization** | Orchestration | Finalize network settings, port exposure, and build commands for all three services. | All services running and communicating correctly. |

---

## Phase 4: 📈 Reporting and Final Polish

The final phase adds complexity, reporting features, and completes documentation as required for the project submission.

| Step | Task | Layer | Specific Operations | Output/Goal |
| :--- | :--- | :--- | :--- | :--- |
| **4.1** | **Report SQL Logic** | `Infrastructure` | Implement complex reports in `SqlFiles/` for Admin reports: **Total Sales** (previous month/specific day), **Top 5 Customers** (Last 3 Months), and **Top 10 Selling Books** (Last 3 Months). | Code for **Total Times a Specific Book Has Been Ordered** (replenishment orders). |
| **4.2** | **Reporting Service** | `Application`/`Api` | Create `ReportService.cs` and corresponding API endpoints to expose all five **Admin Only** reports. | Functioning Admin Reports screen in the UI. |
| **4.3** | **Customer Past Orders** | `Frontend`/`Api` | Implement logic for the Customer to **View past orders** in detail (order no, date, books ISBN, total price, etc.). | Completed UI screen showing detailed order history. |
| **4.4** | **Sample Data** | `Database` | Populate `3_sample_data.sql` with sufficient data to fully demonstrate all features, including reports and trigger logic. | Fully populated database ready for demo. |
| **4.5** | **Documentation** | Repository | Prepare the final **Project Report** covering implemented features, ERD, Relational Schema, and UI logic descriptions. | Project ready for submission. |