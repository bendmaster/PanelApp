using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HomeApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoilReaderPanel.Data;
using SoilReaderPanel.Models;
using SoilReaderPanel.Services;

namespace SoilReaderPanel.Controllers
{
    [Authorize("isAdmin")]
    public class PanelController : Controller
    {
        private readonly IDeviceRepository _deviceReposity;

        public PanelController(IDeviceRepository deviceRepository)
        {
            _deviceReposity = deviceRepository;
        }
        public IActionResult Index()
        {
            List<DeviceViewModel> listOfDevices = _deviceReposity.GetAllDevices();
            
            return View("Panel", listOfDevices);
        }

        [HttpGet]
        public IActionResult AddDevice()
        {
            return View(new Device());
        }

        [HttpPost]
        public IActionResult AddDevice(Device device)
        {
            if (ModelState.IsValid)
            {
                bool pass = _deviceReposity.AddDevice(device);
                if (pass)
                {
                    return RedirectToAction("Index");
                }
            }
            return View("AddDevice", device);
        }

        [HttpGet]
        public IActionResult EditDevice(int id)
        {
            Device device = _deviceReposity.GetDeviceById(id);

            return View(device);
        }

        [HttpGet]
        public IActionResult DeviceDetails(int id)
        {
            var device = _deviceReposity.GetDeviceById(id);
            var eventList = _deviceReposity.GetAllDeviceEventsById(id);

            return View(new DeviceViewModel(device, eventList[0].data, eventList) { });
        }
    }
}