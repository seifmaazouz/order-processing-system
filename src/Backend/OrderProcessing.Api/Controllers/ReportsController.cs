using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // a) Total sales for books in the previous month
    [HttpGet("total-sales/previous-month")]
    public async Task<ActionResult<SalesReportDto>> GetTotalSalesPreviousMonth()
    {
        var result = await _reportService.GetTotalSalesPreviousMonthAsync();
        return Ok(result);
    }

    // b) The total sales for books on a certain day
    // Usage: GET /api/reports/total-sales/by-date?date=2025-01-01
    [HttpGet("total-sales/by-date")]
    public async Task<ActionResult<SalesReportDto>> GetTotalSalesByDate([FromQuery] DateTime date)
    {
        var result = await _reportService.GetTotalSalesByDateAsync(date);
        return Ok(result);
    }

    // c) Top 5 Customers (Last 3 Months)
    [HttpGet("top-5-customers")]
    public async Task<ActionResult<IEnumerable<TopCustomerDto>>> GetTop5Customers()
    {
        var result = await _reportService.GetTop5CustomersAsync();
        return Ok(result);
    }

    // d) Top 10 Selling Books (Last 3 Months)
    [HttpGet("top-10-selling-books")]
    public async Task<ActionResult<IEnumerable<TopSellingBookDto>>> GetTop10SellingBooks()
    {
        var result = await _reportService.GetTop10SellingBooksAsync();
        return Ok(result);
    }

    // e) Total Number of Times a Specific Book Has Been Ordered (Replenishment)
    // Usage: GET /api/reports/book-order-count?isbn=978-3-16-148410-0
    [HttpGet("book-order-count")]
    public async Task<ActionResult<BookReplenishmentCountDto>> GetBookOrderCount([FromQuery] string isbn)
    {
        var result = await _reportService.GetBookReplenishmentCountAsync(isbn);
        
        if (result == null) 
            return NotFound($"No replenishment history found for book {isbn}");

        return Ok(result);
    }
}