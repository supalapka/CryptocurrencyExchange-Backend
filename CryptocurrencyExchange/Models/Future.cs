﻿namespace CryptocurrencyExchange.Models
{
    public class Future
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Margin { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsLiquidated { get; set; }
        public int Leverage { get; set; }
        public PositionType Position { get; set; }
    }
}