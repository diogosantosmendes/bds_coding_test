using Application.Inventory.DTOs;
using Application.Shared.Types;
using Domain.Auctions.Entities;
using Domain.Auctions.Enums;
using Domain.Inventory.Enums;
using Newtonsoft.Json;
using System.Text;

namespace Tests.Integration.WebApi
{
    [TestFixture]
    public class TestControllers
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Program> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Add_SuccessWhen_VehicleIsValid()
        {
            // Arrange
            var vehicleDto = new AddVehicleDto
            {
                Id = "5",
                Type = VehicleType.SUV,
                ManufacturerId = "1",
                ModelId = "1",
                Year = 2021,
                StartingBid = 10000,
                Seats = 5
            };
            var content = new StringContent(JsonConvert.SerializeObject(vehicleDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Vehicles", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result>(responseString);

            // Assert
            Assert.IsTrue(result.Ok);
        }

        [Test]
        public async Task Search_SuccessWhen_FiltersAreValid()
        {
            // Arrange
            var page = new Page { Index = 1, Size = 10 };
            var filter = new Filter { Field = "ManufacturerId", Value = "1", Comparator = Filter.FilterComparisionOperator.EQUAL };
            var sort = new Sort { By = "Year", Desc = true };
            var query = $"?Index={page.Index}&Size={page.Size}&Field={filter.Field}&Value={filter.Value}&Comparator={filter.Comparator}&By={sort.By}&Desc={sort.Desc}";

            // Act
            var response = await _client.GetAsync($"/Vehicles{query}");
            var responseString = await response.Content.ReadAsStringAsync();

            // C# is no able to parse IVehicle because is an interface and so Newtonsoft.Json is not able to
            // instantiate a class from an interface
            //var result = JsonConvert.DeserializeObject<PageResult<IVehicle>>(responseString);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(responseString.Contains("\"ok\":true"));
        }

        [Test]
        public async Task Start_SuccessWhen_VehicleIsNotInAuction()
        {
            // Arrange
            var vehicleId = "2";
            var content = new StringContent(JsonConvert.SerializeObject(vehicleId), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Auctions", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DataResult<Auction>>(responseString);

            // Assert
            Assert.IsTrue(result.Ok);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(vehicleId, result.Data.VehicleId);
            Assert.AreEqual(AuctionState.STARTED, result.Data.State);
        }

        [Test]
        public async Task Bid_SuccessWhen_BidIsValid()
        {
            // Arrange
            var value = 2000 ;
            var auctionId = "1";
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/Auctions/bid/{auctionId}", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result>(responseString);

            // Assert
            Assert.IsTrue(result.Ok);

        }

        [Test]
        public async Task Close_SuccessWhen_AuctionIsOpen()
        {
            // Arrange
            var auctionId = "1";

            // Act
            var response = await _client.PutAsync($"/Auctions/close/{auctionId}", null);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result>(responseString);

            // Assert
            Assert.IsTrue(result.Ok);
        }
    }
}
