using MvvmCross.Tests;
using SoundByte.Core.Services.Implementations;
using System.Threading.Tasks;
using Xunit;

namespace SoundByte.Core.Tests.Services
{
    public class SearchPreviewServiceTests : MvxIoCSupportingTest
    {
        public SearchPreviewService SearchPreviewService { get; } = new SearchPreviewService();

        [Fact]
        public async Task TestEmpty()
        {
            var results = await SearchPreviewService.GetSearchSuggestionsAsync(string.Empty);
            Assert.Empty(results);
        }

        [Fact]
        public async Task TestKnown()
        {
            var results = await SearchPreviewService.GetSearchSuggestionsAsync("Monstercat");
            Assert.NotEmpty(results);
        }
    }
}