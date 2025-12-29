using System;
using System.Threading.Tasks;
using FluentAssertions;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Domain.ValueObjects;
using Xunit;

namespace OrderProcessing.Tests.Services
{
    public class UserServiceProfileValidationTests
    {
        private User GetTestUser() => new User(
            username: "testuser",
            email: "old@example.com",
            phoneNumber: "1234567890",
            firstName: "OldFirst",
            lastName: "OldLast",
            passwordHash: "hash",
            role: UserTypes.Customer,
            address: "Old Address"
        );

        [Fact]
        public void User_Should_Throw_When_Email_Invalid()
        {
            Action act = () => new User("testuser", "invalidemail", "1234567890", "First", "Last", "hash", UserTypes.Customer, "Address");
            act.Should().Throw<ArgumentException>().WithMessage("Invalid email format.");
        }

        [Fact]
        public void User_Should_Throw_When_Phone_Invalid()
        {
            Action act = () => new User("testuser", "test@example.com", "12", "First", "Last", "hash", UserTypes.Customer, "Address");
            act.Should().Throw<ArgumentException>().WithMessage("Phone number must be 7-20 digits.");
        }

        [Fact]
        public void User_Should_Throw_When_FirstName_Empty()
        {
            Action act = () => new User("testuser", "test@example.com", "1234567890", "", "Last", "hash", UserTypes.Customer, "Address");
            act.Should().Throw<ArgumentException>().WithMessage("First name is required.");
        }

        [Fact]
        public void User_Should_Throw_When_LastName_Empty()
        {
            Action act = () => new User("testuser", "test@example.com", "1234567890", "First", "", "hash", UserTypes.Customer, "Address");
            act.Should().Throw<ArgumentException>().WithMessage("Last name is required.");
        }

        [Fact]
        public void User_Should_Throw_When_Email_Empty()
        {
            Action act = () => new User("testuser", "", "1234567890", "First", "Last", "hash", UserTypes.Customer, "Address");
            act.Should().Throw<ArgumentException>().WithMessage("Email is required.");
        }

        [Fact]
        public void User_Should_Throw_When_Phone_Empty()
        {
            Action act = () => new User("testuser", "test@example.com", "", "First", "Last", "hash", UserTypes.Customer, "Address");
            act.Should().Throw<ArgumentException>().WithMessage("Phone number is required.");
        }

        [Fact]
        public void User_Should_Succeed_With_Valid_Data()
        {
            var user = new User("testuser", "test@example.com", "9876543210", "First", "Last", "hash", UserTypes.Customer, "Address");
            user.Email.Should().Be("test@example.com");
            user.PhoneNumber.Should().Be("9876543210");
        }

        [Fact]
        public void UpdateProfile_Should_Throw_When_Email_Invalid()
        {
            var user = GetTestUser();
            Action act = () => user.UpdateProfile(email: "invalidemail");
            act.Should().Throw<ArgumentException>().WithMessage("Invalid email format.");
        }

        [Fact]
        public void UpdateProfile_Should_Throw_When_Phone_Invalid()
        {
            var user = GetTestUser();
            Action act = () => user.UpdateProfile(phoneNumber: "12");
            act.Should().Throw<ArgumentException>().WithMessage("Phone number must be 7-20 digits.");
        }

        [Fact]
        public void UpdateProfile_Should_Throw_When_FirstName_Empty()
        {
            var user = GetTestUser();
            Action act = () => user.UpdateProfile(firstName: "");
            act.Should().Throw<ArgumentException>().WithMessage("First name is required.");
        }

        [Fact]
        public void UpdateProfile_Should_Throw_When_LastName_Empty()
        {
            var user = GetTestUser();
            Action act = () => user.UpdateProfile(lastName: "");
            act.Should().Throw<ArgumentException>().WithMessage("Last name is required.");
        }

        [Fact]
        public void UpdateProfile_Should_Throw_When_Email_Empty()
        {
            var user = GetTestUser();
            Action act = () => user.UpdateProfile(email: "");
            act.Should().Throw<ArgumentException>().WithMessage("Email is required.");
        }

        [Fact]
        public void UpdateProfile_Should_Throw_When_Phone_Empty()
        {
            var user = GetTestUser();
            Action act = () => user.UpdateProfile(phoneNumber: "");
            act.Should().Throw<ArgumentException>().WithMessage("Phone number is required.");
        }

        [Fact]
        public void UpdateProfile_Should_Succeed_With_Valid_Data()
        {
            var user = GetTestUser();
            user.UpdateProfile(email: "new@example.com", phoneNumber: "9876543210", firstName: "NewFirst", lastName: "NewLast", address: "New Address");
            user.Email.Should().Be("new@example.com");
            user.PhoneNumber.Should().Be("9876543210");
            user.FirstName.Should().Be("NewFirst");
            user.LastName.Should().Be("NewLast");
            user.Address.Should().Be("New Address");
        }
    }
}
