namespace OrderProcessing.Domain.Entities
 { 
    public class CardHolder
     { 
        public string CardNumber;
        public string Userame;
        public CardHolder( string cardNumber, string userName )
            {
             CardNumber=cardNumber; 
             Userame=userName;
            } 
    } 
}