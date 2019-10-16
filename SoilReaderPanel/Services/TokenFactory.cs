using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoilReaderPanel.Services
{
    public class TokenFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ParticleAuthCreds _auth;
        private readonly Uri authUri = new Uri("https://api.particle.io/oauth/token/");
        private readonly Uri deviceUri = new Uri("https://api.particle.io/v1/devices");
        private string acces_token;

        public TokenFactory(IHttpClientFactory httpClientFactory, IConfiguration config)
        { 
            _httpClientFactory = httpClientFactory;
            _auth = new ParticleAuthCreds(
                username: config["particle:username"],
                password: config["particle:password"],
                client_id: config["particle:client_id"],
                client_secret: config["particle:client_secret"]
                );

        }    

        public IConfiguration Configuration { get; }
           
        public async Task<string> GetTokenAsync()
        {             
            GenerateToken();

            return acces_token;
        }

        private void RefreshToken()
        {

        }

        private async Task<bool> GenerateToken()
        {
            var client = _httpClientFactory.CreateClient();
            string postString = $"grant_type=password&username={_auth.username}&password={_auth.password}&client_id={_auth.client_id}&client_secret={_auth.client_secret}";
            StringContent body = new StringContent(postString,
            Encoding.UTF8,
            "application/x-www-form-urlencoded");
            //HttpResponseMessage result = await client.PostAsync("https://api.particle.io/v1/devices/2d0041001851353530333932/soil?access_token=087f59e117c21b29a805b67ae4e349c784593cbf", body);
            HttpResponseMessage result = await client.PostAsync("https://api.particle.io/oauth/token", body);
            var obl = result.Content.ReadAsStringAsync().Result;
            return true;

        }

        

    }
    class ParticleAuthCreds
    {
        public readonly string username;
        public readonly string password;
        public readonly string client_id;
        public readonly string client_secret;

        public ParticleAuthCreds(string username, string password, string client_id, string client_secret)
        {
            this.username = username;
            this.password = password;
            this.client_id = client_id;
            this.client_secret = client_secret;
        }
    }
}
