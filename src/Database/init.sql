CREATE DATABASE OrdProSys;

USE DATABASE OrdProSys;

CREATE TYPE category_enum AS ENUM ('Science', 'Art', 'Religion', 'History', 'Geography');
CREATE TYPE order_status_enum AS ENUM ('Confirmed', 'Pending', 'Canceled');

CREATE TABLE Category (
    CatID SERIAL PRIMARY KEY,
    CatName category_enum NOT NULL
);

CREATE TABLE Publisher (
    PubID SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Address" VARCHAR(100),
    PhoneNumber VARCHAR(15)
);

CREATE TABLE Book (
    ISBN SERIAL PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    PubYear INT,
    SellingPrice DECIMAL(10,2),
    Quantity INT NOT NULL,
    Threshold INT NOT NULL,
    CatID INT NOT NULL,
    PubID INT NOT NULL,
    FOREIGN KEY (CatID) REFERENCES Category(CatID),
    FOREIGN KEY (PubID) REFERENCES Publisher(PubID)
);

CREATE TABLE BookAuthor (
    ISBN INT NOT NULL,
    AuthorName VARCHAR(50) NOT NULL,
    PRIMARY KEY (ISBN, AuthorName),
    FOREIGN KEY (ISBN) REFERENCES Books(ISBN)
);

CREATE TABLE Customer (
    Username VARCHAR(50) PRIMARY KEY,
    "Password" VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    ShipAddress VARCHAR(100) NOT NULL,
    Email VARCHAR(50),
    PhoneNumber VARCHAR(15)
);

CREATE TABLE "Order" (
    OrderID SERIAL PRIMARY KEY,
    OrderDate DATE NOT NULL,
    "Status" order_status_enum,
    TotalPrice DECIMAL(10,2) NOT NULL,
    PubID INT NOT NULL,
    CustName VARCHAR(50) NOT NULL,
    FOREIGN KEY (PubID) REFERENCES Publisher(PubID),
    FOREIGN KEY (CustName) REFERENCES Customer(Username)
);

CREATE TABLE CreditCard (
    CardNum BIGINT UNIQUE NOT NULL,
    ExpiryDate DATE NOT NULL,
    CustName VARCHAR(50) NOT NULL,
    FOREIGN KEY (CustName) REFERENCES Customer(Username)
);

CREATE TABLE ShoppingCart (
    CartID SERIAL PRIMARY KEY,
    CustName VARCHAR(50) NOT NULL,
    FOREIGN KEY (CustName) REFERENCES Customer(Username)
);

CREATE TABLE OrderItem (
    ISBN INT NOT NULL,
    OrderNum INT NOT NULL,
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    PRIMARY KEY (ISBN, OrderNum),
    FOREIGN KEY (ISBN) REFERENCES Books(ISBN),
    FOREIGN KEY (OrderNum) REFERENCES "Order"(OrderID)
);

CREATE TABLE CartItem (
    ISBN INT NOT NULL,
    CartID INT NOT NULL,
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    PRIMARY KEY (ISBN, CartID),
    FOREIGN KEY (ISBN) REFERENCES Books(ISBN),
    FOREIGN KEY (CartID) REFERENCES ShoppingCart(CartID)
);
