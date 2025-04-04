using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Municipality_Management_System.Models;

namespace MunicipalityManagementSystemV2.Data
{
    public class MunicipalityManagementSystemV2Context : DbContext
    {
        public MunicipalityManagementSystemV2Context (DbContextOptions<MunicipalityManagementSystemV2Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CitizenManagementModel>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<StaffManagementModel>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }


        public DbSet<Municipality_Management_System.Models.CitizenManagementModel> CitizenManagementModel { get; set; } = default!;
        public DbSet<Municipality_Management_System.Models.ReportsModel> ReportsModel { get; set; } = default!;
        public DbSet<Municipality_Management_System.Models.ServiceRequestModel> ServiceRequestModel { get; set; } = default!;
        public DbSet<Municipality_Management_System.Models.StaffManagementModel> StaffManagementModel { get; set; } = default!;
    }
}
