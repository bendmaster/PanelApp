﻿using HomeApp.Data;
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

        public DeviceViewModel(Device device, string reading)
        {
            base.DeviceID = device.DeviceID;
            base.ParticleDeviceID = device.ParticleDeviceID;
            base.DeviceName = device.DeviceName;
            base.ImageLocation = device.ImageLocation;
            Reading = reading;
        }
    }
}
