using HomeApp.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoilReaderPanel.Models;
using SoilReaderPanel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace SoilReaderPanel.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options, TokenFactory tf) : base(options) { }

        public DbSet<Device> Device { get; set; }
        public DbSet<DeviceEvent> DeviceEvent { get; set; }

    }
}
