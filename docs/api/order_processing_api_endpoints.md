# 📘 Order Processing System – REST API Endpoints

This document extracts **clean, requirement-driven REST API endpoints** directly from the project PDF (Database Systems – Fall 2025).

The API supports **Administrators** and **Customers** and follows a layered architecture:
**Controller → Service → Repository → Database**

---

## 🔐 Authentication & Accounts

### Register (Customer)
```
POST /api/auth/register
```
**Input (DTO)**
- username
- password
- firstName
- lastName
- email
- phone
- shippingAddress

**Output**
- 201 Created

---

### Login (Admin / Customer)
```
POST /api/auth/login
```
**Input**
- username
- password

**Output**
- JWT / session token

---

### Logout (Customer)
```
POST /api/auth/logout
```
**Effect**
- Clears shopping cart

---

## 👤 Customer Profile

### Edit Personal Information
```
PUT /api/customers/me
```
**Input (DTO)**
- password (optional)
- email
- phone
- shippingAddress

---

## 📚 Books

### Add New Book (Admin Only)
```
POST /api/books
```
**Input (BookCreateDto)**
- isbn
- title
- publicationYear
- sellingPrice
- quantity
- threshold
- catId
- pubId

**Output (BookDetailsDto)**
- isbn
- title
- authors
- category
- publisher
- sellingPrice
- quantity

📌 DTOs are returned — entities are never exposed

---

### Modify Existing Book (Admin Only)
```
PUT /api/books/{isbn}
```
**Input (BookUpdateDto)**
- title
- sellingPrice
- threshold
- catId
- pubId

---

### Update Stock After Sale
```
PATCH /api/books/{isbn}/stock
```
**Input**
- quantitySold

📌 Stock cannot become negative (DB trigger enforced)

---

### Search for Books (Admin & Customer)
```
GET /api/books/search
```
**Query Parameters**
- isbn
- title
- category
- author
- publisher

**Output (BookDetailsDto[])**
- book details
- availability (quantity)

---

## 🏭 Publisher Orders (Replenishment)

### Automatic Order Placement
```
(No direct API – handled by DB trigger)
```
📌 Trigger fires when stock drops below threshold
📌 Order quantity is fixed

---

### Confirm Publisher Order (Admin Only)
```
POST /api/orders/{orderId}/confirm
```
**Effect**
- Order status → Confirmed
- Quantity added to book stock

---

## 🛒 Shopping Cart (Customer)

### Add Book to Cart
```
POST /api/cart/items
```
**Input**
- isbn
- quantity

---

### View Shopping Cart
```
GET /api/cart
```
**Output**
- items
- individual prices
- total price

---

### Remove Item from Cart
```
DELETE /api/cart/items/{isbn}
```

---

## 💳 Checkout

### Checkout Cart
```
POST /api/cart/checkout
```
**Input**
- creditCardNumber
- expiryDate

**Effect**
- Validates credit card
- Deducts book stock
- Records sale transaction

---

## 📦 Customer Orders

### View Past Orders
```
GET /api/orders/me
```
**Output**
- orderNo
- orderDate
- books (ISBN + title)
- totalPrice

---

## 📊 Reports (Admin Only)

### Total Sales – Previous Month
```
GET /api/reports/sales/previous-month
```

---

### Total Sales – Specific Day
```
GET /api/reports/sales/by-date?date=YYYY-MM-DD
```

---

### Top 5 Customers (Last 3 Months)
```
GET /api/reports/top-customers
```

---

### Top 10 Selling Books (Last 3 Months)
```
GET /api/reports/top-books
```

---

### Total Times a Book Was Ordered (Replenishment)
```
GET /api/reports/book-orders/{isbn}
```

---

## ✅ Design Notes (Aligned with Project PDF)

- DTOs are returned from controllers
- Entities remain inside the domain layer
- Categories and Publishers are reference data (no CRUD APIs required)
- Integrity is enforced using:
  - Foreign keys
  - Constraints
  - Database triggers
- Triggers handle:
  - Preventing negative stock
  - Automatic replenishment orders

---

📄 **Use this file directly in your project report or as API documentation for the team.**

