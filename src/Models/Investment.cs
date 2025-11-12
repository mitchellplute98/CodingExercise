using System.ComponentModel.DataAnnotations;

namespace CodingExercise.Models
{
    /// <summary>
    /// Represents an investment entity
    /// </summary>
    public class Investment
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "Shares must be positive")]
        public decimal Shares { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Cost basis must be positive")]
        public decimal CostBasisPerShare { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Current price must be positive")]
        public decimal CurrentPrice { get; set; }
        
        public DateTime PurchaseDate { get; set; }
    }
}
