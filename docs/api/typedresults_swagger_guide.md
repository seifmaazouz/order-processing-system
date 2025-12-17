# Using Typed Results for Swagger in ASP.NET Core

This document explains how to make your API endpoints Swagger-compatible **without adding multiple `[ProducesResponseType]` annotations** by using **Typed Results**. Typed Results improve Swagger documentation by allowing the framework to automatically detect all possible response types.

---

## 1. What are Typed Results?

Typed Results are a feature in **ASP.NET Core 7+** that allow you to define multiple possible return types for an endpoint in a **type-safe way**. Swagger automatically detects the response types and displays them accurately in the UI.

---

## 2. Basic Example for GET

Instead of:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<MyDto>> GetItem(int id)
{
    var item = await _service.GetItemAsync(id);
    if (item == null) return NotFound();
    return Ok(item);
}
```

Use Typed Results:

```csharp
[HttpGet("{id}")]
public async Task<Results<Ok<MyDto>, NotFound>> GetItem(int id)
{
    var item = await _service.GetItemAsync(id);
    return item is null ? TypedResults.NotFound() : TypedResults.Ok(item);
}
```

### Swagger Output

| Status | Response  |
| ------ | --------- |
| 200    | MyDto     |
| 404    | Not Found |

---

## 3. Example for POST

```csharp
[HttpPost]
public async Task<Results<Created<MyDto>, BadRequest, Conflict>> AddItem(MyCreateDto dto)
{
    var item = await _service.AddItemAsync(dto);
    return TypedResults.Created($"/api/items/{item.Id}", item);
}
```

### Swagger Output

| Status | Response    |
| ------ | ----------- |
| 201    | MyDto       |
| 400    | Bad Request |
| 409    | Conflict    |

---

## 4. Example Controller Using Typed Results

```csharp
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _service;

    public ItemsController(IItemService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<Results<Ok<MyDto>, NotFound>> GetItem(int id)
    {
        var item = await _service.GetItemAsync(id);
        return item is null ? TypedResults.NotFound() : TypedResults.Ok(item);
    }

    [HttpPost]
    public async Task<Results<Created<MyDto>, BadRequest, Conflict>> AddItem(MyCreateDto dto)
    {
        var item = await _service.AddItemAsync(dto);
        return TypedResults.Created($"/api/items/{item.Id}", item);
    }
}
```

This shows how Typed Results work in traditional controllers, not only in minimal APIs.

---

## 5. Benefits

- **Swagger-compatible:** all response types appear automatically.
- **No redundant annotations**.
- **Type-safe:** compiler ensures you return only the declared types.
- **Clean:** simpler and easier to read than `[ProducesResponseType]` everywhere.

---

## 6. Requirements

- ASP.NET Core **7.0+**
- `[ApiController]` attribute on your controller
- Endpoints should return `Results<T1, T2, ...>` instead of `ActionResult<T>`.

---

## 7. References

- [ASP.NET Core Minimal APIs: Typed Results](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-results)
- [Swagger OpenAPI & Typed Results](https://learn.microsoft.com/en-us/aspnet/core/tutorials/minimal-apis/openapi)

