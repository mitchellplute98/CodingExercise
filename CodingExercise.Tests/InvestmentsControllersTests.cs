using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;
using System.Net;
using System.Text.Json;
using CodingExercise.Controllers;
using CodingExercise.Services;
using CodingExercise.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodingExercise.Tests;

public class InvestmentsControllerTests
{
    private readonly Mock<IInvestmentService> _mockService;
    private readonly Mock<ILogger<InvestmentsController>> _mockLogger;
    private readonly InvestmentsController _controller;

    public InvestmentsControllerTests()
    {
        _mockService = new Mock<IInvestmentService>();
        _mockLogger = new Mock<ILogger<InvestmentsController>>();
        _controller = new InvestmentsController(_mockService.Object, _mockLogger.Object);
    }

    // Tests the happy path where a valid userId returns investment data.
    // Should return HTTP 200 with a list of investment summaries.
    [Fact]
    public async Task GetUserInvestments_ValidUserId_ReturnsOkWithInvestments()
    {
        var userId = "user1";
        var expectedInvestments = new List<InvestmentSummary>
        {
            new() { Id = 1, Name = "Apple" },
            new() { Id = 2, Name = "Microsoft" }
        };

        _mockService.Setup(s => s.GetUserInvestments(userId))
                   .ReturnsAsync(expectedInvestments);

        var result = await _controller.GetUserInvestments(userId);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        okResult.Value.Should().BeEquivalentTo(expectedInvestments);
    }

    // Tests how the controller handles an empty string userId.
    // Should return a bad request stating User ID is required.
    [Fact]
    public async Task GetUserInvestments_EmptyUserId_ReturnsBadRequest()
    {
        var userId = "";

        var result = await _controller.GetUserInvestments(userId);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result.Result!;
        badRequestResult.Value.Should().Be("User ID is required");
    }

    // Tests how the controller handles whitespace-only userId.
    // Should return a bad request since whitespace is not a valid user ID.
    [Fact]
    public async Task GetUserInvestments_WhitespaceUserId_ReturnsBadRequest()
    {
        var userId = "   ";

        var result = await _controller.GetUserInvestments(userId);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // Tests how the controller handles unexpected service exceptions.
    // Should catch the exception and return HTTP 500 without exposing internal details.
    [Fact]
    public async Task GetUserInvestments_ServiceThrowsException_ReturnsInternalServerError()
    {
        var userId = "user1";
        _mockService.Setup(s => s.GetUserInvestments(userId))
                   .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetUserInvestments(userId);

        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(500);
    }

    // Tests the happy path for getting detailed investment information.
    // Should return HTTP 200 with complete investment details including calculations.
    [Fact]
    public async Task GetInvestmentDetails_ValidInvestmentId_ReturnsOkWithDetails()
    {
        var investmentId = 1;
        var expectedDetails = new InvestmentDetails
        {
            Id = 1,
            Name = "Apple",
            Shares = 100,
            CostBasisPerShare = 150.00m,
            CurrentValue = 17550.00m,
            CurrentPrice = 175.50m,
            Term = "Long Term",
            TotalGainLoss = 2550.00m
        };

        _mockService.Setup(s => s.GetInvestmentDetails(investmentId))
                   .ReturnsAsync(expectedDetails);

        var result = await _controller.GetInvestmentDetails(investmentId);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        okResult.Value.Should().BeEquivalentTo(expectedDetails);
    }

    // Tests validation for investment ID of zero.
    // Should return HTTP 400 since investment IDs must be positive numbers.
    [Fact]
    public async Task GetInvestmentDetails_InvalidInvestmentId_ReturnsBadRequest()
    {
        var investmentId = 0;

        var result = await _controller.GetInvestmentDetails(investmentId);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result.Result!;
        badRequestResult.Value.Should().Be("Investment ID must be greater than 0");
    }

    // Tests validation for negative investment IDs.
    // Should return HTTP 400 since negative numbers are not valid investment IDs.
    [Fact]
    public async Task GetInvestmentDetails_NegativeInvestmentId_ReturnsBadRequest()
    {
        var investmentId = -1;

        var result = await _controller.GetInvestmentDetails(investmentId);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // Tests behavior when an investment ID doesn't exist in the system.
    // Should return HTTP 404 with a descriptive message about the missing investment.
    [Fact]
    public async Task GetInvestmentDetails_InvestmentNotFound_ReturnsNotFound()
    {
        var investmentId = 999;
        _mockService.Setup(s => s.GetInvestmentDetails(investmentId))
                   .ReturnsAsync((InvestmentDetails?)null);

        var result = await _controller.GetInvestmentDetails(investmentId);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result.Result!;
        notFoundResult.Value.Should().Be($"Investment with ID {investmentId} not found for user");
    }

    // Tests exception handling for the investment details endpoint.
    // Should catch service exceptions and return HTTP 500 with a safe error message.
    [Fact]
    public async Task GetInvestmentDetails_ServiceThrowsException_ReturnsInternalServerError()
    {
        var investmentId = 1;
        _mockService.Setup(s => s.GetInvestmentDetails(investmentId))
                   .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetInvestmentDetails(investmentId);

        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result!;
        objectResult.StatusCode.Should().Be(500);
    }

    // Tests the health check endpoint to ensure the API is operational.
    // Should always return HTTP 200 with status information for monitoring purposes.
    [Fact]
    public void HealthCheck_Always_ReturnsOkWithHealthStatus()
    {
        var result = _controller.HealthCheck();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        
        var healthStatus = okResult.Value;
        healthStatus.Should().NotBeNull();
        
        // Use reflection to check the anonymous object properties
        var statusProperty = healthStatus!.GetType().GetProperty("Status");
        statusProperty?.GetValue(healthStatus).Should().Be("Healthy");
        
        var serviceProperty = healthStatus.GetType().GetProperty("Service");
        serviceProperty?.GetValue(healthStatus).Should().Be("Investment Performance API");
    }
}
