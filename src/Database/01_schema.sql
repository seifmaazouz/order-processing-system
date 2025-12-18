CREATE TYPE category_enum AS ENUM ('Science','Art','Religion','History','Geography');
CREATE TYPE order_status_enum AS ENUM ('Confirmed','Pending','Canceled');
CREATE TYPE role_enum AS ENUM ('Customer', 'Admin');

CREATE TABLE Category (
    CatID SERIAL PRIMARY KEY,
    CatName category_enum NOT NULL
);

CREATE TABLE Publisher (
    PubID SERIAL PRIMARY KEY,
    PubName VARCHAR(50) NOT NULL,
    "Address" VARCHAR(100),
    PhoneNumber VARCHAR(15)
);

CREATE TABLE Book (
    ISBN VARCHAR(17) PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    PublicationYear INT,
    SellingPrice DECIMAL(10,2),
    Quantity INT NOT NULL,
    Threshold INT NOT NULL,
    CatID INT NOT NULL REFERENCES Category(CatID),
    PubID INT NOT NULL REFERENCES Publisher(PubID)
);

CREATE TABLE BookAuthor (
    ISBN VARCHAR(17) NOT NULL REFERENCES Book(ISBN),
    AuthorName VARCHAR(50) NOT NULL,
    PRIMARY KEY (ISBN, AuthorName)
);

CREATE TABLE "User" (
    Username VARCHAR(50) PRIMARY KEY,
    "Password" VARCHAR(50) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    ShipAddress VARCHAR(100) NOT NULL,
    Email VARCHAR(50),
    PhoneNumber VARCHAR(15),
    "Role" role_enum NOT NULL
);

CREATE TABLE "Order" (
    OrderID SERIAL PRIMARY KEY,
    OrderDate DATE NOT NULL,
    "Status" order_status_enum,
    TotalPrice DECIMAL(10,2) NOT NULL,
    PubID INT NOT NULL REFERENCES Publisher(PubID),
    CustName VARCHAR(50) NOT NULL REFERENCES "User"(Username)
);

CREATE TABLE CreditCard (
    CardNum BIGINT PRIMARY KEY,
    ExpiryDate DATE NOT NULL,
    CustName VARCHAR(50) NOT NULL REFERENCES "User"(Username)
);

CREATE TABLE ShoppingCart (
    CartID SERIAL PRIMARY KEY,
    CustName VARCHAR(50) NOT NULL REFERENCES "User"(Username)
);

CREATE TABLE OrderItem (
    ISBN VARCHAR(17) NOT NULL REFERENCES Book(ISBN),
    OrderNum INT NOT NULL REFERENCES "Order"(OrderID),
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    PRIMARY KEY (ISBN, OrderNum)
);

CREATE TABLE CartItem (
    ISBN VARCHAR(17) NOT NULL REFERENCES Book(ISBN),
    CartID INT NOT NULL REFERENCES ShoppingCart(CartID),
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    PRIMARY KEY (ISBN, CartID)
);