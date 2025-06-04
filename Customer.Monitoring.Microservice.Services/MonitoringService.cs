using Customer.Monitoring.Microservice.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer.Monitoring.Microservice.App
{
    public class MonitoringService : IMonitoringServices
    {
        private readonly IMonitoringRepository _monitoringRepository;

        public MonitoringService(IMonitoringRepository monitoringRepository)
        {
            _monitoringRepository = monitoringRepository;
        }

        public async Task<List<Monitoring_i>> GetClusterMetricsAsync(string clusterName)
        {
            return await _monitoringRepository.GetClusterMetricsAsync(clusterName);
        }

        public async Task<List<Monitoring_i>> GetClusterMetricsCPUAsync(string clusterName)
        {
            return await _monitoringRepository.GetClusterMetricsCPUAsync(clusterName);
        }
    }
}
