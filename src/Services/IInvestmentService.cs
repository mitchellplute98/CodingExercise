using CodingExercise.Models;

namespace CodingExercise.Services
{
    /// <summary>
    /// Interface for investment service operations
    /// </summary>
    public interface IInvestmentService
    {
        Task<IEnumerable<InvestmentSummary>> GetUserInvestments(string userId);
        Task<InvestmentDetails?> GetInvestmentDetails(int investmentId);
    }
}
