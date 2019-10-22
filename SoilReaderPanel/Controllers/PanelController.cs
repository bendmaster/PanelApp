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
        
        private readonly AppDbContext _context;


        public PanelController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<DeviceViewModel> listOfDevices = _context.getAllDevices();
            
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