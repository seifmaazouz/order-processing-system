namespace OrderProcessing.Domain.Entities
{
    public class Publisher
    {
        public int PubID { private set; get; } // Primary key
        public string Name { private set; get; }
        public string Address { private set; get; }
        public string PhoneNumber { private set; get; }

        // Navigation property (publisher:book => one-to-many relationship)
        public ICollection<Book> Books { get; } = new List<Book>();

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