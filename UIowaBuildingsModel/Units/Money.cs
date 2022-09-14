namespace EmissionsMonitorModel.Units
{
    public class Money
    {
        public decimal Amount { get; set; }

        public Currencies? Currency { get; set; }

        public static Money FromUsDollars(decimal amount)
        {
            return new Money { Amount = amount, Currency = Currencies.UsDollar };
        }

        public static Money FromEuros(decimal amount)
        {
            return new Money { Amount = amount, Currency = Currencies.Euro };
        }
    }

    public enum Currencies
    {
        UsDollar,
        Euro
    }
}
