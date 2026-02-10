using System.ComponentModel.DataAnnotations.Schema;

namespace CryptocurrencyExchange.Core.Models
{
    public class FutureHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; } //id must be equel FutureId
        public int FutureId { get; set; }
        public double MarkPrice { get; set; }
        public bool IsLiquidated { get; set; }

    }
}
