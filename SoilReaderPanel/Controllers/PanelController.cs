using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HomeApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SoilReaderPanel.Models;
using SoilReaderPanel.Services;

namespace SoilReaderPanel.Controllers
{
    public class PanelController : Controller
    {
        private readonly TokenFactory tokenClient;
        //public PanelController(IHttpClientFactory httpClientFactory, IConfiguration config) 
        //{
        //    tokenClient = new TokenFactory(httpClientFactory, config);
        //}
        public PanelController(TokenFactory tf)
        {
            tokenClient = tf;
        }
        public async Task<IActionResult> Index()
        {
            List<DeviceViewModel> listOfDevices = new List<DeviceViewModel>();

            var readings = new List<Task<string>>();

            for (int i = 0; i < 90; i++)
            {
                readings.Add(tokenClient.GetData("2d0041001851353530333932", "soil"));
            }

            for (int i=0; i < 9; i++)
            {
                //string reading = tokenClient.GetData("2d0041001851353530333932", "soil");
                listOfDevices.Add(new DeviceViewModel(i, "2d0041001851353530333932", "Office Spider Plant", null, readings[i].Result));
            }
            
            return View("Panel", listOfDevices);
        }
    }
}