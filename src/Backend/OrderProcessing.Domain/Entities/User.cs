namespace OrderProcessing.Domain.Entities{
    class User
    {
        public int UserId{set;get;}
        public string Role{set;get;}
        public string Email{set;get;}
        public string Username{set;get;}
        public string Password{set; get;}
        public string Hash{set;get;}
        public string Salt{set;get;}

    }
}