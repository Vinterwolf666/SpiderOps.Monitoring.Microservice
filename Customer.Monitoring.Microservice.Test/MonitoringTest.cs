using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Monitoring.Microservice.App;
using Customer.Monitoring.Microservice.Domain;
using Google.Api;

namespace Customer.Monitoring.Tests
{
    public class MonitoringServiceTests
    {
        private readonly Mock<IMonitoringRepository> _mockRepository;
        private readonly MonitoringService _service;

        public MonitoringServiceTests()
        {
            _mockRepository = new Mock<IMonitoringRepository>();
            _service = new MonitoringService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetClusterMetricsAsync_ReturnsExpectedMetrics()
        {
            // Arrange
            var clusterName = "test-cluster";
            var expectedMetrics = new List<Monitoring_i>
            {
                new Monitoring_i(),
                new Monitoring_i()
            };

            _mockRepository
                .Setup(repo => repo.GetClusterMetricsAsync(clusterName))
                .ReturnsAsync(expectedMetrics);

            // Act
            var result = await _service.GetClusterMetricsAsync(clusterName);

            // Assert
            Assert.Equal(expectedMetrics, result);
            _mockRepository.Verify(repo => repo.GetClusterMetricsAsync(clusterName), Times.Once);
        }

        [Fact]
        public async Task GetClusterMetricsCPUAsync_ReturnsExpectedMetrics()
        {
            // Arrange
            var clusterName = "test-cluster";
            var expectedCPUMetrics = new List<Monitoring_i>
            {
                new Monitoring_i(),
                new Monitoring_i()
            };

            _mockRepository
                .Setup(repo => repo.GetClusterMetricsCPUAsync(clusterName))
                .ReturnsAsync(expectedCPUMetrics);

            // Act
            var result = await _service.GetClusterMetricsCPUAsync(clusterName);

            // Assert
            Assert.Equal(expectedCPUMetrics, result);
            _mockRepository.Verify(repo => repo.GetClusterMetricsCPUAsync(clusterName), Times.Once);
        }
    }
}
