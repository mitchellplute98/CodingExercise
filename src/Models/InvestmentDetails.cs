namespace CodingExercise.Models
{
    /// <summary>
    /// Represents detailed investment performance data
    /// </summary>
    public class InvestmentDetails
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Shares { get; set; }
        public decimal CostBasisPerShare { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal CurrentPrice { get; set; }
        public string Term { get; set; } = string.Empty;
        public decimal TotalGainLoss { get; set; }
    }
}
