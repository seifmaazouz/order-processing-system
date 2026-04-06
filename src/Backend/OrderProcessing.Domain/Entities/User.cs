using OrderProcessing.Domain.ValueObjects;

namespace OrderProcessing.Domain.Entities
{
    public class User
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Address { get; private set; }
        public string PasswordHash { get; private set; }
        public UserTypes Role { get; private set; } // Roles defined in ValueObjects

        public User(
            string username,
            string email,
            string phoneNumber,
            string firstName,
            string lastName,
            string passwordHash,
            UserTypes role,
            string address)
        {
            Validate(username, email, phoneNumber, firstName, lastName, address);
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void UpdateProfile(string? email = null, string? phoneNumber = null, string? firstName = null, string? lastName = null, string? address = null)
        {
            var newEmail = email ?? Email;
            var newPhone = phoneNumber ?? PhoneNumber;
            var newFirst = firstName ?? FirstName;
            var newLast = lastName ?? LastName;
            var newAddress = address ?? Address;
            Validate(Username, newEmail, newPhone, newFirst, newLast, newAddress);
            Email = newEmail;
            PhoneNumber = newPhone;
            FirstName = newFirst;
            LastName = newLast;
            Address = newAddress;
        }

        private static void Validate(string username, string email, string phone, string firstName, string lastName, string address)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format.");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number is required.");
            // Support optional leading '+' (international format) followed by digits
            var digitsOnly = phone.StartsWith('+') ? phone.Substring(1) : phone;
            if (!digitsOnly.All(char.IsDigit))
                throw new ArgumentException("Phone number must contain only digits (after optional leading '+').");
            if (digitsOnly.Length < 7 || digitsOnly.Length > 20)
                throw new ArgumentException("Phone number must be 7-20 digits.");
            if (!string.IsNullOrEmpty(address) && address.Length > 500)
                throw new ArgumentException("Address is too long (max 500 characters).");
        }
    }
}
