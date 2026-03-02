namespace CryptocurrencyExchange.Exceptions
{
    public class StakingCoinNotFoundException : Exception
    {
        public StakingCoinNotFoundException()
            : base("Staking coin does not avaiable for staking or nor found")
        {
        }

        public StakingCoinNotFoundException(string message)
            : base(message)
        {
        }

        public StakingCoinNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
