using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // a) Total sales for books in the previous month
    [HttpGet("total-sales/previous-month")]
    public async Task<Ok<SalesReportDto>> GetTotalSalesPreviousMonth()
    {
        var result = await _reportService.GetTotalSalesPreviousMonthAsync();
        return TypedResults.Ok(result);
    }

    // b) The total sales for books on a certain day
    [HttpGet("total-sales/by-date")]
    public async Task<Ok<SalesReportDto>> GetTotalSalesByDate([FromQuery] DateOnly? date) // nullable because if no input, model would have defaulted to 0001-01-01
    {
        var result = await _reportService.GetTotalSalesByDateAsync(date);
        return TypedResults.Ok(result);
    }

    // c) Top 5 Customers (Last 3 Months)
    [HttpGet("top-5-customers")]
    public async Task<Ok<IEnumerable<TopCustomerDto>>> GetTop5Customers()
    {
        var result = await _reportService.GetTop5CustomersAsync();
        return TypedResults.Ok(result);
    }

    // d) Top 10 Selling Books (Last 3 Months)
    [HttpGet("top-10-selling-books")]
    public async Task<Ok<IEnumerable<TopSellingBookDto>>> GetTop10SellingBooks()
    {
        var result = await _reportService.GetTop10SellingBooksAsync();
        return TypedResults.Ok(result);
    }

    // e) Total Number of Times a Specific Book Has Been Ordered (Replenishment)
    [HttpGet("book-order-count")]
    public async Task<Results<Ok<BookReplenishmentCountDto>, NotFound<ErrorResponse>>> GetBookOrderCount([FromQuery] string isbn)
    {
        var result = await _reportService.GetBookReplenishmentCountAsync(isbn);
        
        if (result == null) 
            return TypedResults.NotFound(new ErrorResponse($"No replenishment history found for book {isbn}.", 404));

        return TypedResults.Ok(result);
    }
}