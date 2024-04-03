using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Assert = Xunit.Assert;

namespace Mediporta_Rekrutacja.Tests
{
    public class UnitTest_PostgresDatabaseService
    {
        public PostgresDatabaseService postgresDatabaseService;
        public StackOverflowAPIService stackOverflowAPIService;
        public Mock<HttpClient> mockHttpClient;

        public UnitTest_PostgresDatabaseService()
        {
            postgresDatabaseService = new PostgresDatabaseService(
                new Mock<ILogger<PostgresDatabaseService>>().Object);
            mockHttpClient = new Mock<HttpClient>(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
            });
            stackOverflowAPIService = new StackOverflowAPIService(mockHttpClient.Object,
                new Mock<ILogger<StackOverflowAPIService>>().Object);
        }

        [Fact]
        public async Task CreateTagTable_TruncateAndInsertTags_Success()
        {
            var tags = new List<StackOverflowTag>
            {
                new StackOverflowTag { Name = "tag1", Count = 100 },
                new StackOverflowTag { Name = "tag2", Count = 200 }
            };

            await postgresDatabaseService.CreateTagTable(tags);

            var result = postgresDatabaseService.GetTags(1, 2);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPercentageOfTags_ReturnsTagsList_Success()
        {
            int page = 1;
            int pageSize = 10;

            var tags = await stackOverflowAPIService.GetTags(page, pageSize);

            await postgresDatabaseService.CreateTagTable(tags);
            var result = postgresDatabaseService.GetPercentageOfTags(page, pageSize);

            Assert.NotNull(result);
            Assert.Equal(typeof(List<PercentageOfTags>), result.GetType());
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public async Task GetTags_ReturnsTagsList_Success()
        {
            int page = 1;
            int pageSize = 10;
            TagColumn sortByColumn = TagColumn.name;
            SortingType sortingType = SortingType.asc;

            var tags = await stackOverflowAPIService.GetTags(page, pageSize);

            await postgresDatabaseService.CreateTagTable(tags);
            var result = postgresDatabaseService.GetTags(page, pageSize, sortByColumn, sortingType);

            Assert.NotNull(result);
            Assert.Equal(10, result.Count);
            Assert.Equal(typeof(List<StackOverflowTag>), result.GetType());
        }
    }
}
