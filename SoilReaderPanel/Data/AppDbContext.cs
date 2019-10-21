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
        private readonly TokenFactory _tokenClient;
        public AppDbContext(DbContextOptions<AppDbContext> options, TokenFactory tf) : base(options) 
        {
            _tokenClient = tf;
        }

        public DbSet<Device> Device { get; set; }
        public DbSet<DeviceEvent> DeviceEvent { get; set; }
        
        public Device getDevice(int deviceId) 
        {
            var query = from d in this.Device
                        where d.DeviceID == deviceId
                        select d;

            var device = query.First();

            return device;
        }
        public List<DeviceViewModel> getAllDevices() 
        {
            var deviceList = this.Device.ToList();

            List<DeviceViewModel> AllDevices = new List<DeviceViewModel>();

            var readings = new List<Task<string>>();

            for (int i = 0; i < deviceList.Count; i++)
            {
                readings.Add(_tokenClient.GetData(deviceList[i].ParticleDeviceID, "soil"));
            }

            for (int i = 0; i < deviceList.Count; i++)
            {
                AllDevices.Add(new DeviceViewModel(deviceList[i], readings[i].Result));
            }

            return AllDevices;
        }

        public void getAllDeviceEvents(int deviceId) { 
        
            //TODO

        }

    }
}
