INSERT INTO Publisher (PubName, "Address", PhoneNumber) VALUES
('Pearson', '221B Baker Street, London', '1234567890'),
('OReilly Media', '1005 Gravenstein Highway N, Sebastopol', '0987654321'),
('Penguin Books', '80 Strand, London', '1122334455');

INSERT INTO Book (ISBN, Title, PublicationYear, SellingPrice, Quantity, Threshold, Category, PubID) VALUES
('978-0-123456-01-0','Physics 101',2020,50.00,100,10,'Science',1),
('978-0-123456-02-7','Art History',2019,70.00,50,5,'Art',2),
('978-0-123456-03-4','World Religion',2021,60.00,80,10,'Religion',3),
('978-0-123456-04-1','History of Europe',2018,40.00,60,5,'History',2),
('978-0-123456-05-8','Geography Basics',2022,55.00,70,10,'Geography',1);

INSERT INTO BookAuthor (ISBN, AuthorName) VALUES
('978-0-123456-01-0', 'John Smith'),
('978-0-123456-02-7', 'Alice Brown'),
('978-0-123456-03-4', 'David Johnson');

INSERT INTO "User" (Username, "Password", FirstName, LastName, ShipAddress, Email, PhoneNumber, "Role")
VALUES 
('alice', 'password123', 'Alice', 'Smith', '123 Main St', 'alice@example.com', '1234567890', 'Customer'),
('bob', 'securepass', 'Bob', 'Johnson', '456 Oak Ave', 'bob@example.com', '2345678901', 'Customer'),
('admin1', 'AQAAAAIAAYagAAAAEGL1pP+69sDtJqgi1d1bUpgVjbLjPP3IIvYq353cIskTaORz/66zdUnobNyrIfqU8A==', 'Admin', 'One', '789 Pine Rd', 'admin1@example.com', '3456789012', 'Admin'),
('admin2', 'adminpass', 'Admin', 'Two', '101 Maple St', 'admin2@example.com', '4567890123', 'Admin'),
('charlie', 'charliepwd', 'Charlie', 'Brown', '202 Elm St', 'charlie@example.com', '5678901234', 'Customer');

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
('978-0-123456-01-0', 1, 1, 50.00),
('978-0-123456-02-7', 2, 3, 70.00);

INSERT INTO AdminOrder (OrderDate, "Status", TotalPrice, PubID, CustName) VALUES
('2020-01-15','Confirmed',170.00,1,'admin1'),
('2020-03-22','Confirmed',60.00,2,'admin2'),
('2021-05-10','Pending',175.00,3,'admin1'),
('2021-07-18','Confirmed',50.00,1,'admin2'),
('2022-09-05','Confirmed',200.00,2,'admin1'),
('2022-11-12','Confirmed',150.00,3,'admin2'),
('2023-02-28','Confirmed',150.00,1,'admin1'),
('2023-04-14','Pending',70.00,2,'admin2'),
('2023-06-30','Confirmed',160.00,3,'admin1'),
('2023-08-25','Confirmed',55.00,1,'admin2'),
('2024-10-08','Confirmed',120.00,2,'admin1'),
('2024-12-01','Confirmed',115.00,3,'admin2'),
('2020-02-20','Pending',80.00,1,'admin1'),
('2020-04-17','Confirmed',160.00,2,'admin2'),
('2021-06-09','Confirmed',70.00,3,'admin1'),
('2021-08-21','Confirmed',165.00,1,'admin2'),
('2022-10-03','Confirmed',120.00,2,'admin1'),
('2022-12-19','Confirmed',90.00,3,'admin2'),
('2023-01-11','Pending',195.00,1,'admin1'),
('2024-11-27','Confirmed',60.00,2,'admin2');

INSERT INTO AdminOrderItem (ISBN, OrderNum, Quantity, UnitPrice) VALUES
('978-0-123456-01-0',1,2,50.00),
('978-0-123456-02-7',1,1,70.00),
('978-0-123456-03-4',2,1,60.00),
('978-0-123456-04-1',3,3,40.00),
('978-0-123456-05-8',3,1,55.00),
('978-0-123456-01-0',4,1,50.00),
('978-0-123456-02-7',5,2,70.00),
('978-0-123456-03-4',5,1,60.00),
('978-0-123456-04-1',6,1,40.00),
('978-0-123456-05-8',6,2,55.00),
('978-0-123456-01-0',7,3,50.00),
('978-0-123456-02-7',8,1,70.00),
('978-0-123456-03-4',9,2,60.00),
('978-0-123456-04-1',9,1,40.00),
('978-0-123456-05-8',10,1,55.00),
('978-0-123456-01-0',11,1,50.00),
('978-0-123456-02-7',11,1,70.00),
('978-0-123456-03-4',12,1,60.00),
('978-0-123456-05-8',12,1,55.00),
('978-0-123456-04-1',13,2,40.00),
('978-0-123456-01-0',14,2,50.00),
('978-0-123456-03-4',14,1,60.00),
('978-0-123456-02-7',15,1,70.00),
('978-0-123456-05-8',16,3,55.00),
('978-0-123456-03-4',17,2,60.00),
('978-0-123456-04-1',18,1,40.00),
('978-0-123456-01-0',18,1,50.00),
('978-0-123456-02-7',19,2,70.00),
('978-0-123456-05-8',19,1,55.00),
('978-0-123456-03-4',20,1,60.00);

INSERT INTO CustomerOrder (OrderDate, "Status", TotalPrice, CustName) VALUES
('2025-12-01','Confirmed',150.00,'alice'),
('2025-12-02','Confirmed',210.00,'bob'),
('2025-11-28','Pending',120.00,'charlie'),
('2025-11-30','Confirmed',90.00,'alice'),
('2025-11-25','Canceled',50.00,'bob'),
('2025-12-10','Confirmed',200.00,'charlie'),
('2025-10-15','Confirmed',75.00,'alice'),
('2025-10-20','Confirmed',180.00,'bob'),
('2025-12-05','Confirmed',60.00,'charlie'),
('2025-12-07','Confirmed',130.00,'bob'),
('2025-11-15','Confirmed',110.00,'alice'),
('2025-11-18','Pending',140.00,'charlie'),
('2025-12-12','Confirmed',95.00,'alice'),
('2025-12-13','Confirmed',85.00,'bob'),
('2025-12-14','Confirmed',120.00,'charlie'),
('2025-12-15','Confirmed',150.00,'alice'),
('2025-12-16','Pending',90.00,'bob'),
('2025-12-17','Confirmed',175.00,'charlie'),
('2025-12-18','Confirmed',60.00,'alice'),
('2025-12-19','Confirmed',80.00,'bob');

INSERT INTO CustomerOrderItem (ISBN, OrderNum, Quantity, UnitPrice) VALUES
('978-0-123456-01-0',1,2,50.00),
('978-0-123456-02-7',1,1,50.00),
('978-0-123456-03-4',2,3,70.00),
('978-0-123456-04-1',2,1,0.00),
('978-0-123456-01-0',4,1,90.00),
('978-0-123456-05-8',6,2,100.00),
('978-0-123456-02-7',8,1,180.00),
('978-0-123456-03-4',10,1,130.00),
('978-0-123456-04-1',11,2,55.00),
('978-0-123456-05-8',12,1,140.00),
('978-0-123456-01-0',13,1,95.00),
('978-0-123456-02-7',14,1,85.00),
('978-0-123456-03-4',15,2,60.00),
('978-0-123456-01-0',17,2,90.00),
('978-0-123456-02-7',18,1,175.00),
('978-0-123456-03-4',19,1,60.00),
('978-0-123456-05-8',20,1,80.00),
('978-0-123456-01-0',3,1,120.00),
('978-0-123456-02-7',5,2,50.00),
('978-0-123456-03-4',7,1,75.00),
('978-0-123456-04-1',9,1,60.00),
('978-0-123456-05-8',10,2,60.00),
('978-0-123456-01-0',12,1,140.00),
('978-0-123456-02-7',13,1,95.00),
('978-0-123456-03-4',14,1,85.00),
('978-0-123456-04-1',16,1,150.00),
('978-0-123456-03-4',1,1,60.00),
('978-0-123456-05-8',19,1,55.00);