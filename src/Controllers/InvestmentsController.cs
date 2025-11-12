using Microsoft.AspNetCore.Mvc;
using CodingExercise.Models;
using CodingExercise.Services;
using System.ComponentModel.DataAnnotations;

namespace CodingExercise.Controllers
{
    /// <summary>
    /// Controller for managing investment operations and performance data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InvestmentsController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;
        private readonly ILogger<InvestmentsController> _logger;

        public InvestmentsController(IInvestmentService investmentService, ILogger<InvestmentsController> logger)
        {
            _investmentService = investmentService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of current investments for the user
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>List of investment summaries containing ID and name</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<InvestmentSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvestmentSummary>>> GetUserInvestments(
            [Required] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("GetUserInvestments called with empty userId");
                    return BadRequest("User ID is required");
                }

                var investments = await _investmentService.GetUserInvestments(userId);
                return Ok(investments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting investments for user: {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get details for a user's specific investment
        /// </summary>
        /// <param name="investmentId">The investment identifier</param>
        /// <returns>Detailed investment performance data</returns>
        [HttpGet("investment/{investmentId}")]
        [ProducesResponseType(typeof(InvestmentDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InvestmentDetails>> GetInvestmentDetails(
            [Required][Range(1, int.MaxValue)] int investmentId)
        {
            try
            {
                if (investmentId <= 0)
                {
                    _logger.LogWarning("GetInvestmentDetails called with invalid investmentId: {InvestmentId}", investmentId);
                    return BadRequest("Investment ID must be greater than 0");
                }

                var investmentDetails = await _investmentService.GetInvestmentDetails(investmentId);
                
                if (investmentDetails == null)
                {
                    _logger.LogInformation("Investment not found for user: investmentId: {InvestmentId}", investmentId);
                    return NotFound($"Investment with ID {investmentId} not found for user");
                }

                return Ok(investmentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting investment details for investmentId: {InvestmentId}", investmentId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Health check endpoint for the investments API
        /// </summary>
        /// <returns>API health status</returns>
        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> HealthCheck()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Service = "Investment Performance API"
            });
        }
    }
}
