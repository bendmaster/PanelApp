﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeApp.Data
{
    public class Device
    {
        [Key]
        [Required]
        [Display(Name ="Device ID")]
        public string DeviceID { get; set; }
        [Required]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }
        public IEnumerable<DeviceEvent> Events {get; set;}
    }

    public class DeviceEvent
    {
        [Key]
        public int eventId { get; set; }
        [Display(Name = "Event")]
        public string eventType { get; set; }

        [Display(Name = "Reading")]
        string data { get; set; }
        [Timestamp]
        public DateTime TIMESTAMP { get; set; }
        public Device Device { get; set; }
        
    
    }
}