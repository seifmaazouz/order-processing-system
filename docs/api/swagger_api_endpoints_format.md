# 📘 Order Processing System – API Endpoints (Swagger‑Ready)

This document lists all REST API endpoints required by the project specification in a **Swagger/OpenAPI‑friendly table format**.

---

## 🔐 Authentication & Accounts

| Method | Endpoint | Role | Request Body | Response | Notes |
|------|--------|------|--------------|----------|-------|
| POST | /api/auth/register | Customer | RegisterCustomerDto | 201 Created | Create new customer account |
| POST | /api/auth/login | Admin, Customer | LoginDto | AuthTokenDto | Returns JWT/session token |
| POST | /api/auth/logout | Customer | — | 204 No Content | Clears shopping cart |

---

## 👤 Customer Profile

| Method | Endpoint | Role | Request Body | Response | Notes |
|------|--------|------|--------------|----------|-------|
| PUT | /api/customers/me | Customer | UpdateCustomerDto | 204 No Content | Edit profile & password |

---

## 📚 Books

### Add & Modify Books (Admin)

| Method | Endpoint | Role | Request Body | Response | Notes |
|------|--------|------|--------------|----------|-------|
| POST | /api/books | Admin | BookCreateDto | BookDetailsDto | Add new book with threshold |
| PUT | /api/books/{isbn} | Admin | BookUpdateDto | 204 No Content | Modify existing book |
| PATCH | /api/books/{isbn}/stock | Admin | UpdateStockDto | 204 No Content | Trigger enforces no negative stock |

---

### Search Books (Admin & Customer)

| Method | Endpoint | Role | Query Params | Response | Notes |
|------|--------|------|--------------|----------|-------|
| GET | /api/books/search | Admin, Customer | isbn, title, category, author, publisher | BookDetailsDto[] | Returns availability |

---

## 🏭 Publisher Orders (Replenishment)

| Method | Endpoint | Role | Request Body | Response | Notes |
|------|--------|------|--------------|----------|-------|
| POST | /api/orders/{orderId}/confirm | Admin | — | 204 No Content | Adds ordered quantity to stock |

📌 Order placement is **automatic via DB trigger** when stock drops below threshold.

---

## 🛒 Shopping Cart

| Method | Endpoint | Role | Request Body | Response | Notes |
|------|--------|------|--------------|----------|-------|
| POST | /api/cart/items | Customer | AddCartItemDto | 204 No Content | Add book to cart |
| GET | /api/cart | Customer | — | CartDetailsDto | Shows items & total |
| DELETE | /api/cart/items/{isbn} | Customer | — | 204 No Content | Remove item |

---

## 💳 Checkout

| Method | Endpoint | Role | Request Body | Response | Notes |
|------|--------|------|--------------|----------|-------|
| POST | /api/cart/checkout | Customer | CheckoutDto | OrderSummaryDto | Deducts stock & creates sale |

---

## 📦 Customer Orders

| Method | Endpoint | Role | Query Params | Response | Notes |
|------|--------|------|--------------|----------|-------|
| GET | /api/orders/me | Customer | — | OrderDetailsDto[] | View past orders |

---

## 📊 Reports (Admin Only)

| Method | Endpoint | Response | Notes |
|------|--------|----------|-------|
| GET | /api/reports/sales/previous-month | SalesReportDto | Total sales last month |
| GET | /api/reports/sales/by-date | SalesReportDto | Query: date |
| GET | /api/reports/top-customers | CustomerSalesDto[] | Last 3 months |
| GET | /api/reports/top-books | BookSalesDto[] | Last 3 months |
| GET | /api/reports/book-orders/{isbn} | int | Replenishment count |

---

## 📌 Design Notes

- Controllers **only return DTOs**
- Entities remain inside Domain
- Integrity enforced by **DB constraints & triggers**
- Categories & Publishers are reference data (no CRUD)

---

✔ Ready for Swagger / OpenAPI conversion

