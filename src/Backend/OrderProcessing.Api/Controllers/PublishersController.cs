using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Api.Models;
using OrderProcessing.Application.DTOs.Publisher;
using OrderProcessing.Application.Interfaces;

namespace OrderProcessing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<Ok<IEnumerable<PublisherDto>>> GetAllPublishers()
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return TypedResults.Ok(publishers);
        }

        [HttpGet("{id}")]
        public async Task<Results<Ok<PublisherDto>, NotFound<ErrorResponse>>> GetPublisherById(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
                return TypedResults.NotFound(new ErrorResponse($"Publisher {id} not found.", 404));

            return TypedResults.Ok(publisher);
        }
    }
}
