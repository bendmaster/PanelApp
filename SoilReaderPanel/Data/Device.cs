using SoilReaderPanel.Data;
using SoilReaderPanel.Models;
using SoilReaderPanel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeApp.Data
{
    public class Device
    {
        [Key, Required]
        public int DeviceID { get; set; }
        [Display(Name = "Device ID"), Required]
        public string ParticleDeviceID { get; set; }
        [Required, Display(Name = "Device Name")]
        public string DeviceName { get; set; }
        [Display(Name="Current Image"), DisplayFormat(NullDisplayText = "/img/circuit-board.svg", ApplyFormatInEditMode = true)]
        public string ImageLocation { get; set; }
        public IEnumerable<DeviceEvent> Events {get; set;}
    }

    public class DeviceEvent
    {
        [Key]
        public int eventId { get; set; }
        [Display(Name = "Event")]
        public string eventType { get; set; }

        [Display(Name = "Reading")]
        public string data { get; set; }
        [Required]
        public DateTime TIMESTAMP { get; set; }
        public Device Device { get; set; }
        
    
    }


    public interface IDeviceRepository : IDisposable
    {
        public bool AddDevice(Device device);
        public string UpdateDevice(Device device);
        public bool DeleteDevice(Device device);
        public Device GetDeviceById(int deviceId);
        public List<DeviceViewModel> GetAllDevices();

        
        public List<DeviceEvent> GetAllDeviceEventsById(int deviceId);        

    }

    public class DeviceRepository : IDeviceRepository
    {
        private readonly TokenFactory _tokenClient;
        private readonly AppDbContext _context;
        static private Dictionary<string, string> deviceReadings = new Dictionary<string, string>();
        private Dictionary<string, Task<string>> deviceReadingsAsync = new Dictionary<string, Task<string>>();

        public DeviceRepository(TokenFactory tf, AppDbContext context) {
            _context = context;
            _tokenClient = tf;
        }
        
        public Device GetDeviceById(int deviceId)
        {
            var query = from d in _context.Device
                        where d.DeviceID == deviceId
                        select d;

            var device = query.First();

            return device;
        }
        public List<DeviceViewModel> GetAllDevices()
        {
            var deviceList = _context.Device.ToList();

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
                    AllDevices.Add(new DeviceViewModel(deviceList[i], deviceReadings[deviceID], null));
                }
                else
                {
                    var newDeviceEvent = deviceReadingsAsync[deviceID].Result;
                    _context.DeviceEvent.Add(new DeviceEvent()
                    {
                        eventType = "soil",
                        data = newDeviceEvent,
                        Device = deviceList[i],
                        TIMESTAMP = DateTime.Now.ToUniversalTime()
                    });
                    deviceReadings.Add(deviceID, newDeviceEvent);
                    AllDevices.Add(new DeviceViewModel(deviceList[i], deviceReadings[deviceID], null));
                }

            }

            _context.SaveChanges();

            return AllDevices;
        }

        public string UpdateDevice(Device device)
        {
            //TODO: implement way to update device reading either on request or by daemon
            var newEvent = _tokenClient.GetData(device.ParticleDeviceID, "soil");
            _context.DeviceEvent.Add(new DeviceEvent()
            {
                eventType = "soil",
                data = newEvent.Result,
                Device = device
            });
            _context.SaveChanges();
            return newEvent.Result;
        }

        public List<DeviceEvent> GetAllDeviceEventsById(int deviceId)
        {

            //TODO: Get all device events to display history
            List<DeviceEvent> events = _context.DeviceEvent.Where(d => d.Device.DeviceID == deviceId).ToList();

            return events;


        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool AddDevice(Device device)
        {
            _context.Device.Add(device);
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDevice(Device device)
        {
            throw new NotImplementedException();
        }
    }




}
