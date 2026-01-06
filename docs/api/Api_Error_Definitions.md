# API Error Handling & ServiceResult Pattern

This document defines how the OrderProcessing API communicates errors to the frontend using standardized JSON structures, along with the `ServiceResult<T>` pattern for Application Layer services.

---

## 1. Problem Details Object

All API error responses (HTTP 4xx and 5xx) must use this structure:

```json
{
    "type": "string",
    "title": "string",
    "status": 0,
    "detail": "string",
    "instance": "string",
    "errors": {
        "string": ["string"]
    }
}
```

**Fields:**

- `type`: URI reference identifying the error type (e.g., `/errors/validation-error`)  
- `title`: Human-readable summary  
- `status`: HTTP status code  
- `detail`: Specific explanation  
- `instance`: URI of the API call  
- `errors`: Optional field-level errors (for 400 Bad Request)

---

## 2. Standard HTTP Error Types

| Type | HTTP Status | Description | Frontend Action |
|------|-------------|-------------|----------------|
| `/errors/validation-error` | 400 | Payload failed model validation | Show field-level messages |
| `/errors/not-found` | 404 | Resource not found | Display "Resource Not Found" |
| `/errors/unauthorized` | 401 | Authentication failed | Redirect to login |
| `/errors/forbidden` | 403 | User lacks permission | Display "Access Denied" |
| `/errors/internal-server-error` | 500 | Server exception | Show generic error |

---

## 3. Custom Domain Error Types

| Type | HTTP Status | Description | Triggered By |
|------|-------------|-------------|--------------|
| `/errors/stock/negative-quantity` | 400 | Sale would result in negative stock | `Book.DeductStock()` or validation |
| `/errors/stock/low-inventory` | 400 | Requested quantity exceeds available stock | Checkout Service |
| `/errors/checkout/credit-card-invalid` | 400 | Invalid card info | Checkout Service or mock gateway |
| `/errors/admin/threshold-violation` | 400 | Invalid admin threshold | Admin validation |
| `/errors/customer/account-exists` | 409 | Username/email already registered | Customer sign-up |

---

## 4. ServiceResult<T> Pattern

`ServiceResult<T>` is used in the **Application Layer** to standardize service outcomes:

```csharp
public class ServiceResult<T>
{
    public T Value { get; init; }          // DTO on success
    public bool IsSuccess { get; init; }   // True if operation succeeded
    public string ErrorCode { get; init; } // e.g., "NotFound", "Validation", "StockError"
    public string ErrorMessage { get; init; } // Human-readable error
}
```

**Usage in a service:**

```csharp
public async Task<ServiceResult<OrderDto>> PlaceOrderAsync(OrderCreateDto orderDto)
{
    var stock = await _repository.GetStockAsync(orderDto.BookId);
    if (stock < orderDto.Quantity)
    {
        return new ServiceResult<OrderDto>
        {
            IsSuccess = false,
            ErrorCode = "StockError",
            ErrorMessage = $"Requested quantity ({orderDto.Quantity}) exceeds stock ({stock})."
        };
    }

    var order = await _repository.AddOrderAsync(orderDto);

    return new ServiceResult<OrderDto>
    {
        IsSuccess = true,
        Value = new OrderDto
        {
            Id = order.Id,
            BookId = order.BookId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice
        }
    };
}
```

**Mapping ServiceResult to API response in Controller:**

```csharp
var result = await _orderService.PlaceOrderAsync(dto);

if (!result.IsSuccess)
{
    return result.ErrorCode switch
    {
        "StockError" => BadRequest(new ProblemDetails
        {
            Type = "/errors/stock/low-inventory",
            Title = result.ErrorMessage,
            Status = 400,
            Instance = HttpContext.Request.Path
        }),
        _ => StatusCode(500, new ProblemDetails
        {
            Type = "/errors/internal-server-error",
            Title = result.ErrorMessage,
            Status = 500,
            Instance = HttpContext.Request.Path
        })
    };
}

return Ok(result.Value);
```

**Advantages:**

- Avoids exceptions for expected business failures  
- Standardizes success/failure handling  
- Keeps Application Layer independent of HTTP  
- Makes mapping to Problem Details consistent  

---

## 5. Example Low Inventory API Response

```json
{
  "type": "/errors/stock/low-inventory",
  "title": "Insufficient Stock Available.",
  "status": 400,
  "detail": "Cannot complete checkout. Requested 5 copies of 'Book Title X', only 2 available.",
  "instance": "/api/checkout",
  "errors": {
    "items[0].quantity": ["Only 2 copies available."]
  }
}
```

