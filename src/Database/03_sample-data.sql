INSERT INTO Publisher (PubName, "Address", PhoneNumber) VALUES
('Pearson', '221B Baker Street, London', '1234567890'),
('OReilly Media', '1005 Gravenstein Highway N, Sebastopol', '0987654321'),
('Penguin Books', '80 Strand, London', '1122334455');

INSERT INTO Book (ISBN, Title, PublicationYear, SellingPrice, Quantity, Threshold, Category, PubID) VALUES
('978-0-123456-01-0','Physics 101',2020,50.00,100,10,'Science',1),
('978-0-123456-02-7','Art History',2019,70.00,50,5,'Art',2),
('978-0-123456-03-4','World Religion',2021,60.00,80,10,'Religion',3),
('978-0-123456-04-1','History of Europe',2018,40.00,60,5,'History',2),
('978-0-123456-05-8','Geography Basics',2022,55.00,70,10,'Geography',1),
('978-0-123456-06-5','Chemistry Fundamentals',2023,65.00,90,8,'Science',1),
('978-0-123456-07-2','Modern Art Techniques',2020,75.00,40,4,'Art',2),
('978-0-123456-08-9','Eastern Religions',2019,55.00,85,9,'Religion',3),
('978-0-123456-09-6','Ancient Civilizations',2017,45.00,65,6,'History',2),
('978-0-123456-10-2','World Geography',2021,60.00,75,7,'Geography',1),
('978-0-123456-11-9','Biology Essentials',2022,70.00,95,10,'Science',1),
('978-0-123456-12-6','Renaissance Art',2018,80.00,35,3,'Art',2),
('978-0-123456-13-3','Philosophy of Religion',2020,50.00,70,8,'Religion',3),
('978-0-123456-14-0','Medieval History',2016,35.00,55,5,'History',2),
('978-0-123456-15-7','Climate and Weather',2023,58.00,80,9,'Geography',1),
('978-0-123456-16-4','Quantum Physics',2024,85.00,45,5,'Science',1),
('978-0-123456-17-1','Digital Art',2021,90.00,30,2,'Art',2),
('978-0-123456-18-8','Comparative Religion',2019,62.00,78,7,'Religion',3),
('978-0-123456-19-5','Colonial History',2015,42.00,60,6,'History',2),
('978-0-123456-20-1','Urban Geography',2022,67.00,72,8,'Geography',1),
('978-0-123456-21-8','Organic Chemistry',2023,78.00,88,9,'Science',1),
('978-0-123456-22-5','Abstract Expressionism',2020,95.00,25,2,'Art',2),
('978-0-123456-23-2','Mythology and Religion',2021,53.00,82,8,'Religion',3),
('978-0-123456-24-9','World War II History',2018,48.00,68,7,'History',2),
('978-0-123456-25-6','Geological Sciences',2024,72.00,76,8,'Geography',1);

INSERT INTO BookAuthor (ISBN, AuthorName) VALUES
('978-0-123456-01-0', 'John Smith'),
('978-0-123456-02-7', 'Alice Brown'),
('978-0-123456-03-4', 'David Johnson'),
('978-0-123456-04-1', 'William Thompson'),
('978-0-123456-05-8', 'Maria Rodriguez'),
('978-0-123456-06-5', 'Emma Wilson'),
('978-0-123456-07-2', 'Michael Davis'),
('978-0-123456-08-9', 'Sarah Miller'),
('978-0-123456-09-6', 'James Garcia'),
('978-0-123456-10-2', 'Linda Martinez'),
('978-0-123456-11-9', 'Robert Anderson'),
('978-0-123456-12-6', 'Patricia Taylor'),
('978-0-123456-13-3', 'Christopher Thomas'),
('978-0-123456-14-0', 'Jennifer Hernandez'),
('978-0-123456-15-7', 'Daniel Moore'),
('978-0-123456-16-4', 'Nancy Jackson'),
('978-0-123456-17-1', 'Mark White'),
('978-0-123456-18-8', 'Lisa Harris'),
('978-0-123456-19-5', 'Paul Clark'),
('978-0-123456-20-1', 'Karen Lewis'),
('978-0-123456-21-8', 'Steven Robinson'),
('978-0-123456-22-5', 'Betty Walker'),
('978-0-123456-23-2', 'George Hall'),
('978-0-123456-24-9', 'Helen Young'),
('978-0-123456-25-6', 'Jason King');

-- User accounts with hashed passwords (plain text passwords shown in comments)
-- alice: password123
-- bob: password123
-- admin1: admin123
-- admin2: password123
-- charlie: password123
INSERT INTO "User" (Username, "Password", FirstName, LastName, ShipAddress, Email, PhoneNumber, "Role")
VALUES
('alice', 'AQAAAAIAAYagAAAAEMD+TC4vGWXdx6oWqwp1f3HiGx3HjvLiGVzaHELWgzPN8CtLUQfxoKGEH3fDnnK8nw==', 'Alice', 'Smith', '123 Main St', 'alice@example.com', '1234567890', 'Customer'),
('bob', 'AQAAAAIAAYagAAAAEN0d8QzVjQq7XMGYQH8eLkqO9vHfRvP8qNkMjXlGdKQ2tQK6V5wKQ8VtqQX3Hh8jBw==', 'Bob', 'Johnson', '456 Oak Ave', 'bob@example.com', '2345678901', 'Customer'),
('admin1', 'AQAAAAIAAYagAAAAEGL1pP+69sDtJqgi1d1bUpgVjbLjPP3IIvYq353cIskTaORz/66zdUnobNyrIfqU8A==', 'Admin', 'One', '789 Pine Rd', 'admin1@example.com', '3456789012', 'Admin'),
('admin2', 'AQAAAAIAAYagAAAAEN0d8QzVjQq7XMGYQH8eLkqO9vHfRvP8qNkMjXlGdKQ2tQK6V5wKQ8VtqQX3Hh8jBw==', 'Admin', 'Two', '101 Maple St', 'admin2@example.com', '4567890123', 'Admin'),
('charlie', 'AQAAAAIAAYagAAAAEN0d8QzVjQq7XMGYQH8eLkqO9vHfRvP8qNkMjXlGdKQ2tQK6V5wKQ8VtqQX3Hh8jBw==', 'Charlie', 'Brown', '202 Elm St', 'charlie@example.com', '5678901234', 'Customer');

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
('978-0-123456-02-7',1,1,70.00),
('978-0-123456-03-4',2,3,60.00),
('978-0-123456-04-1',2,1,40.00),
('978-0-123456-01-0',3,1,50.00),
('978-0-123456-05-8',4,2,55.00),
('978-0-123456-02-7',5,1,70.00),
('978-0-123456-03-4',6,1,60.00),
('978-0-123456-04-1',7,1,40.00),
('978-0-123456-05-8',8,1,55.00),
('978-0-123456-01-0',9,1,50.00),
('978-0-123456-02-7',10,1,70.00),
('978-0-123456-03-4',11,1,60.00),
('978-0-123456-04-1',12,1,40.00),
('978-0-123456-05-8',13,1,55.00),
('978-0-123456-01-0',14,1,50.00),
('978-0-123456-02-7',15,1,70.00),
('978-0-123456-03-4',16,1,60.00),
('978-0-123456-04-1',17,1,40.00),
('978-0-123456-05-8',18,1,55.00),
('978-0-123456-01-0',19,1,50.00);