namespace OrderProcessing.Domain.Entities
{
    public class User
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; } // "Admin" or "Customer"
        public List<CreditCard> Cards { get; private set; } 
        public List<Order> Orders {private set; get;}
        public User(
            string username,
            string email,
            string phoneNumber,
            string firstName,
            string lastName,
            string address,
            string passwordHash,
            string role)
        {
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PasswordHash = passwordHash;
            Role = role;

            Cards = new List<CreditCard>();
            Orders=new List<Order>();
        }
    }
}
