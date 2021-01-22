using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RateLimiting.Host.Cache;
using System;
using System.Net;
using Xunit;

namespace RateLimiting.Tests.Cache
{
    public class RateLimitVerifierTest
    {
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<ConnectionInfo> _mockConnection;
        private readonly Mock<ILogger<RateLimitVerifier>> _mockLogger;
        private readonly RateLimitVerifier _target;

        public RateLimitVerifierTest()
        {
            _mockLogger = new Mock<ILogger<RateLimitVerifier>>();
            _target = new RateLimitVerifier(_mockLogger.Object);

            //Arrange
            var data = new byte[4];
            new Random().NextBytes(data);
            _mockConnection = new Mock<ConnectionInfo>();
            _mockConnection.Setup(x => x.RemoteIpAddress).Returns(new IPAddress(data));
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext.Setup(x => x.Connection).Returns(_mockConnection.Object);

        }

        [Fact]
        public void UnknownRequestor()
        {
            //act
            var result = _target.Verify(_mockHttpContext.Object, "Test", 10, 1);

            //assert
            Assert.True(result);
        }

        [Fact]
        public void KnownRequestor_BelowLimitRequests()
        {
            //act
            _target.Verify(_mockHttpContext.Object, "Test", 10, 2);
            var result = _target.Verify(_mockHttpContext.Object, "Test", 10, 2); //reach the limit

            //assert
            Assert.True(result);
        }

        [Fact]
        public void KnownRequestor_AboveLimitRequests()
        {
            //act
            _target.Verify(_mockHttpContext.Object, "Test", 10, 1); //reach the limit
            var result = _target.Verify(_mockHttpContext.Object, "Test", 10, 1); 

            //assert
            Assert.False(result);
        }

    }
}
