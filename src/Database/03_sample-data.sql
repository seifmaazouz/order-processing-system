INSERT INTO Publisher (PubName, "Address", PhoneNumber) VALUES
('Pearson', '221B Baker Street, London', '1234567890'),
('OReilly Media', '1005 Gravenstein Highway N, Sebastopol', '0987654321'),
('Penguin Books', '80 Strand, London', '1122334455');

INSERT INTO Book (ISBN, Title, PublicationYear, SellingPrice, Quantity, Threshold, Category, PubID) VALUES
('978-3-16-148410-0', 'Physics Fundamentals', 2020, 59.99, 100, 20, 'Science', 1),
('978-1-23-456789-7', 'Modern Art Guide', 2018, 39.50, 50, 10, 'Art', 2),
('978-0-12-345678-9', 'World Religions', 2019, 49.00, 30, 5, 'Religion', 3);

INSERT INTO BookAuthor (ISBN, AuthorName) VALUES
('978-3-16-148410-0', 'John Smith'),
('978-1-23-456789-7', 'Alice Brown'),
('978-0-12-345678-9', 'David Johnson');

INSERT INTO "User" (Username, "Password", FirstName, LastName, ShipAddress, Email, PhoneNumber, "Role")
VALUES 
('alice', 'password123', 'Alice', 'Smith', '123 Main St', 'alice@example.com', '1234567890', 'Customer'),
('bob', 'securepass', 'Bob', 'Johnson', '456 Oak Ave', 'bob@example.com', '2345678901', 'Customer'),
('admin1', 'adminpass', 'Admin', 'One', '789 Pine Rd', 'admin1@example.com', '3456789012', 'Admin'),
('admin2', 'adminpass', 'Admin', 'Two', '101 Maple St', 'admin2@example.com', '4567890123', 'Admin'),
('charlie', 'charliepwd', 'Charlie', 'Brown', '202 Elm St', 'charlie@example.com', '5678901234', 'Customer');

INSERT INTO "Order" (OrderDate, "Status", TotalPrice, PubID, CustName) VALUES
('2025-12-01', 'Confirmed', 119.98, 1, 'alice'),
('2025-12-02', 'Pending', 39.50, 2, 'bob');

INSERT INTO OrderItem (ISBN, OrderNum, Quantity, UnitPrice) VALUES
('978-3-16-148410-0', 1, 2, 59.99),
('978-1-23-456789-7', 2, 1, 39.50);

INSERT INTO CreditCard (CardNumber, ExpiryDate) VALUES
(4111111111111111, '2027-12-31'),
(4222222222222222, '2026-06-30');

INSERT INTO CardHolder (CardNumber, Username) VALUES
(4111111111111111, 'alice'),
(4222222222222222, 'bob');

INSERT INTO ShoppingCart (CustName) VALUES
('alice'),
('bob');

INSERT INTO CartItem (ISBN, CartID, Quantity, UnitPrice) VALUES
('978-0-12-345678-9', 1, 1, 49.00),
('978-3-16-148410-0', 2, 3, 59.99);
