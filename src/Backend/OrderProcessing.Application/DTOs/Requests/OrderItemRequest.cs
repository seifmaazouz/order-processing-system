using System.Security.Cryptography.X509Certificates;

namespace OrderProcessing.Application.Requests
{
    public record OrderItemRequest(
        public int Quantity,
        public int ISBN
         );
}