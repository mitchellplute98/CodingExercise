using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;
using CodingExercise.Services;
using CodingExercise.Models;

namespace CodingExercise.Tests;

public class InvestmentServiceTests
{
    private readonly Mock<ILogger<InvestmentService>> _mockLogger;
    private readonly InvestmentService _service;

    public InvestmentServiceTests()
    {
        _mockLogger = new Mock<ILogger<InvestmentService>>();
        _service = new InvestmentService(_mockLogger.Object);
    }

    // Tests that the service returns investment data for a valid existing user.
    // Should return a non-empty list with valid investment summaries.
    [Fact]
    public async Task GetUserInvestments_ExistingUser_ReturnsInvestments()
    {
        var userId = "user1";

        var result = await _service.GetUserInvestments(userId);

        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(investment => 
        {
            investment.Id.Should().BePositive();
            investment.Name.Should().NotBeNullOrEmpty();
        });
    }

    // Tests that the service returns an empty list for users that don't exist.
    // Should return an empty collection rather than throwing an exception.
    [Fact]
    public async Task GetUserInvestments_NonExistentUser_ReturnsEmptyList()
    {
        var userId = "nonexistentuser";

        var result = await _service.GetUserInvestments(userId);

        result.Should().BeEmpty();
    }

    // Tests that user ID matching is case-insensitive.
    // Should return same results for "user1" and "USER1".
    [Fact]
    public async Task GetUserInvestments_CaseInsensitive_ReturnsInvestments()
    {
        var userId = "USER1"; // Uppercase version

        var result = await _service.GetUserInvestments(userId);

        result.Should().NotBeEmpty();
    }

    // Tests that the service returns complete investment details for valid investment IDs.
    [Fact]
    public async Task GetInvestmentDetails_ExistingInvestment_ReturnsDetails()
    {
        var investmentId = 1; // Apple investment

        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(investmentId);
        result.Name.Should().Be("Apple");
        result.Shares.Should().BePositive();
        result.CostBasisPerShare.Should().BePositive();
        result.CurrentPrice.Should().BePositive();
        result.CurrentValue.Should().BePositive();
        result.Term.Should().BeOneOf("Short Term", "Long Term");
    }

    // Tests that the service returns null for investment IDs that don't exist.
    [Fact]
    public async Task GetInvestmentDetails_NonExistentInvestment_ReturnsNull()
    {
        var investmentId = 999;

        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().BeNull();
    }

    // Tests that investments with gains show positive total gain/loss values.
    [Theory]
    [InlineData(1)] // Apple - should be profitable
    [InlineData(3)] // Google - should be profitable  
    [InlineData(5)] // Meta - should be profitable
    public async Task GetInvestmentDetails_ProfitableInvestments_HasPositiveGains(int investmentId)
    {
        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        result!.TotalGainLoss.Should().BePositive();
        result.CurrentValue.Should().BeGreaterThan(result.Shares * result.CostBasisPerShare);
    }

    // Tests that investments with losses show negative total gain/loss values.
    [Theory]
    [InlineData(2)] // Microsoft - should be loss
    [InlineData(4)] // Tesla - should be loss
    public async Task GetInvestmentDetails_LossInvestments_HasNegativeGains(int investmentId)
    {
        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        result!.TotalGainLoss.Should().BeNegative();
        result.CurrentValue.Should().BeLessThan(result.Shares * result.CostBasisPerShare);
    }

    // Tests that current value calculation is correct (shares × current price).
    [Fact]
    public async Task GetInvestmentDetails_CalculatesCurrentValueCorrectly()
    {
        var investmentId = 1; // Apple investment

        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        var expectedCurrentValue = result!.Shares * result.CurrentPrice;
        result.CurrentValue.Should().Be(expectedCurrentValue);
    }

    // Tests that total gain/loss calculation is correct (current value - total cost).
    [Fact]
    public async Task GetInvestmentDetails_CalculatesTotalGainLossCorrectly()
    {
        var investmentId = 1; // Apple investment

        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        var totalCost = result!.Shares * result.CostBasisPerShare;
        var expectedGainLoss = result.CurrentValue - totalCost;
        result.TotalGainLoss.Should().Be(expectedGainLoss);
    }

    // Tests that investments held for more than 365 days are classified as "Long Term".
    [Theory]
    [InlineData(1)] // Apple - Long term (400+ days)
    [InlineData(3)] // Google - Long term (600+ days)
    public async Task GetInvestmentDetails_LongTermInvestments_ReturnsLongTerm(int investmentId)
    {
        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        result!.Term.Should().Be("Long Term");
    }

    // Tests that investments held for 365 days or less are classified as "Short Term".
    [Theory]
    [InlineData(2)] // Microsoft - Short term (200 days)
    [InlineData(4)] // Tesla - Short term (150 days)
    [InlineData(5)] // Meta - Short term (30 days)
    public async Task GetInvestmentDetails_ShortTermInvestments_ReturnsShortTerm(int investmentId)
    {
        var result = await _service.GetInvestmentDetails(investmentId);

        result.Should().NotBeNull();
        result!.Term.Should().Be("Short Term");
    }
}
