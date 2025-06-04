using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Customer.Monitoring.Microservice.App;
using Customer.Monitoring.Microservice.Domain;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using Grpc.Auth;
using Grpc.Core;

namespace Customer.Monitoring.Microservice.Infrastructure
{
    public class MonitoringRepository : IMonitoringRepository
    {
        private readonly string _projectId = "spiderops"; // Project ID
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string DropboxUrl = "https://www.dropbox.com/scl/fi/9hm7gqwaaymm1uo1u5n5n/spiderops-fcc51c76307a.json?rlkey=4yvewm2zsdk9kjki0p6dqsu4y&dl=1"; // URL del JSON

        private async Task<MetricServiceClient> CreateMetricServiceClientAsync()
        {
            Console.WriteLine("Descargando credenciales desde Dropbox...");

            var credentialStream = await _httpClient.GetStreamAsync(DropboxUrl);
            var credential = GoogleCredential.FromStream(credentialStream)
                                              .CreateScoped(MetricServiceClient.DefaultScopes);

            var channelCredentials = credential.ToChannelCredentials();

            var client = await new MetricServiceClientBuilder
            {
                ChannelCredentials = channelCredentials
            }.BuildAsync();

            Console.WriteLine("Cliente MetricServiceClient creado exitosamente.");
            return client;
        }

        public async Task<List<Monitoring_i>> GetClusterMetricsAsync(string clusterName)
        {
            const string metricType = "kubernetes.io/container/memory/used_bytes";

            var client = await CreateMetricServiceClientAsync();

            var request = new ListTimeSeriesRequest
            {
                Name = $"projects/{_projectId}",
                Filter = $"resource.type=\"k8s_container\" AND metric.type=\"{metricType}\" AND resource.labels.cluster_name=\"{clusterName}\"",
                Interval = new TimeInterval
                {
                    EndTime = Timestamp.FromDateTime(DateTime.UtcNow),
                    StartTime = Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(-5))
                },
                View = ListTimeSeriesRequest.Types.TimeSeriesView.Full
            };

            try
            {
                var response = client.ListTimeSeries(request);

                var metrics = new List<Monitoring_i>();

                foreach (var timeSeries in response)
                {
                    foreach (var point in timeSeries.Points)
                    {
                        var rawValue = point.Value.DoubleValue != 0 ? point.Value.DoubleValue : point.Value.Int64Value;

                        // Aquí convertimos de bytes a GB
                        var valueInGB = rawValue / 1_073_741_824.0;

                        metrics.Add(new Monitoring_i
                        {
                            ProjectId = _projectId,
                            ClusterName = clusterName,
                            MetricType = timeSeries.Metric.Type,
                            Value = valueInGB, // <-- Aquí guardas el valor ya convertido a GB
                            Timestamp = point.Interval.EndTime.ToDateTime(),
                            LabelsJson = System.Text.Json.JsonSerializer.Serialize(timeSeries.Resource.Labels)
                        });
                    }
                }

                return metrics;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                Console.WriteLine($"No se encontraron métricas para el clúster {clusterName}: {ex.Status.Detail}");
                return new List<Monitoring_i>(); // Devuelve lista vacía si no hay métricas
            }
        }


        public async Task<List<Monitoring_i>> GetClusterMetricsCPUAsync(string clusterName)
        {
            const string metricType = "kubernetes.io/container/cpu/core_usage_time";

            var client = await CreateMetricServiceClientAsync();

            var request = new ListTimeSeriesRequest
            {
                Name = $"projects/{_projectId}",
                Filter = $"resource.type=\"k8s_container\" AND metric.type=\"{metricType}\" AND resource.labels.cluster_name=\"{clusterName}\"",
                Interval = new TimeInterval
                {
                    EndTime = Timestamp.FromDateTime(DateTime.UtcNow),
                    StartTime = Timestamp.FromDateTime(DateTime.UtcNow.AddMinutes(-5))
                },
                View = ListTimeSeriesRequest.Types.TimeSeriesView.Full
            };

            try
            {
                var response = client.ListTimeSeries(request);

                var metrics = new List<Monitoring_i>();

                foreach (var timeSeries in response)
                {
                    // Ordenamos los puntos por tiempo
                    var sortedPoints = new List<Point>(timeSeries.Points);
                    sortedPoints.Sort((a, b) => a.Interval.EndTime.CompareTo(b.Interval.EndTime));

                    for (int i = 1; i < sortedPoints.Count; i++)
                    {
                        var previous = sortedPoints[i - 1];
                        var current = sortedPoints[i];

                        var deltaValue = (current.Value.DoubleValue != 0 ? current.Value.DoubleValue : current.Value.Int64Value)
                                       - (previous.Value.DoubleValue != 0 ? previous.Value.DoubleValue : previous.Value.Int64Value);

                        var deltaTime = current.Interval.EndTime.ToDateTime() - previous.Interval.EndTime.ToDateTime();
                        var deltaSeconds = deltaTime.TotalSeconds;

                        double cpuUsagePercent = (deltaValue / deltaSeconds) * 100;

                        metrics.Add(new Monitoring_i
                        {
                            ProjectId = _projectId,
                            ClusterName = clusterName,
                            MetricType = timeSeries.Metric.Type,
                            Value = cpuUsagePercent, // Ahora es % de uso
                            Timestamp = current.Interval.EndTime.ToDateTime(),
                            LabelsJson = System.Text.Json.JsonSerializer.Serialize(timeSeries.Resource.Labels)
                        });
                    }
                }

                return metrics;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                Console.WriteLine($"No se encontraron métricas para el clúster {clusterName}: {ex.Status.Detail}");
                return new List<Monitoring_i>(); // Devuelve lista vacía si no hay métricas
            }
        }



        public Task<List<Monitoring_i>> GetClusterMetricsByTypeAsync(string clusterName, string metricType)
        {
            throw new NotImplementedException();
        }
    }
}
