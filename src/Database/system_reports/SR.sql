--a)
-- all sales are orders that are confirmed and within one month
SELECT OrderID, OrderDate, TotalPrice
FROM CustomerOrder 
WHERE "Status" = 'Confirmed' 
  AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
  AND OrderDate <= CURRENT_DATE;
-- this select returns the total revenue from all the sales
SELECT SUM(TotalPrice) AS totalPrice
FROM CustomerOrder
WHERE "Status" = 'Confirmed' 
  AND OrderDate >= CURRENT_DATE - INTERVAL '1 month' 
  AND OrderDate <= CURRENT_DATE;

--b)
-- all sales are orders that are confirmed and within the day of the inserted date
-- admin that inserts the date inserts it here
SELECT OrderID, OrderDate, TotalPrice
FROM CustomerOrder
WHERE "Status" = 'Confirmed'
  AND OrderDate = "insert_date here from backend";
-- this select returns the total revenue from all the sales on that day
SELECT SUM(TotalPrice) AS totalPrice
FROM CustomerOrder
WHERE "Status" = 'Confirmed'
  AND OrderDate = "insert_date here from backend";

--c)
SELECT CustName, SUM(TotalPrice) AS Revenue_From_Customer
FROM CustomerOrder
WHERE "Status" = 'Confirmed' 
  AND OrderDate >= CURRENT_DATE - INTERVAL '3 months' 
  AND OrderDate <= CURRENT_DATE
GROUP BY CustName
ORDER BY Revenue_From_Customer DESC
LIMIT 5;

--d)
SELECT b.ISBN, b.Title, SUM(oi.Quantity) AS Total_No_of_Copies
FROM CustomerOrderItem AS oi
JOIN Book AS b ON b.ISBN = oi.ISBN
JOIN CustomerOrder AS o ON o.OrderID = oi.OrderNum
WHERE o."Status" = 'Confirmed' 
  AND o.OrderDate >= CURRENT_DATE - INTERVAL '3 months' 
  AND o.OrderDate <= CURRENT_DATE
GROUP BY b.ISBN, b.Title
ORDER BY Total_No_of_Copies DESC
LIMIT 10;

--e)
SELECT SUM(Quantity) AS Total_Books_Ordered
FROM AdminOrderItem
WHERE ISBN = 'insert_ISBN_here'