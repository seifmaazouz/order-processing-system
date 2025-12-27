using System.Security.Cryptography.X509Certificates;

namespace OrderProcessing.Application.DTOs.Requests
{
    public record OrderItemRequest(
         int Quantity,
         string ISBN);
}