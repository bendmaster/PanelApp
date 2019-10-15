using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeApp.Data;
using Microsoft.AspNetCore.Mvc;

namespace SoilReaderPanel.Controllers
{
    public class PanelController : Controller
    {
        public IActionResult Index()
        {
            List<Device> listOfDevices = new List<Device>();

            for (int i=0; i < 10; i++)
            {
                listOfDevices.Add(new Device()
                {
                    DeviceID = "Sup3rT3xtf0RD3vic3C0d3",
                    DeviceName = "MyFirstDevice",
                });
            }
            
            return View("Panel", listOfDevices);
        }
    }
}