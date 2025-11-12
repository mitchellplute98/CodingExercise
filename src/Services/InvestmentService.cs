using CodingExercise.Models;
using CodingExercise.Services;

namespace CodingExercise.Services
{
    /// <summary>
    /// Service for managing investment operations and calculations
    /// </summary>
    public class InvestmentService : IInvestmentService
    {
        private readonly ILogger<InvestmentService> _logger;
        
        // Mock data for demonstration - in production this would come from a database
        private readonly List<Investment> _investments;

        public InvestmentService(ILogger<InvestmentService> logger)
        {
            _logger = logger;
            _investments = GenerateMockData();
        }

        public async Task<IEnumerable<InvestmentSummary>> GetUserInvestments(string userId)
        {
            try
            {
                _logger.LogInformation("Getting investments for user: {UserId}", userId);
                
                var userInvestments = _investments
                    .Where(i => i.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                    .Select(i => new InvestmentSummary
                    {
                        Id = i.Id,
                        Name = i.Name
                    })
                    .ToList();

                _logger.LogInformation("Found {Count} investments for user: {UserId}", userInvestments.Count, userId);
                
                return await Task.FromResult(userInvestments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting investments for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<InvestmentDetails?> GetInvestmentDetails(int investmentId)
        {
            try
            {
                _logger.LogInformation("Getting investment details for investmentId: {InvestmentId}", investmentId);
                
                var investment = _investments.FirstOrDefault(i => i.Id == investmentId);

                if (investment == null)
                {
                    _logger.LogWarning("Investment not found for investmentId: {InvestmentId}", investmentId);
                    return null;
                }

                var details = CalculateInvestmentDetails(investment);
                
                _logger.LogInformation("Successfully calculated investment details for investmentId: {InvestmentId}", investmentId);
                
                return await Task.FromResult(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting investment details for investmentId: {InvestmentId}", investmentId);
                throw;
            }
        }

        private InvestmentDetails CalculateInvestmentDetails(Investment investment)
        {
            var currentValue = investment.Shares * investment.CurrentPrice;
            var totalCost = investment.Shares * investment.CostBasisPerShare;
            var totalGainLoss = currentValue - totalCost;
            var term = (DateTime.Now - investment.PurchaseDate).Days <= 365 ? "Short Term" : "Long Term";

            return new InvestmentDetails
            {
                Id = investment.Id,
                Name = investment.Name,
                Shares = investment.Shares,
                CostBasisPerShare = investment.CostBasisPerShare,
                CurrentValue = currentValue,
                CurrentPrice = investment.CurrentPrice,
                Term = term,
                TotalGainLoss = totalGainLoss
            };
        }

        private List<Investment> GenerateMockData()
        {
            return new List<Investment>
            {
                new Investment
                {
                    Id = 1,
                    Name = "Apple",
                    UserId = "user1",
                    Shares = 100,
                    CostBasisPerShare = 150.00m,
                    CurrentPrice = 175.50m,
                    PurchaseDate = DateTime.Now.AddDays(-400),
                },
                new Investment
                {
                    Id = 2,
                    Name = "Microsoft",
                    UserId = "user1",
                    Shares = 50,
                    CostBasisPerShare = 300.00m,
                    CurrentPrice = 285.75m,
                    PurchaseDate = DateTime.Now.AddDays(-200),
                },
                new Investment
                {
                    Id = 3,
                    Name = "Google",
                    UserId = "user1",
                    Shares = 200,
                    CostBasisPerShare = 400.00m,
                    CurrentPrice = 420.25m,
                    PurchaseDate = DateTime.Now.AddDays(-600),
                },
                new Investment
                {
                    Id = 4,
                    Name = "Tesla",
                    UserId = "user2",
                    Shares = 25,
                    CostBasisPerShare = 800.00m,
                    CurrentPrice = 750.00m,
                    PurchaseDate = DateTime.Now.AddDays(-150),
                },
                new Investment
                {
                    Id = 5,
                    Name = "Meta",
                    UserId = "user1",
                    Shares = 10,
                    CostBasisPerShare = 1000.00m,
                    CurrentPrice = 1025.50m,
                    PurchaseDate = DateTime.Now.AddDays(-30),
                }
            };
        }
    }
}
