using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;

namespace Mediporta_Rekrutacja.Tests
{
    public class UnitTest_DatabaseTagController
    {
        public Mock<IDatabaseService> mockPostgresDatabaseService = new Mock<IDatabaseService>();
        public Mock<ILogger<DatabaseTagController>> mockLogger = new Mock<ILogger<DatabaseTagController>>();

        public DatabaseTagController databaseTagController;

        public UnitTest_DatabaseTagController()
        {
            databaseTagController = new DatabaseTagController(mockPostgresDatabaseService.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ShouldReturnAllTagsFromDatabase()
        {
            int page = 0;
            int size = 10;

            var databaseTags = new List<StackOverflowTag>() { new StackOverflowTag() { Name = "C#", Count = 123 } };
            mockPostgresDatabaseService.Setup(x => x.GetTags(page, size, TagColumn.id, SortingType.asc)).Returns(databaseTags);

            var tags = await databaseTagController.GetTagsAsync(page, size);
            var result = tags.Result as OkObjectResult;

            Assert.StrictEqual(databaseTags, result.Value);
            ClassicAssert.AreEqual(200, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnBadRequest()
        {
            int page = 0;
            int size = 10;

            mockPostgresDatabaseService.Setup(x => x.GetTags(page, size, TagColumn.id, SortingType.asc)).Throws(new Exception("Failed to retrieve tags from StackOverflow API"));

            var tags = await databaseTagController.GetTagsAsync(page, size);
            var result = tags.Result as BadRequestResult;

            ClassicAssert.AreEqual(400, result.StatusCode);
        }


    }
}