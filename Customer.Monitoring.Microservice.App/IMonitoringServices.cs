using Customer.Monitoring.Microservice.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Customer.Monitoring.Microservice.App
{
    public interface IMonitoringServices
    {

        Task<List<Monitoring_i>> GetClusterMetricsAsync(string clusterName);
        Task<List<Monitoring_i>> GetClusterMetricsCPUAsync(string clusterName);


    }
}
