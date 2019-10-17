using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HomeApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoilReaderPanel.Data;
using SoilReaderPanel.Models;
using SoilReaderPanel.Services;

namespace SoilReaderPanel.Controllers
{
    public class PanelController : Controller
    {
        
        private readonly AppDbContext _context;
        //public PanelController(IHttpClientFactory httpClientFactory, IConfiguration config) 
        //{
        //    tokenClient = new TokenFactory(httpClientFactory, config);
        //}
        public PanelController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<DeviceViewModel> listOfDevices = _context.getAllDevices();

            var readings = new List<Task<string>>();

            //for (int i = 0; i < 90; i++)
            //{
            //    readings.Add(_tokenClient.GetData("2d0041001851353530333932", "soil"));
            //}

            //for (int i=0; i < 9; i++)
            //{
            //    //string reading = tokenClient.GetData("2d0041001851353530333932", "soil");
            //    listOfDevices.Add(new DeviceViewModel(i, "2d0041001851353530333932", "Office Spider Plant", null, readings[i].Result));
            //}
            
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
                _context.Device.Add(device);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("AddDevice", device);
        }

        [HttpGet]
        public IActionResult EditDevice(int id)
        {
            Device device = _context.getDevice(id);

            return View(device);
        }
    }
}