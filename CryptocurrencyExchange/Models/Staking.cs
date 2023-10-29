namespace CryptocurrencyExchange.Models
{
    public class Staking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StakingCoinId { get; set; }
        public StakingCoin StakingCoin { get; set; }
        public double Amount { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInMonth { get; set; }
        public bool IsCompleted { get; set; }
    }
}
