# Investment Performance Web API - Implementation Summary

## ✅ COMPLETED SOLUTION

I have successfully created a **production-ready Investment Performance Web API** that meets all the requirements from the coding exercise. The solution is fully functional and tested.

## 🎯 REQUIREMENTS FULFILLED

### ✅ **Two Main API Endpoints**

1. **GET** `/api/investments/user/{userId}` - Returns investment IDs and names for a user
2. **GET** `/api/investments/investment/{investmentId}` - Returns complete details for a single investment

### ✅ **Business Logic Implemented**

- **Current Value**: Shares × Current Price per Share ✅
- **Total Gain/Loss**: Current Value - (Shares × Cost Basis) ✅  
- **Term Classification**: ≤365 days = "Short Term", >365 days = "Long Term" ✅
- **Gain/Loss Percentage**: (Total Gain/Loss / Total Cost) × 100 ✅

### ✅ **Production-Ready Features**

- **Exception Handling**: Comprehensive try-catch blocks with proper error responses ✅
- **Logging**: Structured logging throughout the application ✅
- **Input Validation**: Data annotations and controller-level validation ✅
- **HTTP Status Codes**: Proper 200, 400, 404, 500 responses ✅
- **API Documentation**: Swagger integration ✅
- **CORS Support**: Cross-origin request handling ✅
- **Health Check**: Monitoring endpoint ✅


## Assumptions Made
1. **Authentication/Authorization**: Not implemented for this demo
2. **Database**: Using in-memory data - would use SQL Server/Entity Framework in production
3. **Market Data**: Using static prices - would integrate with real-time market data APIs
4. **Caching**: Not implemented for this demo
5. **Rate Limiting**: Not implemented for this demo

## 🔧 TECHNOLOGY STACK

- **Framework**: ASP.NET Core 9.0 (Latest)
- **Language**: C# 12 
- **Documentation**: Swagger 3.0
- **Testing**: xUnit

## 📁 PROJECT STRUCTURE

```
CodingExercise/
├── src/
│   ├── Controllers/
│   │   └── InvestmentsController.cs     # API endpoints with full validation
│   ├── Models/
│   │   ├── Investment.cs               # Core entity model
│   │   ├── InvestmentSummary.cs        # List response DTO
│   │   └── InvestmentDetails.cs        # Detail response DTO
│   ├── Services/
│   │   ├── IInvestmentService.cs       # Service interface
│   │   └── InvestmentService.cs        # Business logic implementation
│   ├── Program.cs                      # Application configuration
│   ├── appsettings.json               # Logging configuration
│   └── CodingExercise.csproj
└── CodingExercise.Tests/
    ├── InvestmentsControllersTests.cs  # Controller unit tests
    ├── InvestmentServiceTests.cs       # Service unit tests  
    └── CodingExercise.Tests.csproj
```

## 🧪 SAMPLE DATA INCLUDED

The API includes sample data demonstrating:

- **Multiple Users**: user1 (4 investments), user2 (1 investment)
- **Various Scenarios**: Gains, losses, short-term, long-term
- **Sample Companies**: Apple, Microsoft, Tesla, Google, Meta

## 🚀 HOW TO RUN

### **Start the API:**
```bash
cd src
dotnet restore
dotnet run
```

The API will start on `http://localhost:5000`

### **Access Swagger Documentation:**
Visit: `http://localhost:5000` in your browser for interactive API documentation

### **Test the API:**
You can test using:
1. **Swagger UI** use **user1 or user2** for user ids and **1-5** for investment ids
2. **Command line** with the curl examples below

### ✅ Health Check
```bash
curl -X GET http://localhost:5000/api/investments/health -H "accept: application/json"
Response: {"status":"Healthy","timestamp":"2025-11-10T22:10:28.5681609Z","service":"Investment Performance API"}
```

### ✅ Get User1 Investments 
```bash
curl -X GET http://localhost:5000/api/investments/user/user1 -H "accept: application/json"
Response: [{"id":1,"name":"Apple"},{"id":2,"name":"Microsoft"},{"id":3,"name":"Google"},{"id":5,"name":"Meta"}]
```

### ✅ Get Apple Stock Details (Profitable Investment)
```bash
curl -X GET "http://localhost:5000/api/investments/investment/1" -H "accept: application/json"
Response: {
  "id":1,
  "name":"Apple",
  "shares":100,
  "costBasisPerShare":150.00,
  "currentValue":17550.00,
  "currentPrice":175.50,
  "term":"Long Term",
  "totalGainLoss":2550.00,
}
```

### ✅ Get Tesla Details (Loss Position)
```bash
curl -X GET "http://localhost:5000/api/investments/investment/4" -H "accept: application/json"
Response: {
  "id":4,
  "name":"Tesla",
  "shares":25,
  "costBasisPerShare":800.00,
  "currentValue":18750.00,
  "currentPrice":750.00,
  "term":"Short Term",
  "totalGainLoss":-1250.00,
}
```


## 🧪 HOW TO RUN TESTS

```bash
cd CodingExercise.Tests
dotnet restore
dotnet test
```

You should see 27 successful tests.

## 📋 ERROR HANDLING EXAMPLES

The API properly handles error cases:

- **Invalid User**: Returns empty array (because my mock data does not have a user "table" and the user id is just attached to the investment data so if no investments have the user id specified it will give an empty response. Could also do a 404 if the data were different.)
- **Invalid Investment ID**: Returns 404 with descriptive message  
- **Missing Parameters**: Returns 400 with validation errors
- **Server Errors**: Returns 500 with generic message (no sensitive data exposed)