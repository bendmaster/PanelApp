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
        static private Dictionary<string, string> deviceReadings = new Dictionary<string, string>();
        private Dictionary<string, Task<string>> deviceReadingsAsync = new Dictionary<string, Task<string>>();
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
                string deviceID = deviceList[i].ParticleDeviceID;
                //Caching to limit calls to device and improve consistency
                if (!deviceReadings.ContainsKey(deviceID))
                {
                    deviceReadingsAsync.Add(deviceID, _tokenClient.GetData(deviceList[i].ParticleDeviceID, "soil"));
                }
            }

            for (int i = 0; i < deviceList.Count; i++)
            {
                string deviceID = deviceList[i].ParticleDeviceID;
                if (deviceReadings.ContainsKey(deviceID))
                {
                    AllDevices.Add(new DeviceViewModel(deviceList[i], deviceReadings[deviceID]));                    
                }
                else
                {
                    var newDeviceEvent = deviceReadingsAsync[deviceID].Result;
                    DeviceEvent.Add(new DeviceEvent()
                    {
                        eventType = "soil",
                        data = newDeviceEvent,
                        Device = deviceList[i]
                    });
                    deviceReadings.Add(deviceID, newDeviceEvent);
                    AllDevices.Add(new DeviceViewModel(deviceList[i], deviceReadings[deviceID]));
                }
                
            }

            this.SaveChanges();

            return AllDevices;
        }

        public string updateDevice(Device device)
        {
            //TODO: implement way to update device reading either on request or by daemon
            var newEvent = _tokenClient.GetData(device.ParticleDeviceID, "soil");
            DeviceEvent.Add(new DeviceEvent()
            {
                eventType = "soil",
                data = newEvent.Result,
                Device = device
            });
            this.SaveChanges();
            return newEvent.Result;
        }

        public List<DeviceEvent> getAllDeviceEvents(int deviceId) {

            //TODO: Get all device events to display history
            List<DeviceEvent> events = DeviceEvent.Where(d => d.Device.DeviceID == deviceId).ToList();

            return events;


        }

    }
}
