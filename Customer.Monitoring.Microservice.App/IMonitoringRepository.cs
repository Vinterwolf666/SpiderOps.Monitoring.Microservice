
using Customer.Monitoring.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer.Monitoring.Microservice.App
{
    public interface IMonitoringRepository
    {
        
        Task<List<Monitoring_i>> GetClusterMetricsAsync(string clusterName);


        Task<List<Monitoring_i>> GetClusterMetricsByTypeAsync(string clusterName, string metricType);

        Task<List<Monitoring_i>> GetClusterMetricsCPUAsync(string clusterName);

    }
}
