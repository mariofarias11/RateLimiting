using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RateLimiting.Controllers;
using RateLimiting.Host.Cache;
using Xunit;

namespace RateLimiting.Tests.Controllers
{
    public class RateLimitingControllerTest
    {
        private readonly RateLimitingController _target;
        private readonly Mock<IRateLimitVerifier> _mockRateLimitVerifier;

        public RateLimitingControllerTest()
        {
            _mockRateLimitVerifier = new Mock<IRateLimitVerifier>();
            _target = new RateLimitingController(_mockRateLimitVerifier.Object);
        }

        [Fact]
        public void ReturnSuccess()
        {
            //arrange
            _mockRateLimitVerifier.Setup(x => x.Verify(It.IsAny<HttpContext>(), 
                                                       It.IsAny<string>(), 
                                                       It.IsAny<int>(), 
                                                       It.IsAny<int>()))
                                  .Returns(true);

            //act
            var result = _target.Try();
            var assert = result as OkResult;

            //assert
            Assert.True(assert.StatusCode == 200);
        }

        [Fact]
        public void ReturnTooManyRequests()
        {
            //arrange
            //arrange
            _mockRateLimitVerifier.Setup(x => x.Verify(It.IsAny<HttpContext>(),
                                                       It.IsAny<string>(),
                                                       It.IsAny<int>(),
                                                       It.IsAny<int>()))
                                  .Returns(false);

            //act
            var result = _target.Try();
            var assert = result as StatusCodeResult;

            //assert
            Assert.True(assert.StatusCode == 429);
        }
    }
}
