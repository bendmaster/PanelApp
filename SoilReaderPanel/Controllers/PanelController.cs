using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HomeApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoilReaderPanel.Services;

namespace SoilReaderPanel.Controllers
{
    public class PanelController : Controller
    {
        private readonly TokenFactory tokenClient;
        public PanelController(IHttpClientFactory httpClientFactory, IConfiguration config) 
        {
            tokenClient = new TokenFactory(httpClientFactory, config);
        }
        public async Task<IActionResult> Index()
        {
            List<Device> listOfDevices = new List<Device>();

            var result = (await tokenClient.GetTokenAsync());

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