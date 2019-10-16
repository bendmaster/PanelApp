using HomeApp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SoilReaderPanel.Models
{
    public class DeviceViewModel : Device
    {     
        [Display(Name = "Reading")]
        public string Reading { get; set; }

        public DeviceViewModel(int deviceid, string particleid, string name, string img, string reading)
        {
            base.DeviceID = deviceid;
            base.ParticleDeviceID = particleid;
            base.DeviceName = name;
            base.ImageLocation = img;
            Reading = reading;
        }
    }
}
