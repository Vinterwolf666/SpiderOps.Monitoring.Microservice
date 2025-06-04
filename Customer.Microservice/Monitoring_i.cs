using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customer.Monitoring.Microservice.Domain
{
    [Table("MonitoringCluster")]
    public class Monitoring_i
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ProjectId { get; set; }
        public string ClusterName { get; set; }
        public string MetricType { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string LabelsJson { get; set; }
    }
}
