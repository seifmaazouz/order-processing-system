namespace OrderProcessing.Domain.Models;

public record CartItemReadModel(
    string ISBN,
    int Quantity,
    decimal UnitPrice
);
