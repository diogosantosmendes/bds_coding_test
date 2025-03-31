using Application.Shared.Types;
using Microsoft.AspNetCore.Mvc;
using Application.Auctions.Abstractions.services;
using Domain.Auctions.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuctionsController : ControllerBase
    {

        private readonly ILogger<AuctionsController> _logger;
        private readonly IAuctionsService _auctionsService;

        public AuctionsController(ILogger<AuctionsController> logger, IAuctionsService auctionsService)
        {
            _logger = logger;
            _auctionsService = auctionsService;
        }


        [HttpPost]
        public Task<DataResult<Auction>> Start([FromBody] string vehicleId)
        {
            return _auctionsService.StartAsync(vehicleId);
        }

        [HttpPost("bid/{auctionId}")]
        public Task<Result> Bid([FromRoute] string auctionId, [FromBody] float value)
        {
            // as we dont have authz to get the user id from Claims (HttpContext.User), I will genrate a random guid for the user id
            return _auctionsService.BidAsync(Guid.NewGuid().ToString(), auctionId, value);
        }

        [HttpPut("close/{auctionId}")]
        public Task<Result> Close([FromRoute] string auctionId)
        {
            return _auctionsService.CloseAsync(auctionId);
        }
    }
}
