\c ordprosys 

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'category_enum') THEN
        CREATE TYPE category_enum AS ENUM ('Science', 'Art', 'Religion', 'History', 'Geography');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'order_status_enum') THEN
        CREATE TYPE order_status_enum AS ENUM ('Confirmed', 'Pending', 'Canceled');
    END IF;
END$$;

CREATE TABLE IF NOT EXISTS Category (
    CatID SERIAL PRIMARY KEY,
    CatName category_enum NOT NULL
);

CREATE TABLE IF NOT EXISTS Publisher (
    PubID SERIAL PRIMARY KEY,
    PubName VARCHAR(50) NOT NULL,
    "Address" VARCHAR(100),
    PhoneNumber VARCHAR(15)
);

CREATE TABLE IF NOT EXISTS Book (
    ISBN VARCHAR(17) PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    PublicationYear INT,
    SellingPrice DECIMAL(10,2),
    Quantity INT NOT NULL,
    Threshold INT NOT NULL,
    CatID INT NOT NULL REFERENCES Category(CatID),
    PubID INT NOT NULL REFERENCES Publisher(PubID)
);

CREATE TABLE IF NOT EXISTS BookAuthor (
    ISBN VARCHAR(17) NOT NULL REFERENCES Book(ISBN),
    AuthorName VARCHAR(50) NOT NULL,
    PRIMARY KEY (ISBN, AuthorName)
);

CREATE TABLE IF NOT EXISTS Customer (
    Username VARCHAR(50) PRIMARY KEY,
    "Password" VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    ShipAddress VARCHAR(100) NOT NULL,
    Email VARCHAR(50),
    PhoneNumber VARCHAR(15)
);

CREATE TABLE IF NOT EXISTS "Order" (
    OrderID SERIAL PRIMARY KEY,
    OrderDate DATE NOT NULL,
    "Status" order_status_enum,
    TotalPrice DECIMAL(10,2) NOT NULL,
    PubID INT NOT NULL REFERENCES Publisher(PubID),
    CustName VARCHAR(50) NOT NULL REFERENCES Customer(Username)
);

CREATE TABLE IF NOT EXISTS CreditCard (
    CardNum BIGINT PRIMARY KEY,
    ExpiryDate DATE NOT NULL,
    CustName VARCHAR(50) NOT NULL REFERENCES Customer(Username)
);

CREATE TABLE IF NOT EXISTS ShoppingCart (
    CartID SERIAL PRIMARY KEY,
    CustName VARCHAR(50) NOT NULL REFERENCES Customer(Username)
);

CREATE TABLE IF NOT EXISTS OrderItem (
    ISBN VARCHAR(17) NOT NULL REFERENCES Book(ISBN),
    OrderNum INT NOT NULL REFERENCES "Order"(OrderID),
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    PRIMARY KEY (ISBN, OrderNum)
);

CREATE TABLE IF NOT EXISTS CartItem (
    ISBN VARCHAR(17) NOT NULL REFERENCES Book(ISBN),
    CartID INT NOT NULL REFERENCES ShoppingCart(CartID),
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    PRIMARY KEY (ISBN, CartID)
);

CREATE OR REPLACE FUNCTION prevent_negative_quantity()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.Quantity < 0 THEN
        RAISE EXCEPTION 'Book quantity cannot be negative';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_trigger WHERE tgname = 'trg_prevent_negative_quantity'
    ) THEN
        CREATE TRIGGER trg_prevent_negative_quantity
        BEFORE UPDATE OF Quantity
        ON Book
        FOR EACH ROW
        EXECUTE FUNCTION prevent_negative_quantity();
    END IF;
END
$$;

CREATE OR REPLACE FUNCTION auto_order_on_threshold()
RETURNS TRIGGER AS $$
DECLARE
    order_quantity INT := 50;
BEGIN
    IF NEW.Quantity < NEW.Threshold AND OLD.Quantity >= OLD.Threshold THEN
        INSERT INTO "Order"(OrderDate, "Status", TotalPrice, PubID, CustName)
        VALUES (CURRENT_DATE, 'Pending', order_quantity * NEW.SellingPrice, NEW.PubID, 'admin');

        INSERT INTO OrderItem(ISBN, OrderNum, Quantity, UnitPrice)
        VALUES (
            NEW.ISBN,
            currval(pg_get_serial_sequence('"Order"', 'OrderID')),
            order_quantity,
            NEW.SellingPrice
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_trigger WHERE tgname = 'trg_auto_order_on_threshold'
    ) THEN
        CREATE TRIGGER trg_auto_order_on_threshold
        AFTER UPDATE OF Quantity
        ON Book
        FOR EACH ROW
        EXECUTE FUNCTION auto_order_on_threshold();
    END IF;
END
$$;

INSERT INTO Category (CatName) VALUES
('Science'),
('Art'),
('Religion'),
('History'),
('Geography');

INSERT INTO Publisher (PubName, "Address", PhoneNumber) VALUES
('Pearson', '221B Baker Street, London', '1234567890'),
('OReilly Media', '1005 Gravenstein Highway N, Sebastopol', '0987654321'),
('Penguin Books', '80 Strand, London', '1122334455');

INSERT INTO Book (ISBN, Title, PublicationYear, SellingPrice, Quantity, Threshold, CatID, PubID) VALUES
('978-3-16-148410-0', 'Physics Fundamentals', 2020, 59.99, 100, 20, 1, 1),
('978-1-23-456789-7', 'Modern Art Guide', 2018, 39.50, 50, 10, 2, 2),
('978-0-12-345678-9', 'World Religions', 2019, 49.00, 30, 5, 3, 3);

INSERT INTO BookAuthor (ISBN, AuthorName) VALUES
('978-3-16-148410-0', 'John Smith'),
('978-1-23-456789-7', 'Alice Brown'),
('978-0-12-345678-9', 'David Johnson');

INSERT INTO Customer (Username, "Password", FirstName, LastName, ShipAddress, Email, PhoneNumber) VALUES
('john_doe', 'Pass123!', 'John', 'Doe', '123 Main St, City', 'john@example.com', '5551234567'),
('jane_smith', 'Secret456!', 'Jane', 'Smith', '456 Oak St, City', 'jane@example.com', '5559876543');

INSERT INTO "Order" (OrderID, OrderDate, "Status", TotalPrice, PubID, CustName) VALUES
(1, '2025-12-01', 'Confirmed', 119.98, 1, 'john_doe'),
(2, '2025-12-02', 'Pending', 39.50, 2, 'jane_smith');

INSERT INTO OrderItem (ISBN, OrderNum, Quantity, UnitPrice) VALUES
('978-3-16-148410-0', 1, 2, 59.99),
('978-1-23-456789-7', 2, 1, 39.50);

INSERT INTO CreditCard (CardNum, ExpiryDate, CustName) VALUES
(4111111111111111, '2027-12-31', 'john_doe'),
(4222222222222222, '2026-06-30', 'jane_smith');

INSERT INTO ShoppingCart (CartID, CustName) VALUES
(1, 'john_doe'),
(2, 'jane_smith');

INSERT INTO CartItem (ISBN, CartID, Quantity, UnitPrice) VALUES
('978-0-12-345678-9', 1, 1, 49.00),
('978-3-16-148410-0', 2, 3, 59.99);
