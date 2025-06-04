
using Customer.Monitoring.Microservice.App;
using Customer.Monitoring.Microservice.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer.Monitoring.Microservice.API.Controllers
{
    [ApiController]
    [Route("Customer.Monitoring.Microservice.Customers.API/[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly IMonitoringServices _monitoringService;

        public MonitoringController(IMonitoringServices monitoringService)
        {
            _monitoringService = monitoringService;
        }

        [HttpGet("metrics/{clusterName}")]
        public async Task<ActionResult<List<Monitoring_i>>> GetClusterMetrics(string clusterName)
        {
            var metrics = await _monitoringService.GetClusterMetricsAsync(clusterName);

            if (metrics == null || metrics.Count == 0)
            {
                return NotFound("No metrics found for the specified cluster.");
            }

            return Ok(metrics);
        }

        [HttpGet("metrics/CPU/{clusterName}")]
        public async Task<ActionResult<List<Monitoring_i>>> GetClusterMetricsCPU(string clusterName)
        {
            var metrics = await _monitoringService.GetClusterMetricsCPUAsync(clusterName);

            if (metrics == null || metrics.Count == 0)
            {
                return NotFound("No metrics found for the specified cluster.");
            }

            return Ok(metrics);
        }
    }
}
