# 📘 Order Processing System – REST API Endpoints

This document extracts **clean, requirement-driven REST API endpoints** directly from the project PDF (Database Systems – Fall 2025).

The API supports **Administrators** and **Customers** and follows a layered architecture:
**Controller → Service → Repository → Database**

---

## 🔐 Authentication & Accounts

### Register (Customer)
```
POST /api/user/register
```
**Input (DTO)**
- username
- password
- firstName
- lastName
- email
- phone

**Output**
- 201 Created

---

### Login (Admin / Customer)
```
POST /api/user/login
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
PUT /api/user/me
```
**Input (DTO)**
- password 
- username

---

## 📚 Books

### Get Book by ISBN (Admin & Customer)
```
GET /api/books/{isbn}
```
**Output (BookDetailsDto)**
- isbn
- title
- year
- price
- stock
- category (enum: Science, Art, Religion, History, Geography)
- publisher
- authors
- isAvailable (boolean)

**Responses**
- 200 OK - Book found
- 404 Not Found - Book not found

---

### Get All Books (Admin & Customer)
```
GET /api/books
```
**Output**
- Array of BookDetailsDto

**Response**
- 200 OK

---

### Add New Book (Admin Only)
```
POST /api/books
```
**Input (CreateBookDto)**
- isbn (required)
- title (required)
- publicationYear (required)
- sellingPrice (required)
- quantity (required)
- threshold (required)
- category (required, enum: Science, Art, Religion, History, Geography)
- pubID (required)
- authors (required, array of strings)

**Output (BookDetailsDto)**
- Full book details with location header

**Responses**
- 201 Created - Book created successfully
- 400 Bad Request - Validation errors
- 409 Conflict - Book with ISBN already exists

📌 DTOs are returned — entities are never exposed

---

### Modify Existing Book (Admin Only)
```
PUT /api/books/{isbn}
```
**Input (UpdateBookDto)** - All fields optional
- title
- publicationYear
- sellingPrice
- quantity
- threshold
- category (enum)
- pubID
- authors (null = no change, empty = error, list = update)

**Responses**
- 204 No Content - Update successful
- 400 Bad Request - Validation errors (e.g., negative stock blocked by trigger)
- 404 Not Found - Book not found

📌 Stock cannot become negative (DB trigger enforced)

---

### Delete Book (Admin Only)
```
DELETE /api/books/{isbn}
```
**Responses**
- 204 No Content - Delete successful
- 404 Not Found - Book not found

---

### Search for Books (Admin & Customer)
```
GET /api/books/search
```
**Query Parameters** - All optional
- isbn (string) - Partial match, ignores dashes
- title (string) - Case-insensitive partial match
- category (string) - Exact match: Science, Art, Religion, History, Geography
- author (string) - Case-insensitive partial match
- publisher (string) - Case-insensitive partial match

**Output**
- Array of BookDetailsDto with availability

**Response**
- 200 OK

**Example**
```
GET /api/books/search?category=Science&publisher=Oxford
```

---

### Get Books Below Threshold (Admin Only)
```
GET /api/books/below-threshold
```
**Output**
- Array of BookDetailsDto for books where quantity < threshold

**Response**
- 200 OK

📌 Useful for monitoring inventory and verifying automatic order triggers

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

