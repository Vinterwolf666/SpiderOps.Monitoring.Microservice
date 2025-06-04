
using Customer.Monitoring.Microservice.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Monitoring.Microservice.Infrastructure
{
    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
            :base(options)
        {
            
        }

        public DbSet<Monitoring_i> MonitoringDomain { get; set; }
    }
}
