using Moq;
using Microsoft.Extensions.Logging;
using System.Net;
using Assert = Xunit.Assert;

namespace Mediporta_Rekrutacja.Tests
{
    public class UnitTest_StackOverflowAPIService
    {
        public StackOverflowAPIService stackOverflowAPIService;
        public Mock<HttpClient> mockHttpClient;

        public UnitTest_StackOverflowAPIService()
        {
            mockHttpClient = new Mock<HttpClient>(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });
            stackOverflowAPIService = new StackOverflowAPIService(mockHttpClient.Object, new Mock<ILogger<StackOverflowAPIService>>().Object);
        }

        [Fact]
        public async Task GetTags_ReturnsListOfTags_WhenApiCallIsSuccessful()
        {
            var tags = await stackOverflowAPIService.GetTags(size: 200);

            Assert.NotNull(tags);
            Assert.Equal(200, tags.Count);
        }

        [Fact]
        public void GetTags_ThrowsException_WhenApiCallFails()
        {
            Assert.ThrowsAsync<Exception>(() => stackOverflowAPIService.GetTags());
        }
    }
}

