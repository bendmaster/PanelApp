using SoilReaderPanel.Data;
using SoilReaderPanel.Models;
using SoilReaderPanel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoilReaderPanel.Data
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
        public Task<bool> AddDeviceAsync(Device device);
        public string UpdateDevice(Device device);
        public bool DeleteDevice(Device device);
        public Device GetDeviceById(int deviceId);

        
        public List<DeviceEvent> GetAllDeviceEventsById(int deviceId);
        Task<List<DeviceViewModel>> GetAllDevicesAsync();
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
        public async Task<List<DeviceViewModel>> GetAllDevicesAsync()
        {
            var deviceList = _context.Device.ToList();

            List<DeviceViewModel> AllDevices = new List<DeviceViewModel>();

            var readings = new List<Task<string>>();

            for (int i = 0; i < deviceList.Count; i++)
            {
                string deviceID = deviceList[i].ParticleDeviceID;
                //Caching to limit calls to device and improve consistency
                if (!deviceReadings.ContainsKey(deviceID) || deviceReadings[deviceID] == "Unable to connect to device")
                {
                    deviceReadingsAsync.Add(deviceID, _tokenClient.GetData(deviceList[i].ParticleDeviceID, "soil"));
                }
            }

            for (int i = 0; i < deviceList.Count; i++)
            {
                string deviceID = deviceList[i].ParticleDeviceID;
                if (deviceReadings.ContainsKey(deviceID) && deviceReadings[deviceID] != "Unable to connect to device")
                {
                    AllDevices.Add(new DeviceViewModel(deviceList[i], deviceReadings[deviceID], null));
                }
                else
                {
                    
                    var newDeviceEventAsync = await deviceReadingsAsync[deviceID];
                    //If device is unplugged or unreachable for any other reason
                    var newDeviceEvent = newDeviceEventAsync == "fail" ? "Unable to connect to device" : newDeviceEventAsync;
                    _context.DeviceEvent.Add(new DeviceEvent()
                    {
                        eventType = "soil",
                        data = newDeviceEvent,
                        Device = deviceList[i],
                        TIMESTAMP = DateTime.Now.ToUniversalTime()
                    });
                    deviceReadings[deviceID] = newDeviceEvent;
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

        public async Task<bool> AddDeviceAsync(Device device)
        {
            if (!(await _tokenClient.ValidateID(device.ParticleDeviceID))) return false;
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

public class DeviceRoleInformation
{
    public bool gateway { get; set; }
    public string state { get; set; }
}

public class DeviceNetworkInformation
{
    public string id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public DeviceRoleInformation role { get; set; }
}

public class Variable { }
public class DeviceJSON
{
    public string id { get; set; }
    public string name { get; set; }
    public object last_app { get; set; }
    public string last_ip_address { get; set; }
    public DateTime last_heard { get; set; }
    public int product_id { get; set; }
    public bool connected { get; set; }
    public int platform_id { get; set; }
    public bool cellular { get; set; }
    public object notes { get; set; }
    public DeviceNetworkInformation network { get; set; }
    public string status { get; set; }
    public string serial_number { get; set; }
    public string mobile_secret { get; set; }
    public string current_build_target { get; set; }
    public string system_firmware_version { get; set; }
    public string default_build_target { get; set; }
    public Variable variables { get; set; }
    public List<string> functions { get; set; }
    public bool firmware_updates_enabled { get; set; }
    public bool firmware_updates_forced { get; set; }
}