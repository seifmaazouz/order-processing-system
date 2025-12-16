namespace OrderProcessing.Domain.Entities
{
    public class Publisher
    {
        public int PubID { private set; get; } // Primary key
        public string Name { private set; get; } = null!;
        public string Address { private set; get; } = null!;
        public string PhoneNumber { private set; get; } = null!;

        // Navigation property (publisher:book => one-to-many relationship)
        public ICollection<Book> Books { get; } = new List<Book>();

        private Publisher() { } // For Dapper
        public Publisher(int pubID, string name, string address, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Publisher name cannot be empty");
            }

            PubID = pubID;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }
    }
}