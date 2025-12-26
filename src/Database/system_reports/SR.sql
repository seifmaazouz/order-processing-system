--a)
-- all sales are orders that are confirmed and within one month
SELECT OrderID, OrderDate, "Status", TotalPrice
FROM "Order"
WHERE "Status" = 'Confirmed' 
  AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
  AND OrderDate <= CURRENT_DATE;
-- this select returns the total revenue from all the sales
SELECT SUM(TotalPrice) AS totalPrice
FROM "Order"
WHERE "Status" = 'Confirmed' 
  AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
  AND OrderDate <= CURRENT_DATE;

--b)
-- all sales are orders that are confirmed and within the day of the inserted date
-- admin that inserts the date inserts it here
SELECT OrderID, OrderDate, "Status", TotalPrice
FROM "Order"
WHERE "Status" = 'Confirmed'
  AND OrderDate = "insert_date here from backend";
-- this select returns the total revenue from all the sales on that day
SELECT SUM(TotalPrice) AS totalPrice
FROM "Order"
WHERE "Status" = 'Confirmed'
  AND OrderDate = "insert_date here from backend";

--c)
SELECT CustName, SUM(TotalPrice) AS totalPrice
FROM "Order"
WHERE "Status" = 'Confirmed' 
  AND OrderDate >= CURRENT_DATE - INTERVAL '3 months' 
  AND OrderDate <= CURRENT_DATE
GROUP BY CustName
ORDER BY totalPrice DESC
LIMIT 5;

--d)
SELECT b.ISBN, b.Title, SUM(oi.Quantity) AS Total_No_of_Copies
FROM OrderItem AS oi
JOIN Book AS b ON b.ISBN = oi.ISBN
JOIN "Order" AS o ON o.OrderID = oi.OrderNum
WHERE o."Status" = 'Confirmed' 
  AND o.OrderDate >= CURRENT_DATE - INTERVAL '3 months' 
  AND o.OrderDate <= CURRENT_DATE
GROUP BY b.ISBN, b.Title
ORDER BY Total_No_of_Copies DESC
LIMIT 10;

--e)
SELECT oi.ISBN, COUNT(oi.OrderNum) as Total_No_of_Orders
FROM OrderItem AS oi
JOIN "Order" AS o ON oi.OrderNum = o.OrderID
JOIN "User" AS u ON o.CustName = u.Username
WHERE u."Role" = 'Admin'
GROUP BY oi.ISBN
ORDER BY Total_No_of_Orders DESC;